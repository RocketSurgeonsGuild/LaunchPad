#-------------------------------------------------------------------------------------------------------------
# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License. See https://go.microsoft.com/fwlink/?linkid=2090316 for license information.
#-------------------------------------------------------------------------------------------------------------

# To fully customize the contents of this image, use the following Dockerfile instead:
# https://github.com/microsoft/vscode-dev-containers/tree/v0.128.0/containers/codespaces-linux/.devcontainer/Dockerfile
FROM mcr.microsoft.com/vscode/devcontainers/universal:0-linux

# ** [Optional] Uncomment this section to install additional packages. **
#
RUN apt-get update && \
        export DEBIAN_FRONTEND=noninteractive && \
        apt-get -y install --no-install-recommends apt-transport-https dotnet-sdk-3.1 aspnetcore-runtime-2.1

# RUN wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb &&
#         dpkg -i packages-microsoft-prod.deb &&
#         apt-get update &&
#         apt-get install -y apt-transport-https &&
#         apt-get install -y dotnet-sdk-3.1 &&
#         apt-get install -y aspnetcore-runtime-2.1
