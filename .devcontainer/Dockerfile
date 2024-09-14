#-------------------------------------------------------------------------------------------------------------
# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License. See https://go.microsoft.com/fwlink/?linkid=2090316 for license information.
#-------------------------------------------------------------------------------------------------------------

# To fully customize the contents of this image, use the following Dockerfile instead:
# https://github.com/microsoft/vscode-dev-containers/tree/v0.128.0/containers/codespaces-linux/.devcontainer/Dockerfile
FROM mcr.microsoft.com/vscode/devcontainers/universal:2-linux@sha256:df8fb1f3168bd34b6697c73d0c1103dc35afa6aa6775134dccfc2c7a09195fd2

RUN curl -fsSL https://dotnet.microsoft.com/download/dotnet-core/scripts/v1/dotnet-install.sh | sudo bash -s -- --channel Current --install-dir /home/codespace/.dotnet/
