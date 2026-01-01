using UnityEngine;

public class SuperArmorEnemy : Enemy
{
    [SerializeField] private Sprite[] superArmorMoves;
    private void Awake()
    {
        collider = GetComponent<BoxCollider2D>();
        rigidbody = GetComponent<Rigidbody2D>();

        enemyInfo = dataContainer.enemies[itemIndex];
    }
    public override void OnDamage(int damage)
    {
        if (state != State.Live)
        {
            return;
        }

        AudioManager.Instance.PlaySFX(SoundKey.EnemyHit);
        hp -= (int)(damage * (1 + bleedPower));

        if (hp > 0)
        {
            if (!isSuperArmor)
            {
                if (hp < hpMax * 0.5)
                {
                    OnSuperArmor();
                }
                else
                {
                    character.PlayAnimation(EnemyCharacter.AniType.Hit);
                }
            }

            hpBar.SetActive(true);
            hpGuage.localScale = new Vector3((float)hp / hpMax, 1, 1);
        }
        else
        {
            OnDeath();

            ItemContainer.Instance.Batch(itemIndex, transform.position);
            Player.Instance.IncreaseKill();
        }
    }
    public override void OnSuperArmor()
    {
        base.OnSuperArmor();
        character.OnSuperArmor(superArmorMoves);
        OffAddict();
        OffBleed();
        OffSlow();
    }
    public override void OffSuperArmor()
    {
        base.OffSuperArmor();
        character.OffSuperArmor();
    }
}
