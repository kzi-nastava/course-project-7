using System.Diagnostics.CodeAnalysis;

namespace HIS.Core.PersonModel.DoctorModel.DoctorComparers
{
    public class DoctorCompareByRatingDesc : DoctorComparer
    {
        private readonly IDoctorService _doctorService;

        public DoctorCompareByRatingDesc(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        public override int Compare([AllowNull] Doctor x, [AllowNull] Doctor y)
        {
            return - _doctorService.CalculateRating(x).CompareTo(_doctorService.CalculateRating(y));
        }
    }
}
