#!/usr/bin/env bash

run_tests() {
  local output="$1"

  echo -e "\n\rWaiting for Solr to start..."
  until docker container inspect solr 1>/dev/null 2>/dev/null; do
    echo try 1
    sleep 0.5
  done
  until curl -s http://localhost:8983 1>/dev/null 2>/dev/null; do
    echo try 2
    sleep 0.5
  done

  echo -e "\n\rRunning integration tests..."
  nix run -f https://github.com/NixOS/nixpkgs/archive/3b6c3bee9174dfe56fd0e586449457467abe7116.tar.gz dotnet-sdk_5 -c \
    dotnet test SolrNet.Cloud.Tests --filter 'Category=Integration' 1>$output 2>$output
  ret=$?

  echo -e "\n\rStopping Solr..."
  docker-compose -f cloud-tests-compose.yml down

  return $ret

}

output=$(mktemp)
trap "rm $output" EXIT

run_tests "$output" &
tests=$!

docker-compose -f cloud-tests-compose.yml up --abort-on-container-exit --force-recreate --remove-orphans --renew-anon-volumes

cat $output
wait $tests