let pkgs = (import (fetchTarball "https://github.com/NixOS/nixpkgs/archive/bfe415121972683c4b78666679198c5336fa6095.tar.gz") {});
in

pkgs.stdenv.mkDerivation {

  name = "solrnet";

  buildInputs = [
    pkgs.dotnet-sdk_5
    pkgs.dpkg
    pkgs.jq
    pkgs.curl
    pkgs.findutils
    pkgs.ripgrep
    pkgs.bashInteractive
    (import (fetchTarball "https://github.com/NixOS/nixpkgs/archive/bed08131cd29a85f19716d9351940bdc34834492.tar.gz") {}).docker-compose
  ];
}
