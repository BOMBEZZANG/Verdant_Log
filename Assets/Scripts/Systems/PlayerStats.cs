using System;
using UnityEngine;
using VerdantLog.Data;

namespace VerdantLog.Systems
{
    public class PlayerStats : MonoBehaviour
    {
        private static PlayerStats instance;
        public static PlayerStats Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<PlayerStats>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("PlayerStats");
                        instance = go.AddComponent<PlayerStats>();
                        DontDestroyOnLoad(go);
                    }
                }
                return instance;
            }
        }
        
        public static event Action<int> OnPlayerLevelUp;
        public static event Action<int, int> OnEXPChanged;
        
        [SerializeField] private ProgressionData progressionData;
        [SerializeField] private int currentLevel = 1;
        [SerializeField] private int currentEXP = 0;
        
        public int CurrentLevel => currentLevel;
        public int CurrentEXP => currentEXP;
        
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            InitializeStats();
        }
        
        private void InitializeStats()
        {
            if (progressionData == null)
            {
                progressionData = Resources.Load<ProgressionData>("ProgressionData");
                if (progressionData == null)
                {
                    Debug.LogError("ProgressionData not found! Please create one in Resources folder.");
                    return;
                }
            }
            
            currentLevel = progressionData.StartingLevel;
            currentEXP = progressionData.StartingExp;
        }
        
        public void AddEXP(int amount)
        {
            if (amount <= 0)
                return;
                
            currentEXP += amount;
            Debug.Log($"Gained {amount} EXP. Total: {currentEXP}");
            
            CheckLevelUp();
            
            int requiredEXP = GetRequiredEXPForNextLevel();
            OnEXPChanged?.Invoke(currentEXP, requiredEXP);
        }
        
        private void CheckLevelUp()
        {
            if (progressionData == null)
                return;
                
            int requiredEXP = GetRequiredEXPForNextLevel();
            
            while (currentEXP >= requiredEXP && requiredEXP > 0)
            {
                currentEXP -= requiredEXP;
                currentLevel++;
                
                Debug.Log($"Level Up! Now level {currentLevel}");
                OnPlayerLevelUp?.Invoke(currentLevel);
                
                requiredEXP = GetRequiredEXPForNextLevel();
            }
        }
        
        public int GetRequiredEXPForNextLevel()
        {
            if (progressionData == null)
                return -1;
                
            return progressionData.GetExpRequiredForLevel(currentLevel);
        }
        
        public float GetEXPProgress()
        {
            int requiredEXP = GetRequiredEXPForNextLevel();
            if (requiredEXP <= 0)
                return 1f;
                
            return (float)currentEXP / requiredEXP;
        }
        
        public void SetLevel(int newLevel, int newEXP = 0)
        {
            if (newLevel < 1)
                return;
                
            int oldLevel = currentLevel;
            currentLevel = newLevel;
            currentEXP = Mathf.Max(0, newEXP);
            
            if (currentLevel > oldLevel)
            {
                OnPlayerLevelUp?.Invoke(currentLevel);
            }
            
            int requiredEXP = GetRequiredEXPForNextLevel();
            OnEXPChanged?.Invoke(currentEXP, requiredEXP);
        }
        
        public void ResetStats()
        {
            InitializeStats();
            
            int requiredEXP = GetRequiredEXPForNextLevel();
            OnEXPChanged?.Invoke(currentEXP, requiredEXP);
        }
    }
}