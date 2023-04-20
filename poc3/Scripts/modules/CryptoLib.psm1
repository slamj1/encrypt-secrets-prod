function Write-Green {
    process { Write-Host $_ -ForegroundColor Green }
}

function Write-Red {
    process { Write-Host $_ -ForegroundColor Red }
}

function Write-PFXFile ([string]$PFXPath, [string]$ProtectionExePath, [string]$CertPassword) {
	$TmpPFXFilename = $PFXPath

	Write-Output "Creating PFX...$TmpPFXFilename" | Write-Green
	$process = Start-Process -FilePath $ProtectionExePath `
	   -ArgumentList "CreateNewRSACertificate $TmpPFXFilename `"$CertPassword`"" -PassThru -Wait
  
	#Write-Output "Return is -> $($process.ExitCode)" | Write-Green
	
	if ( $process.ExitCode -ne 1 )
	{
		$TmpPFXFilename = $null
	}

	Return $TmpPFXFilename
}

function Write-SymmetricAndEncrypt([string]$PFXFilename, 
							       [string]$CertPassword,
							       [string]$DataProtectionExePath, 
								   [string]$SecretsFileName, 
                                   [string]$OutputSecretsDirectoryName) {

     #Write-Output "Parms...[$PFXFilename, $CertPassword, $DataProtectionExePath, $SecretsFileName, $OutputSecretsDirectoryName]" | Write-Green
	 $process = Start-Process `
			-FilePath $DataProtectionExePath `
			-ArgumentList "EncryptFileUsingSymmetric $PFXFilename `"$CertPassword`" $SecretsFileName $OutputSecretsDirectoryName" -PassThru -Wait
	  	
	Return $process
}

function Write-CreateDirectory([string]$Directoryname) {

     New-Item -ItemType Directory -Path $Directoryname -Force
}