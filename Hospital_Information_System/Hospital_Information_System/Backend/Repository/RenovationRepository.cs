using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace HospitalIS.Backend.Repository
{
	internal class RenovationRepository : IRepository<Renovation>
	{
        public void Add(Renovation entity)
        {
            List<Renovation> Renovations = IS.Instance.Hospital.Renovations;

            entity.Id = Renovations.Count > 0 ? Renovations.Last().Id + 1 : 0;
            Renovations.Add(entity);
        }

        public Renovation GetById(int id)
        {
            return IS.Instance.Hospital.Renovations.First(e => e.Id == id);
        }

        public void Load(string fullFilename, JsonSerializerSettings settings)
        {
            IS.Instance.Hospital.Renovations = JsonConvert.DeserializeObject<List<Renovation>>(File.ReadAllText(fullFilename), settings);
        }

        public void Remove(Renovation entity)
        {
            entity.Deleted = true;
        }

        public void Remove(Func<Renovation, bool> condition)
        {
            IS.Instance.Hospital.Renovations.ForEach(entity => { if (condition(entity)) Remove(entity); });
        }

        public void Save(string fullFilename, JsonSerializerSettings settings)
        {
            File.WriteAllText(fullFilename, JsonConvert.SerializeObject(IS.Instance.Hospital.Renovations, Formatting.Indented, settings));
        }

        private void ExecuteSplit(Renovation renovation) {
            IS.Instance.RoomRepo.Add(renovation.Room1);
            IS.Instance.RoomRepo.Add(renovation.Room2);
            IS.Instance.RoomRepo.Remove(renovation.Room);
        }

        private void ExecuteMerge(Renovation renovation) {
            if (IS.Instance.RoomRepo.GetById(renovation.Room1.Id) == null)
            {
                IS.Instance.RoomRepo.Add(renovation.Room1);
            }

            foreach (var kv in renovation.Room.Equipment)
            {
                renovation.Room1.Equipment.Add(kv.Key, kv.Value);
            }

            IS.Instance.RoomRepo.Remove(renovation.Room);
        }

        public void Execute(Renovation renovation)
		{
			Thread.Sleep(Math.Max(renovation.GetTimeToLive(), 0));

			if (renovation.Deleted)
				return;

			if (!IS.Instance.Hospital.Renovations.Contains(renovation))
				throw new EntityNotFoundException();

			Console.WriteLine($"Finished renovation {renovation}.");

            if (renovation.IsSplitting()) 
            {
                ExecuteSplit(renovation);
            } 
            else if (renovation.IsMerging()) 
            {
                ExecuteMerge(renovation);
            }

			IS.Instance.RenovationRepo.Remove(renovation);       
		}

		public void AddTask(Renovation renovation)
		{
			Task t = new Task(() => Execute(renovation));
			IS.Instance.Hospital.RenovationTasks.Add(t);
			t.Start();
		}
    }
}
