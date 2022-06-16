using System.Collections.Generic;
using System.Linq;
using HIS.Core.Util;

namespace HIS.Core.PersonModel.UserAccountModel
{
    public static class UserAccountPropertyHelpers
    {
        public static IEnumerable<AccountProperty> GetProperties()
        {
            return Utility.GetEnumValues<AccountProperty>();
        }
    }
}