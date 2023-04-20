<# 
 poc2 Deployment Script
 
 Notes: Requires Powershell core 7+. Add this .ps1 file to your Windows path
        and make sure to run it from a PS Core in Windows Terminal.
		VPN must be on.
		
		Make sure to switch to the appropriate branch to match the
		desired application.
		
 Author: Sam Morreel
 License: MIT

 *** History ***
 
 2023-02-04: s.morreel - Refactored application type processing
 2022-10-14: s.morreel - added to usage example 

 Original Create Date: 2022-09-26
 
 ***         ***
 
 Usage Example: 
	.\deploy-poc2.ps1 poc2 DEVLinux net6.0 s.morreel C:\Users\slam\.ssh\saasy_ssh_key "" "<cert password here>" ""

#>
param(
    [Parameter(Mandatory=$True, ValueFromPipeline=$true)]
    [System.String]
	[ValidateSet("poc2", "poc3")]
    $Application,
	
	[Parameter(Mandatory=$True, ValueFromPipeline=$true)]
    [System.String]
    $Configuration,

    [Parameter(Mandatory=$True, ValueFromPipeline=$true)]
    [System.String]
    $Framework,
	
	[Parameter(Mandatory=$True, ValueFromPipeline=$true)]
    [System.String]
    $SSHUser,
	
	[Parameter(Mandatory=$True, ValueFromPipeline=$true)]
    [System.String]
    $SSHKeyPath,

	[Parameter(Mandatory=$True, ValueFromPipeline=$true)]
	[AllowEmptyString()]
    [System.String]
    $PFXFilename,

	[Parameter(Mandatory=$True, ValueFromPipeline=$true)]
	[AllowEmptyString()]
    [System.String]
    $CertficatePassword,
	
	[Parameter(Mandatory=$True, ValueFromPipeline=$true)]
	[AllowEmptyString()]
    [System.String]
    $OutputSecretsDirectoryName
)

Import-Module .\modules\CryptoLib.psm1 

# Make sure these settings follow your enviroment setup
$DefaultDrive = "d:"
$DefaultSecretsOutputDirectory = "\dev\secrets-deploy"
$UnEncryptedSecretsFilename = "secrets.json"
$DefaultCertficateName = "SymmetricCertificate.pfx"
$r2r = $false
$TargetServer = ""
$BaseRemotePrivateDirectory = "/sc-priv"
$BaseRemoteCertsDirectory = "/sc-certs"
$EncryptedKeyFileName = "symmetric.key.enc"
$PrivateCertsDirPerms = "0500"
$TempDirPerms = "0700"

# The sc-data-protection project should be compiled into an exe for use on the deploy workstation. This is built separately
$DataProtectionExePath = "\dev\encrypt-secrets-prod\sc-data-protection\bin\Debug\net6.0\sc-data-protection.exe"

$settings = @{
	poc3 = 
		@{
			LocalBackendDirectory = "\dev\encrypt-secrets-prod"
			RemoteBackendDirectory = "/poc3"
			ServiceName = "poc3.service"
			ServiceAccount = "poc3"
			SecretsOutputDirectory  = "poc3"
			RemotePrivateDirectory = "poc3"
			RemoteCertsDirectory = "poc3"
			RemoteAppPermissions = "440"
		 }
	}
	
$environments = @{
	DEVLinux = 
		@{
			TargetServer = "192.168.10.45"
		 }
    TESTLinux = 
		@{
			TargetServer = "<your server ip here>"
		 }
    PRODLinux = 
		@{
			TargetServer = "<your server ip here>"
		 }
	}


function BuildBackend
{
	Write-Output "Backend build: Target server is $TargetServer`n" | Write-Green

	Write-Output "Changing Directory..." | Write-Green
	cd $settings.$Application.LocalBackendDirectory

	Write-Output "Cleaning configuration $Configuration`n" | Write-Green
	dotnet clean --configuration $Configuration

	Write-Output "Publishing configuration $Configuration, with framework $Framework, ReadyToRun set to $r2r`n" | Write-Green

	dotnet publish --configuration $Configuration --framework $Framework -r linux-x64 --self-contained $r2r -p:PublishReadyToRun=$r2r /p:TrimUnusedDependencies=true
	Write-Output "" | Write-Green

	Write-Output "Building sc-data-protection exe...`n" | Write-Green

	dotnet build --configuration Debug "sc-data-protection\sc-data-protection.csproj"
	Write-Output "" | Write-Green

	Write-Output "Published: configuration $Configuration, linux-x64 with framework $Framework, ReadyToRun set to $r2r, self-contained $r2r`n" | Write-Green
}

