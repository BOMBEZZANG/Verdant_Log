using UnityEngine;

namespace VerdantLog.Data
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Verdant Log/Data/Item")]
    public class ItemData : ScriptableObject
    {
        [Header("Basic Information")]
        [SerializeField] private string itemID;
        [SerializeField] private string itemName;
        [TextArea(3, 5)]
        [SerializeField] private string description;
        [SerializeField] private Sprite icon;
        
        [Header("Stacking Properties")]
        [SerializeField] private bool isStackable = true;
        [SerializeField] private int maxStackSize = 99;
        
        [Header("Item Type")]
        [SerializeField] private ItemType itemType;
        
        public string ItemID => itemID;
        public string ItemName => itemName;
        public string Description => description;
        public Sprite Icon => icon;
        public bool IsStackable => isStackable;
        public int MaxStackSize => isStackable ? maxStackSize : 1;
        public ItemType Type => itemType;
        
        private void OnValidate()
        {
            if (!isStackable)
            {
                maxStackSize = 1;
            }
            
            if (string.IsNullOrEmpty(itemID))
            {
                itemID = name;
            }
        }
    }
    
    public enum ItemType
    {
        Seed,
        Material,
        Tool,
        Plant,
        Other
    }
}