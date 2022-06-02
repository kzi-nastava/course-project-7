using System;
using System.Collections.Generic;
using System.Linq;

namespace HospitalIS.Backend
{
    static class Utility
    {
        public static List<T> GetEnumValues<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToList();
        }
    }
}
