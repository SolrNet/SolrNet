#!/usr/bin/env -S nix --extra-experimental-features "nix-command flakes" shell .# -c sh

dotnet test --filter 'Category=Integration&FullyQualifiedName!~Cloud' --logger html
