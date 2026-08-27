# Branch protection

`Extended` is the branch this fork develops on and releases from, so nothing should
land on it except through a pull request whose CI run is green.

The two rulesets in [`rulesets/`](rulesets) describe exactly that. They are plain JSON
in the format GitHub's ruleset importer and REST API accept, kept in the repository so
the configuration is reviewable and can be re-applied if it is ever changed by hand.

> These files are **not** applied automatically. GitHub reads rulesets from the
> repository settings, not from the working tree, so somebody with admin rights has to
> import them once (see below). After that, changes to these files are documentation
> only — re-import to apply them.

## What the rulesets do

### `extended-branch-protection.json` — Protect Extended

| Rule | Effect |
| --- | --- |
| `pull_request` | Direct pushes to `Extended` are rejected; changes must arrive as a pull request. Review threads must be resolved, and approvals are dismissed when new commits are pushed. |
| `required_status_checks` | The **Build and test** check from [`workflows/ci.yml`](workflows/ci.yml) must pass, and the branch must be up to date with `Extended` before merging. |
| `non_fast_forward` | No force pushes. |
| `deletion` | The branch cannot be deleted. |

`required_approving_review_count` is set to `0`. That still forces every change through a
pull request with green CI, but it does not deadlock a single maintainer, who cannot
approve their own pull request. Raise it to `1` as soon as there is a second maintainer:

```jsonc
"required_approving_review_count": 1,
"require_last_push_approval": true
```

### `master-lockdown.json` — Lock master

Upstream keeps a `Master` branch that tracks the official Xceed releases. This fork does
not use it, and nothing here should ever write to it. The ruleset blocks `creation`,
`update`, `deletion` and force pushes on `refs/heads/master` and `refs/heads/Master`, so
the branch cannot be created, moved or pushed to at all — including by administrators,
since `bypass_actors` is empty.

If the branch is ever wanted back (to mirror upstream, say), delete this ruleset first,
rather than adding a bypass.

## Applying them

### With the GitHub CLI

```bash
gh api --method POST /repos/shodiwarmic/WpfExtendedToolkit/rulesets \
  --input .github/rulesets/extended-branch-protection.json

gh api --method POST /repos/shodiwarmic/WpfExtendedToolkit/rulesets \
  --input .github/rulesets/master-lockdown.json
```

To update a ruleset that already exists, find its id with
`gh api /repos/shodiwarmic/WpfExtendedToolkit/rulesets` and repeat the call with
`--method PUT` against `/repos/.../rulesets/<id>`.

### In the web UI

**Settings → Rules → Rulesets → New ruleset → Import a ruleset**, then upload the JSON
file. Repeat for the second file.

## Notes and prerequisites

- The **Build and test** context only appears in the status-check picker after the CI
  workflow has run at least once on the repository. Merge the workflow to `Extended`
  first, then import the rulesets.
- If the job in `ci.yml` is ever renamed, the `context` in
  `extended-branch-protection.json` has to be renamed to match, otherwise the required
  check will never be reported and no pull request will be mergeable.
- Rulesets are available on all public repositories. On a **private** repository they
  need GitHub Pro, Team or Enterprise; on a private repository on the Free plan, use the
  classic **Settings → Branches → Add branch protection rule** screen instead and tick
  "Require a pull request before merging", "Require status checks to pass" (selecting
  **Build and test**), "Require branches to be up to date" and leave "Allow force
  pushes" and "Allow deletions" off.
- Rulesets do not apply to pull requests opened from forks until they are merged, which
  is the expected behaviour: the CI workflow still runs on the pull request.
