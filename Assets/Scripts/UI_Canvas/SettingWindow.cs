using UnityEngine;
using UnityEngine.UI;

public class SettingWindow : MonoBehaviour
{
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Toggle leftHandToggle;
    [SerializeField] private Toggle tutorialToggle;

    public float VolumeBGM => bgmSlider.value;
    public float VolumeSFX => sfxSlider.value;
    public bool IsLeftHand => leftHandToggle.isOn;
    public bool IsTutorialVisible => tutorialToggle.isOn;
    public void Open()
    {
        bgmSlider.value = PlayerPrefs.GetFloat(PlayerPrefKeys.VOLUME_BGM);
        sfxSlider.value = PlayerPrefs.GetFloat(PlayerPrefKeys.VOLUME_SFX);
        tutorialToggle.isOn = PlayerPrefs.GetInt(PlayerPrefKeys.VISIBLE_TUTORIAL) == 1;
        leftHandToggle.isOn = PlayerPrefs.GetInt(PlayerPrefKeys.IS_LEFT_HAND) == 1;

        gameObject.SetActive(true);
    }
    public void Close()
    {
        gameObject.SetActive(false);
    }
    public void Save()
    {
        PlayerPrefs.SetFloat(PlayerPrefKeys.VOLUME_BGM, bgmSlider.value);
        PlayerPrefs.SetFloat(PlayerPrefKeys.VOLUME_SFX, sfxSlider.value);
        PlayerPrefs.SetInt(PlayerPrefKeys.VISIBLE_TUTORIAL, tutorialToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt(PlayerPrefKeys.IS_LEFT_HAND, leftHandToggle.isOn ? 1 : 0);
    }
    public void Cancel()
    {
        float volumeBGM = PlayerPrefs.GetFloat(PlayerPrefKeys.VOLUME_BGM);
        float volumeSFX = PlayerPrefs.GetFloat(PlayerPrefKeys.VOLUME_SFX);

        AudioManager.Instance.Init(volumeBGM, volumeSFX);
    }
    public void OnValueChangedVolumeBGM()
    {
        AudioManager.Instance.SetVolumeBGM(bgmSlider.value);
    }
    public void OnValueChangedVolumeSFX()
    {
        AudioManager.Instance.SetVolumeSFX(sfxSlider.value);
    }
}
