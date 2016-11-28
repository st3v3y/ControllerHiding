using ControllerHiding.Security;

namespace ControllerHiding.Extensions
{
    public static class EncryptionExtensions
    {
        public static string Encrypt<TEncryptData>(this TEncryptData encryptData)
        {
            var objectEncryptor = new ObjectCrypter<TEncryptData>();
            return objectEncryptor.EncryptData(encryptData);
        }

        public static TEncryptData Decrypt<TEncryptData>(this string encryptData)
        {
            var objectEncryptor = new ObjectCrypter<TEncryptData>();
            return objectEncryptor.DecryptData(encryptData);
        }
    }
}