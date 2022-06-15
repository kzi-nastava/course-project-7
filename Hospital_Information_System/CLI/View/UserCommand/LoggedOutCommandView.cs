using HIS.Core.PersonModel.UserAccountModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace HIS.CLI.View.UserCommand
{
	internal class LoggedOutCommandView : UserCommandView
	{
		public LoggedOutCommandView(UserAccountView userAccountView)
		{
			AddCommands(new Dictionary<string, Action>
			{
				{ "login", () => LogIn(userAccountView) }
			});
			RemoveCommands(new List<string>
			{
				"logout"
			});
		}
		
		private void LogIn(UserAccountView userAccountView)
		{
			User = userAccountView.CmdLogin();
		}
	}
}
