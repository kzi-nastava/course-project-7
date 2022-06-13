using System.Diagnostics.CodeAnalysis;

namespace HIS.Core.DoctorModel.DoctorComparer
{
    class DoctorCompareByFirstName : DoctorComparer
    {
        public override int Compare([AllowNull] Doctor x, [AllowNull] Doctor y)
        {
            return x.Person.FirstName.CompareTo(y.Person.FirstName);
        }
    }
}
