using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] protected int power;

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(TagKeys.ENEMY))
        {
            collision.gameObject.GetComponent<Enemy>().OnDamage(power + Player.Instance.AdditionalDamage);
        }
    }
}
