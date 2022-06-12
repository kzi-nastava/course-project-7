using System;

namespace HIS.Core.UserAccountModel.UserAccountExceptions
{
    public class UserAccountForcefullyBlockedException : Exception
    {
        public UserAccountForcefullyBlockedException(string exceptionMessage)
            : base($"User account blocked: {exceptionMessage}")
        {
        }
    }
}
