using System;
using System.Text;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CryproExamples.Models;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.IO;

namespace CryproExamples.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Encryption(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return View();

            //hash
            var (sha1Hash,sha512Hash,salt) = HMacHash(password);
            var base64Encoded = Base64Encode(password);

            //AES Encryption
            string encrypted, decrypted;
            using(AesManaged aes = new AesManaged()) {  
                // Encrypt string    
                encrypted = EncryptPassword(password, aes.Key, aes.IV);  

                // Decrypt the bytes to a string.    
                decrypted = DecryptPassword(encrypted, aes.Key, aes.IV);  
            }  

            var model = new CryptoModel(){
                Password = password,
                EncryptedPassword = encrypted,
                DecryptedPassword = decrypted,
                Sha1Hash = sha1Hash,
                Sha1HashUnsalted = Sha1Unsalted(password),
                Sha512Hash = sha512Hash,
                Salt = salt,
                Base64Encoded = base64Encoded,
                Base64Decoded = Base64Decode(base64Encoded),
                Md5Hash = MD5Hash(password)
            };
            return View(model);
        }

        private string EncryptPassword(string password,byte[] Key, byte[] IV)
        {

            byte[] encrypted;  

            // Create a new AesManaged.    
            using(AesManaged aes = new AesManaged()) {  

                // Create encryptor    
                ICryptoTransform encryptor = aes.CreateEncryptor(Key, IV);  
                
                // Create MemoryStream    
                using(MemoryStream ms = new MemoryStream()) {  

                    // Create crypto stream using the CryptoStream class. This class is the key to encryption    
                    // and encrypts and decrypts data from any given stream. In this case, we will pass a memory stream    
                    // to encrypt    
                    using(CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write)) {  
                        // Create StreamWriter and write data to a stream    
                        using(StreamWriter sw = new StreamWriter(cs))  
                        sw.Write(password);  
                        encrypted = ms.ToArray();  
                    }  
                }  
            }  
            // Return encrypted data    
            return Convert.ToBase64String(encrypted);  
        }

        private string DecryptPassword(string encryptedPassword, byte[] Key, byte[] IV)
        {
            string plaintext = null;  
            // Create AesManaged    
            using(AesManaged aes = new AesManaged()) {  
                // Create a decryptor    
                ICryptoTransform decryptor = aes.CreateDecryptor(Key, IV);  
                // Create the streams used for decryption.    
                using(MemoryStream ms = new MemoryStream(Convert.FromBase64String(encryptedPassword))) {  
                    // Create crypto stream    
                    using(CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read)) {  
                        // Read crypto stream    
                        using(StreamReader reader = new StreamReader(cs))  
                        plaintext = reader.ReadToEnd();  
                    }  
                }  
          
                return plaintext;  
            }
        }

        private (string,string,string) HMacHash(string inputText){

             // generate a 128-bit salt using a secure PRNG
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
            //observe weak alg. warning for HMACSHA1
            string sha1Hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: inputText,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            string sha512Hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: inputText,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            var base64Salt = Convert.ToBase64String(salt);
            return (sha1Hashed,sha512Hashed,base64Salt);
        }

        private string Sha1Unsalted(string password){
            var sha = new SHA1Managed();
            return Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(password)));
        }

        public string Base64Encode(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return string.Empty;

            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public string Base64Decode(string base64EncodedString)
        {
            if (string.IsNullOrEmpty(base64EncodedString))
                return string.Empty;

            var data = Convert.FromBase64String(base64EncodedString);
            return System.Text.Encoding.UTF8.GetString(data);
        }

        //Weak alg; do not use for hashing sensitive data!    
        public string MD5Hash(string password)
        {
            byte[] data;

            //observer warning
            using (var hasher = new MD5CryptoServiceProvider())
            {
                data = hasher.ComputeHash(Encoding.UTF8.GetBytes(password));
            }

            return Convert.ToBase64String(data);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
