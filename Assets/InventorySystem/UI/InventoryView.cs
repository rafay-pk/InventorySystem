using System;
using System.Collections.Generic;
using System.Linq;
using InventorySystem.CoreScripts;
using InventorySystem.DataModels;
using InventorySystem.Extensions;
using InventorySystem.Extensions.UIToolkit;
using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.UIElements;
using static System.String;
using Button = UnityEngine.UIElements.Button;

namespace InventorySystem.UI
{
	public class InventoryView : MonoBehaviour
	{
		[SerializeField] private Player player;
		[SerializeField] private UIDocument document;
		[SerializeField] private StyleSheet styleSheet;
		[SerializeField] private Font font;
		[SerializeField] private List<Filter> filters;

		private Inventory Inventory => player.inventory;
		private IEnumerable<InventoryItem> InventoryItems => player.inventory.InventoryItems;
	
		#region State Data
		private bool uiInitialized;
		private Filter selectedFilter;
		private List<InventoryItem> currentInventoryItems;
		private InventoryItem selectedInventoryItem;
		#endregion
	
		#region Visual Elements
		private VisualElement itemRarity;
		private Label capacityLabel, filterLabel, itemLabel, itemWeight, itemPrice, itemDescription, itemRarityLabel;
		private Slider sliderPlayerHealth, sliderPlayerStamina, sliderPlayerMana;
		private Image selectedItemImage, itemImage;
		private GridView inventoryPanel;
		#endregion
	
