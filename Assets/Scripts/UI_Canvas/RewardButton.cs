using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardButton : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI head;
    [SerializeField] private TextMeshProUGUI desc;

    private RewardWindow parent;
    private RewardInfo rewardInfo;
    public void SetRewardInfo(RewardWindow parent, RewardInfo rewardInfo)
    {
        this.parent = parent;
        this.rewardInfo = rewardInfo;

        icon.sprite = rewardInfo.icon;
        head.text = rewardInfo.head;
        desc.text = rewardInfo.desc;
    }
    public void OnClick()
    {
        AudioManager.Instance.PlaySFX(SoundKey.GameTouch);

        Player.Instance.OnReward(rewardInfo);
        ItemContainer.Instance.OnExistItemBox(rewardInfo);

        parent.OnReward(rewardInfo);
    }
}
