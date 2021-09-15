#!/usr/bin/env -S nix run -c sh
dotnet test --filter 'Category!~Integration'
