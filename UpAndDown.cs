using UnityEngine;

public class UpAndDown : MonoBehaviour
{
    Vector2 ogPosition;
    public float range;
    private float angle;
    public float speed;

    RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        ogPosition = rectTransform.anchoredPosition;
    }

    void Update()
    {
        rectTransform.anchoredPosition = ogPosition + new Vector2(0, Mathf.Sin(angle) * range);
        angle += Time.deltaTime * speed;
    }
}