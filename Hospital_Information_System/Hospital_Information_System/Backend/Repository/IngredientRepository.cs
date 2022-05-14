using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace HospitalIS.Backend.Repository
{
    public class IngredientRepository: IRepository<Ingredient>
    {
        public void Add(Ingredient entity)
        {
            List<Ingredient> Ingredients = IS.Instance.Hospital.Ingredients;

            entity.Id = Ingredients.Count > 0 ? Ingredients.Last().Id + 1 : 0;
            Ingredients.Add(entity);
        }

        public Ingredient GetById(int id)
        {
            return IS.Instance.Hospital.Ingredients.First(e => e.Id == id);
        }

        public void Load(string fullFilename, JsonSerializerSettings settings)
        {
            IS.Instance.Hospital.Ingredients =
                JsonConvert.DeserializeObject<List<Ingredient>>(File.ReadAllText(fullFilename), settings);
        }

        public void Remove(Ingredient entity)
        {
            entity.Deleted = true;
        }

        public void Remove(Func<Ingredient, bool> condition)
        {
            IS.Instance.Hospital.Ingredients.ForEach(entity =>
            {
                if (condition(entity)) Remove(entity);
            });
        }

        public void Save(string fullFilename, JsonSerializerSettings settings)
        {
            File.WriteAllText(fullFilename,
                JsonConvert.SerializeObject(IS.Instance.Hospital.Ingredients, Formatting.Indented, settings));
        }

        internal class IngredientReferenceConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(Ingredient);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                JsonSerializer serializer)
            {
                var ingredientID = serializer.Deserialize<int>(reader);
                return IS.Instance.Hospital.Ingredients.First(ingredient => ingredient.Id == ingredientID);
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, ((Ingredient) value).Id);
            }
        }

        internal class IngredientListConverter : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                var ingredients = new List<int>();
                foreach (Ingredient ingredient in (List<Ingredient>) value)
                {
                    ingredients.Add(ingredient.Id);
                }

                serializer.Serialize(writer, ingredients);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                JsonSerializer serializer)
            {
                List<int> ingredientsRefs = serializer.Deserialize<List<int>>(reader);
                var ingredients = new List<Ingredient>();
                foreach (int ingredientsRef in ingredientsRefs)
                {
                    ingredients.Add(IS.Instance.IngredientRepo.GetById(ingredientsRef));
                }

                return ingredients;
            }

            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(List<Ingredient>);
            }
        }
    }
}