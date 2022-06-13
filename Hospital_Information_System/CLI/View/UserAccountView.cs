using HIS.Core.PersonModel.UserAccountModel;
using HIS.Core.PersonModel.UserAccountModel.Util;

namespace HIS.CLI.View
{
	internal class UserAccountView : View
	{
		private readonly IUserAccountService _service;

		private const string hintInputUsername = "Enter username: ";
		private const string hintInputPassword = "Enter password: ";

		public UserAccountView(IUserAccountService service, UserAccount user) : base(user)
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
	}
}
