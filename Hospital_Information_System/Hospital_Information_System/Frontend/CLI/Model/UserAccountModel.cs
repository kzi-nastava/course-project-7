using HospitalIS.Backend;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HospitalIS.Frontend.CLI.Model
{
    internal abstract class UserAccountModel
    {
        private const string hintInputUsername = "Enter username: ";
        private const string hintInputPassword = "Enter password: ";

        internal class InvalidLoginAttemptException : Exception
        {
            internal InvalidLoginAttemptException(string errorMessage) : base($"Login failed: {errorMessage}.")
            {

            }
        }

        internal static UserAccount Login(string inputCancelString)
        {
            while (true)
            {
                Console.Write(hintInputUsername);
                string username = EasyInput<string>.Get(inputCancelString);

                Console.Write(hintInputPassword);
                string password = EasyInput<string>.Get(inputCancelString);

                try
                {
                    UserAccount account = AttemptLogin(username, password);
                    return account;
                }
                catch (InvalidLoginAttemptException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        internal static UserAccount AttemptLogin(string username, string password)
        {
            foreach (UserAccount ua in GetNonDeletedAccounts())
            {
                if (ua.Username == username && ua.Password == password)
                {
                    if (ua.Blocked) throw new InvalidLoginAttemptException("Account is blocked");
                    return ua;
                }
            }

            throw new InvalidLoginAttemptException("Invalid credentials");
        }

        internal static List<UserAccount> GetNonDeletedAccounts()
        {
            return IS.Instance.Hospital.UserAccounts.Where(u => !u.Deleted).ToList();
        }
    }
}
