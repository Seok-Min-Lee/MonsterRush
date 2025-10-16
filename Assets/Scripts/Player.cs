using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public static Player Instance;
    [SerializeField] private Transform[] weaponContainers;
    [SerializeField] private float speed;
    [SerializeField] private int strength;

    [Header("UI")]
    [SerializeField] private VariableJoystick joystick;
    [SerializeField] private GameObject canvasGO;
    [SerializeField] private Image hpGuage;
    [SerializeField] private Image expGuage;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI killText;

    public int Strength => strength;

    public Vector3 moveVec { get; private set; }

    private int exp = 0;
    private int expMax = 100;
    private int level = 1;
    private int killCount = 0;

    private void Start()
    {
        Instance = this;

        canvasGO.SetActive(false);
        hpGuage.fillAmount = 1f;

        expGuage.fillAmount = 0f;
        levelText.text = level.ToString();
        killText.text = killCount.ToString();
    }
    private void Update()
    {
        if (Time.timeScale == 0)
        {
            return;
        }

#if UNITY_EDITOR
        transform.position += moveVec;
#else
        transform.position += new Vector3(joystick.Horizontal, joystick.Vertical, 0f) * speed;

        float angle = Mathf.Atan2(joystick.Vertical, joystick.Horizontal) * Mathf.Rad2Deg;
        weaponContainers[1].rotation = Quaternion.Euler(0, 0, angle);
#endif
    }
    public void GainExp(int value)
    {
        exp += value;
        killCount++;

        if (exp > expMax)
        {
            level++;
            exp -= expMax;
            expMax = (int)(expMax * 1.1f);

            levelText.text = level.ToString();

            // 입력 초기화
            var pointerData = new PointerEventData(EventSystem.current)
            {
                pointerId = -1,
                position = Input.mousePosition
            };
            ExecuteEvents.Execute<IPointerUpHandler>(joystick.gameObject, pointerData, ExecuteEvents.pointerUpHandler);
            ExecuteEvents.Execute<IEndDragHandler>(joystick.gameObject, pointerData, ExecuteEvents.endDragHandler);

            //
            GameCtrl.Instance.OnLevelUp();
        }

        expGuage.fillAmount = exp / (float)expMax;
        Debug.Log(expGuage.fillAmount);
    }
    private void OnMove(InputValue value)
    {
        Vector2 v = value.Get<Vector2>();

        if (v != null)
        {
            moveVec = new Vector3(v.x, v.y, 0f) * speed;

            float angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
            weaponContainers[1].rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
