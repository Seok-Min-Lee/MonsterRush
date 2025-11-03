using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameCtrl : MonoBehaviour
{
    public static GameCtrl Instance;

    [SerializeField] private TextMeshProUGUI timeText;

    [SerializeField] private TextMeshProUGUI timeScoreText;
    [SerializeField] private TextMeshProUGUI killScoreText;

    [SerializeField] private GameObject pauseWindow;
    [SerializeField] private GameObject rewardWindow;
    [SerializeField] private GameObject scoreWindow;
    [SerializeField] private GameObject settingWindow;

    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    [SerializeField] private RewardButton[] rewardButtons;
    [SerializeField] private RewardInfo[] rewardInfoes;

    private Dictionary<int, RewardInfo> rewardInfoDictionary = new Dictionary<int, RewardInfo>();
    private float timer = 0f;
    private void Awake()
    {
        Instance = this;

        foreach (RewardInfo info in rewardInfoes)
        {
            rewardInfoDictionary.Add(info.id, info);
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
    }
    private void Update()
    {
        timer += Time.deltaTime;

        int minutes = (int)(timer / 60);
        int seconds = (int)(timer % 60);

        timeText.text = $"{minutes:00}:{seconds:00}";
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
        int levelA = Player.Instance.weaponALevel;
        int levelB = Player.Instance.weaponBLevel;
        int levelC = Player.Instance.weaponCLevel;
        int levelD = Player.Instance.weaponDLevel;

        if (levelA < 16)
        {
            samples.Add(levelA < 8 ? rewardInfoDictionary[0] : rewardInfoDictionary[1]);
        }

        if (levelB < 16)
        {
            samples.Add(levelB < 8 ? rewardInfoDictionary[10] : rewardInfoDictionary[11]);
        }
        if (levelC < 16)
        {
            samples.Add(levelC < 8 ? rewardInfoDictionary[20] : rewardInfoDictionary[21]);
        }
        if (levelD < 16)
        {
            samples.Add(levelD < 8 ? rewardInfoDictionary[30] : rewardInfoDictionary[31]);
        }

        samples.Add(rewardInfoDictionary[90]);
        samples.Add(rewardInfoDictionary[91]);
        samples.Add(rewardInfoDictionary[92]);
        samples.Add(rewardInfoDictionary[93]);

        List<RewardInfo> randoms = Utils.Shuffle<RewardInfo>(samples);

        for (int i = 0; i < rewardButtons.Length; i++)
        {
            rewardButtons[i].Init(randoms[i]);
        }

        rewardWindow.SetActive(true);
    }
    public void OnClickReward()
    {
        rewardWindow.SetActive(false);
        Time.timeScale = 1f;
    }
    public void OnGameEnd()
    {
        AudioManager.Instance.PlaySFX(SoundKey.GameEnd);
        Time.timeScale = 0f;

        float time = Time.time;
        int minutes = (int)(time / 60);
        int seconds = (int)(time % 60);

        timeScoreText.text = $"생존 시간\n{minutes:00}:{seconds:00}";
        killScoreText.text = $"처치 수\n {Player.Instance.killCount.ToString("#,##0")}";

        scoreWindow.SetActive(true);
    }
}
