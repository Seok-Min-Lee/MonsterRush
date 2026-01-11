using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    public enum AniType { Idle, Move, Hit }
    private const float FRAME_INTERVAL = 0.03333f;

    [SerializeField] private Sprite[] idles;
    [SerializeField] private Sprite[] moves;
    [SerializeField] private Sprite[] hits;

    private SpriteRenderer renderer;

    private AniType aniType = AniType.Idle;
    private int index = 0;
    private float timer = 0f;
    private void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
    }
    public void UpdateTick(float time)
    {
        // animation
        timer += time;
        if (timer > FRAME_INTERVAL)
        {
            if (aniType == AniType.Move)
            {
                renderer.sprite = moves[index++ % moves.Length];
            }
            else if (aniType == AniType.Hit)
            {
                if (index < hits.Length)
                {
                    renderer.sprite = hits[index++];
                }
                else
                {
                    aniType = lastType;
                    index = 0;
                }
            }
            else
            {
                renderer.sprite = idles[index++ % idles.Length];
            }

            timer = 0f;
        }
    }

    private AniType lastType;
    public void PlayAnimation(AniType type)
    {
        if (aniType == type)
        {
            return;
        }

        if (aniType == AniType.Hit)
        {
            lastType = type;
            return;
        }

        lastType = aniType;
        aniType = type;
        index = 0;
        timer = type != AniType.Hit ? 0f : FRAME_INTERVAL;
    }
    public void FlipX(bool value)
    {
        renderer.flipX = value;
    }
    public void Init(Vector3 position)
    {
        transform.position = position;
    }
}
