using System;

namespace VerdantLog.Core
{
    [Serializable]
    public class InventoryItem
    {
        public string itemID;
        public int quantity;
        
        public InventoryItem(string itemID, int quantity)
        {
            this.itemID = itemID;
            this.quantity = quantity;
        }
    }
}