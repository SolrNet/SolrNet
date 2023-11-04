#!/usr/bin/env -S nix --extra-experimental-features "nix-command flakes" shell .# -c sh

set -eu

function github {
  local path=$1
  shift
  curl -s -u mausch:$GITHUB_TOKEN -H 'Accept: application/vnd.github.v3+json' https://api.github.com${path} $@
}

packages="$(github /orgs/SolrNet/packages?package_type=nuget | jq -r '.[].name')"

for p in $packages; do
  echo Deleting $p
  github /orgs/SolrNet/packages/nuget/$p -XDELETE
done
