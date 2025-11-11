using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardButton : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI desc;

    private RewardInfo rewardInfo;
    public void Init(RewardInfo rewardInfo)
    {
        this.rewardInfo = rewardInfo;

        icon.sprite = rewardInfo.icon;
        desc.text = rewardInfo.desc;
    }
    public void OnClick()
    {
        AudioManager.Instance.PlaySFX(SoundKey.GameTouch);

        if (GameCtrl.Instance != null)
        {
            GameCtrl.Instance.OnClickReward();
        }

        if (Player.Instance != null)
        {
            Player.Instance.OnReward(rewardInfo);
        }
    }
}
