using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameCtrl : MonoBehaviour
{
    public static GameCtrl Instance { get; private set; }

    [SerializeField] private ScreenSwitcher screenSwitcher;
    [SerializeField] private CanvasGroup canvasGroup;

    [SerializeField] private TextMeshProUGUI timeText;

    [SerializeField] private NormalWindow normalWindow;
    [SerializeField] private GameObject pauseWindow;
    [SerializeField] private RewardWindow rewardWindow;
    [SerializeField] private EndingWindow endingWindow;
    [SerializeField] private SettingWindow settingWindow;
    [SerializeField] private TutorialWindow tutorialWindow;

    [SerializeField] private DataContainer dataContainer;

    private int lastLevel = 1;
    private int levelUpCount = 0;
    private float timer = 0f;
    private void Awake()
    {
        Instance = this;

        rewardWindow.Init(dataContainer.rewards);
    }
    private void Start()
    {
        timeText.text = "00:00";

        if (!AudioManager.Instance.isLoadComplete)
        {
            AudioManager.Instance.Load(() =>
            {
                float volumeBGM = PlayerPrefs.GetFloat(PlayerPrefKeys.VOLUME_BGM);
                float volumeSFX = PlayerPrefs.GetFloat(PlayerPrefKeys.VOLUME_SFX);

                AudioManager.Instance.Init(volumeBGM, volumeSFX);
                AudioManager.Instance.PlayBGM(SoundKey.BGM);
            });
        }
#if UNITY_EDITOR
        Application.targetFrameRate = 30;
        QualitySettings.vSyncCount = 0;
#endif

        screenSwitcher.Show(
            preprocess: () => 
            {
                canvasGroup.alpha = 0f;

                // 튜토리얼 UI 표시
                bool isVisibleTutorial = PlayerPrefs.GetInt(PlayerPrefKeys.VISIBLE_TUTORIAL) == 1;
                StaticValues.isWait = isVisibleTutorial;
                tutorialWindow.Init(isVisibleTutorial);

                if (!StaticValues.isWait)
                {
                    OnGameStart();
                }

                // 좌/우손잡이 설정
                bool isLeftHand = PlayerPrefs.GetInt(PlayerPrefKeys.IS_LEFT_HAND) == 1;
                Player.Instance.OnChangeUI(isLeftHand);
                tutorialWindow.OnChangeUI(isLeftHand);
            }, 
            postprocess: () => 
            {
                canvasGroup.DOFade(1f, 0.25f);
            }
        );
    }
    private void Update()
    {
        if (Time.timeScale == 0 || StaticValues.isWait)
        {
            return;
        }

        //
        if (levelUpCount > 0)
        {
            LevelUpProcess();
        }

        //
        timer += Time.deltaTime;
        timeText.text = Utils.FormatTimeTommss(timer);
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Player.Instance.OnClickLevelUp();
        }
#endif
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

        // 게임 결과 기록 저장
        int[] playerStats = new int[4] { Player.Instance.Heal, Player.Instance.Magnet, Player.Instance.Speed, Player.Instance.Strength };
        int[] weaponLevels = new int[4] { Player.Instance.WeaponALevel, Player.Instance.WeaponBLevel, Player.Instance.WeaponCLevel, Player.Instance.WeaponDLevel };

        GameResultLog newLog = new GameResultLog(
            characterNum: StaticValues.playerCharacterNum,
            level: Player.Instance.Level,
            killCount: Player.Instance.killCount,
            playerStats: playerStats,
            weaponLevels: weaponLevels,
            playerAbilities: Player.Instance.PlayerAbilities.ToArray(),
            weaponAbilities: Player.Instance.WeaponAbilities.ToArray(),
            playTime: timer,
            dateTime: System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        );

        if (!GameDataContainer.Instance.IsLoaded)
        {
            GameDataContainer.Instance.LoadGameResultLogs();
        }
        GameDataContainer.Instance.TryAddGameResultLog(newLog);
        GameDataContainer.Instance.TrySaveGameResultLogs();

        // 씬 전환
        screenSwitcher.Hide(
            preprocess: () =>
            {
                StaticValues.isWait = true;
                canvasGroup.DOFade(0f, 0.25f);
            },
            postprocess: () => 
            { 
                UnityEngine.SceneManagement.SceneManager.LoadScene("01_Title"); 
            }
        );
    }
    public void OnClickSetting()
    {
        AudioManager.Instance.PlaySFX(SoundKey.GameTouch);
        settingWindow.Open();
        pauseWindow.SetActive(false);
    }
    public void OnClickSaveSeting()
    {
        AudioManager.Instance.PlaySFX(SoundKey.GameTouch);

        // 설정 저장
        settingWindow.Save();
        settingWindow.Close();

        // 튜토리얼 UI 표시 여부 및 좌/우손잡이 설정 적용
        tutorialWindow.OnChangeTutorial(settingWindow.IsTutorialVisible);
        Player.Instance.OnChangeUI(settingWindow.IsLeftHand);
        tutorialWindow.OnChangeUI(settingWindow.IsLeftHand);

        pauseWindow.SetActive(true);
    }
    public void OnClickCancelSetting()
    {
        AudioManager.Instance.PlaySFX(SoundKey.GameTouch);

        // 설정 취소
        settingWindow.Cancel();
        settingWindow.Close();

        pauseWindow.SetActive(true);
    }
    public void OnGameStart()
    {
        StaticValues.isWait = false;
        tutorialWindow.OnGameStart();
        normalWindow.Alert($"목표: Lv {StaticValues.CHECKPOINT_LEVEL} 도달!");
    }
    public void OnGameChallenge()
    {
        StaticValues.isWait = false;
        GameCtrl.Instance.OnLevelUp();

        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.25f);
        seq.AppendCallback(() => normalWindow.Show());
        seq.AppendInterval(1f);
        seq.AppendCallback(() => normalWindow.Alert("목표: 오래 살아남기!\n<color=#FF8000>주의! 강한 적 등장..!</color>"));
    }
    public void OnLevelUp()
    {
        levelUpCount++;
    }
    public void OnReward(RewardInfo rewardInfo)
    {
        Time.timeScale = 1f;
        tutorialWindow.OnReward(rewardInfo);
    }
    public void OnGameEnd(GameResult result)
    {
        switch (result)
        {
            // 게임 클리어
            case GameResult.Clear:
                OnGameEndClear();
                break;
            // 게임 패배
            case GameResult.Defeat:
                OnGameEndDefeat();
                break;
            // 챌린지 모드 종료
            case GameResult.Challenge:
                OnGameEndChallenge();
                break;
        }
    }
    private void OnGameEndClear()
    {
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

        // 엔딩 연출
        seq.AppendCallback(() =>
        {
            normalWindow.Hide();
            endingWindow.Init(GameResult.Clear, timer);
        });

        // 남아있는 몬스터 제거
        List<Enemy> enemies = EnemyContainer.Instance.GetActiveEnemyAll();
        foreach (Enemy enemy in enemies.OrderBy(e => e.toPlayer.sqrMagnitude))
        {
            enemy.OnShutdown();

            seq.AppendCallback(() => enemy.OnDeath());
            seq.AppendInterval(Time.deltaTime);
        }

        // 남아있는 아이템 박스 오픈
        List<ItemBox> boxes = ItemContainer.Instance.GetItemBoxAll();
        for (int i = 0; i < boxes.Count; i++)
        {
            seq.AppendCallback(() => boxes[i].OnOpen());
            seq.AppendInterval(Time.deltaTime);
        }

        // 남아있는 아이템 흡수
        List<Item> items = ItemContainer.Instance.GetUndetectedsAll();
        Vector3 playerPosition = Player.Instance.transform.position;

        items.Sort((a, b) =>
        {
            float da = (a.transform.position - playerPosition).sqrMagnitude;
            float db = (b.transform.position - playerPosition).sqrMagnitude;

            return da.CompareTo(db);
        });

        foreach (Item item in items.OrderBy(i => (i.transform.position - playerPosition).sqrMagnitude))
        {
            Vector3 toItem = item.transform.position - playerPosition;

            if (toItem.sqrMagnitude > 36)
            {
                item.transform.position = playerPosition + toItem.normalized * 6;
            }

            seq.AppendCallback(() => item.OnDetected());
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
    private void LevelUpProcess()
    {
        Time.timeScale = 0f;

        AudioManager.Instance.PlaySFX(SoundKey.PlayerLevelUp);

        int targetLevel = lastLevel + 1;
        EnemyContainer.Instance.UpdateByLevel(targetLevel);
        rewardWindow.OpenByLevel(targetLevel);

        lastLevel = targetLevel;
        levelUpCount--;
    }
}
