#!/usr/bin/env sh

docker-compose -f cloud-tests-compose.yml up --build --exit-code-from integration_tests --abort-on-container-exit --force-recreate --remove-orphans --renew-anon-volumes

