#!/usr/bin/env nix-shell
#! nix-shell -i sh

set -e
set -o pipefail

dotnet tool restore
version_js="$(dotnet tool run nbgv get-version --format json)"
export Version=$(echo $version_js | jq -r '.AssemblyInformationalVersion')
echo Tag: $Version
export PackageVersion=$(echo $version_js | jq -r '.NuGetPackageVersion')

public_tag=$(git tag --points-at HEAD)
if [ -n "$public_tag" ]; then
  export PackageVersion=$(echo $version_js | jq -r '.SimpleVersion')
  echo "Public release $public_tag"
fi

dotnet pack

if [ -n "$GITHUB_TOKEN" ]; then
  dotnet nuget add source --username mausch --password "$GITHUB_TOKEN" --store-password-in-clear-text --name github "https://nuget.pkg.github.com/SolrNet/index.json"
  for nupkg in "$(find | rg nupkg)"; do
    echo "Publishing $nupkg to github"
    dotnet nuget push --source "github" $nupkg &
  done
  wait
  if [ -n "$GITHUB_ENV" ]; then
    echo "VersionTag=$Version" >> $GITHUB_ENV  # for github actions
  fi
else
  echo "GITHUB_TOKEN not defined, won't push packages to github"
fi

if [ -n "$NUGET_API_KEY" ]; then
  for nupkg in "$(find | rg nupkg)"; do
    echo "Publishing $nupkg to nuget.org"
    dotnet nuget push $nupkg --api-key $NUGET_API_KEY --source https://api.nuget.org/v3/index.json &
  done
  wait
else
  echo "NUGET_API_KEY not defined, won't push packages to nuget.org"
fi

echo $version_js
echo Tag: $Version