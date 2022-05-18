#!/usr/bin/env nix-shell
#! nix-shell -i sh

export SOLR_VERSION=${SOLR_VERSION:-8.8.2}

run_tests() {
  local stop="$1"
  local output="$2"

  echo -e "\n\rRunning integration tests..."
  dotnet test --filter 'Category=Integration&FullyQualifiedName!~Cloud' --logger html 1>$output 2>$output
  ret=$?

  if [ -n "$stop" ]; then
    echo -e "\n\rStopping Solr..."
	docker stop solr_cloud
	docker stop solr_cloud_auth
  fi
  return $ret
}

setupSolrCore() {
  local coreName="$1"
  local dockerContainerName="$2"
  local solrPort="$3"
  
  docker exec $dockerContainerName solr create_collection -c $coreName -d sample_techproducts_configs 1>/dev/null 2>/dev/null
  docker exec $dockerContainerName post -c $coreName 'example/exampledocs/' 1>/dev/null 2>/dev/null

  curl -s -X POST -H 'Content-type:application/json' -d '{
	"update-requesthandler": {
	  "name": "/select",
	  "class": "solr.SearchHandler",
	  "last-components": ["spellcheck"]
	}
  }' http://localhost:$solrPort/solr/$coreName/config >/dev/null
}

create_solr() {
  echo -e "\n\rWaiting for Solr to start..."
  until docker container inspect solr_cloud 1>/dev/null 2>/dev/null; do
    sleep 0.5
  done
  until curl -s http://localhost:8983 1>/dev/null 2>/dev/null; do
    sleep 0.5
  done

  echo -e "\n\rSetting up Solr collection and documents..."
  
  setupSolrCore techproducts solr_cloud 8983
  setupSolrCore core0 solr_cloud 8983
  setupSolrCore core1 solr_cloud 8983
  setupSolrCore entity1 solr_cloud 8983
  setupSolrCore entity2 solr_cloud 8983
  
  echo -e "\n\rSolr available at http://localhost:8983\n\r"

  set -x
}

create_solr_auth() {
  local next="$1"

  echo -e "\n\rWaiting for Solr_BasicAuth to start..."
  until docker container inspect solr_cloud_auth 1>/dev/null 2>/dev/null; do
    sleep 0.5
  done
  until curl -s http://localhost:8984 1>/dev/null 2>/dev/null; do
    sleep 0.5
  done
  
  echo -e "\n\rPreparing Solr_BasicAuth auth..."
  
  # output default (official Solr documentation) security.json to working directory
	echo '{
		"authentication":{ 
		"blockUnknown": true, 
		"class":"solr.BasicAuthPlugin",
		"credentials":{"solr":"IV0EHq1OnNrj6gvRCwvFwTrZ1+z1oBbnQdiVC3otuq0= Ndd7LKvVBAaZIF0QAVi1ekCfAJXr1GGfLtRUXhgrF8c="}, 
		"realm":"My Solr users", 
		"forwardCredentials": false 
		},
		"authorization":{
		"class":"solr.RuleBasedAuthorizationPlugin",
		"permissions":[{"name":"security-edit",
			"role":"admin"}], 
		"user-role":{"solr":"admin"} 
		}
	}' > security.json
		
  # apply security.json to solr server
  authcontainerId=$(docker inspect -f '{{.Id}}' solr_cloud_auth)
  docker cp security.json $authcontainerId:/security.json

  echo -e "\n\rSetting up Solr_BasicAuth collection and documents..."
  
  setupSolrCore techproducts solr_cloud_auth 8984
  
  # enable basic auth after setup of collections has completed
  echo -e "\n\rSetting up Zookeeper in Solr_BasicAuth..." 
  if [ "${SOLR_VERSION}" = "5.5.5" ]; then
	docker exec solr_cloud_auth server/scripts/cloud-scripts/zkcli.sh -zkhost localhost:9983 -cmd putfile /security.json /security.json
  else
	# docker 6+
	docker exec solr_cloud_auth bin/solr zk cp file:/security.json zk:/security.json -z localhost:9983
  fi
  
  echo -e "\n\rSolr_BasicAuth available at http://localhost:8984\n\r"

  set -x
}

output=$(mktemp)
trap "rm $output" EXIT

# start docker run jobs in background
docker run --rm -p 8983:8983 --name solr_cloud solr:$SOLR_VERSION solr start -cloud -f >solr_output.txt &
docker run --rm -p 8984:8983 --name solr_cloud_auth solr:$SOLR_VERSION solr start -cloud -f >solr_output_auth.txt  &

for i in create_solr create_solr_auth; do
	"$i" & pids+=($!)
done
wait "${pids[@]}"

run_tests stop $output
testRunner=$?

cat $output

exit $testRunner