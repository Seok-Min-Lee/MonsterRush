using UnityEngine;

public class DragSign : MonoBehaviour
{
    [SerializeField] float radius;
    [SerializeField] float speed;
    [SerializeField] Vector2 offset;

    private RectTransform rectTransform;
    private float angle = 0f;
    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    public void Update()
    {
        angle -= speed * Mathf.PI;
        rectTransform.anchoredPosition = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius + offset;
    }
}
