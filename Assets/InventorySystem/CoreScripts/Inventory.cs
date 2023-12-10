using System.Collections.Generic;
using System.Linq;
using InventorySystem.DataModels;
using InventorySystem.Extensions;
using UnityEngine;

namespace InventorySystem.CoreScripts
{
	public class Inventory : PersistentMonoBehaviour
	{
		public SortingType selectedSortingType;
		public bool selectedAscendingValue = true;
		public float Capacity 
		{ 
			get => _capacity;
			private set => _capacity = value;
		}
		[SerializeField] private float _capacity = 100f;
		private float weight;
		public float RefreshWeight() => weight = inventoryItems.Sum(inventoryItem =>
			inventoryItem.item.isQuestItem ? 0f : inventoryItem.item.weight * inventoryItem.count);
		public IEnumerable<InventoryItem> InventoryItems => inventoryItems;
		[SerializeField] private List<InventoryItem> inventoryItems;
		private void Start()
		{
			inventoryItems ??= new List<InventoryItem>();
		}

		public bool CanAccomodate(float itemWeight) => weight + itemWeight <= Capacity;
		public InventoryResponse AddItem(Item item, int count = 1)
		{
			if (count <= 0) return new InventoryResponse { success = false, message = "Count must be positive." };
			if (!item.isQuestItem && weight + item.weight * count > Capacity) 
				return new InventoryResponse { success = false, message = "Inventory is full." };
			var inventoryItem = inventoryItems.Find(it => it.item == item);
			if (inventoryItem == null)
			{
				inventoryItems.Add(new InventoryItem { item = item, count = count, });
			}
			else
			{
				inventoryItem.count += count;
			}
			RefreshWeight();
			return InventoryResponse.Successful;
		}

		public InventoryResponse RemoveItem(Item item, int count = 1)
		{
			if (count <= 0) return new InventoryResponse { success = false, message = "Count must be positive." };
			var inventoryItem = inventoryItems.Find(it => it.item == item);
			if (inventoryItem == null) return new InventoryResponse { success = false, message = $"Item {item.name} not found in inventory." };
			if (inventoryItem.count < count) return new InventoryResponse { success = false, message = $"Not enough {item.name} in inventory." };
			inventoryItem.count -= count;
			RefreshWeight();
			if (inventoryItem.count == 0)
			{
				inventoryItems.Remove(inventoryItem);
				return new InventoryResponse {success = true, flag = InventoryResponseFlag.ItemCountDepleted};
			}
			return InventoryResponse.Successful;
		}
	
		public InventoryResponse SetCapacity(float capacity)
		{
			if (capacity < 0) return new InventoryResponse { success = false, message = "Capacity must be positive." };
			if (capacity < weight) return new InventoryResponse { success = false, message = "Capacity cannot be less than current weight." };
			Capacity = capacity;
			return InventoryResponse.Successful;
		}
	
#if UNITY_EDITOR
		public void RefreshItemData(ItemRepository repo)
		{
			inventoryItems.ForEach(inventoryItem => { inventoryItem.item = repo.GetItem(inventoryItem.item.name); });
		}
#endif
	}
}