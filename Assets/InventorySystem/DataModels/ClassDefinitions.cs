using System;
using InventorySystem.CoreScripts;
using UnityEngine;
using UnityEngine.UIElements;

namespace InventorySystem.DataModels
{
	[Serializable] public class Item
	{
		public string name;
		public string description;
		public Sprite sprite;
		public string spriteAssetPath;
		public string spriteName;
		public Rarity rarity;
		public float weight;
		public float basePrice;
		public bool isQuestItem;

		public static bool operator ==(Item left, Item right) => left?.name == right?.name;
		public static bool operator !=(Item left, Item right) => !(left == right);
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return obj.GetType() == GetType() && Equals((Item)obj);
		}
		protected bool Equals(Item other) => name == other.name;
		public override int GetHashCode() => name != null ? name.GetHashCode() : 0;

		public virtual InventoryResponse Use(Player player) => new();
	}
	
	public enum Rarity
	{
		Common,
		Uncommon,
		Rare,
		Epic,
		Legendary,
		Mythic
	}
	
	#region Items
	[Serializable] public class Potion : Item
	{
		public float healthRecover;
		public float staminaRecover;
		public float manaRecover;

		public override InventoryResponse Use(Player player)
		{
			player.Health += healthRecover;
			player.Stamina += staminaRecover;
			player.Mana += manaRecover;
			return InventoryResponse.Successful;
		}
	}

	[Serializable] public class Weapon : Item
	{
		public float damage;
		public float attackSpeed;
		public float range;

		public override InventoryResponse Use(Player player)
		{
			var inventory = player.inventory;
			var oldWeapon = player.equippedWeapon;
			var playerHasWeapon = player.equippedWeapon?.name != "";
			if (playerHasWeapon)
			{
				if (!inventory.CanAccomodate(weight - player.equippedWeapon.weight))
				{
					return new InventoryResponse
					{
						success = false, 
						message = "Not enough space in inventory.",
						flag = InventoryResponseFlag.DidNotUseItem
					};
				}
			}
			player.equippedWeapon = this;
			if (playerHasWeapon) inventory.AddItem(oldWeapon);
			return InventoryResponse.Successful;
		}
	}

	[Serializable] public class Armor : Item
	{
		public float defense;
	
		public override InventoryResponse Use(Player player)
		{
			var inventory = player.inventory;
			var oldArmor = player.equippedArmor;
			var playerHasArmor = player.equippedArmor?.name != "";
			if (playerHasArmor)
			{
				if (!inventory.CanAccomodate(weight - player.equippedArmor.weight))
				{
					return new InventoryResponse
					{
						success = false, 
						message = "Not enough space in inventory.",
						flag = InventoryResponseFlag.DidNotUseItem
					};
				}
			}
			player.equippedArmor = this;
			if (playerHasArmor) inventory.AddItem(oldArmor);
			return InventoryResponse.Successful;
		}
	}
	#endregion
	
	[Serializable] public class InventoryItem
	{
		public Item item;
		public int count;
		public Image image;
	}

	public class Response
	{
		public bool success;
		public string message;

		public static implicit operator bool(Response response) => response.success;
		public static implicit operator string(Response response) => $"{response.success}: {response.message}";
	}

	public class InventoryResponse : Response
	{
		public InventoryResponseFlag flag = InventoryResponseFlag.Default;
		public static implicit operator InventoryResponseFlag(InventoryResponse response) => response.flag;
		
		private static InventoryResponse successful;
		public static InventoryResponse Successful => successful ??= new InventoryResponse { success = true };
	}

	public enum InventoryResponseFlag
	{
		Default,
		ItemCountDepleted,
		DidNotUseItem,
	}
	
	public enum SortingType
	{
		Alphabetically,
		Rarity,
		Weight,
		Price,
	}
}