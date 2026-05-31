#Requires -Version 5.1
# Worktree coordinator — state I/O. Human view: .\scripts\worktree-coordinator.ps1 show
[CmdletBinding()]
param(
    [Parameter(Position = 0)]
    [ValidateSet('init', 'scan', 'show', 'get', 'set-prog', 'load', 'add-agent', 'remove-agent')]
    [string] $Action = 'get',

    [ValidateSet('main', 'agent')]
    [string] $Role,

    [string] $Task,
    [string[]] $Done,
    [string[]] $Wip,
    [string[]] $Todo,
    [string[]] $Blk,

    [string] $InFile
)

$ErrorActionPreference = 'Stop'
$RepoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$WtDir = Join-Path $RepoRoot 'worktrees'
$StatePath = Join-Path $WtDir 'state.json'
$PathsPath = Join-Path $WtDir 'paths.local.json'
$AgentRel = '../LearnURP-cursor'

function Read-Raw([string] $Path) {
    if (-not (Test-Path $Path)) { return $null }
    [System.IO.File]::ReadAllText($Path, [System.Text.UTF8Encoding]::new($false))
}

function Read-State {
    $raw = Read-Raw $StatePath
    if (-not $raw) { return $null }
    $raw | ConvertFrom-Json
}

function Esc-J([string] $s) {
    if ($null -eq $s) { return 'null' }
    '"' + ($s -replace '\\', '\\\\' -replace '"', '\"' -replace "`n", '\n' -replace "`r", '') + '"'
}

function JArr([string[]] $a) {
    if (-not $a -or $a.Count -eq 0) { return '[]' }
    '[' + (($a | ForEach-Object { Esc-J $_ }) -join ',') + ']'
}

function Emit-Tree([hashtable] $t) {
    $chg = $t.chg
    $rem = $t.rem
    $p = $t.prog
    @"
{"role":$(Esc-J $t.role),"br":$(Esc-J $t.br),"head":$(if ($t.head) { Esc-J $t.head } else { 'null' }),"subj":$(if ($t.subj) { Esc-J $t.subj } else { 'null' }),"at":$(if ($t.at) { Esc-J $t.at } else { 'null' }),"dirty":$(if ($t.dirty) { 'true' } else { 'false' }),"chg":{"s":$(JArr $chg.s),"u":$(JArr $chg.u),"n":$(JArr $chg.n)},"rem":{"up":$(if ($rem.up) { Esc-J $rem.up } else { 'null' }),"ah":$($rem.ah),"bh":$($rem.bh)},"prog":{"task":$(Esc-J $p.task),"done":$(JArr $p.done),"wip":$(JArr $p.wip),"todo":$(JArr $p.todo),"blk":$(JArr $p.blk)}}
"@
}

function Write-State([hashtable] $State) {
    Ensure-Dir
    $State['ts'] = (Get-Date).ToUniversalTime().ToString('yyyy-MM-ddTHH:mm:ssZ')
    $wt = $State.wt
    $json = '{"v":1,"repo":' + (Esc-J $State.repo) + ',"ts":' + (Esc-J $State.ts) + ',"wt":{"main":' + (Emit-Tree $wt.main) + ',"agent":' + (Emit-Tree $wt.agent) + '}}'
    [System.IO.File]::WriteAllText($StatePath, $json + "`n", [System.Text.UTF8Encoding]::new($false))
}

function Ensure-Dir {
    if (-not (Test-Path $WtDir)) { New-Item -ItemType Directory -Path $WtDir | Out-Null }
}

function To-StrList($v) {
    if ($null -eq $v) { return @() }
    if ($v -is [System.Array]) { return @($v | ForEach-Object { "$_" }) }
    @("$v")
}

function Invoke-Git([string] $Dir, [string[]] $GitArgs) {
    Push-Location $Dir
    try {
        $o = git @GitArgs 2>&1
        if ($LASTEXITCODE -ne 0) { throw "git $($GitArgs -join ' ') @ $Dir : $o" }
        ($o | Out-String).Trim()
    }
    finally { Pop-Location }
}

