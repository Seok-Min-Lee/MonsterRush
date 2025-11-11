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
    private bool isExpand = false;
    private void Awake()
    {
        base.Init();
        stateToggle.Init(isExpand);
    }
    private void FixedUpdate()
    {
        transform.Rotate(Vector3.back * speed);
    }
    public override void OnClickStateToggle()
    {
        base.OnClickStateToggle();

        isExpand = !isExpand;
        stateToggle.Init(isExpand);
        RefreshTransform();
    }
    public override void StrengthenFirst()
    {
        base.StrengthenFirst();
        RefreshTransform();
    }
    public override void StrengthenSecond()
    {
        for (int i = 0; i < weapons.Count; i++)
        {
            weapons[i].Strengthen();
        }
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
