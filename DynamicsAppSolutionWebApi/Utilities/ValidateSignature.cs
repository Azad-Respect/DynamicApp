using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace DynamicsAppSolutionWebApi.Utilities
{
    public static class ValidateSignature
    {
        public static void Check(string userName, string password, string currentUTCTime, string signature)
        {
            if (String.IsNullOrEmpty(userName))
            {
                throw new Exception("Username is null.");
            }
            if (String.IsNullOrEmpty(password))
            {
                throw new Exception("Password is null.");
            }
            if (String.IsNullOrEmpty(currentUTCTime))
            {
                throw new Exception("CurrentUTCTime is null.");
            }
            if (String.IsNullOrEmpty(signature))
            {
                throw new Exception("Signature is null.");
            }

            string input = userName + password + currentUTCTime + "2B05456F-7B1C-482D-8FB1-1F50350C31DA";

            string md5Hash1;

            using (MD5 md5Hash2 = MD5.Create())
                md5Hash1 = GetMd5Hash(md5Hash2, input);

            string base64String = Convert.ToBase64String(new UTF8Encoding().GetBytes(md5Hash1));

            if (signature != base64String)
                throw new Exception("Invalid signature.");
        }

        private static string GetMd5Hash(MD5 md5Hash, string input)
        {
            byte[] hash = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder stringBuilder = new StringBuilder();
            for (int index = 0; index < hash.Length; ++index)
                stringBuilder.Append(hash[index].ToString("x2"));
            return stringBuilder.ToString();
        }
    }
}