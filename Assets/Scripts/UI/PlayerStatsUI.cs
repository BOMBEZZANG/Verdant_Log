using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VerdantLog.Core;
using VerdantLog.Systems;

namespace VerdantLog.UI
{
    public class PlayerStatsUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI expText;
        [SerializeField] private Slider expBar;
        [SerializeField] private GameObject levelUpEffect;
        
        private void Start()
        {
            GameEvents.OnEXPChanged += UpdateEXPDisplay;
            GameEvents.OnPlayerLevelUp += OnLevelUp;
            
            UpdateDisplay();
        }
        
        private void OnDestroy()
        {
            GameEvents.OnEXPChanged -= UpdateEXPDisplay;
            GameEvents.OnPlayerLevelUp -= OnLevelUp;
        }
        
        private void UpdateEXPDisplay(int currentEXP, int requiredEXP)
        {
            UpdateDisplay();
        }
        
        private void OnLevelUp(int newLevel)
        {
            UpdateDisplay();
            
            if (levelUpEffect != null)
            {
                GameObject effect = Instantiate(levelUpEffect, transform.position, Quaternion.identity);
                Destroy(effect, 2f);
            }
        }
        
        private void UpdateDisplay()
        {
            var stats = PlayerStats.Instance;
            
            if (levelText != null)
            {
                levelText.text = $"Level {stats.CurrentLevel}";
            }
            
            int requiredEXP = stats.GetRequiredEXPForNextLevel();
            
            if (expText != null)
            {
                expText.text = $"EXP: {stats.CurrentEXP}/{requiredEXP}";
            }
            
            if (expBar != null)
            {
                expBar.value = stats.GetEXPProgress();
            }
        }
    }
}