using System.Web.Script.Serialization;

namespace ControllerHiding.Security
{
    public class ObjectCrypter<TEncryptData> : StringCrypter
    {
        private const string CryptPurpose = "ObjectCrypter";

        public string EncryptData(TEncryptData @object)
        {
            var javaScriptSerializer = new JavaScriptSerializer(new SimpleTypeResolver());
            var serializedData = javaScriptSerializer.Serialize(@object);
            return Protect(serializedData, CryptPurpose);
        }

        public TEncryptData DecryptData(string text)
        {
            var encryptedtext = Unprotect(text, CryptPurpose);
            var javaScriptSerializer = new JavaScriptSerializer(new SimpleTypeResolver());
            return javaScriptSerializer.Deserialize<TEncryptData>(encryptedtext);
        }
    }
}