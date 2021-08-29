let dotnet_pkg = (import (fetchTarball "https://github.com/NixOS/nixpkgs/archive/bfe415121972683c4b78666679198c5336fa6095.tar.gz") {});
in
[
  dotnet_pkg.dotnet-sdk_5
  dotnet_pkg.dpkg
  (import (fetchTarball "https://github.com/NixOS/nixpkgs/archive/bed08131cd29a85f19716d9351940bdc34834492.tar.gz") {}).docker-compose
]