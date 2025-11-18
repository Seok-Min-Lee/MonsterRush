using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameCtrl : MonoBehaviour
{
    public static GameCtrl Instance;

    [SerializeField] private TextMeshProUGUI timeText;

    [SerializeField] private TextMeshProUGUI timeScoreText;
    [SerializeField] private TextMeshProUGUI LevelScoreText;
    [SerializeField] private TextMeshProUGUI killScoreText;

    [SerializeField] private GameObject normalWindow;
    [SerializeField] private GameObject tutorialWindow;
    [SerializeField] private GameObject pauseWindow;
    [SerializeField] private GameObject rewardWindow;
    [SerializeField] private GameObject scoreWindow;
    [SerializeField] private GameObject settingWindow;

    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Toggle leftHandToggle;
    [SerializeField] private Toggle tutorialToggle;

    [SerializeField] private RewardButton[] rewardButtons;
    [SerializeField] private RewardInfo[] rewardInfoes;

    private Dictionary<int, RewardInfo> rewardInfoDictionary = new Dictionary<int, RewardInfo>();
    private Dictionary<int, RewardInfo> specialRewardDictionary = new Dictionary<int, RewardInfo>();
    private float timer = 0f;
    private void Awake()
    {
        Instance = this;

        foreach (RewardInfo info in rewardInfoes)
        {
            if (info.isSpecial)
            {
                specialRewardDictionary.Add(info.groupId + info.index, info);
            }
            else
            {
                rewardInfoDictionary.Add(info.groupId + info.index, info);
            }
        }
    }
    private void Start()
    {
        timeText.text = "00:00";

        if (!AudioManager.Instance.isLoadComplete)
        {
            AudioManager.Instance.Load(() =>
            {
                float volumeBGM = PlayerPrefs.GetFloat("volumeBGM");
                float volumeSFX = PlayerPrefs.GetFloat("volumeSFX");

                AudioManager.Instance.Init(volumeBGM, volumeSFX);
                AudioManager.Instance.PlayBGM(SoundKey.BGM);
            });
        }
#if UNITY_EDITOR
        Application.targetFrameRate = 30;
        QualitySettings.vSyncCount = 0;
#endif

        // 튜토리얼 UI 표시
        int playCount = PlayerPrefs.GetInt("playCount");
        if (playCount == 0 || PlayerPrefs.GetInt("visibleTutorial") == 1)
        {
            tutorialWindow.SetActive(true);
            StaticValues.isTutorial = true;
        }
        PlayerPrefs.SetInt("playCount", playCount + 1);

        //
        bool isLeftHand = PlayerPrefs.GetInt("isLeftHand") == 1;
        Player.Instance.OnChangeUI(isLeftHand);
    }
    private void Update()
    {
        timer += Time.deltaTime;

        int minutes = (int)(timer / 60);
        int seconds = (int)(timer % 60);

        timeText.text = $"{minutes:00}:{seconds:00}";
    }
    public void OnClickStart()
    {
        tutorialWindow.SetActive(false);
        StaticValues.isTutorial = false;
    }
    public void OnClickPause()
    {
        AudioManager.Instance.PlaySFX(SoundKey.GameTouch);
        Time.timeScale = 0f;
        pauseWindow.SetActive(true);
    }
    public void OnClickResume()
    {
        AudioManager.Instance.PlaySFX(SoundKey.GameTouch);
        pauseWindow.SetActive(false);
        Time.timeScale = 1f;
    }
    public void OnClickHome()
    {
        AudioManager.Instance.PlaySFX(SoundKey.GameTouch);
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("01_Title");
    }
    public void OnClickSetting()
    {
        AudioManager.Instance.PlaySFX(SoundKey.GameTouch);
        settingWindow.SetActive(true);
        pauseWindow.SetActive(false);

        bgmSlider.value = PlayerPrefs.GetFloat("volumeBGM");
        sfxSlider.value = PlayerPrefs.GetFloat("volumeSFX");
        tutorialToggle.isOn = PlayerPrefs.GetInt("visibleTutorial") == 1;
        leftHandToggle.isOn = PlayerPrefs.GetInt("isLeftHand") == 1;
    }
    public void OnValueChangedVolumeBGM()
    {
        AudioManager.Instance.SetVolumeBGM(bgmSlider.value);
    }
    public void OnValueChangedVolumeSFX()
    {
        AudioManager.Instance.SetVolumeSFX(sfxSlider.value);
    }
    public void OnClickSaveSeting()
    {
        AudioManager.Instance.PlaySFX(SoundKey.GameTouch);
        settingWindow.SetActive(false);
        pauseWindow.SetActive(true);

        PlayerPrefs.SetFloat("volumeBGM", bgmSlider.value);
        PlayerPrefs.SetFloat("volumeSFX", sfxSlider.value);
        PlayerPrefs.SetInt("visibleTutorial", tutorialToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt("isLeftHand", leftHandToggle.isOn ? 1 : 0);

        Player.Instance.OnChangeUI(leftHandToggle.isOn);
    }
    public void OnClickCancelSetting()
    {
        AudioManager.Instance.PlaySFX(SoundKey.GameTouch);
        settingWindow.SetActive(false);
        pauseWindow.SetActive(true);

        float volumeBGM = PlayerPrefs.GetFloat("volumeBGM");
        float volumeSFX = PlayerPrefs.GetFloat("volumeSFX");

        AudioManager.Instance.Init(volumeBGM, volumeSFX);
    }
    public void OnLevelUp()
    {
        Time.timeScale = 0f;

        List<RewardInfo> samples = new List<RewardInfo>();

        int[,] temp = new int[,]
        {
            { Player.Instance.weaponALevel, 1000, 1001, 1002, 1099 },
            { Player.Instance.weaponBLevel, 2000, 2001, 2002, 2099 },
            { Player.Instance.weaponCLevel, 3000, 3001, 3002, 3099 },
            { Player.Instance.weaponDLevel, 4000, 4001, 4002, 4099 },
        };
        int rows = temp.GetLength(0);

        for (int i = 0; i < rows; i++)
        {
            int level = temp[i, 0];
            if (level < 1)
            {
                samples.Add(rewardInfoDictionary[temp[i, 1]]);
            }
            else if (level < 8)
            {
                samples.Add(rewardInfoDictionary[temp[i, 2]]);
            }
            else if (level < 16)
            {
                samples.Add(rewardInfoDictionary[temp[i, 3]]);
            }

            if (level > 0 && specialRewardDictionary.ContainsKey(temp[i, 3]) && Random.Range(0, 10) == 0)
            {
                samples.Add(specialRewardDictionary[temp[i, 4]]);
            }
        }

        samples.Add(rewardInfoDictionary[0]);
        samples.Add(rewardInfoDictionary[1]);
        samples.Add(rewardInfoDictionary[2]);
        samples.Add(rewardInfoDictionary[3]);

        List<RewardInfo> randoms = Utils.Shuffle<RewardInfo>(samples);

        for (int i = 0; i < rewardButtons.Length; i++)
        {
            rewardButtons[i].Init(randoms[i]);
        }

        rewardWindow.SetActive(true);
    }
    public void OnClickReward(RewardInfo rewardInfo)
    {
        rewardWindow.SetActive(false);

        if (rewardInfo.isSpecial)
        {
            specialRewardDictionary.Remove(rewardInfo.groupId + rewardInfo.index);
        }

        Time.timeScale = 1f;
    }
    public void OnGameEnd()
    {
        Sequence seq = DOTween.Sequence();

        // 엔딩 연출
        seq.AppendCallback(() =>
        {
            AudioManager.Instance.PlaySFX(SoundKey.GameEnd);

            int minutes = (int)(timer / 60);
            int seconds = (int)(timer % 60);
            timeScoreText.text = $"{minutes:00}:{seconds:00}";
            LevelScoreText.text = $"{Player.Instance.Level}";
            killScoreText.text = $"{Player.Instance.killCount.ToString("#,##0")}";

            normalWindow.GetComponent<CanvasGroup>().DOFade(0f, 0.5f);
            Player.Instance.OnDeath();
        });

        // 딜레이
        seq.AppendInterval(2.5f);

        // 결과창 표시
        seq.AppendCallback(() => 
        {
            scoreWindow.SetActive(true);
            CanvasGroup cg = scoreWindow.GetComponent<CanvasGroup>();
            cg.alpha = 0f;
            cg.DOFade(1f, 0.5f);
        });
    }
}
