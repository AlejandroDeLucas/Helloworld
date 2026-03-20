using System.Collections.Generic;
using UnityEngine;

namespace TinyHunter.Data.Items
{
    [CreateAssetMenu(menuName = "TinyHunter/Data/Item Database")]
    public class ItemDatabase : ScriptableObject
    {
        public ItemDefinition[] Items;

        public ItemDefinition FindById(string id)
        {
            foreach (var item in Items)
            {
                if (item != null && item.ItemId == id) return item;
            }

            return null;
        }

        public T FindById<T>(string id) where T : ItemDefinition
        {
            return FindById(id) as T;
        }
    }
}
