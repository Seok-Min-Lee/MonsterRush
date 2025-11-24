using UnityEngine;

public class InitCtrl : MonoBehaviour
{
    private void Start()
    {
        // PlayerPrefs 없는 경우 기본값으로 초기화
        float volumeBGM = PlayerPrefs.GetFloat("volumeBGM", 0.75f);
        float volumeSFX = PlayerPrefs.GetFloat("volumeSFX", 0.75f);
        int visibleTutorial = PlayerPrefs.GetInt("visibleTutorial", 0);
        int isLeftHand = PlayerPrefs.GetInt("isLeftHand", 1);

        PlayerPrefs.SetFloat("volumeBGM", volumeBGM);
        PlayerPrefs.SetFloat("volumeSFX", volumeSFX);
        PlayerPrefs.SetInt("visibleTutorial", visibleTutorial);
        PlayerPrefs.SetInt("isLeftHand", isLeftHand);

        UnityEngine.SceneManagement.SceneManager.LoadScene("01_Title");
    }
}
