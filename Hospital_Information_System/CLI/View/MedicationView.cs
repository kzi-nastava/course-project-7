using HIS.Core.MedicationModel;
using HIS.Core.MedicationModel.IngredientModel;
using HIS.Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.CLI.View
{
	internal class MedicationView : View
	{
		private readonly IMedicationService _service;
		private readonly IIngredientService _ingredientService;
		private IEnumerable<MedicationProperty> _properties;

		private static readonly string hintInputName = "Enter name";
		private static readonly string hintSelectIngredients = "Select ingredients";
		private static readonly string errNameExists = "Name already exists";


		public MedicationView(IMedicationService service, IIngredientService ingredientService)
		{
			_service = service;
			_ingredientService = ingredientService;
			_properties = Utility.GetEnumValues<MedicationProperty>();
		}

		internal void CmdCreateAndSendForReview()
		{
			var medication = Input(_properties);
			_service.Add(medication);
			Print("TODO: Send for review instead of directly adding to database");
		}

		private Medication Input(IEnumerable<MedicationProperty> whichProperties)
		{
			Medication m = new Medication();

			if (whichProperties.Contains(MedicationProperty.NAME))
			{
				Hint(hintInputName);
				m.Name = InputName();
			}
			if (whichProperties.Contains(MedicationProperty.INGREDIENTS))
			{
				Hint(hintSelectIngredients);
				m.Ingredients = InputIngredients().ToList();
			}

			return m;
		}

		private string InputName()
		{
			return EasyInput<string>.Get(
				new List<Func<string, bool>>() { s => _service.GetByName(s).Count() == 0 },
				new[] { errNameExists },
				_cancel
			);
		}

		private IList<Ingredient> InputIngredients()
		{
			return EasyInput<Ingredient>.SelectMultiple(_ingredientService.GetAll().ToList(), _cancel);
		}
	}
}
