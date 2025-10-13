using UnityEngine;

public class TrackingCamera : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private float smoothSpeed = 5f; // 따라가는 속도 (보간)

    private Transform player;
    private void LateUpdate()
    {
        if (player == null)
        {
            player = Player.Instance.transform;
            return;
        }

        Vector3 targetPos = player.position + offset;
        Vector3 smoothPos = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);

        transform.position = new Vector3(smoothPos.x, smoothPos.y, transform.position.z);
    }
}
