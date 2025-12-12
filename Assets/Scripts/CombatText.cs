using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshPro))]
public class CombatText : MonoBehaviour
{
    public enum Type { Damage, Heal, Cancel, StateChange }

    public struct Preset
    {
        public Preset(Type type, float size, float duration, Color color)
        {
            this.type = type;
            this.size = size;
            this.duration = duration;
            this.color = color;
        }
        public Type type;
        public float size;
        public float duration;
        public Color color;
    }

    private readonly Vector2 STATE_CHANGE_START = new Vector2(0f, 1f);
    private readonly Vector2 STATE_CHANGE_END = new Vector2(0f, 1.1f);

    private Preset[] options = new Preset[4]
    {
        new Preset(Type.Damage, 2f, 0.5f, Color.red),
        new Preset(Type.Heal, 2f, 0.5f, Color.green),
        new Preset(Type.Cancel, 2f, 0.5f, Color.white),
        new Preset(Type.StateChange, 1.5f, 3f, Color.yellow),
    };

    private Dictionary<Type, Preset> presetDictionary = new Dictionary<Type, Preset>();
    private TextMeshPro text;

    private void Awake()
    {
        text = GetComponent<TextMeshPro>();

        for (int i = 0; i < options.Length; i++)
        {
            presetDictionary.Add(options[i].type, options[i]);
        }
    }
    public void Init(Type type, string value, Transform parent)
    {

        if (presetDictionary.TryGetValue(type, out Preset option))
        {
            gameObject.SetActive(true);
            transform.parent = parent;

            Vector2 startPos, endPos;
            if (type == Type.StateChange)
            {
                startPos = STATE_CHANGE_START;
                endPos = STATE_CHANGE_END;
            }
            else
            {
                startPos = new Vector2(Random.Range(-0.25f, 0.25f), Random.Range(-0.1f, 0.1f));
                endPos = startPos + new Vector2(0, 0.2f);
            }

            text.text = value;
            text.fontSize = option.size;
            text.color = option.color;
            text.rectTransform.anchoredPosition = startPos;
            text.rectTransform.DOAnchorPos(endPos, option.duration).OnComplete(() =>
            {
                UIContainer.Instance.Push(this);
                gameObject.SetActive(false);
            });
        }
    }
}
