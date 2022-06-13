using System.Diagnostics.CodeAnalysis;

namespace HIS.Core.PersonModel.DoctorModel.DoctorComparers
{
    public class DoctorCompareByLastName : DoctorComparer
    {
        public override int Compare([AllowNull] Doctor x, [AllowNull] Doctor y)
        {
            return x.Person.LastName.CompareTo(y.Person.LastName);
        }
    }
}
