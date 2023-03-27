using System;
using System.IO;
using System.Collections.Generic;

namespace sf.systems.rentals.cars
{
    public class AuthenticationManager
    {
        private readonly string credentialsFilePath = "data/credentials.txt";
        private readonly string rolesFilePath = "data/roles.txt";
        private readonly Dictionary<string, string> credentials;
        private readonly Dictionary<string, UserRole> userRoles;
        private readonly SecurityManager securityManager;

        public AuthenticationManager(SecurityManager securityManager)
        {
            this.securityManager = securityManager;
            credentials = new Dictionary<string, string>();
            userRoles = new Dictionary<string, UserRole>();
        }

        public bool Login(string username, string password)
        {
            if (credentials.ContainsKey(username) && credentials[username] == HashPassword(password))
            {
                Console.WriteLine("Login successful.");
                return true;
            }
            else
            {
                Console.WriteLine("Login failed.");
                return false;
            }
        }

        public void Logout()
        {
            Console.WriteLine("Logout successful.");
        }

        public void LoadCredentials()
        {
            if (!File.Exists(credentialsFilePath)) throw new FileNotFoundException("credentialsFilePath");

            List<string> lines = DataManager.ReadLinesFromFile(credentialsFilePath);
            foreach (string line in lines)
            {
                string[] parts = line.Split(',');
                if (parts.Length == 2)
                {
                    string username = parts[0].Trim();
                    string passwordHash = parts[1].Trim();
                    credentials[username] = passwordHash;
                }
            }
        }
        public void LoadRoles()
        {
            if (!File.Exists(credentialsFilePath)) throw new FileNotFoundException("credentialsFilePath");

            List<string> lines = DataManager.ReadLinesFromFile(credentialsFilePath);
            foreach (string line in lines)
            {
                string[] parts = line.Split(',');
                if (parts.Length == 2)
                {
                    string username = parts[0].Trim();
                    UserRole role = (UserRole)Convert.ToInt32(parts[1].Trim());
                    userRoles[username] = role;
                }
            }
        }

        public void SaveCredentials()
        {
            List<string> lines = new List<string>();
            foreach (KeyValuePair<string, string> credential in credentials)
            {
                string line = credential.Key + "," + credential.Value;
                lines.Add(line);
            }
            DataManager.WriteLinesToFile(credentialsFilePath, lines);
        }

        public void SaveRoles()
        {
            List<string> lines = new List<string>();
            foreach (KeyValuePair<string, UserRole> role in userRoles)
            {
                string line = role.Key + "," + Convert.ToInt32(role.Value);
                lines.Add(line);
            }
            DataManager.WriteLinesToFile(credentialsFilePath, lines);
        }

        public void RegisterUser(string username, string password, UserRole role)
        {
            credentials[username] = HashPassword(password);
            SaveCredentials();

            userRoles[username] = role;
            SaveRoles();

            Console.WriteLine("User registered.");
        }

        public void UnregisterUser(string username)
        {
            if (credentials.ContainsKey(username))
            {
                credentials.Remove(username);
                SaveCredentials();
                Console.WriteLine("User unregistered.");
            }
            else if (userRoles.ContainsKey(username))
            {
                userRoles.Remove(username);
                SaveRoles();
            }
            else
            {
                Console.WriteLine("User not found.");
            }
        }

        private string HashPassword(string password)
        {
            return securityManager.Encrypt(password);
        }
    }
}
