
using System;
using System.Diagnostics;

namespace HospitalIS.Backend.Util
{
	struct DateTimeRange
	{
		public DateTime Start;
		public DateTime End;

		public DateTimeRange(DateTime start, DateTime end)
		{
			Debug.Assert(start < end);
			Start = start;
			End = end;
		}

		public bool Contains(DateTime dt)
		{
			return Start <= dt && dt <= End;
		}

		public override string ToString()
		{
			const string format = "dd.MM.yyyy. HH:mm:ss";

			string start = Start == DateTime.MinValue ? "" : $"{Start.ToString(format)}";
			string end = End == DateTime.MaxValue ? "" : $"{End.ToString(format)}";

			return $"({start} - {end})";
		}
	}
}