namespace CryproExamples.Models
{
    public class CryptoModel
    {
        public string Password {get;set;}

        public string EncryptedPassword {get;set;}

        public string DecryptedPassword {get;set;}

        public string Sha512Hash {get;set;}
        
        public string Sha1Hash {get;set;}

        public string Sha1HashUnsalted{get;set;}

        public string Salt {get;set;}

        public string Md5Hash {get;set;}

        public string Base64Encoded {get;set;}

        public string Base64Decoded {get;set;}
    }
}