using System;
using System.Diagnostics.CodeAnalysis;

namespace HIS.Core.PersonModel.DoctorModel.DoctorComparers
{
    public class DoctorCompareByRatingDesc : DoctorComparer
    {
        public override int Compare([AllowNull] Doctor x, [AllowNull] Doctor y)
        {
            //  TODO: Implement.
            //return -DoctorController.CalculateRating(x).CompareTo(DoctorController.CalculateRating(y));
            throw new NotImplementedException();
        }
    }
}
