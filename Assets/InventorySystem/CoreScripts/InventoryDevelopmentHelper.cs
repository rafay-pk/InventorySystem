using System;
using System.Collections;
using System.Collections.Generic;
using InventorySystem.DataModels;
using InventorySystem.UI;
using UnityEngine;

namespace InventorySystem.CoreScripts
{
    public class InventoryDevelopmentHelper : MonoBehaviour
    {
        [SerializeField] private Inventory inventory;
        [SerializeField] private InventoryView inventoryView;
        [SerializeField] private ItemRepository repo;
        [SerializeField] private List<CheatItem> itemsReference;
        [SerializeField] private List<CheatItem> itemsToAdd;
        private IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();
            inventory ??= GetComponent<Inventory>();
            inventory.RefreshItemData(repo);
            itemsToAdd.ForEach(thing => inventory.AddItem(repo.GetItem(thing.name), thing.count));
            inventoryView.RefreshInventoryItemsView();
        }
        [Serializable] private class CheatItem
        {
            public string name;
            public int count;
        }
    }
}
