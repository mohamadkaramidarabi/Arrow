# Publishing Arrow.Core to NuGet

This repository publishes **Arrow.Core** using [NuGet trusted publishing](https://learn.microsoft.com/en-us/nuget/nuget-org/trusted-publishing) (OIDC). No long-lived NuGet API keys are stored in GitHub.

## One-time setup on nuget.org

1. Sign in to [nuget.org](https://www.nuget.org/).
2. Open **Account** → **Trusted publishers** → **Add trusted publisher**.
3. Choose **GitHub Actions** and enter:

   | Field | Value |
   |---|---|
   | Repository owner | `mohamadkaramidarabi` |
   | Repository name | `Arrow` |
   | Workflow file | `publish-nuget.yml` |
   | Environment | *(leave empty unless you use GitHub Environments)* |

4. Save the policy. It applies to packages owned by your nuget.org account (or organization).

## One-time setup on GitHub

Add a repository secret:

| Secret | Value |
|---|---|
| `NUGET_USERNAME` | Your **nuget.org profile name** (not your email) |

No `NUGET_API_KEY` secret is required.

## How to publish

### Recommended: version tag

Create and push a semver tag. The workflow strips the leading `v`:

```bash
git tag v0.1.0
git push origin v0.1.0
```

This runs [`.github/workflows/publish-nuget.yml`](../.github/workflows/publish-nuget.yml), which:

1. Runs tests
2. Packs `Arrow.Core` with the tag version
3. Exchanges a GitHub OIDC token for a short-lived NuGet API key (`NuGet/login@v1`)
4. Pushes `.nupkg` and `.snupkg` to nuget.org

### Manual run

In GitHub: **Actions** → **Publish NuGet** → **Run workflow**, and enter a version.

## CI

Every push/PR to `master` or `main` runs [`.github/workflows/ci.yml`](../.github/workflows/ci.yml) (build + test only).

## Local pack (dry run)

```bash
dotnet pack src/core/Arrow.Core/Arrow.Core.csproj -c Release -o ./artifacts
```

## Troubleshooting

- **Trusted publishing not available** — NuGet rolls this out gradually; check your account settings.
- **Policy mismatch** — The workflow filename on nuget.org must match exactly: `publish-nuget.yml`.
- **Private repository** — Policies may be temporarily active for 7 days until the first successful publish provides GitHub repo IDs.
- **Package ID taken** — Change `PackageId` in `Arrow.Core.csproj` if `Arrow.Core` is unavailable on nuget.org.
- **409 on main package + 404 on symbols** — A previous upload of that version exists in NuGet but **failed validation** and never went live. Check **Manage Packages** on nuget.org for the error, then publish a **new version** (e.g. `1.0.1`). Re-pushing the same version will always skip the main package and fail symbols.
- **Symbols 404 after main push** — The workflow waits until the main package is live on nuget.org before pushing symbols. If the job fails at the wait step, fix validation errors on nuget.org first.
