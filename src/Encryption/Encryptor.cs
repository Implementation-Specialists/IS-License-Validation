using System.Security.Cryptography;
using System.Text;

namespace IS.LicenseValidation.Encryption;

public class Encryptor : IEncryptor
{
    private const int KeySize = 256;
    private const int BlockSize = 128;

    public string Encrypt(byte[] data, string password)
    {
        using var aes = Aes.Create();
        aes.BlockSize = BlockSize;
        aes.KeySize = KeySize;

        aes.GenerateIV();
        aes.Key = SHA256.HashData(Encoding.UTF8.GetBytes(password));

        byte[] encryptedValue;

        using var encryptor = aes.CreateEncryptor();
        using var memoryStream = new MemoryStream();
        using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
        using (var streamWriter = new StreamWriter(cryptoStream))
        {
            streamWriter.Write(Encoding.UTF8.GetString(data));
        }

        encryptedValue = memoryStream.ToArray();

        var encryptedData = new byte[aes.IV.Length + encryptedValue.Length];

        Array.Copy(aes.IV, 0, encryptedData, 0, aes.IV.Length);
        Array.Copy(memoryStream.ToArray(), 0, encryptedData, aes.IV.Length, encryptedValue.Length);

        return Convert.ToBase64String(encryptedData);
    }

    public bool TryDecrypt(string encryptedValue, string password, out byte[] bytes)
    {
        try
        {
            byte[] encryptedBytes = Convert.FromBase64String(encryptedValue);
            var iv = new byte[16];
            var encryptedData = new byte[encryptedBytes.Length - iv.Length];

            Array.Copy(encryptedBytes, 0, iv, 0, 16);
            Array.Copy(encryptedBytes, iv.Length, encryptedData, 0, encryptedData.Length);

            using var aes = Aes.Create();
            aes.Key = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor();
            using var memoryStream = new MemoryStream(encryptedData);
            using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            using var streamReader = new StreamReader(cryptoStream);

            bytes = Encoding.UTF8.GetBytes(streamReader.ReadToEnd());

            return true;
        }
        catch (CryptographicException)
        {
            bytes = [];
            return false;
        }
    }
}
