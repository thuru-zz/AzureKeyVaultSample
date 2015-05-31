using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using System.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Azure.KeyVault.WebKey;

namespace KeyVaultConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var vaultUrl = ConfigurationManager.ConnectionStrings["KeyVaultUrl"].ConnectionString;

            var vaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetAccessTokenAsync));

            // secrets

            // creating a secret
            // setting the secret for multiple times for the same secret key will create different versions of the secret
            var newSecret = vaultClient.SetSecretAsync(vaultUrl, "secret-key", "you secret").GetAwaiter().GetResult();

            // retrieving the secret
            var retrievedSecret = vaultClient.GetSecretAsync(vaultUrl, "secret-key", newSecret.SecretIdentifier.Version);

            // delete a secret, deletes all versions
            vaultClient.DeleteSecretAsync(vaultUrl, "secret-key");

            // keys
            
            // create a key

            var keyBundle = new KeyBundle()
            {
                Key = new JsonWebKey() { Kty = JsonWebKeyType.RsaHsm.ToString() },
            };

            var newKey = vaultClient.CreateKeyAsync(vaultUrl, "key-name", keyBundle.Key.Kty).GetAwaiter().GetResult();

            // encrypt or decrypt

            string algorithm = "RSA_OAEP";
            
            // load your content as a byte array
            byte [] filebuffer = new byte [1024];

            var encryptedResult = vaultClient.EncryptAsync(vaultUrl, newKey.KeyIdentifier.Name, newKey.KeyIdentifier.Version, algorithm, filebuffer)
                .GetAwaiter().GetResult();

            // result is a byte []
            var result = encryptedResult.Result;

            var decryptedResult = vaultClient.DecryptAsync(encryptedResult.Kid, algorithm, result).GetAwaiter().GetResult();

            Console.ReadKey();
        }

        private static async Task<string> GetAccessTokenAsync(string authority, string resource, string scope)
        {
            var appId = ConfigurationManager.ConnectionStrings["KeyVaultAppId"].ConnectionString;
            var appSecret = ConfigurationManager.ConnectionStrings["KeyVaultAppKey"].ConnectionString;

            var clientCredential = new ClientCredential(appId, appSecret);

            var authenticationContext = new AuthenticationContext(authority, false);
            var result = await authenticationContext.AcquireTokenAsync(resource, clientCredential);

            return result.AccessToken;
        }
    }
}
