@echo off
setlocal EnableDelayedExpansion

:: Move to the root of the Git repository
for /f "delims=" %%i in ('git rev-parse --show-toplevel 2^>nul') do cd /d "%%i"

:: Confirm we're in a Git repository
git rev-parse --is-inside-work-tree >nul 2>&1
if errorlevel 1 (
    echo ERROR: This is not a Git repository.
    pause
    exit /b
)

:: Ask for source branch
set /p source_branch=Enter the source branch to deploy from (e.g., main):

:: Check if source branch exists
git show-ref --verify --quiet refs/heads/%source_branch%
if errorlevel 1 (
    echo ERROR: Source branch "%source_branch%" does not exist.
    pause
    exit /b
)

:: Ask for target branch (default: deploy)
set /p target_branch=Enter the target branch (default is deploy):
if "%target_branch%"=="" set target_branch=deploy

echo.
echo Deploying from: %source_branch%
echo Deploying to:   %target_branch% (as a single commit)
pause

:: Delete local target branch if it exists (to avoid stale/conflicting history)
git show-ref --verify --quiet refs/heads/%target_branch%
if not errorlevel 1 (
    git branch -D %target_branch%
)

:: Fetch latest remote branches
git fetch origin

:: Create a new tree object from the source branch
for /f %%t in ('git rev-parse "refs/heads/%source_branch%^{tree}"') do set tree=%%t

:: Create a new commit from that tree
set commit_message=Deploy: snapshot from %source_branch%
for /f %%c in ('echo %commit_message% ^| git commit-tree !tree! -m "!commit_message!"') do set commit=%%c

:: Force-push the commit to the remote target branch
git push -f origin !commit!:refs/heads/%target_branch%

echo.
echo Deployment from %source_branch% to %target_branch% complete!
pause
