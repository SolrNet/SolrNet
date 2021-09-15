#!/usr/bin/env -S nix run -c sh

export SOLR_VERSION=${SOLR_VERSION:-8.8.2}

if dpkg --compare-versions "$SOLR_VERSION" lt 7; then
  echo "Cloud tests not set up yet for Solr $SOLR_VERSION"
  exit 0
fi

run_tests() {
  local output="$1"

  echo -e "\n\rWaiting for Solr to start..."
  until docker container inspect solr 1>/dev/null 2>/dev/null; do
    sleep 0.5
  done
  until curl -s http://localhost:8983 1>/dev/null 2>/dev/null; do
    sleep 0.5
  done

  echo -e "\n\rRunning integration tests..."
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