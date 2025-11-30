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

    [SerializeField] private NormalWindow normalWindow;
    [SerializeField] private GameObject pauseWindow;
    [SerializeField] private GameObject rewardWindow;
    [SerializeField] private EndingWindow endingWindow;
    [SerializeField] private GameObject settingWindow;
    [SerializeField] private TutorialWindow tutorialWindow;

    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Toggle leftHandToggle;
    [SerializeField] private Toggle tutorialToggle;

    [SerializeField] private RewardButton[] rewardButtons;
    [SerializeField] private RewardInfo[] rewardInfoes;

    private Dictionary<RewardInfo.Type, Dictionary<int, RewardInfo>> rewardGroup = new Dictionary<RewardInfo.Type, Dictionary<int, RewardInfo>>();
    private float timer = 0f;
    private void Awake()
    {
        Instance = this;

        foreach (IGrouping<RewardInfo.Type, RewardInfo> group in rewardInfoes.GroupBy(x => x.type))
        {
            Dictionary<int, RewardInfo> dictionary = new Dictionary<int, RewardInfo>();

            foreach (RewardInfo info in group)
            {
                dictionary.Add(info.UniqueKey, info);
            }

            rewardGroup.Add(group.Key, dictionary);
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
        bool isVisibleTutorial = PlayerPrefs.GetInt("visibleTutorial") == 1;
        StaticValues.isWait = isVisibleTutorial;
        tutorialWindow.Init(isVisibleTutorial);

        if (!StaticValues.isWait)
        {
            OnGameStart();
        }

        //
        bool isLeftHand = PlayerPrefs.GetInt("isLeftHand") == 1;
        Player.Instance.OnChangeUI(isLeftHand);
        tutorialWindow.OnChangeUI(isLeftHand);
    }
    private void Update()
    {
        if (StaticValues.isWait)
        {
            return;
        }

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

        if (!rewardWindow.gameObject.activeSelf)
        {
            Time.timeScale = 1f;
        }
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

        tutorialWindow.OnChangeTutorial(tutorialToggle.isOn);

        Player.Instance.OnChangeUI(leftHandToggle.isOn);
        tutorialWindow.OnChangeUI(leftHandToggle.isOn);
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
    public void OnGameStart()
    {
        StaticValues.isWait = false;
        tutorialWindow.OnGameStart();
        normalWindow.Alert("목표: Lv 80 도달!");
    }
    public void OnGameChallenge()
    {
        StaticValues.isWait = false;
        normalWindow.Show();
        DOVirtual.DelayedCall(1f, () => { normalWindow.Alert("목표: 오래 살아남기!"); });
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
                samples.Add(rewardGroup[RewardInfo.Type.Weapon][temp[i, 1]]);
            }
            else if (level < 8)
            {
                samples.Add(rewardGroup[RewardInfo.Type.Weapon][temp[i, 2]]);
            }
            else if (level < 16)
            {
                samples.Add(rewardGroup[RewardInfo.Type.Weapon][temp[i, 3]]);
            }

            if (level > 0 && rewardGroup[RewardInfo.Type.Ability].ContainsKey(temp[i, 4]) && Random.Range(0, 10) == 0)
            {
                samples.Add(rewardGroup[RewardInfo.Type.Ability][temp[i, 4]]);
            }
        }

        samples.Add(rewardGroup[RewardInfo.Type.Player][0]);
        samples.Add(rewardGroup[RewardInfo.Type.Player][1]);
        samples.Add(rewardGroup[RewardInfo.Type.Player][2]);
        samples.Add(rewardGroup[RewardInfo.Type.Player][3]);

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
        
        if (rewardInfo.type == RewardInfo.Type.Ability)
        {
            rewardGroup[RewardInfo.Type.Ability].Remove(rewardInfo.UniqueKey);
        }

        Time.timeScale = 1f;

        tutorialWindow.OnReward(rewardInfo);
    }
    public void OnGameEnd(GameResult result)
    {
        switch (result)
        {
            case GameResult.Clear:
                OnGameEndClear();
                break;
            case GameResult.Defeat:
                OnGameEndDefeat();
                break;
            case GameResult.Challenge:
                OnGameEndChallenge();
                break;
        }
    }
    private void OnGameEndClear()
    {
        List<Enemy> enemies = EnemyContainer.Instance.GetActiveEnemyAll();
        Vector3 playerPosition = Player.Instance.transform.position;

        Sequence seq = DOTween.Sequence();

        // 엔딩 연출
        seq.AppendCallback(() =>
        {
            StaticValues.isWait = true;
            AudioManager.Instance.PlaySFX(SoundKey.GameEndClear);

            normalWindow.Alert("목표 달성! (Lv 80 도달)");
        });

        // 딜레이
        seq.AppendInterval(2f);

        seq.AppendCallback(() =>
        {
            normalWindow.Hide();
            endingWindow.Init(GameResult.Clear, timer);
        });

        // 몬스터 제거
        foreach (Enemy enemy in enemies.OrderBy(e => e.toPlayer.sqrMagnitude))
        {
            enemy.OnShutdown();

            seq.AppendCallback(() => enemy.OnDeath());
            seq.AppendInterval(Time.deltaTime);
        }

        // 딜레이
        seq.AppendInterval(1f);

        // 결과창 표시
        seq.AppendCallback(() =>
        {
            endingWindow.gameObject.SetActive(true);
            endingWindow.Show();
        });
    }
    private void OnGameEndDefeat()
    {
        Sequence seq = DOTween.Sequence();

        // 엔딩 연출
        seq.AppendCallback(() =>
        {
            AudioManager.Instance.PlaySFX(SoundKey.GameEndDefeat);

            normalWindow.Hide();
            endingWindow.Init(GameResult.Defeat, timer);
        });

        // 딜레이
        seq.AppendInterval(2.5f);

        // 결과창 표시
        seq.AppendCallback(() =>
        {
            endingWindow.gameObject.SetActive(true);
            endingWindow.Show();
        });
    }
    private void OnGameEndChallenge()
    {
        Sequence seq = DOTween.Sequence();

        // 엔딩 연출
        seq.AppendCallback(() =>
        {
            AudioManager.Instance.PlaySFX(SoundKey.GameEndChallenge);

            normalWindow.Hide();
            endingWindow.Init(GameResult.Challenge, timer);
        });

        // 딜레이
        seq.AppendInterval(2.5f);

        // 결과창 표시
        seq.AppendCallback(() =>
        {
            endingWindow.gameObject.SetActive(true);
            endingWindow.Show();
        });
    }
}
