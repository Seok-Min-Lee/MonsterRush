using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class RewardWindow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI headName;
    [SerializeField] private TextMeshProUGUI headDesc;

    [SerializeField] private RewardButton[] rewardButtons;

    private Dictionary<int, RewardInfo> weaponRewards = new Dictionary<int, RewardInfo>();
    private Dictionary<int, RewardInfo> playerRewards = new Dictionary<int, RewardInfo>();
    private Dictionary<int, RewardInfo> abilityRewards = new Dictionary<int, RewardInfo>();
    
#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            rewardButtons[0].OnClick();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            rewardButtons[1].OnClick();
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            rewardButtons[2].OnClick();
        }
    }
#endif
public void Init(IEnumerable<RewardInfo> rewardInfoes)
    {
        foreach (RewardInfo rewardInfo in rewardInfoes)
        {
            if (rewardInfo.type == RewardInfo.Type.Weapon)
            {
                weaponRewards.Add(rewardInfo.UniqueKey, rewardInfo);
            }
            else if (rewardInfo.type == RewardInfo.Type.Player)
            {
                playerRewards.Add(rewardInfo.UniqueKey, rewardInfo);
            }
            else if (rewardInfo.type == RewardInfo.Type.Ability)
            {
                abilityRewards.Add(rewardInfo.UniqueKey, rewardInfo);
            }
        }
    }
    public void OnClickReward(RewardInfo rewardInfo)
    {
        if (abilityRewards.ContainsKey(rewardInfo.UniqueKey))
        {
            abilityRewards.Remove(rewardInfo.UniqueKey);
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
            headName.color = new Color(1f, 0.5f, 0f);
            headDesc.text = "아래 보상들은 이후에 얻을 수 없습니다!";
            headDesc.color = new Color(0.9f, 0.45f, 0f);

            samples.Add(abilityRewards[96]);
            samples.Add(abilityRewards[97]);
            samples.Add(abilityRewards[99]);
        }
        else
        {
            headName.text = "레벨업 보상";
            headName.color = new Color(0f, 0.5f, 0f);
            headDesc.text = "아래 보상들 중에서 1개를 선택할 수 있습니다.";
            headDesc.color = new Color(0f, 0.45f, 0f);

            int[,] levelAndGroupIds = new int[4, 2]
            {
                { Player.Instance.WeaponALevel, 1000 },
                { Player.Instance.WeaponBLevel, 2000 },
                { Player.Instance.WeaponCLevel, 3000 },
                { Player.Instance.WeaponDLevel, 4000 },
            };

            int firstEndCount = 0;
            int levelTotal = 0;
            int rows = levelAndGroupIds.GetLength(0);

            for (int i = 0; i < rows; i++)
            {
                int level = levelAndGroupIds[i, 0];
                int groupId = levelAndGroupIds[i, 1];
                levelTotal += level;

                if (level < 1)
                {
                    // 무기 획득
                    samples.Add(weaponRewards[groupId + 0]);
                }
                else if (level < 8)
                {
                    // 1차 강화
                    samples.Add(weaponRewards[groupId + 1]);
                }
                else
                {
                    firstEndCount++;
                }

                // 2차 강화
                if (8 <= level && level < 16 && Player.Instance.IsAvailableSecondEnhance)
                {
                    samples.Add(weaponRewards[groupId + 2]);
                }

                // 특수 능력
                if (level > 0 && abilityRewards.ContainsKey(groupId + 99) && Random.Range(0, 10) == 0)
                {
                    samples.Add(abilityRewards[groupId + 99]);
                }
            }

            // 플레이어 능력
            samples.Add(playerRewards[0]);
            samples.Add(playerRewards[1]);
            samples.Add(playerRewards[2]);
            samples.Add(playerRewards[3]);

            // 2차 강화 개방
            if (firstEndCount > 0 && levelTotal > 20 && abilityRewards.ContainsKey(98))
            {
                samples.Add(abilityRewards[98]);
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
