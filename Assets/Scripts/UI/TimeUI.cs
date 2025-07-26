using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VerdantLog.Core;
using VerdantLog.Systems;

namespace VerdantLog.UI
{
    public class TimeUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private TextMeshProUGUI dayNightText;
        [SerializeField] private Image dayNightIcon;
        [SerializeField] private Sprite dayIcon;
        [SerializeField] private Sprite nightIcon;
        
        private void Start()
        {
            GameEvents.OnTimeOfDayChanged += UpdateDayNightDisplay;
            UpdateDisplay();
        }
        
        private void OnDestroy()
        {
            GameEvents.OnTimeOfDayChanged -= UpdateDayNightDisplay;
        }
        
        private void Update()
        {
            UpdateDisplay();
        }
        
        private void UpdateDisplay()
        {
            var timeManager = TimeManager.Instance;
            
            if (timeText != null)
            {
                timeText.text = timeManager.GetFormattedTime();
            }
            
            UpdateDayNightDisplay(timeManager.CurrentTimeOfDay);
        }
        
        private void UpdateDayNightDisplay(string timeOfDay)
        {
            if (dayNightText != null)
            {
                dayNightText.text = timeOfDay;
            }
            
            if (dayNightIcon != null)
            {
                if (timeOfDay == "Day" && dayIcon != null)
                {
                    dayNightIcon.sprite = dayIcon;
                }
                else if (timeOfDay == "Night" && nightIcon != null)
                {
                    dayNightIcon.sprite = nightIcon;
                }
            }
        }
    }
}