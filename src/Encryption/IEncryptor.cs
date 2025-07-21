namespace IS.LicenseValidation.Encryption;

public interface IEncryptor
{
    bool TryDecrypt(string encryptedValue, string password, out byte[] bytes);

    string Encrypt(byte[] bytes, string password);
}
