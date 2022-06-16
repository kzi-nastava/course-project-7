using System;
using System.Collections.Generic;
using System.Text;

namespace HIS.Core.PollModel
{
	public static class PollHelpers
	{
		public static Dictionary<string, IList<double>> ReduceToRatings(IEnumerable<Poll> polls)
		{
			var summaryAverageCollected = new Dictionary<string, IList<double>>();
			foreach (var poll in polls)
			{
				var questions = poll.GetQuestions();

				foreach (var q in questions)
				{
					if (!summaryAverageCollected.ContainsKey(q))
						summaryAverageCollected[q] = new List<double>();
					summaryAverageCollected[q].Add(poll.GetRating(q));
				}
			}

			return summaryAverageCollected;
		}
	}
}
