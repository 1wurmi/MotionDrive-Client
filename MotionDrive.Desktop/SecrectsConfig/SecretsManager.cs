using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MotionDrive.Desktop.SecrectsConfig;
public class SecretsManager
{
    public string GetSecretConfigPath()
    {
        string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string appFolder = System.IO.Path.Combine(appDataFolder, "MotionDrive");
        if (!System.IO.Directory.Exists(appFolder))
        {
            System.IO.Directory.CreateDirectory(appFolder);
        }
        return Path.Combine(appFolder, "secrets.dat");
    }

    public void SaveSecrets(SecretConfigModel secrets)
    {
        // Save secrets to a file
        byte[] encryptedSecrets = ProtectedData.Protect(
            Encoding.UTF8.GetBytes(JsonSerializer.Serialize(secrets)),
            null,
            DataProtectionScope.CurrentUser
        );


        System.IO.File.WriteAllBytes(this.GetSecretConfigPath(), encryptedSecrets);

    }

    public SecretConfigModel LoadSecrets()
    {

        string path = GetSecretConfigPath();

        if (!File.Exists(path))
        {
            return new SecretConfigModel();
        }

        var encryptedData = System.IO.File.ReadAllBytes(this.GetSecretConfigPath());

        var decryptedData = ProtectedData.Unprotect(
            encryptedData,
            null,
            DataProtectionScope.CurrentUser
        );

        return JsonSerializer.Deserialize<SecretConfigModel>(Encoding.UTF8.GetString(decryptedData));
    }

    public void DeleteSecrets()
    {
        string path = GetSecretConfigPath();
        // Delete file

        File.Delete(path);
    }
}
