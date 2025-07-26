using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VerdantLog.Core;

namespace VerdantLog.UI
{
    public class NotificationUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject notificationPrefab;
        [SerializeField] private Transform notificationContainer;
        [SerializeField] private float notificationDuration = 3f;
        [SerializeField] private float fadeOutDuration = 0.5f;
        
        private void Start()
        {
            GameEvents.OnNotification += ShowNotification;
        }
        
        private void OnDestroy()
        {
            GameEvents.OnNotification -= ShowNotification;
        }
        
        public void ShowNotification(string message)
        {
            if (notificationPrefab == null || notificationContainer == null)
                return;
                
            GameObject notification = Instantiate(notificationPrefab, notificationContainer);
            TextMeshProUGUI text = notification.GetComponentInChildren<TextMeshProUGUI>();
            
            if (text != null)
            {
                text.text = message;
            }
            
            StartCoroutine(FadeOutAndDestroy(notification));
        }
        
        private IEnumerator FadeOutAndDestroy(GameObject notification)
        {
            yield return new WaitForSeconds(notificationDuration);
            
            CanvasGroup canvasGroup = notification.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = notification.AddComponent<CanvasGroup>();
            }
            
            float elapsedTime = 0f;
            while (elapsedTime < fadeOutDuration)
            {
                elapsedTime += Time.deltaTime;
                canvasGroup.alpha = 1f - (elapsedTime / fadeOutDuration);
                yield return null;
            }
            
            Destroy(notification);
        }
    }
}