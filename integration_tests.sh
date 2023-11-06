#!/usr/bin/env -S nix --extra-experimental-features "nix-command flakes" --accept-flake-config shell .# -c sh

dotnet test --filter 'Category=Integration&FullyQualifiedName!~Cloud' --logger html
