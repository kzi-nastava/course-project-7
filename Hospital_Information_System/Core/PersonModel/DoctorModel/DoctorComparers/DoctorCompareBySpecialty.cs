using System.Diagnostics.CodeAnalysis;

namespace HIS.Core.PersonModel.DoctorModel.DoctorComparers
{
    public class DoctorCompareBySpecialty : DoctorComparer
    {
        public override int Compare([AllowNull] Doctor x, [AllowNull] Doctor y)
        {
            return x.Specialty.CompareTo(y.Specialty);
        }
    }
}
