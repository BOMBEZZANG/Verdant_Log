using System;
using UnityEngine;

namespace VerdantLog.Data
{
    [Serializable]
    public class CultivationCondition
    {
        [SerializeField] private ConditionType conditionType;
        [SerializeField] private string conditionValue;
        
        public ConditionType Type => conditionType;
        public string Value => conditionValue;
        
        public CultivationCondition(ConditionType type, string value)
        {
            conditionType = type;
            conditionValue = value;
        }
    }
    
    public enum ConditionType
    {
        TimeOfDay,      // Value: "Day" or "Night"
        ItemUsed,       // Value: ItemID of the item that must be used
        Adjacency,      // Value: PlantID that must be adjacent
        Weather,        // Value: Weather type (for future expansion)
        Season          // Value: Season type (for future expansion)
    }
}