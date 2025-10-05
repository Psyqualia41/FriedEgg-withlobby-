using UnityEngine;

public class ParallaxUI : MonoBehaviour
{
    [System.Serializable]
    public class ParallaxLayer
    {
        public RectTransform layer;
        public float parallaxFactor;
    }

    public ParallaxLayer[] layers; 
    public float smoothing = 5f;
    private Vector2 lastMousePosition;

    void Start()
    {
        lastMousePosition = Input.mousePosition; 
    }

    void Update()
    {
        Vector2 mouseDelta = (Vector2)Input.mousePosition - lastMousePosition;

        if (mouseDelta.magnitude > 0) 
        {
            foreach (var layer in layers)
            {
                if (layer.layer != null)
                {
                    Vector2 targetPosition = layer.layer.anchoredPosition + (mouseDelta * layer.parallaxFactor);
                    layer.layer.anchoredPosition = Vector2.Lerp(layer.layer.anchoredPosition, targetPosition, Time.deltaTime * smoothing);
                }
            }
        }

        lastMousePosition = Input.mousePosition;
    }
}
