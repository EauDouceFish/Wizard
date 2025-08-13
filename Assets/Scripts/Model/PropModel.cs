using QFramework;
using System.Collections.Generic;
using UnityEngine;

public class PropModel : AbstractModel
{
    // ���ӵ�е�ף�������ֵ�
    public Dictionary<AbstractBlessingProp, int> OwningPropDict = new();
    private Dictionary<int, List<AbstractBlessingProp>> rarityPropDict = new();
    private List<AbstractBlessingProp> currentAvailableProps = new();

    Storage storage;

    protected override void OnInit()
    {
        storage = this.GetUtility<Storage>();       
        InitRarityPropDict();
    }

    // ���ݳ�ʼ��
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
    /// ���ѡ��count��ϡ�ж��¿��õĵ��ߣ���������ܵ�����������count���򷵻ؿ�������������
    /// ��������ѡ���ߵģ��ͻ᲻����
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
                        continue; // ����Ѿ�ӵ�е���������������������������
                    }
                    availableProps.Add(prop);
                }
            }
        }

        // ���ѡ�� count ������
        List<AbstractBlessingProp> selectedProps = new();
        for (int i = 0; i < count && availableProps.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, availableProps.Count);

            AbstractBlessingProp selectedProp = availableProps[randomIndex];
            selectedProps.Add(availableProps[randomIndex]);
            availableProps.RemoveAt(randomIndex); // �����ظ�ѡ��
        }

        return selectedProps;
    }


    /// <summary>
    /// ��ȡ��ǰѡ���֣���ѡ�ĵ���
    /// </summary>
    public List<AbstractBlessingProp> GetCurrentSelectedProps() => currentAvailableProps;

    public void SetCurrentAvailableProps(List<AbstractBlessingProp> props) => currentAvailableProps = props;
}