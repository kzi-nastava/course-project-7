using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using HIS.Core.AppointmentModel;
using HIS.Core.PersonModel;
using HIS.Core.PersonModel.UserAccountModel;
using HIS.Core.PersonModel.UserAccountModel.Util;

namespace HIS.CLI.View
{
	internal class UserAccountView : AbstractView
	{
		private readonly IUserAccountService _service;

		private const string hintInputUsername = "Enter username: ";
		private const string hintInputPassword = "Enter password: ";
		
		private const string hintSelectAccounts = "Select accounts by their number, separated by whitespace.\nEnter a newline to finish";
		private const string hintSelectProperties = "Select properties by their number, separated by whitespace.\nEnter a newline to finish";
		private const string hintSelectPatientAccount = "Select account, by its number.\nEnter a newline to finish";
		private const string hintAccountUpdated = "You've successfully updated an account!";
		private const string hintUsername = "Enter account username: ";
		private const string hintPassword = "Enter account password: ";
		private const string hintFirstName = "Enter first name: ";
		private const string hintLastName = "Enter last name: ";
		private const string hintGender = "Select gender: ";
		private const string errAccountUsernameNonEmpty = "Username must not be empty!";
		private const string errAccountPasswordNonEmpty = "Password must not be empty!";
		private const string errFirstNameNonEmpty = "First name must not be empty!";
		private const string errLastNameNonEmpty = "Last name must not be empty!";

		public UserAccountView(IUserAccountService service)
		{
			_service = service;
		}

		internal UserAccount CmdLogin()
		{
			while (true)
			{
				Hint(hintInputUsername);
				string username = EasyInput<string>.Get(_cancel);

				Hint(hintInputPassword);
				string password = EasyInput<string>.Get(_cancel);

				try
				{
					UserAccount account = _service.AttemptLogin(username, password);
					return account;
				}
				catch (InvalidLoginAttemptException e)
				{
					Error(e.Message);
				}
			}
		}

		internal void CmdCreate()
		{
			try
			{
				UserAccount account = InputAccount(UserAccountPropertyHelpers.GetProperties());
				_service.Add(account);
			}
			catch (NothingToSelectException e)
			{
				Error(e.Message);
			}
		}

		internal void CmdRead()
		{
			foreach (UserAccount account in _service.GetAll().ToList())
			{
				if (account.Type == UserAccount.AccountType.PATIENT)
					Print(account.ToString());	
			}
		}
		
		internal void CmdUpdate()
		{
			try
			{
				UserAccount account = SelectModifiableAccount();
				var selectedProperties = SelectModifiableProperties();
				var updatedAccount = InputAccount(selectedProperties);
				_service.Update(account, updatedAccount, selectedProperties);
				Hint(hintAccountUpdated);
			}
			catch ( NothingToSelectException e)
			{
				Error(e.Message);
			}
		}
		internal void CmdDelete()
		{
			var selectedAccounts = SelectModifiableAccounts();
			foreach (UserAccount account in selectedAccounts)
			{
				_service.Remove(account);
			}

		}
		internal void CmdBlock()
		{
			try
			{
				var selectedAccounts = SelectBlockableAccounts();
				foreach (UserAccount account in selectedAccounts)
				{
					account.Blocked = UserAccount.BlockedBy.SECRETARY;
				}
			}
			catch (NothingToSelectException e)
			{
				Error(e.Message);
			}
		}
		internal void CmdUnblock()
		{
			try
			{
				var selectedAccounts = SelectUnblockableAccounts();
				foreach (UserAccount account in selectedAccounts)
				{
					account.Blocked = UserAccount.BlockedBy.NONE;
				}
			}
			catch (NothingToSelectException e)
			{
				Error(e.Message);
			}
		}

		private UserAccount SelectModifiableAccount()
		{
			Hint(hintSelectPatientAccount);
			return EasyInput<UserAccount>.Select(_service.GetModifiable(User), _cancel);
		}
		
		private List<UserAccount> SelectModifiableAccounts()
		{
			Hint(hintSelectAccounts);
			return EasyInput<UserAccount>.SelectMultiple(_service.GetModifiable(User).ToList(), _cancel).ToList();
		}

		private IEnumerable<AccountProperty> SelectModifiableProperties()
		{
			Hint(hintSelectProperties);
			return EasyInput<AccountProperty>.SelectMultiple(UserAccountPropertyHelpers.GetProperties().ToList(),
				acc => acc.ToString(), _cancel);
		}

		private IEnumerable<UserAccount> SelectBlockableAccounts()
		{
			Hint(hintSelectPatientAccount);
			return EasyInput<UserAccount>.SelectMultiple(_service.GetNotBlockedPatientAccounts().ToList(), _cancel).ToList();
		}
		
		private IEnumerable<UserAccount> SelectUnblockableAccounts()
		{
			Hint(hintSelectPatientAccount);
			return EasyInput<UserAccount>.SelectMultiple(_service.GetBlockedPatientAccounts().ToList(), _cancel).ToList();
		}
		
		private UserAccount InputAccount(IEnumerable<AccountProperty> whichProperties)
		{
			UserAccount account = new UserAccount(UserAccount.AccountType.PATIENT);

			if (whichProperties.Contains(AccountProperty.USERNAME))
			{
				Hint(hintUsername);
				account.Username = InputUsername();
			}

			if (whichProperties.Contains(AccountProperty.PASSWORD))
			{
				Hint(hintPassword);
				account.Password = InputPassword();
			}
            
			if (whichProperties.Contains(AccountProperty.FIRSTNAME))
			{
				Hint(hintFirstName);
				account.Person.FirstName = InputFirstName();
			}
            
			if (whichProperties.Contains(AccountProperty.LASTNAME))
			{
				Hint(hintLastName);
				account.Person.LastName = InputLastName();
			}
            
			if (whichProperties.Contains(AccountProperty.GENDER))
			{
				Hint(hintGender);
				account.Person.Gender = InputGender();
			}

			return account;
		}
		
		private static string InputUsername()
		{
			return EasyInput<string>.Get(
				new List<Func<string, bool>>() {s => s.Length != 0},
				new[] { errAccountUsernameNonEmpty },
				_cancel
			);
		}
		private static string InputPassword()
		{
			return EasyInput<string>.Get(
				new List<Func<string, bool>>() {s => s.Length != 0},
				new[] { errAccountPasswordNonEmpty },
				_cancel
			);
		}
		private static string InputFirstName()
		{
			return EasyInput<string>.Get(
				new List<Func<string, bool>>() {s => s.Length != 0},
				new[] { errFirstNameNonEmpty },
				_cancel
			);
		}
        
		private static string InputLastName()
		{
			return EasyInput<string>.Get(
				new List<Func<string, bool>>() {s => s.Length != 0},
				new[] { errLastNameNonEmpty },
				_cancel
				
			);
		}
        
		private static Person.PersonGender InputGender()
		{
			return EasyInput<Person.PersonGender>.Select(
				Enum.GetValues(typeof(Person.PersonGender)).Cast<Person.PersonGender>().ToList(),
				_cancel
			);
		}
	}
}
