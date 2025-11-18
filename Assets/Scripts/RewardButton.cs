using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardButton : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI head;
    [SerializeField] private TextMeshProUGUI desc;

    private RewardInfo rewardInfo;
    public void Init(RewardInfo rewardInfo)
    {
        this.rewardInfo = rewardInfo;

        icon.sprite = rewardInfo.icon;
        head.text = $"{rewardInfo.head}";
        desc.text = rewardInfo.desc;

        head.color = rewardInfo.isSpecial ? Color.yellow : Color.white;
    }
    public void OnClick()
    {
        AudioManager.Instance.PlaySFX(SoundKey.GameTouch);
        GameCtrl.Instance.OnClickReward(rewardInfo);
        Player.Instance.OnReward(rewardInfo);
    }
}
