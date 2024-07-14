{
  inputs = {
    nixpkgs.url = "github:NixOS/nixpkgs/nixpkgs-unstable";
    systems.url = "github:nix-systems/default";
    devenv.url = "github:cachix/devenv";
  };

  nixConfig = {
    extra-trusted-public-keys = "devenv.cachix.org-1:w1cLUi8dv3hnoSPGAuibQv+f9TZLr6cv/Hm9XgU50cw=";
    extra-substituters = "https://devenv.cachix.org";
  };

  outputs = { self, nixpkgs, devenv, systems, ... } @ inputs:
    let
      forEachSystem = nixpkgs.lib.genAttrs (import systems);
      packagesf = pkgs: with pkgs; [
        dotnet-sdk_6
        dpkg
        jq
        curl
        findutils
        ripgrep
        bashInteractive
      ];
    in
    {
      packages = forEachSystem (
        system:
          let
            pkgs = nixpkgs.legacyPackages.${system};
          in
          {
            default = pkgs.symlinkJoin {
              name = "solrnet";
              paths = packagesf pkgs;
            };
          }
      );
      devShells = forEachSystem
        (system:
          let
            pkgs = nixpkgs.legacyPackages.${system};
          in
          {
            default = devenv.lib.mkShell {
              inherit inputs pkgs;
              modules = [
                {
                  devcontainer.enable = true;
                  # https://devenv.sh/reference/options/
                  packages = packagesf pkgs ++ [
                    (pkgs.vscode-with-extensions.override {
                        vscode = pkgs.vscodium;
                        vscodeExtensions = with pkgs.vscode-extensions; [
                          ms-dotnettools.csharp
                          jnoortheen.nix-ide
                        ];
                    })
                  ];

                  enterShell = ''
                    codium .
                  '';
                }
              ];
            };
          });
    };
}
