using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem.DataModels
{
	[CreateAssetMenu]
	public class ItemRepository : ScriptableObject
	{
		[SerializeField] private List<Potion> potions;
		[SerializeField] private List<Weapon> weapons;
		[SerializeField] private List<Armor> armors;

		#region Space Optimized Version
		public Item GetItem(string itemName)
		{
			Item item = armors.Find(a => a.name == itemName);
			if (item != null) return item;
			item = potions.Find(p => p.name == itemName);
			if (item != null) return item;
			item = weapons.Find(w => w.name == itemName);
			return item;
		}

		#region Generic Version
		// public Item GetItem<T>(string itemName) where T : Item => typeof(T).Name switch
		// {
		// 	"Armor" => armors.Find(a => a.name == itemName) as T,
		// 	"Potion" => potions.Find(p => p.name == itemName) as T,
		// 	"Weapon" => weapons.Find(w => w.name == itemName) as T,
		// 	_ => throw new ArgumentOutOfRangeException()
		// };
		#endregion
		#endregion

		#region Performance Optimized Version
		// private Dictionary<string, int> hashMap;
		// private static bool initialized;
		// private void Initialize()
		// {
		// 	hashMap = new Dictionary<string, int>();
		// 	MergeIntoHashMap(armors);
		// 	MergeIntoHashMap(potions);
		// 	MergeIntoHashMap(weapons);
		// 	initialized = true;
		// }
		// private void MergeIntoHashMap<T>(IEnumerable<T> items) where T: Item
		// {
		// 	hashMap = hashMap.Concat(items.ToHashList(item => item.name))
		// 		.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
		// }
		// public Item GetItem<T>(string itemName) where T : Item
		// {
		// 	if (!initialized) Initialize();
		// 	if (hashMap.TryGetValue(itemName, out var i))
		// 	{
		// 		return typeof(T).Name switch
		// 		{
		// 			"Armor" => armors[i] as T,
		// 			"Potion" => potions[i] as T,
		// 			"Weapon" => weapons[i] as T,
		// 			_ => throw new ArgumentOutOfRangeException()
		// 		};
		// 	}
		// 	Debug.LogError($"Item with name {itemName} not found in the repository.");
		// 	return default;
		// }
		#endregion
	}
}
