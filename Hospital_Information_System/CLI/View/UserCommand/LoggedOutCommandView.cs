using HIS.Core.PersonModel.UserAccountModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace HIS.CLI.View.UserCommand
{
	internal class LoggedOutCommandView : UserCommandView
	{
		public LoggedOutCommandView(UserAccount user, UserAccountView userAccountView) : base(user)
		{
			AddCommands(new Dictionary<string, Action>
			{
				{ "login", () => LogIn(userAccountView) }
			});
		}
		
		private void LogIn(UserAccountView userAccountView)
		{
			var newAccount = userAccountView.CmdLogin();
			_user.Username = newAccount.Username;
			_user.Password = newAccount.Password;
			_user.Blocked = newAccount.Blocked;
			_user.Deleted = newAccount.Deleted;
			_user.Id = newAccount.Id;
			_user.Person = newAccount.Person;
			_user.Type = newAccount.Type;
		}
	}
}
