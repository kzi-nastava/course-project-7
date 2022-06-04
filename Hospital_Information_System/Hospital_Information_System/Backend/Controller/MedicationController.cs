using System;
using System.Collections.Generic;
using System.Linq;
using HospitalIS.Backend.Util;

namespace HospitalIS.Backend.Controller
{
	internal static class MedicationController
	{
		internal enum MedicationProperty
		{
			NAME, INGREDIENTS
		}
		internal static readonly List<MedicationProperty> medicationPropertiesAll = Enum.GetValues(typeof(MedicationProperty)).Cast<MedicationProperty>().ToList();

		public static List<Medication> GetMedications()
		{
			return IS.Instance.Hospital.Medications.Where(meds => !meds.Deleted).ToList();
		}

		public static void Copy(Medication src, Medication dest, List<MedicationProperty> whichProperties)
		{
			if (whichProperties.Contains(MedicationProperty.NAME)) dest.Name = src.Name;
			if (whichProperties.Contains(MedicationProperty.INGREDIENTS)) {
				dest.Ingredients.Clear();
				dest.Ingredients.AddRange(src.Ingredients);
			}
		}
		
		public static bool IsMedicationSafe(List<Ingredient> medicationIngredients,
			List<Ingredient> patientsAllergies)
		{
			for (int i = 0; i < medicationIngredients.Count(); i++)
			{
				for (int j = 0; j < patientsAllergies.Count(); j++)
				{
					if (medicationIngredients[i] == patientsAllergies[j])
					{
						return false; 
					}
				}
			}
			return true;
		}
	}
}
