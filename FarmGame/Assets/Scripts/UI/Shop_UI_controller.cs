using Game.Items;
using Game.Shop;
using Game.Utils;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Shop
{
    public class Shop_UI_controller : MonoBehaviour
    {
        [Header("Tabs")]
        public List<ShopTab_scriptable> shopTabs;

        [Header("Carousel")]
        public Button Button_Prev;
        public Button Button_Next;
        public Button Button_Buy;

        [Space]
        public Image Image_prevItem;
        public Image Image_selectedItem;
        public Image Image_nextItem;

        [Header("Texts")]
        public TextMeshProUGUI Text_itemName; 
        public TextMeshProUGUI Text_itemPrice;

        //Internal variables
        internal ShopTab_scriptable selectedTab;
        [SerializeField] internal int currentItemIndex = 0;
        
        [SerializeField] internal Item_scriptable Selected_item => selectedTab.tabItemsData[currentItemIndex];

        void Awake()
        {
            // liga navegação
            if (Button_Prev) Button_Prev.onClick.AddListener(Previous_item);
            if (Button_Next) Button_Next.onClick.AddListener(Next_item);
            if (Button_Buy) Button_Buy.onClick.AddListener(Buy_current);
        }

        private void Start()
        {
            selectedTab = shopTabs[0];
            currentItemIndex = 0;
        }

        /// MAIN METHODS
        public void Set_Carousel(ShopTab_scriptable tab)
        {
            selectedTab = tab;
            currentItemIndex = 0;

            Refresh();

            //Audio
            Game_utils.Instance.Create_sound("UI_Click", "Audios/UI/Click_1", Vector2.zero);
        }

        private void Previous_item()
        {
            if (selectedTab == null && currentItemIndex > 0) { return; }

            currentItemIndex--;
            currentItemIndex = Mathf.Clamp(currentItemIndex, 0, selectedTab.tabItemsData.Count - 1);

            Refresh();

            //Audio
            Game_utils.Instance.Create_sound("UI_Click", "Audios/UI/Click_1", Vector2.zero);
        }

        private void Next_item()
        {
            if (selectedTab == null && currentItemIndex < shopTabs[currentItemIndex].tabItemsData.Count - 1) { return; }

            currentItemIndex++;
            currentItemIndex = Mathf.Clamp(currentItemIndex, 0, selectedTab.tabItemsData.Count - 1);

            Refresh();
        
            //Audio
            Game_utils.Instance.Create_sound("UI_Click", "Audios/UI/Click_1", Vector2.zero);
        }

        private void Buy_current()
        {

        }

        private void Refresh()
        {
            if (selectedTab == null) return;

            var items = selectedTab.tabItemsData;

            // Not found item
            if (items == null || items.Count == 0)
            {
                Image_prevItem.sprite = null;
                Image_selectedItem.sprite = null;
                Image_nextItem.sprite = null;

                Text_itemName.text = "";
                Text_itemPrice.text = "";
                return;
            }

            // Set sprites
            int prevIndex = Mathf.Clamp(currentItemIndex - 1, 0, items.Count - 1);
            int nextIndex = Mathf.Clamp(currentItemIndex + 1, 0, items.Count - 1);

            Image_prevItem.sprite = items[prevIndex].Icon;
            Image_selectedItem.sprite = items[currentItemIndex].Icon;
            Image_nextItem.sprite = items[nextIndex].Icon;

            // Enable/Disable image if null
            Image_prevItem.enabled = Image_prevItem.sprite != null;
            Image_selectedItem.enabled = Image_selectedItem.sprite != null;
            Image_nextItem.enabled = Image_nextItem.sprite != null;

            // Texts
            Text_itemName.text = items[currentItemIndex].ItemName;
            Text_itemPrice.text = $"x{items[currentItemIndex].Price}";
        }
    }
}