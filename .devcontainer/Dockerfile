#-------------------------------------------------------------------------------------------------------------
# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License. See https://go.microsoft.com/fwlink/?linkid=2090316 for license information.
#-------------------------------------------------------------------------------------------------------------

# To fully customize the contents of this image, use the following Dockerfile instead:
# https://github.com/microsoft/vscode-dev-containers/tree/v0.128.0/containers/codespaces-linux/.devcontainer/Dockerfile
FROM mcr.microsoft.com/vscode/devcontainers/universal:2-linux@sha256:468fdde10b4dcd013ef25228f50631ee9c4488dfcdeeee3777051efd1137ecb4

RUN curl -fsSL https://dotnet.microsoft.com/download/dotnet-core/scripts/v1/dotnet-install.sh | sudo bash -s -- --channel Current --install-dir /home/codespace/.dotnet/
