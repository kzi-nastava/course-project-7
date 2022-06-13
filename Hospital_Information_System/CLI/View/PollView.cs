using HIS.Core.PersonModel.UserAccountModel;
using HIS.Core.PollModel;
using System;
using System.Collections.Generic;

namespace HIS.CLI.View
{
	internal class PollView : View
	{
		public PollView(UserAccount user) : base(user)
		{

		}

		internal Dictionary<string, int> GenerateQuestionnaire(IEnumerable<string> questions)
		{
			var questionnaire = new Dictionary<string, int>();
			foreach (string question in questions)
			{
				Hint(question);
				int rating = EasyInput<int>.Get(
					new List<Func<int, bool>> { r => Poll.IsValidRating(r) },
					new string[] { Poll.ErrInvalidRating },
					_cancel);
				questionnaire[question] = rating;
			}
			return questionnaire;
		}
	}
}
