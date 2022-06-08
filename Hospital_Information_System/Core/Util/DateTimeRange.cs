using System;
using System.Diagnostics;

namespace HIS.Core.Util
{
	public struct DateTimeRange
	{
		public DateTime Start;
		public DateTime End;

		public DateTimeRange(DateTime start, DateTime end)
		{
			Start = start;
			End = end;
		}

		public int CompareTo(DateTimeRange other)
		{
			if (Start < other.Start)
				return -1;
			if (Start == other.Start)
				return End.CompareTo(other.End);
			return 1;
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