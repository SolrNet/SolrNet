let pkgs = (import (fetchTarball "https://github.com/NixOS/nixpkgs/archive/e3583ad6e533a9d8dd78f90bfa93812d390ea187.tar.gz") {});
in

pkgs.stdenv.mkDerivation {

  name = "solrnet";

  buildInputs = [
    pkgs.dotnet-sdk_6
    pkgs.dpkg
    pkgs.jq
    pkgs.curl
    pkgs.findutils
    pkgs.ripgrep
    pkgs.bashInteractive
    (import (fetchTarball "https://github.com/NixOS/nixpkgs/archive/bed08131cd29a85f19716d9351940bdc34834492.tar.gz") {}).docker-compose
  ];
}
