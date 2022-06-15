using HIS.Core.EquipmentModel;
using HIS.Core.PersonModel.UserAccountModel;
using HIS.Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.CLI.View
{
	internal class EquipmentView : AbstractView
	{
		private static readonly string hintEnterQuery = "Enter search query";

		private readonly IEquipmentService _service;
		private readonly IList<EquipmentProperty> _searchableProperties;
		private readonly IEnumerable<EquipmentType> _equipmentTypes;
		private readonly IEnumerable<EquipmentUse> _equipmentUses;

		internal EquipmentView(IEquipmentService service)
		{
			_service = service;
			_searchableProperties = Utility.GetEnumValues<EquipmentProperty>().ToList();
			_equipmentTypes = Utility.GetEnumValues<EquipmentType>();
			_equipmentUses = Utility.GetEnumValues<EquipmentUse>();
		}

		internal void CmdSearch()
		{
			var propertiesToSearchOn = EasyInput<EquipmentProperty>.SelectMultiple(_searchableProperties, _cancel);

			Hint(hintEnterQuery);
			var searchQuery = EasyInput<string>.Get(_cancel);
			var searchResult = _service.Search(searchQuery, propertiesToSearchOn);

			foreach (var equipment in searchResult)
			{
				Print(equipment.ToString());
			}
		}

		internal void CmdFilter()
		{
			var filterMapping = new Dictionary<string, Func<IEnumerable<Equipment>>>
			{
				["Filter by type"] = () => _service.FilterByType(SelectType()),
				["Filter by use"] = () => _service.FilterByUse(SelectUse()),
				["Filter by out of stock"] = () => _service.FilterByAmount(num => num == 0),
				["Filter by less than 10"] = () => _service.FilterByAmount(num => num >= 0 && num < 10),
				["Filter by more than 10"] = () => _service.FilterByAmount(num => num >= 10),
			};

			var filterQuery = EasyInput<string>.Select(filterMapping.Keys, _cancel);
			var filterResult = filterMapping[filterQuery]();

			foreach (var equipment in filterResult)
			{
				Print(equipment.ToString() + $" ({_service.GetTotalSupply(equipment)})");
			}
		}

		private EquipmentType SelectType()
		{
			return EasyInput<EquipmentType>.Select(_equipmentTypes, _cancel);
		}

		private EquipmentUse SelectUse()
		{
			return EasyInput<EquipmentUse>.Select(_equipmentUses, _cancel);
		}
	}
}