function GetServiceName
{
    return $settings.$Application.ServiceName
}

function GetPublishDirectory
{
    return "$Application\bin\$Configuration\$Framework\linux-x64\publish"
}

function GetServiceStatus
{
    param($s)

	$serviceName = GetServiceName
	$cmd = "sudo systemctl status $serviceName | sed -n '/.*Active:/p'"

	Write-Output "Service $serviceName status:`n" | Write-Green
	
	$sb = [Scriptblock]::Create($cmd)
	Invoke-Command -Session $s -ScriptBlock $sb
}

function StartService
{
    param($s)

	$serviceName = GetServiceName
	$cmd = "sudo systemctl start $serviceName | sed -n '/.*Active:/p'"

	$sb = [Scriptblock]::Create($cmd)
	Invoke-Command -Session $s -ScriptBlock $sb
}

function StopService
{
    param($s)

	$serviceName = GetServiceName
	$cmd = "sudo systemctl stop $serviceName | sed -n '/.*Active:/p'"

	$sb = [Scriptblock]::Create($cmd)
	Invoke-Command -Session $s -ScriptBlock $sb
}

function CopyBackendToRemote
{
	param($s)
	$dir = $settings.$Application.RemoteBackendDirectory
	
	Write-Output "Changing Directory..." | Write-Green
	cd $settings.$Application.LocalBackendDirectory

	Write-Output "Copying to $dir..." | Write-Green
	Copy-Item -Path (Get-Item -Path .\$(GetPublishDirectory)\*).FullName -ToSession $s $dir -Force
}

function TempChown
{
	param($s)
	
	Write-Output "Changing ownership to $SSHUser...`n" | Write-Green
	
	$frontenddir = GetFrontendDirectory
	$backenddir = GetBackendDirectory
	
	$command="sudo chown -R $SSHUser`:$SSHUser $frontenddir $backenddir"
	$sb = [Scriptblock]::Create($command)
	Invoke-Command -Session $s -ScriptBlock $sb
}

function CreateRemoteDirs
{
	param($s, $RemotePrivDir, $RemoteCertsDir, $AppDir)

	Write-Output "Creating remote directories...$RemotePrivDir, $RemoteCertsDir, $AppDir`n" | Write-Green
	
	$command="sudo mkdir -p $RemotePrivDir $RemoteCertsDir $AppDir"
	$sb = [Scriptblock]::Create($command)
	Invoke-Command -Session $s -ScriptBlock $sb
}

Write-Output "Starting deploy process for $Application"  | Write-Green
Set-Location -Path $DefaultDrive

$r2r = $false
$TargetServer = $environments.$Configuration.TargetServer
$ServiceAccount = $settings.$Application.ServiceAccount

if ( $TargetServer -eq "" )
{
    Write-Output "Cannot determine target server to deploy to. Exiting" | Write-Red
	Start-Sleep -Seconds 2
	Exit
}

# Build the app here

BuildBackend

# Executing secrets processing

if ( $OutputSecretsDirectoryName -eq ""  )
{
    Write-Output "OutputSecretsDirectoryName is empty so assigning to default -> $DefaultSecretsOutputDirectory" | Write-Green
	$OutputSecretsDirectoryName = $DefaultSecretsOutputDirectory 
}

$SecretsLocalOutputDir = "$OutputSecretsDirectoryName\$($settings.$Application.SecretsOutputDirectory)\$Configuration"

Write-Output "Processing secrets, PFX file -> [$PFXFilename], Output Path -> [$SecretsLocalOutputDir]`n" | Write-Green

Write-Output "Creating Directory $SecretsLocalOutputDir`n" | Write-Green
Write-CreateDirectory $SecretsLocalOutputDir

if ( $PFXFilename -eq "" )
{
	$TmpPFXFilename = "$SecretsLocalOutputDir\$DefaultCertficateName"
	
	$PFXFilename = Write-PFXFile $TmpPFXFilename $DataProtectionExePath $CertficatePassword

    if ( $null -eq $PFXFilename )
	{
		Write-Output "CreateNewRSACertificate Failed, please check parameters. Exiting" | Write-Red
		Start-Sleep -Seconds 2
		Exit
	}
}

