#!/usr/bin/env sh
docker run --rm -i \
  -v $PWD:/work \
  -v $HOME/.nuget:/root/.nuget \
  -w /work \
  mcr.microsoft.com/dotnet/sdk:5.0-alpine \
  dotnet test --filter 'Category!=Integration'
