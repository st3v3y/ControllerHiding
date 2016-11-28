using System.Web.Script.Serialization;

namespace ControllerHiding.Security
{
    public class ObjectCrypter<TEncryptData> : StringCrypter
    {
        private const string CryptPurpose = "ObjectCrypter";

        public string EncryptData(TEncryptData @object)
        {
            var serializedData = new JavaScriptSerializer().Serialize(@object);
            return Protect(serializedData, CryptPurpose);
        }

        public TEncryptData DecryptData(string text)
        {
            var encryptedtext = Unprotect(text, CryptPurpose);
            return new JavaScriptSerializer().Deserialize<TEncryptData>(encryptedtext);
        }
    }
}