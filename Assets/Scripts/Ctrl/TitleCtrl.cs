using DG.Tweening;
using UnityEngine;

public class TitleCtrl : MonoBehaviour
{
    [SerializeField] private ScreenSwitcher screenSwitcher;
    [SerializeField] private CanvasGroup canvasGroup;

    [SerializeField] private GameObject homeWindow;
    [SerializeField] private GameObject characterWindow;
    [SerializeField] private SettingWindow settingWindow;
    [SerializeField] private HistoryWindow historyWindow;

    private void Start()
    {
#if UNITY_EDITOR
        // 데이터 에셋 로드
        if (!DataAssetService.Instance.IsLoaded)
        {
            DataAssetService.Instance.Load();
        }
        // 오디오 세팅
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
#endif

        // 화면 전환 모션
        screenSwitcher.Show(
            preprocess: () => 
            {
                canvasGroup.alpha = 0f; 
            }, 
            postprocess: () =>
            {
                homeWindow.SetActive(true);
                characterWindow.SetActive(false);
                canvasGroup.DOFade(1f, 0.25f);
            }
        );
    }
    private void Update()
    {
        // 안드로이드 뒤로가기 버튼 처리
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (characterWindow.activeSelf || historyWindow.gameObject.activeSelf)
            {
                homeWindow.SetActive(true);
                characterWindow.SetActive(false);
                historyWindow.gameObject.SetActive(false);
            }
            else if (settingWindow.gameObject.activeSelf)
            {
                settingWindow.Close();
                homeWindow.SetActive(true);

                float volumeBGM = PlayerPrefs.GetFloat(PlayerPrefKeys.VOLUME_BGM);
                float volumeSFX = PlayerPrefs.GetFloat(PlayerPrefKeys.VOLUME_SFX);

                AudioManager.Instance.Init(volumeBGM, volumeSFX);
            }
        }
    }
    public void OnClickPlay()
    {
        AudioManager.Instance.PlaySFX(SoundKey.GameTouch);

        homeWindow.SetActive(false);
        characterWindow.SetActive(true);
    }
    public void OnClickExit()
    {
        AudioManager.Instance.PlaySFX(SoundKey.GameTouch);
        Application.Quit();
    }
    public void OnClickBack()
    {
        AudioManager.Instance.PlaySFX(SoundKey.GameTouch);

        homeWindow.SetActive(true);
        characterWindow.SetActive(false);
        historyWindow.gameObject.SetActive(false);
    }
    public void OnClickCharacter(int num)
    {
        AudioManager.Instance.PlaySFX(SoundKey.GameTouch);

        // 선택한 캐릭터 정보 저장
        StaticValues.playerCharacterNum = num;

        // 화면 전환 모션
        screenSwitcher.Hide(
            preprocess: () => 
            {
                canvasGroup.DOFade(0f, 0.25f);
            },
            postprocess: () => 
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(SceneNames.GAME);
            }
        );
    }
    public void OnClickHistory()
    {
        AudioManager.Instance.PlaySFX(SoundKey.GameTouch);

        homeWindow.SetActive(false);
        historyWindow.gameObject.SetActive(true);
    }
    public void OnClickSetting()
    {
        AudioManager.Instance.PlaySFX(SoundKey.GameTouch);

        settingWindow.Open();
        homeWindow.SetActive(false);
    }
    public void OnClickSaveSeting()
    {
        AudioManager.Instance.PlaySFX(SoundKey.GameTouch);

        settingWindow.Save();
        settingWindow.Close();

        homeWindow.SetActive(true);
    }
    public void OnClickCancelSetting()
    {
        AudioManager.Instance.PlaySFX(SoundKey.GameTouch);

        settingWindow.Cancel();
        settingWindow.Close();

        homeWindow.SetActive(true);
    }
}
