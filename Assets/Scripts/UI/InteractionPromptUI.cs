using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VerdantLog.Player;

namespace VerdantLog.UI
{
    public class InteractionPromptUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject promptPanel;
        [SerializeField] private TextMeshProUGUI promptText;
        [SerializeField] private Image iconImage;
        [SerializeField] private string interactionKey = "E";
        
        [Header("Animation")]
        [SerializeField] private float fadeSpeed = 5f;
        [SerializeField] private bool animateScale = true;
        [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0.8f, 1, 1);
        
        private CanvasGroup canvasGroup;
        private RectTransform rectTransform;
        private Interactable currentInteractable;
        private float targetAlpha = 0f;
        private float currentScale = 0f;
        
        private void Awake()
        {
            if (promptPanel != null)
            {
                canvasGroup = promptPanel.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = promptPanel.AddComponent<CanvasGroup>();
                }
                
                rectTransform = promptPanel.GetComponent<RectTransform>();
                
                promptPanel.SetActive(false);
                canvasGroup.alpha = 0f;
            }
        }
        
        private void OnEnable()
        {
            PlayerInteractor.OnInteractableDetected += OnInteractableDetected;
            PlayerInteractor.OnInteractableExited += OnInteractableExited;
        }
        
        private void OnDisable()
        {
            PlayerInteractor.OnInteractableDetected -= OnInteractableDetected;
            PlayerInteractor.OnInteractableExited -= OnInteractableExited;
        }
        
        private void Update()
        {
            UpdatePromptVisibility();
            UpdatePromptAnimation();
        }
        
        private void OnInteractableDetected(Interactable interactable)
        {
            currentInteractable = interactable;
            ShowPrompt();
        }
        
        private void OnInteractableExited()
        {
            currentInteractable = null;
            HidePrompt();
        }
        
        private void ShowPrompt()
        {
            if (currentInteractable == null || promptPanel == null)
                return;
                
            promptPanel.SetActive(true);
            targetAlpha = 1f;
            
            UpdatePromptText();
            
            if (animateScale)
            {
                currentScale = 0f;
            }
        }
        
        private void HidePrompt()
        {
            targetAlpha = 0f;
        }
        
        private void UpdatePromptText()
        {
            if (promptText != null && currentInteractable != null)
            {
                string actionName = currentInteractable.GetInteractionName();
                
                if (currentInteractable.RequiresItem())
                {
                    string itemID = currentInteractable.GetRequiredItemID();
                    promptText.text = $"[{interactionKey}] {actionName} (Requires {itemID})";
                }
                else
                {
                    promptText.text = $"[{interactionKey}] {actionName}";
                }
            }
        }
        
        private void UpdatePromptVisibility()
        {
            if (canvasGroup == null || promptPanel == null)
                return;
                
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, fadeSpeed * Time.deltaTime);
            
            if (canvasGroup.alpha < 0.01f && targetAlpha == 0f)
            {
                promptPanel.SetActive(false);
            }
        }
        
        private void UpdatePromptAnimation()
        {
            if (!animateScale || rectTransform == null)
                return;
                
            if (targetAlpha > 0.5f)
            {
                currentScale = Mathf.Lerp(currentScale, 1f, fadeSpeed * Time.deltaTime);
            }
            else
            {
                currentScale = Mathf.Lerp(currentScale, 0f, fadeSpeed * 2f * Time.deltaTime);
            }
            
            float scale = scaleCurve.Evaluate(currentScale);
            rectTransform.localScale = Vector3.one * scale;
        }
        
        public void SetInteractionKey(string key)
        {
            interactionKey = key;
            if (currentInteractable != null)
            {
                UpdatePromptText();
            }
        }
        
        public void SetIcon(Sprite icon)
        {
            if (iconImage != null)
            {
                iconImage.sprite = icon;
                iconImage.gameObject.SetActive(icon != null);
            }
        }
    }
}