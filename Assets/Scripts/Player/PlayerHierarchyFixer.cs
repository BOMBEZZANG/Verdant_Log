using UnityEngine;

namespace VerdantLog.Player
{
    public class PlayerHierarchyFixer : MonoBehaviour
    {
        [ContextMenu("Fix Player Hierarchy")]
        public void FixHierarchy()
        {
            Debug.Log("=== Fixing Player Hierarchy ===");
            
            // Check if this GameObject has the wrong structure
            if (transform.childCount == 0 && GetComponent<SpriteRenderer>() != null)
            {
                Debug.Log("Detected incorrect hierarchy - SpriteRenderer on parent with no children");
                
                // Rename to Player if needed
                if (gameObject.name == "Visual" || gameObject.name.Contains("Visual"))
                {
                    gameObject.name = "Player";
                    Debug.Log("Renamed GameObject to 'Player'");
                }
                
                // Create Visual child
                GameObject visualChild = new GameObject("Visual");
                visualChild.transform.SetParent(transform, false);
                visualChild.transform.localPosition = Vector3.zero;
                visualChild.transform.localScale = Vector3.one;
                Debug.Log("Created 'Visual' child object");
                
                // Move SpriteRenderer to child
                SpriteRenderer sr = GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    // Save sprite data
                    Sprite sprite = sr.sprite;
                    Color color = sr.color;
                    string sortingLayer = sr.sortingLayerName;
                    int orderInLayer = sr.sortingOrder;
                    
                    // Destroy old component
                    DestroyImmediate(sr);
                    
                    // Create new one on child
                    SpriteRenderer newSR = visualChild.AddComponent<SpriteRenderer>();
                    newSR.sprite = sprite;
                    newSR.color = color;
                    newSR.sortingLayerName = sortingLayer;
                    newSR.sortingOrder = orderInLayer;
                    
                    Debug.Log("Moved SpriteRenderer to Visual child");
                }
                
                // Update PlayerController reference
                PlayerController controller = GetComponent<PlayerController>();
                if (controller != null)
                {
                    var srField = controller.GetType().GetField("spriteRenderer", 
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (srField != null)
                    {
                        srField.SetValue(controller, visualChild.GetComponent<SpriteRenderer>());
                        Debug.Log("Updated PlayerController sprite reference");
                    }
                }
                
                Debug.Log("=== Hierarchy Fixed! ===");
                Debug.Log("Structure is now: Player → Visual (child) → SpriteRenderer");
            }
            else if (transform.childCount > 0)
            {
                Debug.Log("Hierarchy appears correct - has child objects");
                
                // Just ensure naming is correct
                if (gameObject.name == "Visual")
                {
                    gameObject.name = "Player";
                    Debug.Log("Renamed main GameObject to 'Player'");
                }
            }
            else
            {
                Debug.LogWarning("No SpriteRenderer found - please add one to Visual child");
            }
        }
        
        private void Reset()
        {
            // Auto-fix on component add
            if (Application.isPlaying)
                return;
                
            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (this != null)
                {
                    FixHierarchy();
                }
            };
        }
    }
}