using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FlameChatClient.Encryption
{
    public class PasswordHasher
    {

        public static string HashPassword(string text)
        {
            byte[] passwordByteArray = Encoding.UTF8.GetBytes(text);


            byte[] hash = SHA256.HashData(passwordByteArray);


            // Convert the byte array to a hexadecimal string.
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                builder.Append(hash[i].ToString("x2"));
            }
            return builder.ToString();

        }
    }
}
