using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public static Player Instance;

    [SerializeField] private float speed;

    private Vector3 moveVec;
    private void Start()
    {
        Instance = this;
    }
    private void Update()
    {
        transform.position += moveVec;
    }
    private void OnMove(InputValue value)
    {
        Vector2 v = value.Get<Vector2>();

        if (v != null)
        {
            moveVec = new Vector3(v.x, v.y, 0f) * speed;
        }
    }
}