function Get-Porcelain([string] $Dir) {
    $lines = (Invoke-Git $Dir @('status', '--porcelain')) -split "`n"
    $s = [System.Collections.Generic.List[string]]::new()
    $u = [System.Collections.Generic.List[string]]::new()
    $n = [System.Collections.Generic.List[string]]::new()
    foreach ($line in $lines) {
        if ([string]::IsNullOrWhiteSpace($line)) { continue }
        if ($line -match '^\?\? (.+)$') { $n.Add($Matches[1].Trim().Trim('"')); continue }
        if ($line -match '^(.{2}) (.+)$') {
            $xy = $Matches[1]
            $p = $Matches[2].Trim().Trim('"')
            if ($xy[0] -ne ' ') { $s.Add($p) }
            if ($xy.Length -gt 1 -and $xy[1] -ne ' ') { $u.Add($p) }
        }
    }
    @{ s = @($s); u = @($u); n = @($n); dirty = ($s.Count + $u.Count + $n.Count) -gt 0 }
}

function Get-Remote([string] $Dir) {
    $br = (Invoke-Git $Dir @('branch', '--show-current')).Trim()
    if (-not $br) { return @{ up = $null; ah = 0; bh = 0 } }
    Push-Location $Dir
    try {
        $up = git rev-parse --abbrev-ref '@{u}' 2>$null
        if ($LASTEXITCODE -ne 0) { return @{ up = $null; ah = 0; bh = 0 } }
        $up = "$up".Trim()
        $ab = (git rev-list --left-right --count "HEAD...$up" 2>$null).Trim() -split '\s+'
        @{ up = $up; bh = [int]$ab[0]; ah = [int]$ab[1] }
    }
    finally { Pop-Location }
}

function Get-Head([string] $Dir) {
    @{
        br   = (Invoke-Git $Dir @('branch', '--show-current')).Trim()
        head = (Invoke-Git $Dir @('rev-parse', '--short', 'HEAD'))
        subj = (Invoke-Git $Dir @('log', '-1', '--format=%s'))
        at   = (Invoke-Git $Dir @('log', '-1', '--format=%cI'))
    }
}

function Discover-Paths {
    $m = @{ main = $RepoRoot }
    $cur = $null
    foreach ($line in ((Invoke-Git $RepoRoot @('worktree', 'list', '--porcelain')) -split "`n")) {
        if ($line -match '^worktree (.+)$') { $cur = $Matches[1].Trim(); continue }
        if ($line -match '^branch refs/heads/' -and $cur -and $cur -ne $RepoRoot) { $m['agent'] = $cur }
    }
    if (-not $m['agent']) {
        $c = Resolve-Path (Join-Path $RepoRoot $AgentRel) -ErrorAction SilentlyContinue
        if ($c) { $m['agent'] = $c.Path }
    }
    $m
}

function Get-Paths {
    if (Test-Path $PathsPath) {
        $p = (Read-Raw $PathsPath) | ConvertFrom-Json
        return @{ main = $p.main; agent = $p.agent }
    }
    $d = Discover-Paths
    Ensure-Dir
    [System.IO.File]::WriteAllText($PathsPath, (@{ main = $d.main; agent = $d.agent } | ConvertTo-Json -Compress) + "`n")
    $d
}

function Default-Prog {
    @{ task = ''; done = [string[]]@(); wip = [string[]]@(); todo = [string[]]@(); blk = [string[]]@() }
}

function Default-Tree([string] $Role) {
    @{
        role = if ($Role -eq 'main') { 'user' } else { 'agent' }
        br   = 'HandManage'
        head = $null
        subj = $null
        at   = $null
        dirty = $false
        chg  = @{ s = @(); u = @(); n = @() }
        rem  = @{ up = $null; ah = 0; bh = 0 }
        prog = Default-Prog
    }
}

function Default-State {
    @{
        v    = 1
        repo = 'LearnURP'
        wt   = @{
            main  = Default-Tree 'main'
            agent = Default-Tree 'agent'
        }
    }
}

