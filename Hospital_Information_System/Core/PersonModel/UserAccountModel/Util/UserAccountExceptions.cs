using System;

namespace HIS.Core.PersonModel.UserAccountModel.Util
{
    public class UserAccountForcefullyBlockedException : Exception
    {
        public UserAccountForcefullyBlockedException(string exceptionMessage)
            : base($"User account blocked: {exceptionMessage}")
        {
        }
    }

    public class InvalidLoginAttemptException : Exception
    {
        public InvalidLoginAttemptException(string errorMessage) : base($"Login failed: {errorMessage}.")
        {

        }
    }
}
