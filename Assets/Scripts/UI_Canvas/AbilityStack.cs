using UnityEngine;

public class AbilityStack : MonoBehaviour
{
    [SerializeField] private GameObject[] blocks;

    private void Start()
    {
        for (int i = 0; i < blocks.Length; i++)
        {
            blocks[i].gameObject.SetActive(false);
        }
    }
    public void Push(int index)
    {
        blocks[index].transform.SetAsLastSibling();
        blocks[index].gameObject.SetActive(true);
    }
}
