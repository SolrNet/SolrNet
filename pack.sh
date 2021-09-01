#!/usr/bin/env -S nix run -c sh

set -e
dotnet tool restore
version_js=$(dotnet tool run nbgv get-version --format json)
export Version=$(echo $version_js | jq -r '.AssemblyInformationalVersion')
export PackageVersion=$(echo $version_js | jq -r '.NuGetPackageVersion')
dotnet pack
if [ -n "$GITHUB_TOKEN" ]; then
  dotnet nuget add source --username mausch --password "$GITHUB_TOKEN" --store-password-in-clear-text --name github "https://nuget.pkg.github.com/SolrNet/index.json"
  for nupkg in "$(find | rg nupkg)"; do
    dotnet nuget push --source "github" $nupkg &
  done
  wait
  echo "::set-env name=VersionTag::$Version" # for github actions
else
  echo "GITHUB_TOKEN not defined, won't push packages"
fi
