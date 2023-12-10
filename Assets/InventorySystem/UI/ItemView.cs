using InventorySystem.DataModels;
using InventorySystem.Extensions;
using UnityEngine;

namespace InventorySystem.UI
{
	public class ItemView : MonoBehaviour
	{
		[SerializeField] private ItemRepository repo;
		[SerializeField] private string itemName;
		[SerializeField] private SpriteRenderer display;
		[SerializeField] private Item item;
		public Item Item => item;
		private void Start()
		{
			item = repo.GetItem(itemName);
			display ??= GetComponentInChildren<SpriteRenderer>();
			display.sprite = item.sprite;
			display.color = item.rarity.GetColor().Scale(0.7f);
		}
	}
}