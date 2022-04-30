using System;
using System.Collections.Generic;
using System.Linq;

namespace HospitalIS.Backend.Controller
{
    public class AccountController
    {
        public enum AccountProperty
        {
            USERNAME,
            PASSWORD,
            FIRSTNAME,
            LASTNAME,
            GENDER
        }
        
        public static List<AccountProperty> GetAccountProperties()
        {
            return Enum.GetValues(typeof(AccountProperty)).Cast<AccountProperty>().ToList();
        }
        
        public static void CopyAccount(UserAccount target, UserAccount source, List<AccountProperty> whichProperties)
        {
            if (whichProperties.Contains(AccountProperty.USERNAME)) target.Username = source.Username;
            if (whichProperties.Contains(AccountProperty.PASSWORD)) target.Password = source.Password;
            if (whichProperties.Contains(AccountProperty.FIRSTNAME)) target.Person.FirstName = source.Person.FirstName;
            if (whichProperties.Contains(AccountProperty.LASTNAME)) target.Person.LastName = source.Person.LastName;
            if (whichProperties.Contains(AccountProperty.GENDER)) target.Person.Gender = source.Person.Gender;
        }
        
        public static List<UserAccount> GetModifiableAccounts()
        {
            return IS.Instance.Hospital.UserAccounts.Where(account => !account.Deleted && account.Type == UserAccount.AccountType.PATIENT).ToList();
        }
        
        public static List<UserAccount> GetAccounts()
        {
            return IS.Instance.Hospital.UserAccounts.Where(account => !account.Deleted).ToList();
        }
        
    }
}