using System;
using System.Collections.Generic;
using System.Text;

namespace HIS.Core.UserAccountModel
{
    public interface IUserAccountService
    {
        IEnumerable<UserAccount> GetAll();
    }
}
