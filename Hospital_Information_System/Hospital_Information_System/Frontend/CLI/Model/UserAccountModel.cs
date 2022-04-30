using HospitalIS.Backend;
using System;
using HospitalIS.Backend.Controller;

namespace HospitalIS.Frontend.CLI.Model
{
    internal abstract class UserAccountModel
    {
        private const string hintInputUsername = "Enter username: ";
        private const string hintInputPassword = "Enter password: ";

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
                    UserAccount account = UserAccountController.AttemptLogin(username, password);
                    return account;
                }
                catch (UserAccountController.InvalidLoginAttemptException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