		private void Start()
		{
			document ??= GetComponent<UIDocument>();
			var root = document.rootVisualElement;
			root.styleSheets.Add(styleSheet);
			root.style.unityFontDefinition = new StyleFontDefinition { value = new FontDefinition { font = font } };

			var background = UIToolkit.Create(new[] { "background", UIToolkit.HORIZONTAL }, root);
		
			#region Filter Ribbon
			var filterRibbon = UIToolkit.Create(new[] { "filter-ribbon" }, background);
			filters.ForEach(filter =>
			{
				var selectionButton = UIToolkit.Create<Button>(new [] { "filter-selection-button" }, filterRibbon);
				filter.image = UIToolkit.Create(filter.sprite, new[] { "filter-icon" }, selectionButton);
				selectionButton.clicked += () => OnFilterSelected(filter);
			});
			selectedFilter = filters[0];
			#endregion
		
			#region Main Panel
			var mainPanel = UIToolkit.Create(new[] { "main-panel", UIToolkit.SPACE_BETWEEN }, background);
			var horizontalLayoutHeaders = UIToolkit.Create(new[] { UIToolkit.HORIZONTAL, UIToolkit.SPACE_BETWEEN }, mainPanel);
			var verticalLayoutLeft = UIToolkit.Create(new[] { UIToolkit.VERTICAL, UIToolkit.LEFT_ALIGNED }, horizontalLayoutHeaders);
			var inventoryLabel = UIToolkit.Create("Inventory", new[] { "inventory-label" }, verticalLayoutLeft);
			filterLabel = UIToolkit.Create("All", new[] { "filter-label" }, verticalLayoutLeft);
			var verticalLayoutRight = UIToolkit.Create(new[] { UIToolkit.VERTICAL, UIToolkit.RIGHT_ALIGNED }, horizontalLayoutHeaders);
			capacityLabel = UIToolkit.Create("", new[] { "capacity-label" }, verticalLayoutRight);
			var horizontalLayoutAscending = UIToolkit.Create(new[] { UIToolkit.HORIZONTAL, UIToolkit.RIGHT_ALIGNED }, verticalLayoutRight);
			var ascendingLabel = UIToolkit.Create("Ascending", new[] { "ascending-label" }, horizontalLayoutAscending);
			var ascendingToggle = UIToolkit.Create(Inventory.selectedAscendingValue, OnAscendingChanged, new[] { "ascending-toggle" }, horizontalLayoutAscending);
			var horizontalLayoutSorting = UIToolkit.Create(new[] { UIToolkit.HORIZONTAL, UIToolkit.MIDDLE_ALIGNED }, verticalLayoutRight);
			var sortLabel = UIToolkit.Create("Sort by", new[] { "sort-label" }, horizontalLayoutSorting);
			var sortDropdown = UIToolkit.Create(Inventory.selectedSortingType, OnSortingChanged,new[] { "sort-dropdown" }, horizontalLayoutSorting);
			var inventoryContainer = UIToolkit.Create(new [] {"inventory-container"}, mainPanel);
			RefreshInventoryItemsView();
			inventoryPanel = new GridView
			{
				itemsSource = currentInventoryItems,
				makeItem = OnGridMakeItem,
				bindItem = OnGridBindItem,
				selectionType = SelectionType.Single,
				itemHeight = 115,
				columnCount = 5,
			};
			inventoryPanel.selectionChanged += selection => OnItemSelected((InventoryItem)selection.FirstOrDefault());
			inventoryPanel.AddToClassList("inventory-panel");
			inventoryContainer.Add(inventoryPanel);
			var horizontalLayoutPlayerStats = UIToolkit.Create(new[] { UIToolkit.HORIZONTAL }, mainPanel);
			sliderPlayerHealth = UIToolkit.Create(player.GetHealthAsPercentage(), OnHealthValueChanged, new[] { "player-health" }, horizontalLayoutPlayerStats);
			sliderPlayerStamina = UIToolkit.Create(player.GetStaminaAsPercentage(), OnStaminaValueChanged, new[] { "player-stamina" }, horizontalLayoutPlayerStats);
			sliderPlayerMana = UIToolkit.Create(player.GetManaAsPercentage(), OnManaValueChanged, new[] { "player-mana" }, horizontalLayoutPlayerStats);
			#endregion
		
			#region Item Panel
			var itemPanel = UIToolkit.Create(new[] { "item-panel", UIToolkit.MIDDLE_ALIGNED }, background);
			itemImage = UIToolkit.Create<Image>(new [] {"item-image"}, itemPanel);
			itemImage.scaleMode = ScaleMode.StretchToFill;
			var itemDetails = UIToolkit.Create(new[] { "item-details-container", UIToolkit.VERTICAL, UIToolkit.SPACE_BETWEEN }, itemPanel);
			itemLabel = UIToolkit.Create("", new [] {"item-label"}, itemDetails);
			var horizontalLayoutPriceWeight = UIToolkit.Create(new[] { UIToolkit.HORIZONTAL, UIToolkit.SPACE_BETWEEN }, itemDetails);
			itemWeight = UIToolkit.Create("", new [] {"item-weight"}, horizontalLayoutPriceWeight);
			itemPrice = UIToolkit.Create("", new [] {"item-price"}, horizontalLayoutPriceWeight);
			var horizontalLayoutRarity = UIToolkit.Create(new[] { UIToolkit.HORIZONTAL, UIToolkit.CENTER }, itemDetails);
			itemRarityLabel = UIToolkit.Create("Rarity: ", new [] {"item-rarity-label"}, horizontalLayoutRarity);
			itemRarity = UIToolkit.Create(new [] {"item-rarity"},horizontalLayoutRarity);
			itemDescription = UIToolkit.Create("", new [] {"item-description"}, itemDetails);
			var horizontalLayoutButtons = UIToolkit.Create(new[] { UIToolkit.HORIZONTAL, UIToolkit.SPACE_AROUND }, itemDetails);
			var useButton = UIToolkit.Create("Use", OnUseButtonClicked, new[] { "button" }, horizontalLayoutButtons);
			var dropButton = UIToolkit.Create("Drop", OnDropButtonClicked, new[] { "button" }, horizontalLayoutButtons);
			#endregion
		
			uiInitialized = true;
			OnFilterSelected(filters[0]);
		}
		private static VisualElement OnGridMakeItem()
		{
			var element = new Image();
			element.AddToClassList("inventory-image");
			UIToolkit.Create("", new[] { "item-count" }, element);
			return element;
		}
		private void OnGridBindItem(VisualElement element, int i)
		{
			var image = (Image)element;
			var inventoryItem = currentInventoryItems[i];
			inventoryItem.image = image;
			var item = inventoryItem.item;
			image.sprite = item.sprite;
			var count = inventoryItem.count;
			element.Q<Label>().text = count == 1 ? "" : count.ToString();
		}
		private void OnItemSelected(InventoryItem inventoryItem)
		{
			selectedInventoryItem = inventoryItem;
			var item = selectedInventoryItem?.item;
			itemLabel.text = item?.name;
			itemWeight.text = $"Weight: {item?.weight}";
			itemPrice.text = $"Price: {item?.basePrice}";
			itemRarityLabel.text = $"Rarity: {item?.rarity}";
			itemRarity.style.backgroundColor = item?.rarity.GetColor() ?? Color.white;
			itemDescription.text = item?.description;
			itemImage.sprite = item?.sprite;
			selectedItemImage?.RemoveFromClassList("selected");
			selectedItemImage = selectedInventoryItem?.image;
			selectedItemImage?.AddToClassList("selected");
		}

