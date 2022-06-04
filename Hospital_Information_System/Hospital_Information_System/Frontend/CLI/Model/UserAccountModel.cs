using HospitalIS.Backend;
using System;
using System.Collections.Generic;
using System.Linq;
using HospitalIS.Backend.Controller;
using HospitalIS.Backend.Repository;

namespace HospitalIS.Frontend.CLI.Model
{
    internal abstract class UserAccountModel
    {
        private const string hintInputUsername = "Enter username: ";
        private const string hintInputPassword = "Enter password: ";
        
        private const string hintSelectAccount = "Select account by its number.\nEnter a newline to finish";
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
        
        internal static void BlockPatientAccount(string inputCancelString)
        {
            Console.WriteLine(hintSelectAccounts);
            
            var selectedAccountsForBlocking = SelectBlockableAccounts(inputCancelString);
            foreach (var account in selectedAccountsForBlocking)
            {
                IS.Instance.UserAccountRepo.BlockBySecretary(account);
            }
        }
        
        internal static void UnblockPatientAccount(string inputCancelString)
        {
            Console.WriteLine(hintSelectAccounts);
            
            var selectedAccountsForUnblocking = SelectUnblockableAccounts(inputCancelString);
            foreach (var account in selectedAccountsForUnblocking)
            {
                IS.Instance.UserAccountRepo.Unblock(account);
            }
        }
        
        internal static void CreatePatientAccount(string inputCancelString)
        {
            UserAccount user = InputAccount(inputCancelString, UserAccountController.GetAccountProperties());
            Patient patient = new Patient(user.Person);
            IS.Instance.MedicalRecordRepo.Add(new MedicalRecord(patient));
            IS.Instance.UserAccountRepo.Add(user);
            IS.Instance.PersonRepo.Add(user.Person);
            IS.Instance.PatientRepo.Add(patient);
        }
        
        internal static void ViewPatientAccounts()
        {
            foreach (var account in IS.Instance.Hospital.UserAccounts)
            {
                if (account.Type == UserAccount.AccountType.PATIENT)
                    Console.WriteLine(account.ToString());
            }
            
        }
        
        internal static void UpdatePatientAccount(string inputCancelString)
        {
            Console.WriteLine(hintSelectPatientAccount);
            
            var userAccount = EasyInput<UserAccount>.Select(UserAccountController.GetModifiableAccounts(), r => r.Username, inputCancelString);
            Console.WriteLine(userAccount.ToString());

            Console.WriteLine(hintSelectProperties);
            var propertiesToUpdate = SelectAccountProperties(inputCancelString);
            var updatedAccount = InputAccount(inputCancelString, propertiesToUpdate);
            UserAccountController.CopyAccount(userAccount, updatedAccount, propertiesToUpdate);
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
        
        private static List<UserAccount> SelectBlockableAccounts(string inputCancelString)
        {
            return EasyInput<UserAccount>.SelectMultiple(UserAccountController.GetNotBlockedPatientAccounts(), r => r.Username, inputCancelString).ToList();
        }
        
        private static List<UserAccount> SelectUnblockableAccounts(string inputCancelString)
        {
            return EasyInput<UserAccount>.SelectMultiple(UserAccountController.GetBlockedAccounts(), r => r.Username, inputCancelString).ToList();
        }
        
        private static List<UserAccount> SelectModifiableAccounts(string inputCancelString)
        {
            return EasyInput<UserAccount>.SelectMultiple(UserAccountController.GetModifiableAccounts(), r => r.Username, inputCancelString).ToList();
        }

        private static List<UserAccountController.AccountProperty> SelectAccountProperties(string inputCancelString)
        {
            return EasyInput<UserAccountController.AccountProperty>.SelectMultiple(
                UserAccountController.GetAccountProperties(),
                e => Enum.GetName(typeof(UserAccountController.AccountProperty), e),
                inputCancelString
            ).ToList();
        }

        private static UserAccount InputAccount(string inputCancelString, List<UserAccountController.AccountProperty> whichProperties)
        {
            UserAccount account = new UserAccount(UserAccount.AccountType.PATIENT);

            if (whichProperties.Contains(UserAccountController.AccountProperty.USERNAME))
            {
                Console.WriteLine(hintUsername);
                account.Username = InputUsername(inputCancelString);
            }

            if (whichProperties.Contains(UserAccountController.AccountProperty.PASSWORD))
            {
                Console.WriteLine(hintPassword);
                account.Password = InputPassword(inputCancelString);
            }
            
            if (whichProperties.Contains(UserAccountController.AccountProperty.FIRSTNAME))
            {
                Console.WriteLine(hintFirstName);
                account.Person.FirstName = InputFirstName(inputCancelString);
            }
            
            if (whichProperties.Contains(UserAccountController.AccountProperty.LASTNAME))
            {
                Console.WriteLine(hintLastName);
                account.Person.LastName = InputLastName(inputCancelString);
            }
            
            if (whichProperties.Contains(UserAccountController.AccountProperty.GENDER))
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
