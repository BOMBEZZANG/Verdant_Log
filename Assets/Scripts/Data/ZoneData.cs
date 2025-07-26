using UnityEngine;

namespace VerdantLog.Data
{
    [CreateAssetMenu(fileName = "New Zone", menuName = "Verdant Log/Data/Zone")]
    public class ZoneData : ScriptableObject
    {
        [Header("Zone Information")]
        [SerializeField] private string zoneID;
        [SerializeField] private string zoneName;
        [TextArea(2, 4)]
        [SerializeField] private string zoneDescription;
        [SerializeField] private Sprite zoneIcon;
        
        [Header("Scene Information")]
        [SerializeField] private string sceneName;
        
        [Header("Unlock Requirements")]
        [SerializeField] private UnlockRequirement unlockRequirement;
        
        public string ZoneID => zoneID;
        public string ZoneName => zoneName;
        public string ZoneDescription => zoneDescription;
        public Sprite ZoneIcon => zoneIcon;
        public string SceneName => sceneName;
        public UnlockRequirement UnlockRequirement => unlockRequirement;
        
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(zoneID))
            {
                zoneID = name;
            }
        }
    }
    
    [System.Serializable]
    public class UnlockRequirement
    {
        public UnlockType unlockType = UnlockType.Level;
        public int requiredLevel = 1;
        public string requiredItemID;
        public int requiredItemCount = 1;
    }
    
    public enum UnlockType
    {
        Always,     // Always unlocked
        Level,      // Requires player level
        Item,       // Requires specific item
        Quest       // For future expansion
    }
}