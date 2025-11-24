using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponContainerA : WeaponContainer<WeaponA>
{
    [SerializeField] private float radiusMin = 0.5f;
    [SerializeField] private float radiusMax = 1f;
    [SerializeField] private float speed = 1f;

    [Header("Debug")]
    [SerializeField] private int powerLevel = 0;
    [SerializeField] private bool isKnockback = false;
    
    private bool isExpand = false;
    private void Awake()
    {
        base.Init();
    }
    private void FixedUpdate()
    {
        if (StaticValues.isWait)
        {
            return;
        }

        for (int i = 0; i < activeCount; i++)
        {
            weapons[i].UpdateTick();
        }

        transform.Rotate(Vector3.back * speed);
    }
    public override void OnClickStateToggle()
    {
        base.OnClickStateToggle();

        isExpand = !isExpand;
        stateToggle.Init(isExpand);
        RefreshTransform();
    }
    public override void Strengthen(int key)
    {
        switch (key)
        {
            case 0: // 획득
            case 1: // 개수 증가
                StrengthenFirst();
                break;
            case 2: // 피해량 증가
                powerLevel++;
                for (int i = 0; i < weapons.Count; i++)
                {
                    weapons[i].PowerUp(powerLevel);
                }
                break;
            case 99: // 넉백 활성화
                isKnockback = true;
                for (int i = 0; i < weapons.Count; i++)
                {
                    weapons[i].ActivateKnockback();
                }
                break;
        }
    }
    private void StrengthenFirst()
    {
        if (activeCount >= WEAPON_COUNT_MAX)
        {
            return;
        }

        if (activeCount == 0)
        {
            stateToggle.Unlock();
            stateToggle.Init(isExpand);
        }

        weapons[activeCount++].gameObject.SetActive(true);
        RefreshTransform();
    }
    private void RefreshTransform()
    {
        // 자식들을 원 위에 동일한 간격으로 배치
        float radius = isExpand ? radiusMax : radiusMin;

        for (int i = 0; i < activeCount; i++)
        {
            float angle = i * Mathf.PI * 2f / activeCount;

            Vector3 pos = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
            Vector3 rot = new Vector3(0, 0, angle * Mathf.Rad2Deg);

            weapons[i].transform.DOLocalMove(pos, 0.5f);
            weapons[i].transform.DOLocalRotate(rot, 0.5f);
        }
    }
}
