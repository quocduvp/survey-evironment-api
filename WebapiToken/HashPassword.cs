using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace WebapiToken
{
    public class HashPassword
    {
        public static string hashPassword(string password)
        {
            //chuyển sang byte
            byte[] encodedPassword = new UTF8Encoding().GetBytes(password.Trim());
            //tiến hành mã hóa md5
            byte[] hash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(encodedPassword);
            //chuyen về dạng chuỗi (tìm những thg có dấu "-" bỏ nó đi
            string encoded = BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
            return encoded;
        }
    }
}