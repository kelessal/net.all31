using System;
using System.Collections.Generic;
using System.Text;

namespace Net.Security
{
    public class PasswordGenerator : IPasswordGenerator
    {

        const string alphaNumerics = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        const string numbers = "1234567890";
        public string CreateRandomPassword(int length, bool onlyNumeric = false)
        {
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            var valid = onlyNumeric ? numbers : alphaNumerics;
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }

        public HashedString CreateHashedString(string password)
        {
            using (var hmac = new global::System.Security.Cryptography.HMACSHA512())
            {
                return new HashedString()
                {
                    Salt = Convert.ToBase64String(hmac.Key),
                    Hash = Convert.ToBase64String(hmac.ComputeHash(global::System.Text.Encoding.UTF8.GetBytes(password)))
                };
            }
        }

        public bool IsMatch(HashedString hash, string password)
        {
            using (var hmac = new global::System.Security.Cryptography.HMACSHA512(Convert.FromBase64String(hash.Salt)))
            {
                var hashedPass = Convert.ToBase64String(hmac.ComputeHash(global::System.Text.Encoding.UTF8.GetBytes(password)));
                return hashedPass == hash.Hash;
            }
        }
    }
}
