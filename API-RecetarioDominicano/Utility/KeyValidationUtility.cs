using System.Security.Cryptography;
using System.Text;
using API_RecetarioDominicano.Models.Context;

namespace API_RecetarioDominicano.Utility
{
    public class KeyValidationUtility
    {
        private readonly DbConnectionContext _context;
        private readonly HttpRequest _request;
        private MessageLoggingUtility errorUtility = new();
        private static readonly string encryptionKey = "LOCAL_ENCRYPTION_KEY";

        public KeyValidationUtility(DbConnectionContext context, HttpRequest request)
        {
            this._context = context;
            this._request = request;
        }
        public string ValidateClientLogin(string clientUserEmail, string clientPassword)
        {

            var clientObjectFound = _context.UserTbl.Where(a => a.UserEmail == clientUserEmail).FirstOrDefault();

            if (clientObjectFound == null)
            {
                return errorUtility.GetErrorDescription("AUTHENTICATION_AT02");
            }

            var encryptedApiKeyFromDatabase = clientObjectFound.UserEncryptionKey;
            var salt = clientObjectFound.UserSalt;

            if (!ValidateClientAuthentication(encryptedApiKeyFromDatabase, clientPassword, salt))
            {
                return errorUtility.GetErrorDescription("AUTHENTICATION_AT03");
            }

            return "Success.";
        }
        public static bool ValidateClientAuthentication(string encryptedKeyFromDatabase, string providedEncryptedKey, string salt)
        {
            string encryptedApiKeyForComparison = EncryptApiKey(providedEncryptedKey, salt);

            return string.Equals(encryptedKeyFromDatabase, encryptedApiKeyForComparison);
        }

        public static string EncryptApiKey(string apiKey, string salt)

        {

            var encryptionKey = KeyValidationUtility.encryptionKey;

            string combinedKey = encryptionKey + salt;

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] combinedKeyBytes = Encoding.UTF8.GetBytes(combinedKey);
                byte[] validKeyBytes = sha256.ComputeHash(combinedKeyBytes);

                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = validKeyBytes;
                    aesAlg.IV = new byte[aesAlg.BlockSize / 8];

                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(apiKey);
                        }

                        // Convert the encrypted bytes to URL-safe base64
                        string encryptedBase64 = Convert.ToBase64String(msEncrypt.ToArray());
                        return encryptedBase64.Replace("+", "-").Replace("/", "_");
                    }
                }
            }
        }

        public static string GenerateSalt()
        {
            string input = DateTime.UtcNow.Ticks.ToString() + Guid.NewGuid().ToString();

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);

                return Convert.ToBase64String(hashBytes);
            }
        }


    }
}
