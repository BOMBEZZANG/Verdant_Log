using System;
using UnityEngine;

namespace VerdantLog.Systems
{
    public class TimeManager : MonoBehaviour
    {
        private static TimeManager instance;
        public static TimeManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<TimeManager>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("TimeManager");
                        instance = go.AddComponent<TimeManager>();
                        DontDestroyOnLoad(go);
                    }
                }
                return instance;
            }
        }
        
        public static event Action<string> OnTimeOfDayChanged;
        
        [Header("Time Settings")]
        [SerializeField] private float dayDuration = 600f; // 10 minutes real time
        [SerializeField] private float currentTime = 0.5f; // 0-1, where 0.5 is noon
        
        [Header("Time Display")]
        [SerializeField] private int displayHour = 12;
        [SerializeField] private int displayMinute = 0;
        
        private string currentTimeOfDay = "Day";
        
        public float CurrentTime => currentTime;
        public string CurrentTimeOfDay => currentTimeOfDay;
        public int Hour => displayHour;
        public int Minute => displayMinute;
        public bool IsDay => currentTimeOfDay == "Day";
        public bool IsNight => currentTimeOfDay == "Night";
        
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            UpdateTimeOfDay();
        }
        
        private void Update()
        {
            if (dayDuration <= 0) return;
            
            currentTime += Time.deltaTime / dayDuration;
            
            if (currentTime >= 1f)
            {
                currentTime -= 1f;
            }
            
            UpdateDisplayTime();
            UpdateTimeOfDay();
        }
        
        private void UpdateDisplayTime()
        {
            float hours = currentTime * 24f;
            displayHour = Mathf.FloorToInt(hours);
            displayMinute = Mathf.FloorToInt((hours - displayHour) * 60f);
        }
        
        private void UpdateTimeOfDay()
        {
            string newTimeOfDay = (currentTime >= 0.25f && currentTime < 0.75f) ? "Day" : "Night";
            
            if (newTimeOfDay != currentTimeOfDay)
            {
                currentTimeOfDay = newTimeOfDay;
                OnTimeOfDayChanged?.Invoke(currentTimeOfDay);
            }
        }
        
        public void SetTime(float normalizedTime)
        {
            currentTime = Mathf.Clamp01(normalizedTime);
            UpdateDisplayTime();
            UpdateTimeOfDay();
        }
        
        public void SetTimeToDay()
        {
            SetTime(0.5f); // Noon
        }
        
        public void SetTimeToNight()
        {
            SetTime(0f); // Midnight
        }
        
        public string GetFormattedTime()
        {
            return $"{displayHour:D2}:{displayMinute:D2}";
        }
    }
}