		private void OnSortingChanged(Enum enumValue)
		{
			Inventory.selectedSortingType = Enum.Parse<SortingType>(enumValue.ToString());
			RefreshInventoryItemsView();
		}
		private void OnAscendingChanged(bool ascending)
		{
			Inventory.selectedAscendingValue = ascending;
			RefreshInventoryItemsView();
		}
		private void OnUseButtonClicked()
		{
			if (selectedInventoryItem == null) return;
			var item = selectedInventoryItem.item;
			var response = item.Use(player);
			if (!response.success) return;
			if (item is Potion)
			{
				sliderPlayerHealth.value = player.GetHealthAsPercentage();
				sliderPlayerStamina.value = player.GetStaminaAsPercentage();
				sliderPlayerMana.value = player.GetManaAsPercentage();
			}
			RemoveSelectedItemOnce();
		}
		private void OnDropButtonClicked()
		{
			RemoveSelectedItemOnce();
		}
		private void RemoveSelectedItemOnce()
		{
			var inventoryItem = selectedInventoryItem;
			var itemCountDepleted = Inventory.RemoveItem(selectedInventoryItem.item) == InventoryResponseFlag.ItemCountDepleted;
			RefreshInventoryItemsView(false);
			if (itemCountDepleted)
			{
				OnItemSelected(null);
			}
			else
			{
				selectedInventoryItem = inventoryItem;
			}
		}
		private void OnFilterSelected(Filter filter)
		{
			selectedFilter = filter;
			var greyedOutColor = new Color(1, 1, 1, 0.3f);
			filters.ForEach(f => f.image.tintColor = greyedOutColor);
			filter.image.tintColor = Color.white;
			filterLabel.text = filter.name;
			RefreshInventoryItemsView();
		}
		public void RefreshInventoryItemsView(bool selectFirstItem = true)
		{
			currentInventoryItems ??= new List<InventoryItem>();
			currentInventoryItems.Clear();
			currentInventoryItems.AddRange(selectedFilter.name switch
			{
				"All" => InventoryItems,
				"Armor" => InventoryItems.Where(it => it.item is Armor),
				"Potions" => InventoryItems.Where(it => it.item is Potion),
				"Weapons" => InventoryItems.Where(it => it.item is Weapon),
				_ => throw new ArgumentOutOfRangeException()
			});
			switch (Inventory.selectedSortingType)
			{
				case SortingType.Alphabetically:
					if (Inventory.selectedAscendingValue) currentInventoryItems.Sort((it1, it2) => Compare(it1.item.name, it2.item.name, StringComparison.Ordinal));
					else currentInventoryItems.Sort((it1, it2) => Compare(it2.item.name, it1.item.name, StringComparison.Ordinal));
					break;
				case SortingType.Price:
					if (Inventory.selectedAscendingValue) currentInventoryItems.Sort((it1, it2) => it1.item.basePrice.CompareTo(it2.item.basePrice));
					else currentInventoryItems.Sort((it1, it2) => it2.item.basePrice.CompareTo(it1.item.basePrice));
					break;
				case SortingType.Rarity:
					if (Inventory.selectedAscendingValue) currentInventoryItems.Sort((it1, it2) => Convert.ToInt32(it1.item.rarity).CompareTo(Convert.ToInt32(it2.item.rarity)));
					else currentInventoryItems.Sort((it1, it2) => Convert.ToInt32(it2.item.rarity).CompareTo(Convert.ToInt32(it1.item.rarity)));
					break;
				case SortingType.Weight:
					if (Inventory.selectedAscendingValue) currentInventoryItems.Sort((it1, it2) => it1.item.weight.CompareTo(it2.item.weight));
					else currentInventoryItems.Sort((it1, it2) => it2.item.weight.CompareTo(it1.item.weight));
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			inventoryPanel?.Refresh();
			capacityLabel.text = $"Capacity: {Inventory.RefreshWeight()}/{Inventory.Capacity}";
			if (selectFirstItem && uiInitialized) OnItemSelected(currentInventoryItems.FirstOrDefault());
		}
		private void OnHealthValueChanged(float value)
		{
			if (sliderPlayerHealth == null) return; 
			sliderPlayerHealth.value = player.GetHealthAsPercentage();
		}
		private void OnStaminaValueChanged(float value)
		{
			if (sliderPlayerStamina == null) return;
			sliderPlayerStamina.value = player.GetStaminaAsPercentage();
		}
		private void OnManaValueChanged(float value)
		{
			if (sliderPlayerMana == null) return;
			sliderPlayerMana.value = player.GetManaAsPercentage();
		}
		[Serializable] private class Filter
		{
			public string name;
			public Sprite sprite;
			public Image image;
		}
	}
}