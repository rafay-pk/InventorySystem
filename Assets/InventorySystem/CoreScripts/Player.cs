using InventorySystem.DataModels;
using InventorySystem.Extensions;
using UnityEngine;

namespace InventorySystem.CoreScripts
{
	public class Player : PersistentMonoBehaviour
	{
		public Inventory inventory;
		
		public Weapon equippedWeapon;
		public Armor equippedArmor;
		public float Encumbrance => equippedArmor?.weight + equippedWeapon?.weight ?? 0f;
		
		[SerializeField] private float health, stamina, mana;
		public float MaxHealth { get; private set; } = 100;
		public float MaxStamina { get; private set; } = 100;
		public float MaxMana { get; private set; } = 100;
		public float Health
		{
			get => health;
			set 
			{
				health = value > MaxHealth ? MaxHealth : value;
				if (health <= 0) Die();
			}
		}
		public float Stamina
		{
			get => stamina;
			set => stamina = value > MaxStamina ? MaxStamina : value;
		}
		public float Mana
		{
			get => mana;
			set => mana = value > MaxMana ? MaxMana : value;
		}
		public float GetHealthAsPercentage() => health / MaxHealth;
		public float GetStaminaAsPercentage() => stamina / MaxStamina;
		public float GetManaAsPercentage() => mana / MaxMana;
		
		private void Awake()
		{
			inventory ??= GetComponent<Inventory>();
		}
		
		private void Die()
		{
			Debug.Log("Player died.");
		}
	}
}