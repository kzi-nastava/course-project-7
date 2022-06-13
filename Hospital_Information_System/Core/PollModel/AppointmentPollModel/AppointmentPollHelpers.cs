using System.Collections.Generic;

namespace HIS.Core.PollModel.AppointmentPollModel
{
    public static class AppointmentPollHelpers
    {
        public const string QServiceQuality = "Service quality";
        public const string QWouldRecommend = "Would recommend";

        public static readonly IEnumerable<string> Questions = new List<string>()
        {
            QServiceQuality, QWouldRecommend
        };
    }
}
