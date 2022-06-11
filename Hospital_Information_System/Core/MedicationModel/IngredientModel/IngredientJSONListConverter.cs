using HIS.Core.Util;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Core.MedicationModel.IngredientModel
{
	internal class IngredientJSONIListConverter : JsonConverter
	{
		internal static IIngredientRepository Repo { get; set; }

		public IngredientJSONIListConverter()
		{
			if (Repo == null) throw new JSONRepoReferenceNullException();
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var writableList = ((IList<Ingredient>)value).Select(ing => ing.Id);
			serializer.Serialize(writer, writableList);
		}

		public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer)
		{
			var readableList = serializer.Deserialize<IList<int>>(reader);
			return readableList.Select(ingId => Repo.Get(ingId));
		}

		public override bool CanConvert(System.Type objectType)
		{
			return objectType == typeof(IList<int>);
		}
	}
}