function Normalize-Prog($p) {
    if (-not $p) { return Default-Prog }
    $task = if ($p -is [hashtable]) { HGet $p 'task' } else { $p.task }
    $done = if ($p -is [hashtable]) { HGet $p 'done' } else { $p.done }
    $wip  = if ($p -is [hashtable]) { HGet $p 'wip' } else { $p.wip }
    $todo = if ($p -is [hashtable]) { HGet $p 'todo' } else { $p.todo }
    $blk  = if ($p -is [hashtable]) { HGet $p 'blk' } else { $p.blk }
    @{
        task = if ($null -ne $task) { [string]$task } else { '' }
        done = [string[]](To-StrList $done)
        wip  = [string[]](To-StrList $wip)
        todo = [string[]](To-StrList $todo)
        blk  = [string[]](To-StrList $blk)
    }
}

function HGet($h, [string] $k) {
    if ($h -is [hashtable]) { return $h[$k] }
    $h.$k
}

function State-ToHashtable($obj) {
    if ($obj -is [hashtable]) { return $obj }
    $wtRoot = HGet $obj 'wt'
    $h = @{ v = HGet $obj 'v'; repo = HGet $obj 'repo'; ts = HGet $obj 'ts'; wt = @{} }
    foreach ($key in @('main', 'agent')) {
        $t = HGet $wtRoot $key
        if (-not $t) { $h.wt[$key] = Default-Tree $key; continue }
        $chg = HGet $t 'chg'
        $rem = HGet $t 'rem'
        $h.wt[$key] = @{
            role  = HGet $t 'role'
            br    = HGet $t 'br'
            head  = HGet $t 'head'
            subj  = HGet $t 'subj'
            at    = HGet $t 'at'
            dirty = [bool](HGet $t 'dirty')
            chg   = @{
                s = To-StrList (HGet $chg 's')
                u = To-StrList (HGet $chg 'u')
                n = To-StrList (HGet $chg 'n')
            }
            rem  = @{
                up = HGet $rem 'up'
                ah = [int](HGet $rem 'ah')
                bh = [int](HGet $rem 'bh')
            }
            prog = Normalize-Prog (HGet $t 'prog')
        }
    }
    $h
}

function Scan-Tree([string] $Path, [hashtable] $Prev) {
    $t = if ($Prev) { $Prev } else { Default-Tree 'x' }
    if (-not $Path -or -not (Test-Path $Path)) {
        $t.head = $null; $t.dirty = $false
        $t.chg = @{ s = @(); u = @(); n = @() }
        return $t
    }
    $h = Get-Head $Path
    $c = Get-Porcelain $Path
    $r = Get-Remote $Path
    $t.br = $h.br; $t.head = $h.head; $t.subj = $h.subj; $t.at = $h.at
    $t.dirty = $c.dirty
    $t.chg = @{ s = [string[]]@($c.s); u = [string[]]@($c.u); n = [string[]]@($c.n) }
    $t.rem = $r
    $t.prog = Normalize-Prog $t.prog
    $t
}

function Action-Init {
    Ensure-Dir
    if (-not (Test-Path $PathsPath)) {
        $ex = Join-Path $WtDir 'paths.local.json.example'
        if (Test-Path $ex) { Copy-Item $ex $PathsPath }
    }
    if (-not (Test-Path $StatePath)) { Write-State (Default-State) }
    Action-Scan -Quiet
}

function Action-Scan([switch] $Quiet) {
    $paths = Get-Paths
    $st = State-ToHashtable (Read-State)
    if (-not $st) { $st = Default-State }
    if (-not $st.repo) { $st.repo = 'LearnURP' }
    foreach ($k in @('main', 'agent')) {
        $prev = $st.wt[$k]
        $st.wt[$k] = Scan-Tree $paths[$k] $prev
        $st.wt[$k].role = if ($k -eq 'main') { 'user' } else { 'agent' }
    }
    Write-State $st
    if (-not $Quiet) { Write-Output $StatePath }
}

