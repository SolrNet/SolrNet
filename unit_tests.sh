#!/usr/bin/env nix-shell
#! nix-shell -i sh
dotnet test --filter 'Category!~Integration' --logger html
