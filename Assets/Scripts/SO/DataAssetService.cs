using UnityEngine;

public class DataAssetService : Singleton<DataAssetService>
{
    public EnemyDataAsset EnemyDataAsset { get; private set; }
    public ItemDataAsset ItemDataAsset { get; private set; }
    public RewardDataAsset RewardDataAsset { get; private set; }

    public bool IsLoaded { get; private set; } = false;
    public void Load()
    {
        EnemyDataAsset = Resources.Load<EnemyDataAsset>("Data/EnemyDataAsset");
        ItemDataAsset = Resources.Load<ItemDataAsset>("Data/ItemDataAsset");
        RewardDataAsset = Resources.Load<RewardDataAsset>("Data/RewardDataAsset");

        IsLoaded = true;
    }
}
