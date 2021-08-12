#!/usr/bin/env sh
nix run -f https://github.com/NixOS/nixpkgs/archive/3b6c3bee9174dfe56fd0e586449457467abe7116.tar.gz dotnet-sdk_5 -c \
  dotnet test --filter 'Category!~Integration'
