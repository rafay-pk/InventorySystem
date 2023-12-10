using System;
using System.Collections.Generic;
using System.Text;
using InventorySystem.DataModels;
using UnityEngine;

namespace InventorySystem.Extensions
{
    public static class ExtensionMethods
    {
        public static Color GetColor(this Rarity rarity)
        {
            return rarity switch
            {
                Rarity.Common => Color.white,
                Rarity.Uncommon => Color.green,
                Rarity.Rare => Color.blue,
                Rarity.Epic => Color.magenta,
                Rarity.Legendary => Color.yellow,
                Rarity.Mythic => Color.red,
                _ => throw new ArgumentOutOfRangeException(nameof(rarity), rarity, null)
            };
        }
        
        public static Color Scale(this Color color, float scale, bool modifyAlpha = false)
        {
            return new Color(color.r * scale, color.g * scale, color.b * scale, (modifyAlpha ? scale : 1f) * color.a);
        }
        
        public static Dictionary<string, int> ToHashList<T>(this IEnumerable<T> itemList, Func<T, string> keySelector)
        {
            var hashTable = new Dictionary<string, int>();
            var index = 0;
            foreach (var item in itemList)
            {
                var key = keySelector(item);
                if (hashTable.ContainsKey(key))
                {
                    Debug.LogWarning("Item with name " + key + $" already exists in {itemList}.");
                    continue;
                }
                hashTable[key] = index++;
            }
            return hashTable;
        }
        
        public static string Unite(this IEnumerable<string> stringList, string separator = null)
        {
            var stringBuilder = new StringBuilder();
            foreach (var str in stringList)
            {
                str.AppendTo(stringBuilder);
                separator?.AppendTo(stringBuilder);
            }
            return stringBuilder.ToString();
        }
        
        public static void AppendTo(this string str, StringBuilder stringBuilder) => stringBuilder.Append(str);
    }
} 