# GitHub Actions Setup Guide

This document provides instructions for setting up the required secrets and configurations for the CI/CD workflows.

## Required Secrets

The following secrets need to be configured in your GitHub repository settings (`Settings > Secrets and variables > Actions`):

### 1. CODECOV_TOKEN

**Required for:** Code coverage reporting

**How to obtain:**
1. Sign up at [codecov.io](https://codecov.io)
2. Add your repository to Codecov
3. Copy the repository upload token
4. Add it as `CODECOV_TOKEN` in GitHub repository secrets

### 2. SONAR_TOKEN

**Required for:** SonarCloud code quality analysis

**How to obtain:**
1. Sign up at [sonarcloud.io](https://sonarcloud.io)
2. Create a new organization or use existing
3. Import your repository
4. Go to `Administration > Analysis Method > GitHub Actions`
5. Copy the provided token
6. Add it as `SONAR_TOKEN` in GitHub repository secrets

**Additional SonarCloud Configuration:**
- Update the organization key in `ci.yml`:
  ```yaml
  /o:"your-organization-name"
  ```
- Update the project key:
  ```yaml
  /k:"your-project-key"
  ```

### 3. NUGET_API_KEY

**Required for:** Publishing packages to NuGet.org

**How to obtain:**
1. Sign up at [nuget.org](https://www.nuget.org)
2. Go to [API Keys](https://www.nuget.org/account/apikeys)
3. Create a new API key with push permissions
4. Copy the key immediately (it won't be shown again)
5. Add it as `NUGET_API_KEY` in GitHub repository secrets

**Important:** Keep this key secure and never commit it to the repository!

## Workflow Configuration

### CI Workflow (`ci.yml`)

**Triggers:**
- Push to `main` or `develop` branches
- Pull requests to `main` or `develop`
- Manual trigger via workflow_dispatch

**Jobs:**
1. **build-and-test**: Matrix build for all business areas
2. **code-coverage**: Aggregates coverage and uploads to Codecov
3. **sonarcloud**: Performs code quality analysis
4. **package**: Creates NuGet packages (only on main/develop)
5. **publish-summary**: Creates workflow summary

### Release Workflow (`release.yml`)

**Triggers:**
- GitHub release published
- Manual trigger with version input

**Purpose:** Builds and publishes NuGet packages to NuGet.org

### PR Validation Workflow (`pr-validation.yml`)

**Triggers:**
- Pull request opened/updated

**Checks:**
- Code formatting (`dotnet format`)
- Build success
- All tests pass
- Code coverage

### CodeQL Workflow (`codeql.yml`)

**Triggers:**
- Push to main/develop
- Pull requests
- Weekly schedule (Mondays)

**Purpose:** Security vulnerability scanning

## Dependabot Configuration

Dependabot is configured to:
- Check for NuGet package updates weekly
- Check for GitHub Actions updates weekly
- Create PRs with appropriate labels and conventional commit messages

## Branch Protection Rules (Recommended)

Configure the following branch protection rules for `main`:

1. **Require pull request reviews before merging**
   - Required approvals: 1

2. **Require status checks to pass before merging**
   - Required checks:
     - `Build & Test`
     - `Code Coverage`
     - `SonarCloud Analysis`
     - `CodeQL`

3. **Require branches to be up to date before merging**

4. **Include administrators**

## Status Badges

The following badges are included in the README:

- **CI Status**: Shows build status
- **Code Coverage**: Shows coverage percentage
- **Quality Gate**: Shows SonarCloud quality gate status
- **License**: Shows MIT license

## Testing the Workflows Locally

### Using act

You can test workflows locally using [act](https://github.com/nektos/act):

```bash
# Install act
brew install act  # macOS
# or
curl https://raw.githubusercontent.com/nektos/act/master/install.sh | sudo bash  # Linux

# Run CI workflow
act push

# Run specific job
act -j build-and-test
```

### Manual Testing

```bash
# Build
dotnet build libraries/cs/Camtify/Camtify.sln

# Run tests with coverage
dotnet test libraries/cs/Camtify/Camtify.sln \
  --collect:"XPlat Code Coverage" \
  --results-directory ./TestResults

# Format check
dotnet format libraries/cs/Camtify/Camtify.sln --verify-no-changes
```

## Troubleshooting

### Coverage Not Uploading

**Issue:** Coverage reports not appearing in Codecov

**Solution:**
1. Verify `CODECOV_TOKEN` is set correctly
2. Check coverage files are generated: `TestResults/**/coverage.cobertura.xml`
3. Verify Codecov upload action version is up-to-date

### SonarCloud Analysis Failing

**Issue:** SonarCloud job fails

**Solutions:**
1. Verify `SONAR_TOKEN` is valid
2. Check organization and project keys match your SonarCloud configuration
3. Ensure repository is imported in SonarCloud
4. Check SonarCloud project settings allow analysis from GitHub Actions

### NuGet Push Failing

**Issue:** Package push to NuGet.org fails

**Solutions:**
1. Verify `NUGET_API_KEY` is valid and has push permissions
2. Check package ID is not already taken
3. Ensure version number is unique (not already published)
4. Verify API key has correct glob pattern for package IDs

## Support

For issues with:
- **GitHub Actions**: Check [GitHub Actions Documentation](https://docs.github.com/en/actions)
- **Codecov**: Visit [Codecov Support](https://docs.codecov.com/)
- **SonarCloud**: Check [SonarCloud Documentation](https://docs.sonarcloud.io/)
- **NuGet**: Visit [NuGet Documentation](https://docs.microsoft.com/en-us/nuget/)

## Additional Resources

- [Conventional Commits](https://www.conventionalcommits.org/)
- [Semantic Versioning](https://semver.org/)
- [GitHub Actions Best Practices](https://docs.github.com/en/actions/learn-github-actions/best-practices-for-workflows)
