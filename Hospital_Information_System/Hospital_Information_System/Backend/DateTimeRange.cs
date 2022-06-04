using System;
using System.Diagnostics;

namespace HospitalIS.Backend
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

		public bool Intersects(DateTimeRange other)
		{
			return (Start >= other.Start && Start <= other.End
			|| End >= other.Start && End <= other.End
			|| other.Start >= Start && other.Start <= End
			|| other.End >= Start && other.End <= End
			);
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