#!/usr/bin/env bash

run_tests() {
  local stop="$1"
  local output="$2"

  echo -e "\n\rRunning integration tests..."
  nix run -f https://github.com/NixOS/nixpkgs/archive/3b6c3bee9174dfe56fd0e586449457467abe7116.tar.gz \
    dotnet-sdk_5 -c dotnet test SolrNet.Tests.Integration --filter 'Category=Integration' 1>$output 2>$output
  ret=$?

  if [ -n "$stop" ]; then
    echo -e "\n\rStopping Solr..."
    docker stop solr_cloud
  fi
  return $ret
}

create_solr() {
  local next="$1"

  echo -e "\n\rWaiting for Solr to start..."
  until docker container inspect solr_cloud 1>/dev/null 2>/dev/null; do
    sleep 0.5
  done
  until curl -s http://localhost:8983 1>/dev/null 2>/dev/null; do
    sleep 0.5
  done

  echo -e "\n\rSetting up Solr collection and documents..."
  docker exec solr_cloud solr create_collection -c techproducts -d sample_techproducts_configs 1>/dev/null 2>/dev/null
  docker exec solr_cloud post -c techproducts 'example/exampledocs/' 1>/dev/null 2>/dev/null

  set -x
  $next
}

output=$(mktemp)
trap "rm $output" EXIT

create_solr "run_tests stop $output" &
tests=$!

docker run --rm -it -p 8983:8983 --name solr_cloud solr:8.8.2 solr start -cloud -f >/dev/null
cat $output
wait $tests
