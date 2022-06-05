using System;
using System.Collections.Generic;
using System.Text;

namespace HIS.Core.EquipmentModel.EquipmentRelocationModel
{
	public interface IEquipmentRelocationRepository
	{
		public void Save();
		public IEnumerable<EquipmentRelocation> Get();
		public EquipmentRelocation Get(int id);
		public void Add(EquipmentRelocation obj);
		public void Remove(EquipmentRelocation obj);
	}
}
