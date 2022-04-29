using HospitalIS.Backend;
using System;
using System.Collections.Generic;
using System.Linq;
using HospitalIS.Backend.Controller;

namespace HospitalIS.Frontend.CLI.Model
{
    internal abstract class UserAccountModel
    {
        private const string hintInputUsername = "Enter username: ";
        private const string hintInputPassword = "Enter password: ";
        
        private const string hintSelectAccounts = "Select accounts by their number, separated by whitespace.\nEnter a newline to finish";
        private const string hintSelectProperties = "Select properties by their number, separated by whitespace.\nEnter a newline to finish";
        private const string hintSelectPatientAccount = "Select account, by its number.\nEnter a newline to finish";
        private const string hintUsername = "Enter account username: ";
        private const string hintPassword = "Enter account password: ";
        private const string hintFirstName = "Enter first name: ";
        private const string hintLastName = "Enter last name: ";
        private const string hintGender = "Select gender: ";
        private const string errAccountUsernameNonEmpty = "Username must not be empty!";
        private const string errAccountPasswordNonEmpty = "Password must not be empty!";
        private const string errFirstNameNonEmpty = "First name must not be empty!";
        private const string errLastNameNonEmpty = "Last name must not be empty!";

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
                    if (ua.Blocked != UserAccount.BlockedBy.NONE) throw new InvalidLoginAttemptException("Account is blocked");
                    return ua;
                }
            }

            throw new InvalidLoginAttemptException("Invalid credentials");
        }

        internal static List<UserAccount> GetNonDeletedAccounts()
        {
            return IS.Instance.Hospital.UserAccounts.Where(u => !u.Deleted).ToList();
        }

        internal static void CreatePatientAccount(string inputCancelString)
        {
            UserAccount user = InputAccount(inputCancelString, AccountController.GetAccountProperties());
            Patient patient = new Patient(user.Person);
            
            IS.Instance.UserAccountRepo.Add(user);
            IS.Instance.PersonRepo.Add(user.Person);
            IS.Instance.PatientRepo.Add(patient);
        }
        
        internal static void UpdatePatientAccount(string inputCancelString)
        {
            Console.WriteLine(hintSelectPatientAccount);
            // username?
            var userAccount = EasyInput<UserAccount>.Select(AccountController.GetModifiableAccounts(), r => r.Username, inputCancelString);
            Console.WriteLine(userAccount.ToString());

            Console.WriteLine(hintSelectProperties);
            var propertiesToUpdate = SelectAccountProperties(inputCancelString);
            var updatedAccount = InputAccount(inputCancelString, propertiesToUpdate);
            AccountController.CopyAccount(userAccount, updatedAccount, propertiesToUpdate);
        }

        internal static void DeleteAccount(string inputCancelString)
        {
            Console.WriteLine(hintSelectAccounts);

            var selectedAccountsForDeletion = SelectModifiableAccounts(inputCancelString);
            foreach (var account in selectedAccountsForDeletion)
            {
                IS.Instance.UserAccountRepo.Remove(account);
                IS.Instance.PersonRepo.Remove(account.Person);
                
                Patient patient = IS.Instance.PatientRepo.GetByPersonId(account.Person.Id);
                IS.Instance.PatientRepo.Remove(patient);
            }		
        }
        
        private static List<UserAccount> SelectModifiableAccounts(string inputCancelString)
        {
            return EasyInput<UserAccount>.SelectMultiple(AccountController.GetModifiableAccounts(), r => r.Username, inputCancelString).ToList();
        }

        private static List<AccountController.AccountProperty> SelectAccountProperties(string inputCancelString)
        {
            return EasyInput<AccountController.AccountProperty>.SelectMultiple(
                AccountController.GetAccountProperties(),
                e => Enum.GetName(typeof(AccountController.AccountProperty), e),
                inputCancelString
            ).ToList();
        }

        private static UserAccount InputAccount(string inputCancelString, List<AccountController.AccountProperty> whichProperties)
        {
            UserAccount account = new UserAccount(UserAccount.AccountType.PATIENT);

            if (whichProperties.Contains(AccountController.AccountProperty.USERNAME))
            {
                Console.WriteLine(hintUsername);
                account.Username = InputUsername(inputCancelString);
            }

            if (whichProperties.Contains(AccountController.AccountProperty.PASSWORD))
            {
                Console.WriteLine(hintPassword);
                account.Password = InputPassword(inputCancelString);
            }
            
            if (whichProperties.Contains(AccountController.AccountProperty.FIRSTNAME))
            {
                Console.WriteLine(hintFirstName);
                account.Person.FirstName = InputFirstName(inputCancelString);
            }
            
            if (whichProperties.Contains(AccountController.AccountProperty.LASTNAME))
            {
                Console.WriteLine(hintLastName);
                account.Person.LastName = InputLastName(inputCancelString);
            }
            
            if (whichProperties.Contains(AccountController.AccountProperty.GENDER))
            {
                Console.WriteLine(hintGender);
                account.Person.Gender = InputGender(inputCancelString);
            }

            return account;
        }

        private static String InputUsername(string inputCancelString)
        {
            return EasyInput<string>.Get(
                new List<Func<string, bool>> {s => s.Length != 0},
                new[] { errAccountUsernameNonEmpty },
                inputCancelString
            );
        }
        
        private static String InputPassword(string inputCancelString)
        {
            return EasyInput<string>.Get(
                new List<Func<string, bool>> {s => s.Length != 0},
                new[] { errAccountPasswordNonEmpty },
                inputCancelString
            );
        }
        
        private static String InputFirstName(string inputCancelString)
        {
            return EasyInput<string>.Get(
                new List<Func<string, bool>> {s => s.Length != 0},
                new[] { errFirstNameNonEmpty },
                inputCancelString
            );
        }
        
        private static String InputLastName(string inputCancelString)
        {
            return EasyInput<string>.Get(
                new List<Func<string, bool>> {s => s.Length != 0},
                new[] { errLastNameNonEmpty },
                inputCancelString
            );
        }
        
        private static Person.PersonGender InputGender(string inputCancelString)
        {
            return EasyInput<Person.PersonGender>.Select(
                Enum.GetValues(typeof(Person.PersonGender)).Cast<Person.PersonGender>().ToList(),
                inputCancelString
            );
        }
    }
}
