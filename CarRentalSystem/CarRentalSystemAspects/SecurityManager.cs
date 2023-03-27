using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace sf.systems.rentals.cars
{
    public class SecurityManager
    {
        public const string KeyFile = "key.bin";
        private readonly byte[] key;

        private static byte[] defaultKey()
        {
            // Generate a 32-byte array
            byte[] key = new byte[32];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(key);
            }

            return key;
        }

        public SecurityManager() : this(defaultKey())
        {
            string keyPath = KeyFile; 
            File.WriteAllBytes(keyPath, this.key);
        }
 
        public SecurityManager(byte[] key)
        {
            if (key == null || key.Length != 32)
            {
                throw new ArgumentException("Key must be a byte array of length 32");
            }

            this.key = key;
        }

        public string Encrypt(string data)
        {
            byte[] iv = new byte[16];
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(dataBytes, 0, dataBytes.Length);
                        cryptoStream.FlushFinalBlock();
                    }
                    byte[] encryptedData = memoryStream.ToArray();
                    return Convert.ToBase64String(encryptedData);
                }
            }
        }

        public string Decrypt(string data)
        {
            byte[] iv = new byte[16];
            byte[] encryptedData = Convert.FromBase64String(data);

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(encryptedData, 0, encryptedData.Length);
                        cryptoStream.FlushFinalBlock();
                    }
                    byte[] decryptedData = memoryStream.ToArray();
                    return Encoding.UTF8.GetString(decryptedData);
                }
            }
        }

        public bool Authorize(User user, string permission)
        {
            if (user == null)
            {
                return false;
            }

            switch (user.Role)
            {
                case UserRole.Admin:
                    return true;
                case UserRole.PowerUser:
                    return permission == "read" || permission == "write";
                case UserRole.User:
                    return permission == "read";
                case UserRole.Guest:
                default:
                    return false;
            }
        }
    }

    public class User
    {
        public string Id { get; }
        public string Name { get; }
        public UserRole Role { get; }

        public User(string id, string name, UserRole role)
        {
            Id = id;
            Name = name;
            Role = role;
        }
    }

    public enum UserRole
    {
        Admin,
        PowerUser,
        User,
        Guest
    }
}
