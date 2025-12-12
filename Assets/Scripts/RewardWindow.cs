using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class RewardWindow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI headName;
    [SerializeField] private TextMeshProUGUI headDesc;

    [SerializeField] private RewardButton[] rewardButtons;

    private Dictionary<RewardInfo.Type, Dictionary<int, RewardInfo>> rewardGroup = new Dictionary<RewardInfo.Type, Dictionary<int, RewardInfo>>();

    public void Init(IEnumerable<RewardInfo> rewardInfoes)
    {
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
    public void OnClickReward(RewardInfo rewardInfo)
    {
        if (rewardInfo.type == RewardInfo.Type.Ability)
        {
            rewardGroup[RewardInfo.Type.Ability].Remove(rewardInfo.UniqueKey);
        }

        gameObject.SetActive(false);
    }
    public void Open()
    {
        //
        List<RewardInfo> samples = new List<RewardInfo>();

        if (Player.Instance.Level == 80)
        {
            headName.text = "체크포인트 보상";
            headName.color = new Color(0.125f, 0.125f, 0.125f);
            headDesc.text = "아래 보상들은 이후에 얻을 수 없습니다.";
            headDesc.color = new Color(0f, 1f, 1f);

            samples.Add(rewardGroup[RewardInfo.Type.Ability][96]);
            samples.Add(rewardGroup[RewardInfo.Type.Ability][97]);
            samples.Add(rewardGroup[RewardInfo.Type.Ability][99]);
        }
        else
        {
            headName.text = "레벨업 보상";
            headName.color = new Color(1f, 1f, 1f);
            headDesc.text = "아래 보상들 중에서 1개를 선택할 수 있습니다.";
            headDesc.color = new Color(0.125f, 0.125f, 0.125f);

            int[,] temp = new int[,]
            {
                { Player.Instance.weaponALevel, 1000, 1001, 1002, 1099 },
                { Player.Instance.weaponBLevel, 2000, 2001, 2002, 2099 },
                { Player.Instance.weaponCLevel, 3000, 3001, 3002, 3099 },
                { Player.Instance.weaponDLevel, 4000, 4001, 4002, 4099 },
            };
            int firstEndCount = 0;
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
                else
                {
                    firstEndCount++;
                }

                if (8 <= level && level < 16 && Player.Instance.IsAvailableSecondEnhance)
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

            if (firstEndCount > 2 && rewardGroup[RewardInfo.Type.Ability].ContainsKey(98))
            {
                samples.Add(rewardGroup[RewardInfo.Type.Ability][98]);
            }
        }

        List<RewardInfo> randoms = Utils.Shuffle<RewardInfo>(samples);

        for (int i = 0; i < rewardButtons.Length; i++)
        {
            rewardButtons[i].Init(randoms[i]);
        }

        gameObject.SetActive(true);
    }
}
