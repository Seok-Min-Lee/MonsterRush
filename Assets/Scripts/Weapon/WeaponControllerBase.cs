using UnityEngine;

public abstract class WeaponControllerBase : MonoBehaviour
{
    protected int LEVEL_MAX = 8;

    [SerializeField] protected StateToggle stateToggle;
    public abstract void OnClickStateToggle();
    public abstract void Init();
    public abstract void Strengthen(int key);
}