# Generate the symmetric key and encrypt the secrets file
$SecretsFile = "$($settings.$Application.LocalBackendDirectory)\$(GetPublishDirectory)\$UnEncryptedSecretsFilename"
Write-Output "Generating symmetric key and encrypting secrets file $($settings.$Application.LocalBackendDirectory)\$(GetPublishDirectory)\$UnEncryptedSecretsFilename`n" | Write-Green

$process = Write-SymmetricAndEncrypt $PFXFilename $CertficatePassword $DataProtectionExePath `
                                     $secretsFile `
									 $SecretsLocalOutputDir

# Failed, so bail
if ( $process.ExitCode -ne 1 )
{
	Write-Output "EncryptFileUsingSymmetric Failed, please check parameters. Exiting" | Write-Red
	Start-Sleep -Seconds 2
	Exit
}

Write-Output "Removing orginal secrets file from publish...`n" | Write-Green
remove-item $SecretsFile

# We're ready to deploy to remote over SSH

Write-Output "SSH: Logging in using -> $SSHKeyPath $SSHUser@$TargetServer`n" | Write-Green
$s = New-PSSession -HostName $SSHUser@$TargetServer -KeyFilePath $SSHKeyPath

# Stops the remote service
StopService($s)

$RemotePrivateDirectory = "$BaseRemotePrivateDirectory/$($settings.$Application.RemotePrivateDirectory)"
$RemoteCertsDirectory = "$BaseRemoteCertsDirectory/$($settings.$Application.RemoteCertsDirectory)"

CreateRemoteDirs $s $RemotePrivateDirectory $RemoteCertsDirectory $settings.$Application.RemoteBackendDirectory

# We need to temporarily change owenership so we can do stuff on the remote
Write-Output "Changing ownership to $SSHUser...`n" | Write-Green
$command="sudo chown -R $SSHUser`:$SSHUser $BaseRemotePrivateDirectory $BaseRemoteCertsDirectory $($settings.$Application.RemoteBackendDirectory)"
$sb = [Scriptblock]::Create($command)
Invoke-Command -Session $s -ScriptBlock $sb

$command="sudo chmod $TempDirPerms -R $BaseRemotePrivateDirectory $BaseRemoteCertsDirectory $($settings.$Application.RemoteBackendDirectory)"
$sb = [Scriptblock]::Create($command)
Invoke-Command -Session $s -ScriptBlock $sb

# Copy over all artifacts for our application to the target box
CopyBackendToRemote($s)
Write-Output "Done copying source files, copying encryption assets..." | Write-Green

Write-Output "Copying encrypted key and certficate from $SecretsLocalOutputDir\$EncryptedKeyFileName to $RemotePrivateDirectory...`n" | Write-Green
Copy-Item $SecretsLocalOutputDir\$EncryptedKeyFileName -ToSession $s $RemotePrivateDirectory
Copy-Item $PFXFilename -ToSession $s $RemoteCertsDirectory

# Change back permissions on core files. Note we have specific permisions required for the Logs and tmp directories
Write-Output "Changing permissions back to minimum required...`n" | Write-Green
$command="sudo bash -c 'find $($settings.$Application.RemoteBackendDirectory) -type f -not -path ''Logs/*'' -not -path ''tmp/*'' -exec chmod $($settings.$Application.RemoteAppPermissions) {} \;'" 
$sb = [Scriptblock]::Create($command)
Invoke-Command -Session $s -ScriptBlock $sb

Write-Output "Changing ownership / permissions back to $ServiceAccount for $RemotePrivateDirectory, $RemoteCertsDirectory...`n" | Write-Green
$command="sudo chown -R $ServiceAccount`:$ServiceAccount $BaseRemotePrivateDirectory $BaseRemoteCertsDirectory $($settings.$Application.RemoteBackendDirectory)"
$sb = [Scriptblock]::Create($command)
Invoke-Command -Session $s -ScriptBlock $sb
$command="sudo chmod $PrivateCertsDirPerms -R $BaseRemotePrivateDirectory $BaseRemoteCertsDirectory"
$sb = [Scriptblock]::Create($command)
Invoke-Command -Session $s -ScriptBlock $sb

# Start the remote service and finish up the session!
StartService($s)
GetServiceStatus($s)

Write-Output "`nDeployment Completed!! Check Service Status ouput above, should be running :)`n" | Write-Green
Write-Output "Closing and removing session.`n" | Write-Green

Exit-PSSession  
Remove-PSSession $s

