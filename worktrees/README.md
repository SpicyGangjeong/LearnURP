# worktrees/

| File | Git | Consumer |
|------|-----|----------|
| `state.json` | yes | agent — compact snapshot |
| `schema.json` | yes | deserialize contract |
| `paths.local.json` | no | PC paths |

```powershell
.\scripts\worktree.ps1 scan          # refresh git fields → state.json
.\scripts\worktree.ps1 get           # stdout: full JSON (agent)
.\scripts\worktree.ps1 show          # human glance
.\scripts\worktree.ps1 set-prog -Role agent -Task hand-ui -Wip CardCanvas.cs -Todo commit
.\scripts\worktree.ps1 load -InFile patch.json   # merge partial {"v":1,"wt":{"agent":{"prog":{...}}}}
```

Agent: read/parse `worktrees/state.json` directly; no need to run script unless refreshing git state.
