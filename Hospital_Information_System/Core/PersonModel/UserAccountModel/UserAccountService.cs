using System;
using System.Collections.Generic;
using System.Text;

namespace HIS.Core.UserAccountModel
{
	public class UserAccountService : IUserAccountService
	{
		private readonly IUserAccountRepository _repo;

		public UserAccountService(IUserAccountRepository repo)
		{
			_repo = repo;
		}

		public IEnumerable<UserAccount> GetAll()
		{
			return _repo.GetAll();
		}
	}
}
