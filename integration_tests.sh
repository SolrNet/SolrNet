#!/usr/bin/env sh

docker-compose -f integration-tests-compose.yml up --build --exit-code-from integration_tests --abort-on-container-exit --force-recreate

