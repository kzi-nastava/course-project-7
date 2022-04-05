using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Hospital_Information_System
{
    class Person
    {
        public String Name { get; set; }
        public int Age { get; set; }
        public List<int> FavNums { get; set; }
        public override String ToString()
        {
            return "Person(" + Name + ", " + Age + ", [" + string.Join(", ", FavNums) + "]" + ")";
        }
        public Person()
        {

        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            const String dataFolder = "../../../data/";

            String jsonText = System.IO.File.ReadAllText(dataFolder + "test.json");
            Person obj = JsonConvert.DeserializeObject<Person>(jsonText);
            Console.WriteLine(obj);

            Person obj2 = new Person
            {
                Name = "Mary",
                Age = 21,
                FavNums = new List<int> { 10, 20, 30, 40, 50 }
            };
            String jsonText2 = JsonConvert.SerializeObject(obj2);
            System.IO.File.WriteAllText(dataFolder + "test2.json", jsonText2);
        }
    }
}
