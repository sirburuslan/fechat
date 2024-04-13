/*
 * @class Encryptor
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-23
 *
 * This class encrypts and decrypts the strings
 */

// Namespace for General Utils
namespace FeChat.Utils.General {

    // System Namespaces
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// Encryptor Class
    /// </summary>
    public class Encryptor {

        /// <summary>
        /// Encrypt a string
        /// </summary>
        /// <param name="plainText">Text to encrypt</param>
        /// <param name="encryptionKey">Key for encryption</param>
        /// <returns>Encoded string</returns>
        public string Encrypt(string plainText, string encryptionKey) {

            // Turn text into bytes
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            // Create an instance of Advanced Encryption Standard
            using Aes aesAlg = Aes.Create();

            // Set key size
            aesAlg.KeySize = 256;

            // Set block size
            aesAlg.BlockSize = 128;

            // Get key from encryption key
            aesAlg.Key = GetKeyBytes(aesAlg, encryptionKey);

            // Initializes a byte array for the Initialization Vector (IV) 
            aesAlg.IV = new byte[aesAlg.BlockSize / 8];

            // Create an encryptor object for encrypting data using the AES
            using ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            // Initialize an instance for MemoryStream
            using MemoryStream msEncrypt = new();

            // Encrypting data
            using CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);

            // Writes a sequence of bytes to the current CryptoStream
            csEncrypt.Write(plainTextBytes, 0, plainTextBytes.Length);

            // Updates the underlying data source
            csEncrypt.FlushFinalBlock();
            
            // Writes the stream contents to a byte array
            byte[] encryptedBytes = msEncrypt.ToArray();

            // Return base 64 string
            return Convert.ToBase64String(encryptedBytes);

        }

        /// <summary>
        /// Decrypt a string
        /// </summary>
        /// <param name="cipherText">String to decrypt</param>
        /// <param name="encryptionKey">Key for encryption</param>
        /// <returns>Decoded string</returns>
        public string Decrypt(string cipherText, string encryptionKey) {

            // Turn base 64 string into bytes
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);

            // Creates a cryptographic object
            using Aes aesAlg = Aes.Create();

            // Set key size
            aesAlg.KeySize = 256;

            // Set block size
            aesAlg.BlockSize = 128;

            // Get key from encryption key
            aesAlg.Key = GetKeyBytes(aesAlg, encryptionKey);

            // Initializes a byte array for the Initialization Vector (IV) 
            aesAlg.IV = new byte[aesAlg.BlockSize / 8];

            // Create a decryptor for AES (Advanced Encryption Standard)
            using ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            // Initializes a new instance of the MemoryStream
            using MemoryStream msDecrypt = new(cipherTextBytes);

            // Create a CryptoStream object for decryption
            using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);

            // Read data from the stream
            using StreamReader srDecrypt = new(csDecrypt);
            
            // Reads all characters from the current position to the end of the stream and return
            return srDecrypt.ReadToEnd();

        }

        /// <summary>
        /// Get bytes from encryption key
        /// </summary>
        /// <param name="algorithm">SymmetricAlgorithm</param>
        /// <param name="encryptionKey">Key for encryption</param>
        /// <returns>Bytes</returns>
        private static byte[] GetKeyBytes(SymmetricAlgorithm algorithm, string encryptionKey) {

            // Get bytes
            byte[] keyBytes = Encoding.UTF8.GetBytes(encryptionKey);

            // Get appropriate number of bytes needed to store the key
            byte[] validKeyBytes = new byte[algorithm.KeySize / 8];

            // Join the bytes
            Array.Copy(keyBytes, validKeyBytes, Math.Min(keyBytes.Length, validKeyBytes.Length));

            return validKeyBytes;

        }
    }

}