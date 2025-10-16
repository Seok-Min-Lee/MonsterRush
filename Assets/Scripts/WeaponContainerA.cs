using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponContainerA : WeaponContainer<WeaponA>
{
    [SerializeField] private float radius = 1f;
    [SerializeField] private float speed = 1f;
    private void Start()
    {
        base.Init();
    }
    private void Update()
    {
        if (Time.timeScale == 0f)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Add();
        }

        transform.Rotate(Vector3.forward * speed);
    }
    public override void Add()
    {
        base.Add();

        List<WeaponA> actives = new List<WeaponA>();
        actives.AddRange(weapons.Where(x => x.gameObject.activeSelf));

        int activeCount = actives.Count;
        for (int i = 0; i < activeCount; i++)
        {
            float angle = i * Mathf.PI * 2f / activeCount; // 0 ~ 360도 균등 분할 (라디안)
            Vector3 pos = Player.Instance.transform.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;

            actives[i].transform.position = pos;

            float degrees = angle * Mathf.Rad2Deg;
            actives[i].transform.rotation = Quaternion.Euler(0, 0, degrees);
        }
    }
}
