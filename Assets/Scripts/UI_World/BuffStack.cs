using System.Collections.Generic;
using UnityEngine;

public class BuffStack : MonoBehaviour
{
    [SerializeField] private BuffBlock[] buffBlocks;

    private List<Vector3> buffStackPositions = new List<Vector3>();

    private void Awake()
    {
        for (int i = 0; i < buffBlocks.Length; i++)
        {
            buffStackPositions.Add(buffBlocks[i].transform.localPosition);
            buffBlocks[i].Hide();
        }
    }
    public void SetValue(int index, string value)
    {
        buffBlocks[index].SetValue(value);

        ShowBlock(index);
    }
    public void Rebuild()
    {
        int idx = 0;
        for (int i = 0; i < buffBlocks.Length; i++)
        {
            Transform child = buffBlocks[i].transform;

            if (child.gameObject.activeSelf)
            {
                child.localPosition = buffStackPositions[idx++];
            }
        }
    }
    public void ShowBlock(int index)
    {
        if (!buffBlocks[index].gameObject.activeSelf)
        {
            buffBlocks[index].Show();
            Rebuild();
        }
    }
    public void ShowBlockAll()
    {
        for (int i = 0; i < buffBlocks.Length; i++)
        {
            buffBlocks[i].Show();
        }
    }
    public void HideBlock(int index)
    {
        if (buffBlocks[index].gameObject.activeSelf)
        {
            buffBlocks[index].Hide();
            Rebuild();
        }
    }
    public void HideBlockAll()
    {
        for(int i = 0; i < buffBlocks.Length; i++)
        {
            buffBlocks[i].Hide();
        }
    }
}
