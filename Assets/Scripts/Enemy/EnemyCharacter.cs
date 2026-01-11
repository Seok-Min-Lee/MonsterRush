using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyCharacter : MonoBehaviour
{
    public enum AniType { Move, Hit }
    public enum ColorType { Default, Slow }
    private const float FRAME_INTERVAL = 0.03333f;

    [SerializeField] private Sprite[] moves;
    [SerializeField] private Sprite[] hits; 
    [SerializeField] private Color slowColor; 
    [SerializeField] private Color defaultColor;

    private SpriteRenderer renderer;
    private Sprite[] moveSequences;

    private ColorType colorType = ColorType.Default;
    private AniType anitype = AniType.Move;
    private int index = 0;
    private float timer = 0f;
    private void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();

        moveSequences = moves;
    }
    public void UpdateTick(float time)
    {
        // animation
        timer += time;
        if (timer > FRAME_INTERVAL)
        {
            if (anitype == AniType.Move)
            {
                renderer.sprite = moveSequences[index++ % moveSequences.Length];
            }
            else
            {
                if (index < hits.Length)
                {
                    renderer.sprite = hits[index++];
                }
                else
                {
                    anitype = AniType.Move;
                    index = 0;
                }
            }

            timer = 0f;
        }
    }
    public void PlayAnimation(AniType type)
    {
        if (anitype == type)
        {
            return;
        }
        anitype = type;

        index = 0;
        timer = type != AniType.Hit ? 0f : FRAME_INTERVAL;
    }
    public void ChangeColor(ColorType type)
    {
        if (colorType == type)
        {
            return;
        }
        colorType = type;

        switch (type)
        {
            case ColorType.Default:
                renderer.color = defaultColor;
                break;

            case ColorType.Slow:
                renderer.color = slowColor;
                break;
        }
    }
    public void FlipX(bool value)
    {
        renderer.flipX = value;
    }
    public void Init(Vector3 position)
    {
        transform.position = position;
    }
    public void OnSuperArmor(Sprite[] moves)
    {
        moveSequences = moves;
    }
    public void OffSuperArmor()
    {
        moveSequences = moves;
    }
}
