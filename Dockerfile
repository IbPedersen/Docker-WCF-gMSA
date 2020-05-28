ARG WIN_IMAGE
ARG WIN_VERSION
# escape=`

FROM mcr.microsoft.com/$WIN_IMAGE:$WIN_VERSION

COPY . c:/
