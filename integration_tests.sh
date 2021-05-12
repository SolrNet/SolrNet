#!/usr/bin/env sh

alias docker-compose="nix run -f https://github.com/NixOS/nixpkgs/archive/bed08131cd29a85f19716d9351940bdc34834492.tar.gz docker-compose -c docker-compose "

docker-compose --version

docker-compose -f integration-tests-compose.yml up --build --exit-code-from integration_tests --abort-on-container-exit --force-recreate --no-color --renew-anon-volumes

