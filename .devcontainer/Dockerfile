#-------------------------------------------------------------------------------------------------------------
# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License. See https://go.microsoft.com/fwlink/?linkid=2090316 for license information.
#-------------------------------------------------------------------------------------------------------------

# To fully customize the contents of this image, use the following Dockerfile instead:
# https://github.com/microsoft/vscode-dev-containers/tree/v0.128.0/containers/codespaces-linux/.devcontainer/Dockerfile
FROM mcr.microsoft.com/vscode/devcontainers/universal:2-linux@sha256:a370bffa6556ae4727f6a46d6d8690478398d1238f97021e9618eaf1738f7434

RUN curl -fsSL https://dotnet.microsoft.com/download/dotnet-core/scripts/v1/dotnet-install.sh | sudo bash -s -- --channel Current --install-dir /home/codespace/.dotnet/
