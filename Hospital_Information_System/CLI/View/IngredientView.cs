using HIS.Core.MedicationModel;
using HIS.Core.MedicationModel.IngredientModel;
using HIS.Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.CLI.View
{
	internal class IngredientView : View
	{
		private static readonly string errNameTaken = "Name already taken";
		private static readonly string hintName = "Enter name";
		private static readonly string warnDependentMedications = "The following medications will also be removed. Proceed?";

		private IIngredientService _service;
		private IMedicationService _medicationService;
		private IEnumerable<IngredientProperty> _properties;

		public IngredientView(IIngredientService service, IMedicationService medicationService)
		{
			_service = service;
			_medicationService = medicationService;
			_properties = Utility.GetEnumValues<IngredientProperty>();
		}

		internal void CmdAdd()
		{
			var newIngredient = Input(_properties);
			_service.Add(newIngredient);
		}

		internal void CmdRead()
		{
			var selected = EasyInput<Ingredient>.Select(_service.GetAll(), _cancel);
			Print(selected.ToString());
		}

		internal void CmdUpdate()
		{
			var ingredientToChange = EasyInput<Ingredient>.Select(_service.GetAll(), _cancel);
			var selectedProperties = EasyInput<IngredientProperty>.SelectMultiple(_properties.ToList(), _cancel);
			var newIngredient = Input(selectedProperties);
			_service.Copy(newIngredient, ingredientToChange, selectedProperties);
		}

		internal void CmdDelete()
		{
			var selected = EasyInput<Ingredient>.SelectMultiple(_service.GetAll().ToList(), _cancel);
			var dependentMedications = selected.SelectMany(ing => _medicationService.GetAllThatUse(ing)).Distinct();

			if (dependentMedications.Count() != 0)
			{
				Hint(warnDependentMedications);
				Print(dependentMedications.Select(med => med.Name).Aggregate((s1, s2) => s1 + ", " + s2));
				if (!EasyInput<bool>.YesNo(_cancel))
				{
					return;
				}
			}

			foreach (var med in dependentMedications)
			{
				_medicationService.Remove(med);
			}

			foreach (var s in selected)
			{
				_service.Remove(s);
			}
		}

		private Ingredient Input(IEnumerable<IngredientProperty> whichProperties)
		{
			Ingredient result = new Ingredient();

			if (whichProperties.Contains(IngredientProperty.NAME))
			{
				Hint(hintName);
				result.Name = InputName();
			}

			return result;
		}

		private string InputName()
		{
			return EasyInput<string>.Get(
				new List<Func<string, bool>>() { s => _service.GetByName(s).Count() == 0 },
				new[] { errNameTaken },
				_cancel
			);
		}
	}
}
