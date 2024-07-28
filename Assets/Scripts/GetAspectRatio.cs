using UnityEngine;

public class ObjectSize : MonoBehaviour
{
    void Start()
    {
        // Get Renderer bounds
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            Bounds rendererBounds = renderer.bounds;
            Vector3 rendererSize = rendererBounds.size;
            Debug.Log($"Renderer Bounds Size: {rendererSize}");
        }
        else
        {
            Debug.LogWarning("No Renderer component found.");
        }

        // Get Collider bounds
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            Bounds colliderBounds = collider.bounds;
            Vector3 colliderSize = colliderBounds.size;
            Debug.Log($"Collider Bounds Size: {colliderSize}");
        }
        else
        {
            Debug.LogWarning("No Collider component found.");
        }
    }
}
