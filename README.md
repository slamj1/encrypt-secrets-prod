# encrypt-secrets-prod
A working POC for encrypting secrets ion any environment, including production

# Introduction 
This repo contains a fully working proof of concept for encrypting secrets in any environment, especially production.

A full instructional video is available on YouTube here <insert link>

# Getting Started
Prerequisites:

- Visual Studio 2022 (any edition)
- Microsoft SlowCheetah extension 
- .NET 6 and .NET 7 SDK installed locally and on the remote Linux box
- Systemd version 247 or higher on the remote Linux box. CentOS/Stream/Alma/Rocky 8.x and similar flavors won't have this version. 
  You'll need to install the 9.x series.
- PowerShell (cross platform) 7.3.x or higher on both Windows and the remote Linux box
- SSH fully setup on the remote Linux box


# Build and Test
1. Clone this repo.
2. Adjust the path and target server settings in the provided script to match your environment.
3. Follow the instructions in the video!

# Starter Linux service file from the video
```
[Unit]
Description=Poc3
After=network.target

[Service]
WorkingDirectory=/poc3
ExecStart=/usr/local/bin/dotnet /poc3/poc3.dll  # this may need to change to /bin/dotnet in your environment
Restart=no
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=poc3
User=poc3
Environment=ASPNETCORE_ENVIRONMENT=DEVLinux
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
```

# Commands used in the video

Installing PowerShell on Windows from cli:

winget install --id Microsoft.Powershell --source winget

Install PowerShell on Linux:

dnf install https://github.com/PowerShell/PowerShell/releases/download/v7.3.3/powershell-7.3.3-1.rh.x86_64.rpm

Add Linux service account:

useradd -s /sbin/nologin poc3

Encrypt contents of file using system-creds:

systemd-creds encrypt -p --name=poc3 tmppass-poc3.txt –

SeLinux commands for adding the custom module:

Setenforce 0
mkdir /sc-selinux && cd /sc-selinux
audit2allow -a -M sc-systemd-creds
semodule -i sc-systemd-creds.pp


# Useful info links

Certificate manager and AES/RSA encryption:

https://github.com/damienbod/AspNetCoreCertificates - Damien Bowden (Microsoft MVP)
https://damienbod.com/2020/08/19/symmetric-and-asymmetric-encryption-in-net-core/

systemd-creds:
https://www.freedesktop.org/software/systemd/man/systemd-creds.html
https://systemd.io/CREDENTIALS/




# Contribute
TODO

