using System.Collections.Generic;

namespace HIS.Core.PollModel.HospitalPollModel
{
    public static class HospitalPollHelpers
    {
        public const string QServiceQuality = "Service quality";
        public const string QHygiene = "Hygiene";
        public const string QSatisfaction = "Satisfaction";
        public const string QWouldRecommend = "Would recommend";

        public static readonly IEnumerable<string> Questions = new List<string>()
        {
            QServiceQuality, QHygiene, QSatisfaction, QWouldRecommend
        };
    }
}
