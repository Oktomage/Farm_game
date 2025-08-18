using Game.Items;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Shop
{
    [CreateAssetMenu(fileName = "New Shop Tab", menuName = "Shop/ShopTab")]
    public class ShopTab_scriptable : ScriptableObject
    {
        [Header("Tab Settings")]
        public string tabName;
        public Sprite tabIcon;
        
        [Space]
        public List<Item_scriptable> tabItemsData = new List<Item_scriptable>();
    }
}
