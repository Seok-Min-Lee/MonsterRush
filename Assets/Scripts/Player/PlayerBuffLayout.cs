using System.Collections.Generic;
using UnityEngine;

public class PlayerBuffLayout : MonoBehaviour
{
    public float spacing = 1f;

    [SerializeField] private Transform[] children;

    private List<Vector3> positions = new List<Vector3>();
    private void Awake()
    {
        for (int i = 0; i < children.Length; i++)
        {
            positions.Add(children[i].localPosition);
        }
    }
    private void StackRebuild()
    {
        int activeCount = 0;
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i].gameObject.activeSelf)
            {
                activeCount++;
            }
        }

        int idx = 0;
        for (int i = 1; i < children.Length; i++)
        {
            Transform child = children[i];

            if (child.gameObject.activeSelf)
            {
                Vector3 pos = positions[idx];
                idx++;
            }
        }
    }
}