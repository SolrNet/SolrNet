#!/usr/bin/env -S nix run -c sh

export SOLR_VERSION=${SOLR_VERSION:-8.8.2}

run_tests() {
  local stop="$1"
  local output="$2"

  echo -e "\n\rRunning integration tests..."
  dotnet test SolrNet.Tests.Integration --filter 'Category=Integration' 1>$output 2>$output
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

  curl -s -X POST -H 'Content-type:application/json' -d '{
    "update-requesthandler": {
      "name": "/select",
      "class": "solr.SearchHandler",
      "last-components": ["spellcheck"]
    }
  }' http://localhost:8983/solr/techproducts/config >/dev/null
  
  echo -e "\n\rSolr available at http://localhost:8983\n\r"

  set -x
  $next
}

output=$(mktemp)
trap "rm $output" EXIT

create_solr "run_tests stop $output" &
# create_solr "true" &
tests=$!

docker run --rm -p 8983:8983 --name solr_cloud solr:$SOLR_VERSION solr start -cloud -f >solr_output.txt
cat $output
wait $tests
