using UnityEngine;
using UnityEngine.UI;

public class TitleCtrl : MonoBehaviour
{
    [SerializeField] private GameObject homeWindow;
    [SerializeField] private GameObject characterWindow;
    [SerializeField] private GameObject settingWindow;

    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Toggle leftHandToggle;
    [SerializeField] private Toggle tutorialToggle;

    [SerializeField] private Image[] titleParts;
    private void Start()
    {
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

        homeWindow.SetActive(true);
        characterWindow.SetActive(false);
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
    }
    public void OnClickCharacter(int num)
    {
        AudioManager.Instance.PlaySFX(SoundKey.GameTouch);

        StaticValues.playerCharacterNum = num;

        UnityEngine.SceneManagement.SceneManager.LoadScene("02_Game");
    }
    public void OnClickSetting()
    {
        AudioManager.Instance.PlaySFX(SoundKey.GameTouch);
        settingWindow.SetActive(true);
        homeWindow.SetActive(false);

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
        homeWindow.SetActive(true);

        PlayerPrefs.SetFloat("volumeBGM", bgmSlider.value);
        PlayerPrefs.SetFloat("volumeSFX", sfxSlider.value);
        PlayerPrefs.SetInt("visibleTutorial", tutorialToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt("isLeftHand", leftHandToggle.isOn ? 1 : 0);
    }
    public void OnClickCancelSetting()
    {
        AudioManager.Instance.PlaySFX(SoundKey.GameTouch);
        settingWindow.SetActive(false);
        homeWindow.SetActive(true);

        float volumeBGM = PlayerPrefs.GetFloat("volumeBGM");
        float volumeSFX = PlayerPrefs.GetFloat("volumeSFX");

        AudioManager.Instance.Init(volumeBGM, volumeSFX);
    }
}
