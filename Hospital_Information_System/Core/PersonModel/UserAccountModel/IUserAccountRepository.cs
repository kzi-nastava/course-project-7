using System;
using System.Collections.Generic;

namespace HIS.Core.PersonModel.UserAccountModel
{
	public interface IUserAccountRepository
	{
		public int GetNextId();
		public void Save();
		public IEnumerable<UserAccount> GetAll();
		public UserAccount Get(int id);
		public UserAccount Add(UserAccount obj);
		public void Remove(UserAccount obj);
	}
}
