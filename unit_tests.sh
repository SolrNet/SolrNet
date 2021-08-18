#!/usr/bin/env sh
nix run -c dotnet test --filter 'Category!~Integration'
