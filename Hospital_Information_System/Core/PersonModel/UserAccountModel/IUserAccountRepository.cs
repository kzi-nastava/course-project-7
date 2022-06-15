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
		public IEnumerable<UserAccount> GetByUsername(string username);
		public IEnumerable<UserAccount> GetModifiable(UserAccount user);
		public IEnumerable<UserAccount> GetNotBlockedPatientAccounts();
		public IEnumerable<UserAccount> GetBlockedPatientAccounts();
	}
}