function Action-Get {
    if (-not (Test-Path $StatePath)) { Action-Init; Action-Scan -Quiet }
    Get-Content $StatePath -Raw -Encoding UTF8
}

function Action-SetProg {
    if (-not $Role) { throw '-Role main|agent required' }
    $st = State-ToHashtable (Read-State)
    if (-not $st) { $st = Default-State }
    $p = $st.wt[$Role].prog
    if ($Task) { $p.task = $Task }
    if ($Done) { $p.done = To-StrList $Done }
    if ($Wip)  { $p.wip  = To-StrList $Wip }
    if ($Todo) { $p.todo = To-StrList $Todo }
    if ($Blk)  { $p.blk  = To-StrList $Blk }
    $st.wt[$Role].prog = $p
    $paths = Get-Paths
    $st.wt[$Role] = Scan-Tree $paths[$Role] $st.wt[$Role]
    Write-State $st
}

function Action-Load {
    if (-not $InFile) { throw '-InFile required' }
    $patch = (Read-Raw $InFile) | ConvertFrom-Json
    $st = State-ToHashtable (Read-State)
    if (-not $st) { $st = Default-State }
    if ((HGet $patch 'v') -and (HGet $patch 'v') -ne 1) { throw "unsupported schema v=$(HGet $patch 'v')" }
    $wtPatch = HGet $patch 'wt'
    if ($wtPatch) {
        foreach ($k in @('main', 'agent')) {
            $src = HGet $wtPatch $k
            if (-not $src) { continue }
            if (-not $st.wt.ContainsKey($k)) { $st.wt[$k] = Default-Tree $k }
            if (HGet $src 'prog') { $st.wt[$k].prog = Normalize-Prog (HGet $src 'prog') }
            if ($null -ne (HGet $src 'role')) { $st.wt[$k].role = HGet $src 'role' }
        }
    }
    Write-State $st
}

function Action-Show {
    if (-not (Test-Path $StatePath)) { Action-Init }
    $s = State-ToHashtable (Read-State)
    $p = Get-Paths
    foreach ($k in @('main', 'agent')) {
        $t = $s.wt[$k]
        $path = $p[$k]
        Write-Host "[$k] $path"
        Write-Host "  $($t.br) @$($t.head) $($t.subj) | dirty=$($t.dirty) | rem $($t.rem.up) +$($t.rem.ah)/-$($t.rem.bh)"
        if ($t.dirty) {
            Write-Host "  chg s=$($t.chg.s -join ',') u=$($t.chg.u -join ',') n=$($t.chg.n -join ',')"
        }
        $g = $t.prog
        Write-Host "  prog task=$($g.task) wip=$($g.wip -join ',') todo=$($g.todo -join ',') done=$($g.done -join ',') blk=$($g.blk -join ',')"
        Write-Host ""
    }
}

function Action-AddAgent {
    $t = Join-Path (Split-Path $RepoRoot -Parent) 'LearnURP-cursor'
    if (-not (Test-Path $t)) { Invoke-Git $RepoRoot @('worktree', 'add', '-f', $t, 'HandManage') }
    Get-Paths | Out-Null
    Action-Scan -Quiet
}

function Action-RemoveAgent {
    $paths = Get-Paths
    if ($paths.agent) { Invoke-Git $RepoRoot @('worktree', 'remove', '--force', $paths.agent) }
    [System.IO.File]::WriteAllText($PathsPath, (@{ main = $paths.main; agent = $null } | ConvertTo-Json -Compress) + "`n")
    Action-Scan -Quiet
}

Push-Location $RepoRoot
try {
    switch ($Action) {
        'init'         { Action-Init }
        'scan'         { Action-Scan }
        'show'         { Action-Show }
        'get'          { Action-Get }
        'set-prog'     { Action-SetProg }
        'load'         { Action-Load }
        'add-agent'    { Action-AddAgent }
        'remove-agent' { Action-RemoveAgent }
        default        { Action-Get }
    }
}
finally { Pop-Location }
