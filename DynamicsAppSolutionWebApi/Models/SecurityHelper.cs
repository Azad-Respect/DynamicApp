using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace DynamicsAppSolutionWebApi.Models
{
    public class SecurityHelper
    {
        public string CreateHashFromUserName(string userName, string currentUTCTime)
        {
            return Convert.ToBase64String(new MD5CryptoServiceProvider().ComputeHash(new UTF8Encoding().GetBytes(userName + currentUTCTime + "2B05456F-7B1C-482D-8FB1-1F50350C31DA")));
        }

        public string CreateHashForString(string toencrypt)
        {
            return Convert.ToBase64String(new MD5CryptoServiceProvider().ComputeHash(new UTF8Encoding().GetBytes(toencrypt + "2B05456F-7B1C-482D-8FB1-1F50350C31DA")));
        }

        public string Encrypt(string toEncrypt, bool useHashing)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(toEncrypt);
            string s = "2B05456F-7B1C-482D-8FB1-1F50350C31DA";
            byte[] numArray;
            if (useHashing)
            {
                MD5CryptoServiceProvider cryptoServiceProvider = new MD5CryptoServiceProvider();
                numArray = cryptoServiceProvider.ComputeHash(Encoding.UTF8.GetBytes(s));
                cryptoServiceProvider.Clear();
            }
            else
                numArray = Encoding.UTF8.GetBytes(s);
            TripleDESCryptoServiceProvider cryptoServiceProvider1 = new TripleDESCryptoServiceProvider();
            cryptoServiceProvider1.Key = numArray;
            cryptoServiceProvider1.Mode = CipherMode.ECB;
            cryptoServiceProvider1.Padding = PaddingMode.PKCS7;
            byte[] inArray = cryptoServiceProvider1.CreateEncryptor().TransformFinalBlock(bytes, 0, bytes.Length);
            cryptoServiceProvider1.Clear();
            return Convert.ToBase64String(inArray, 0, inArray.Length);
        }

        public string Decrypt(string cipherString, bool useHashing)
        {
            try
            {
                byte[] inputBuffer = Convert.FromBase64String(cipherString);
                string s = "2B05456F-7B1C-482D-8FB1-1F50350C31DA";
                byte[] numArray;
                if (useHashing)
                {
                    MD5CryptoServiceProvider cryptoServiceProvider = new MD5CryptoServiceProvider();
                    numArray = cryptoServiceProvider.ComputeHash(Encoding.UTF8.GetBytes(s));
                    cryptoServiceProvider.Clear();
                }
                else
                    numArray = Encoding.UTF8.GetBytes(s);
                TripleDESCryptoServiceProvider cryptoServiceProvider1 = new TripleDESCryptoServiceProvider();
                cryptoServiceProvider1.Key = numArray;
                cryptoServiceProvider1.Mode = CipherMode.ECB;
                cryptoServiceProvider1.Padding = PaddingMode.PKCS7;
                byte[] bytes = cryptoServiceProvider1.CreateDecryptor().TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);
                cryptoServiceProvider1.Clear();
                return Encoding.UTF8.GetString(bytes);
            }
            catch
            {
                return string.Empty;
            }
        }

        public void ValidateSignature(string userName, string currentUTCTime, string signature)
        {
            if (signature != this.CreateHashFromUserName(userName, currentUTCTime))
                throw new Exception("Invalid signature.");
        }
    }
}