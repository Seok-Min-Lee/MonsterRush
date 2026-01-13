using UnityEngine;

public class TitleDummy : MonoBehaviour
{
    [SerializeField][Range(0f, 2f)] private float speed;
    [SerializeField][Range(0, 10)] private int interval;

    private Vector3 direction;
    private float timer = 0f;
    private void Start()
    {
        direction = GetRandomDirection();
    }
    private void Update()
    {
        if (timer > interval)
        {
            timer = 0f;
            direction = GetRandomDirection();
        }

        timer += Time.deltaTime;
    }
    private void FixedUpdate()
    {
        transform.position += direction * Time.deltaTime * speed;
    }
    private Vector3 GetRandomDirection()
    {
        Vector2 rVec = Random.insideUnitCircle;

        return new Vector3(rVec.x, rVec.y, 0).normalized;
    }
}
