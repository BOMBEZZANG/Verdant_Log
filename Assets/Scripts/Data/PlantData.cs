using UnityEngine;

namespace VerdantLog.Data
{
    [CreateAssetMenu(fileName = "New Plant", menuName = "Verdant Log/Data/Plant")]
    public class PlantData : ScriptableObject
    {
        [Header("Basic Information")]
        [SerializeField] private string plantID;
        [SerializeField] private string plantName;
        [TextArea(3, 5)]
        [SerializeField] private string description;
        
        [Header("Growth Properties")]
        [SerializeField] private Sprite[] growthStageSprites;
        [SerializeField] private float growthTime = 60f;
        [SerializeField] private int growthStages = 3;
        
        [Header("Rewards")]
        [SerializeField] private int expValue = 10;
        [SerializeField] private ItemData harvestYield;
        [SerializeField] private int harvestYieldAmount = 1;
        
        [Header("Seed")]
        [SerializeField] private ItemData seedItem;
        
        public string PlantID => plantID;
        public string PlantName => plantName;
        public string Description => description;
        public Sprite[] GrowthStageSprites => growthStageSprites;
        public float GrowthTime => growthTime;
        public int GrowthStages => growthStages;
        public int ExpValue => expValue;
        public ItemData HarvestYield => harvestYield;
        public int HarvestYieldAmount => harvestYieldAmount;
        public ItemData SeedItem => seedItem;
        
        public float TimePerStage => growthTime / growthStages;
        
        public Sprite GetGrowthSprite(int stage)
        {
            if (growthStageSprites == null || growthStageSprites.Length == 0)
                return null;
                
            stage = Mathf.Clamp(stage, 0, growthStageSprites.Length - 1);
            return growthStageSprites[stage];
        }
        
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(plantID))
            {
                plantID = name;
            }
            
            if (growthStages < 1)
            {
                growthStages = 1;
            }
        }
    }
}