using System;
using System.Collections.Generic;
using System.Linq;

namespace HospitalIS.Backend.Controller
{
	internal static class RenovationController
	{
		public static List<Renovation> GetRenovations()
		{
			return IS.Instance.Hospital.Renovations.Where(ren => !ren.Deleted).ToList();
		}
		public static bool IsRenovating(Room room, DateTime timestamp)
		{
			return GetRenovations().Count(ren => ren.Room == room && ren.Start <= timestamp && timestamp <= ren.End) > 0;
		}
	}
}
