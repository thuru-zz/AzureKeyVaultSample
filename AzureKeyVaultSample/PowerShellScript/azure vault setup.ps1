# Remove any Azure accounts from powershell
Remove-AzureAccount

# Add your new account
# before doing this create a adadmin in massrover AD (adadmin@massrover.onmicrosoft.com) with globla admin privilege. 
# Assign him as a Co-admninistrator in the production subscription
Add-AzureAccount

# change power shell mode
Switch-AzureMode AzureResourceManager

# create a resource group
New-AzureResourceGroup -Name 'vaultgroup' -Location 'North Europe' -Verbose

# create the Vault
# if you use this without changing Vault URI = https://massroverkeyvault.vault.azure.net
New-AzureKeyVault -VaultName 'massroverkeyvault' -ResourceGroupName 'vaultgroup' -Location 'North Europe' -Verbose

# set permissision for the access proxy app
# you need the client app id you created in massrover AD
Set-AzureKeyVaultAccessPolicy -VaultName 'massroverkeyvault' -ServicePrincipalName 'client app id' -PermissionsToSecrets all


#Operations

# create a secret 
$secret = ConvertTo-SecureString 'secret value' -AsPlainText -Force

# in the massrover secret key also not stored any where it is calculated in the algorthim with cusotmer id and secret type (en enum)
Set-AzureKeyVaultSecret -VaultName 'massroverkeyvault' -Name 'secret key' -SecretValue $secret

# list the secrets in the vault
Get-AzureKeyVaultSecret -VaultName 'massroverkeyvault'

# remove a secret
Remove-AzureKeyVaultSecret -VaultName 'massroverkeyvault' -Name 'secret-key'

# delete the vault
Remove-AzureKeyVault -VaultName 'massroverkeyvault' -ResourceGroupName 'vaultgroup'

# delete the resource group
Remove-AzureResourceGroup -Name 'vaultgroup'