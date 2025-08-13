using QFramework;
using System.Collections.Generic;
using UnityEngine;

public class PropModel : AbstractModel
{
    // 玩家拥有的祝福道具字典
    public Dictionary<AbstractBlessingProp, int> OwningPropDict = new();
    private Dictionary<int, List<AbstractBlessingProp>> rarityPropDict = new();
    private List<AbstractBlessingProp> currentAvailableProps = new();

    Storage storage;

    protected override void OnInit()
    {
        storage = this.GetUtility<Storage>();       
        InitRarityPropDict();
    }

    // 数据初始化
    private void InitRarityPropDict()
    {
        GameObject[] allBlessingPropPrefabs = storage.GetAllBlessingProps();

        foreach (GameObject propPrefab in allBlessingPropPrefabs)
        {
            AbstractBlessingProp blessingProp = propPrefab.GetComponent<AbstractBlessingProp>();
            if (blessingProp != null)
            {
                int rarity = blessingProp.GetPropRarity();
                if (!rarityPropDict.ContainsKey(rarity))
                {
                    rarityPropDict[rarity] = new List<AbstractBlessingProp>();
                }
                rarityPropDict[rarity].Add(blessingProp);
            }
        }
    }


    /// <summary>
    /// 随机选择count个稀有度下可用的道具，如果可用总道具数量不足count，则返回可用总数个道具
    /// 超过最大可选道具的，就会不可用
    /// </summary>

    public List<AbstractBlessingProp> GetRandomBlessingPropsFiltered(int rarity, int count)
    {
        List<AbstractBlessingProp> availableProps = new();

        for (int i = 1; i <= rarity; i++)
        {
            if (rarityPropDict.ContainsKey(i) && rarityPropDict[i] != null)
            {
                foreach (AbstractBlessingProp prop in rarityPropDict[i])
                {
                    if (OwningPropDict.ContainsKey(prop) && OwningPropDict[prop] >= prop.PropData.maxAvailableCount)
                    {
                        continue; // 如果已经拥有的数量超过最大可用数量，则跳过
                    }
                    availableProps.Add(prop);
                }
            }
        }

        // 随机选择 count 个道具
        List<AbstractBlessingProp> selectedProps = new();
        for (int i = 0; i < count && availableProps.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, availableProps.Count);

            AbstractBlessingProp selectedProp = availableProps[randomIndex];
            selectedProps.Add(availableProps[randomIndex]);
            availableProps.RemoveAt(randomIndex); // 避免重复选择
        }

        return selectedProps;
    }


    /// <summary>
    /// 获取当前选择种，可选的道具
    /// </summary>
    public List<AbstractBlessingProp> GetCurrentSelectedProps() => currentAvailableProps;

    public void SetCurrentAvailableProps(List<AbstractBlessingProp> props) => currentAvailableProps = props;
}