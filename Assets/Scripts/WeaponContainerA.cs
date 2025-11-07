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
    private void Update()
    {
        if (Time.timeScale == 0f)
        {
            return;
        }

        transform.Rotate(Vector3.back * speed);
    }
    public override void OnClickStateToggle()
    {
        isExpand = !isExpand;
        stateToggle.Init(isExpand);
        RefreshTransform(0.5f);
    }
    public override void StrengthenFirst()
    {
        base.StrengthenFirst();
        RefreshTransform(0.5f);
    }
    public override void StrengthenSecond()
    {
        base.StrengthenSecond();

        for (int i = 0; i < weapons.Count; i++)
        {
            weapons[i].Strengthen();
        }
    }
    private void RefreshTransform(float delay = 0f)
    {
        float radius = isExpand ? radiusMax : radiusMin;

        for (int i = 0; i < activeCount; i++)
        {
            float angle = i * Mathf.PI * 2f / activeCount; // 0 ~ 360도 균등 분할 (라디안)

            Vector3 pos = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
            Vector3 rot = new Vector3(0, 0, angle * Mathf.Rad2Deg);

            weapons[i].transform.DOLocalMove(pos, delay);
            weapons[i].transform.DOLocalRotate(rot, delay);
        }
    }
}
