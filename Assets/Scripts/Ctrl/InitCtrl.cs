using UnityEngine;

public class InitCtrl : MonoBehaviour
{
    private void Start()
    {
        // PlayerPrefs 없는 경우 기본값으로 초기화
        float volumeBGM = PlayerPrefs.GetFloat(PlayerPrefKeys.VOLUME_BGM, 0.5f);
        float volumeSFX = PlayerPrefs.GetFloat(PlayerPrefKeys.VOLUME_SFX, 0.5f);
        int visibleTutorial = PlayerPrefs.GetInt(PlayerPrefKeys.VISIBLE_TUTORIAL, 1);
        int isLeftHand = PlayerPrefs.GetInt(PlayerPrefKeys.IS_LEFT_HAND, 1);

        PlayerPrefs.SetFloat(PlayerPrefKeys.VOLUME_BGM, volumeBGM);
        PlayerPrefs.SetFloat(PlayerPrefKeys.VOLUME_SFX, volumeSFX);
        PlayerPrefs.SetInt(PlayerPrefKeys.VISIBLE_TUTORIAL, visibleTutorial);
        PlayerPrefs.SetInt(PlayerPrefKeys.IS_LEFT_HAND, isLeftHand);

        // 게임 기록 불러오기
        GameHistoryService.Instance.Load();
        DataAssetService.Instance.Load();

        UnityEngine.SceneManagement.SceneManager.LoadScene(SceneNames.TITLE);
    }
}
