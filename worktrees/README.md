# worktrees/ (worktree coordinator)

| File | Git | Consumer |
|------|-----|----------|
| `state.json` | yes | agent — compact snapshot |
| `schema.json` | yes | deserialize contract |
| `paths.local.json` | no | PC paths |

```powershell
.\scripts\worktree-coordinator.ps1 scan
.\scripts\worktree-coordinator.ps1 get
.\scripts\worktree-coordinator.ps1 show
.\scripts\worktree-coordinator.ps1 set-prog -Role agent -Task hand-ui -Wip CardCanvas.cs -Todo commit
.\scripts\worktree-coordinator.ps1 load -InFile patch.json
```

New chat: `.cursor/hooks/session-start.ps1` runs `scan` and injects boot context. Rule: `.cursor/rules/worktree-coordinator.mdc`.

Agent: read `worktrees/state.json` directly; run coordinator only to refresh git fields.
