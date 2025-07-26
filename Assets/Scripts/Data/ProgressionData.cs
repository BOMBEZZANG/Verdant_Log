using UnityEngine;

namespace VerdantLog.Data
{
    [CreateAssetMenu(fileName = "ProgressionData", menuName = "Verdant Log/Data/Progression Data")]
    public class ProgressionData : ScriptableObject
    {
        [Header("Experience Curve")]
        [Tooltip("EXP required for each level (index = level - 1)")]
        [SerializeField] private int[] expToNextLevel = new int[]
        {
            100,   // Level 1 -> 2
            150,   // Level 2 -> 3
            200,   // Level 3 -> 4
            300,   // Level 4 -> 5
            500,   // Level 5 -> 6
            750,   // Level 6 -> 7
            1000,  // Level 7 -> 8
            1500,  // Level 8 -> 9
            2000,  // Level 9 -> 10
            3000   // Level 10+
        };
        
        [Header("Starting Values")]
        [SerializeField] private int startingLevel = 1;
        [SerializeField] private int startingExp = 0;
        
        public int StartingLevel => startingLevel;
        public int StartingExp => startingExp;
        
        public int GetExpRequiredForLevel(int currentLevel)
        {
            if (currentLevel <= 0)
                return 0;
                
            int index = currentLevel - 1;
            
            if (index >= expToNextLevel.Length)
            {
                return expToNextLevel[expToNextLevel.Length - 1];
            }
            
            return expToNextLevel[index];
        }
        
        public int GetTotalExpForLevel(int targetLevel)
        {
            if (targetLevel <= 1)
                return 0;
                
            int totalExp = 0;
            for (int level = 1; level < targetLevel; level++)
            {
                totalExp += GetExpRequiredForLevel(level);
            }
            
            return totalExp;
        }
    }
}