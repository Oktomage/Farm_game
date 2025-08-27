using Game.Items;
using Game.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class UI_recipe_slot : MonoBehaviour
    {
        [Header("Data")]
        internal Game_utils.Converted_recipe Recipe;

        [Header("Components")]
        public Image Image_recipe_icon;
        public GameObject Materials_holder;

        internal void Configure(Game_utils.Converted_recipe recipe)
        {
            // Set
            Recipe = recipe;

            Image_recipe_icon.sprite = recipe.Craft_result.Icon;

            foreach(Item_scriptable material in recipe.Recipe_materials.ToArray())
            {
                GameObject image_obj = Game_utils.Instance.Create_gameObject(Materials_holder);
                Image img = image_obj.AddComponent<Image>();

                // Set
                img.sprite = material.Icon;
            }
        }
    }
}