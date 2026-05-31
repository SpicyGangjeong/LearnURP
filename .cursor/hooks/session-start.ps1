# sessionStart: refresh worktrees/state.json and inject coordinator boot context.
$ErrorActionPreference = 'SilentlyContinue'
$null = [Console]::In.ReadToEnd()

$repo = (Resolve-Path (Join-Path $PSScriptRoot '../..')).Path
$coord = Join-Path $repo 'scripts/worktree-coordinator.ps1'
$statePath = Join-Path $repo 'worktrees/state.json'
$pathsPath = Join-Path $repo 'worktrees/paths.local.json'

if (Test-Path $coord) {
    & powershell -NoProfile -ExecutionPolicy Bypass -File $coord scan 2>&1 | Out-Null
}

$agentPath = Join-Path (Split-Path $repo -Parent) 'LearnURP-cursor'
if (Test-Path $pathsPath) {
    try {
        $p = Get-Content $pathsPath -Raw | ConvertFrom-Json
        if ($p.agent) { $agentPath = $p.agent }
    } catch {}
}

$state = ''
if (Test-Path $statePath) {
    $state = [System.IO.File]::ReadAllText($statePath).Trim()
}

$ctx = @"
Worktree coordinator is active for this session.

Required (do not ask the user to repeat this):
- Read worktrees/state.json first (refreshed by sessionStart hook).
- Run .\scripts\worktree-coordinator.ps1 scan if state may be stale after user edits.
- Code changes and git commits only under: $agentPath
- Main user folder (do not edit for implementation): $repo
- Shared branch: HandManage

state.json:
$state
"@

@{ additional_context = $ctx } | ConvertTo-Json -Compress
