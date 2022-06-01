using System.Collections.Generic;
using Newtonsoft.Json;
using HospitalIS.Backend.Repository;

namespace HospitalIS.Backend
{
    public class Medication : Entity
    {
        public string Name { get; set; }
        
        [JsonConverter(typeof(IngredientRepository.IngredientListConverter))]
        public List<Ingredient> Ingredients { get; set; }

        public Medication()
        {
        }

        public Medication(string name, List<Ingredient> ingredients)
        {
            Name = name;
            Ingredients = ingredients;
        }
        
        public override string ToString()
        {
            return $"Medication{{Id = {Id}, Name ={Name}, Ingredients = {ConvertIngredientListToString(Ingredients)}}}";
        }
        
        internal static string ConvertIngredientListToString(List<Ingredient> entry)
        {
            string result = "";
            for (int i = 0; i <= entry.Count - 1; i++)
            {
                result += entry[i].Name;
                if (i < entry.Count - 1)
                {
                    result += ", ";
                }
            }

            return result;
        }
    }
}