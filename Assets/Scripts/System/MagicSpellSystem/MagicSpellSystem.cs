using QFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MagicSpellSystem : AbstractSystem
{
    const int MAX_ELEMENT_COUNT = 5;
    private Storage storage;
    private MagicSpellGuide magicSpellGuide;
    private MagicSpellModel magicSpellModel;
    private MagicInputModel magicInputModel;
    Dictionary<List<MagicElement>, SpecialSpellData> specialSpellDict;


    // ��ǰ����������
    List<MagicElement> currentElements = new();
    // ������ɫӳ��
    List<ColorMap> colorMap;

    protected override void OnInit()
    {
        storage = this.GetUtility<Storage>();
        magicSpellGuide = storage.GetMagicSpellGuide();
        specialSpellDict = magicSpellGuide.specialSpellGuideDict;
        colorMap = storage.GetColorMaps();
        magicSpellModel = this.GetModel<MagicSpellModel>();
        magicInputModel = this.GetModel<MagicInputModel>();
    }

    #region �ⲿ����

    public void AddElement(MagicElement element)
    {
        if (currentElements.Count >= MAX_ELEMENT_COUNT)
        {
            return;
        }
        currentElements.Add(element);
        currentElements = ProcessElementCombination(currentElements);
        magicInputModel.SetInputElements(currentElements);
    }

    public void ClearElements()
    {
        currentElements.Clear();
    }

    public List<MagicElement> GetCurrentElements()
    {
        return new List<MagicElement>(currentElements);
    }

    /// <summary>
    /// ��������,��ȡҪ�ͷŵķ���ʵ��
    /// </summary>
    public SpellInstanceBase GetSpellToCast(List<MagicElement> elements)
    {
        if (elements == null || elements.Count == 0)
        {
            return null;
        }

        if (specialSpellDict != null && specialSpellDict.ContainsKey(elements))
        {
            return null;
        }
        else
        {
            BasicSpellBaseData basicSpellData = GetBasicSpellDataByElements(elements);
            //Debug.Log($"�����а�������Ԫ�أ�{string.Join(", ", elements)}");
            BasicSpellInstance basicSpellInstance = CreateBasicSpellInstanceFromData(basicSpellData);

            return basicSpellInstance;
        }
    }

    #endregion

    #region �ڲ�ʵ��

    private List<MagicElement> ProcessElementCombination(List<MagicElement> inputElements)
    {
        List<MagicElement> result = new(inputElements);

        if (result.Contains(MagicElement.Fire) && result.Contains(MagicElement.Ice))
        {
            result.Remove(MagicElement.Fire);
            result.Remove(MagicElement.Ice);
            result.Add(MagicElement.Water);
        }
        return result;
    }
    /// <summary>
    /// ����Ԫ�����ȷ����������
    /// </summary>
    public BasicSpellType DetermineSpellType(List<MagicElement> elements)
    {
        bool hasRock = elements.Contains(MagicElement.Rock);
        bool hasNature = elements.Contains(MagicElement.Nature);
        int elementCount = elements.Count;

        if (elementCount == 1)
        {
            if (hasRock) return BasicSpellType.Ball;
            if (hasNature) return BasicSpellType.Ray;
            return BasicSpellType.Spray;
        }
        else // 2+ ��Ԫ��
        {
            if (hasNature && hasRock) return BasicSpellType.Vine;
            if (hasRock) return BasicSpellType.Ball;
            if (hasNature) return BasicSpellType.Ray;
            return BasicSpellType.Spray;
        }
    }

    private BasicSpellBaseData GetBasicSpellDataByElements(List<MagicElement> elements)
    {
        BasicSpellType spellType = DetermineSpellType(elements);

        BasicSpellBaseData template = GetBasicSpellDataTemplate(spellType);

        BasicSpellBaseData modifiedSpell = ModifySpellWithElements(template, elements);

        return modifiedSpell;
    }

    /// <summary>
    /// ��ȡ�����ķ�������ģ��
    /// </summary>
    public BasicSpellBaseData GetBasicSpellDataTemplate(BasicSpellType spellType)
    {
        return spellType switch
        {
            BasicSpellType.Spray => magicSpellGuide.sprayTemplate,
            BasicSpellType.Ball => magicSpellGuide.ballTemplate,
            BasicSpellType.Ray => magicSpellGuide.rayTemplate,
            BasicSpellType.Vine => magicSpellGuide.vineTemplate,
            BasicSpellType.Buff => magicSpellGuide.buffTemplate,
            _ => magicSpellGuide.sprayTemplate
        };
    }

    /// <summary>
    /// ͨ�������ģ���Ԫ���б��޸�ģ��Ĭ�ϵ�ֵ
    /// </summary>
    private BasicSpellBaseData ModifySpellWithElements(BasicSpellBaseData template, List<MagicElement> elements)
    {
        BasicSpellBaseData modifiedSpell = ScriptableObject.Instantiate(template);
        // �޸���Ч��ɫ
        ModifySpellColor(modifiedSpell, elements);

        // �����˺�
        float damageMultiplier = CalculateElementDamageNumMultiplier(elements);
        float originalDamage = modifiedSpell.baseDamage;
        modifiedSpell.baseDamage *= damageMultiplier;
        Debug.Log($"�ܹ���:{elements.Count}��Ԫ�أ����������˺���Ϊ {originalDamage} x {damageMultiplier} = {modifiedSpell.baseDamage} ");

        // ���Է���Ԫ��״̬
        return modifiedSpell;
    }

    // Ⱦɫ�߼�����
    // 1.�ȳ�ȥNature��Rock������Ƿ��з�Ӧ
    // �з�Ӧ����ɫ
    // ��+ˮ
    // ��+ˮ
       
    // 2.�޷�Ӧ����ô����Ԫ�ؼ��Ϊ����Ԫ�أ�ֱ���޸�������Ч��ɫ��ÿ��Ԫ�ص������Է���Buff����ͼ��Ӧ
    // ������Ч���б���ˮ���Ͱ��ձ�/��/ˮ��ɫ�����û�б���ˮ���Ͱ���Nature����ʯ��ɫ����Ȼ��û��ɫ
    private void ModifySpellColor(BasicSpellBaseData template, List<MagicElement> elements)
    {
        // 1. ȥ��Nature��Rock
        List<MagicElement> mainElements = elements.Where(e => e == MagicElement.Fire || e == MagicElement.Water || e == MagicElement.Ice).ToList();

        // 2. ���Ԫ���Ƿ�Ӧ
        bool hasFire = mainElements.Contains(MagicElement.Fire);
        bool hasWater = mainElements.Contains(MagicElement.Water);
        bool hasIce = mainElements.Contains(MagicElement.Ice);

        if (hasFire && hasWater) // ��+ˮ
        {
            template.spellColor = MixColors(GetElementColor(MagicElement.Fire, colorMap), GetElementColor(MagicElement.Water, colorMap));
        }
        else if (hasIce && hasWater) // ��+ˮ
        {
            template.spellColor = MixColors(GetElementColor(MagicElement.Ice, colorMap), GetElementColor(MagicElement.Water, colorMap));
        }
        else if (mainElements.Count > 0) // �б�/��/ˮ����֮һ������Ԫ����ɫȾɫ
        {
            // ����Ԫ��Ⱦɫ
            template.spellColor = GetElementColor(mainElements[0], colorMap);
            // ��Ŀ���϶�ӦBuff
        }
        else // û�б���ˮ�����Nature��RockȥȾɫ
        {
            if (elements.Contains(MagicElement.Nature))
                template.spellColor = GetElementColor(MagicElement.Nature, colorMap);
            else if (elements.Contains(MagicElement.Rock))
                template.spellColor = GetElementColor(MagicElement.Rock, colorMap);
            else
                template.spellColor = Color.black; // ��ɫ
        }
    }

    /// <summary>
    /// �����˺����ʣ�Ԫ��Խ�౶��Խ��
    /// </summary>
    private float CalculateElementDamageNumMultiplier(List<MagicElement> elements)
    {
        float multiplier = 1f;

        foreach (var element in elements)
        {
            switch (element)
            {
                case MagicElement.Fire:
                    multiplier += magicSpellModel.FireElementMultiplier;
                    break;
                case MagicElement.Water:
                    multiplier += magicSpellModel.WaterElementMultiplier;
                    break;
                case MagicElement.Ice:
                    multiplier += magicSpellModel.IceElementMultiplier;
                    break;
                case MagicElement.Nature:
                    multiplier += magicSpellModel.NatureElementMultiplier;
                    break;
                case MagicElement.Rock:
                    multiplier += magicSpellModel.RockElementMultiplier;
                    break;
            }
        }

        return multiplier;
    }

    private BasicSpellInstance CreateBasicSpellInstanceFromData(BasicSpellBaseData basicSpellData)
    {
        GameObject spellObject = new($"BasicSpell_{basicSpellData.basicSpellType}");

        BasicSpellInstance spellInstance = basicSpellData.basicSpellType switch
        {
            BasicSpellType.Spray => spellObject.AddComponent<SpraySpellInstance>(),
            BasicSpellType.Ball => spellObject.AddComponent<BallSpellInstance>(),
            BasicSpellType.Ray => spellObject.AddComponent<RaySpellInstance>(),
            BasicSpellType.Vine => spellObject.AddComponent<VineSpellInstance>(),
            BasicSpellType.Buff => spellObject.AddComponent<BuffSpellInstance>(),
            _ => spellObject.AddComponent<SpraySpellInstance>()
        };

        spellInstance.SetSpellData(basicSpellData);

        return spellInstance;
    }

    public BasicSpellInstance CreateBasicSpellInstanceByEnum(BasicSpellType basicSpellType)
    {
        BasicSpellBaseData template = GetBasicSpellDataTemplate(basicSpellType);

        GameObject spellObject = new($"BasicSpell_{basicSpellType}");

        BasicSpellInstance spellInstance = basicSpellType switch
        {
            BasicSpellType.Spray => spellObject.AddComponent<SpraySpellInstance>(),
            BasicSpellType.Ball => spellObject.AddComponent<BallSpellInstance>(),
            BasicSpellType.Ray => spellObject.AddComponent<RaySpellInstance>(),
            BasicSpellType.Vine => spellObject.AddComponent<VineSpellInstance>(),
            BasicSpellType.Buff => spellObject.AddComponent<BuffSpellInstance>(),
            _ => spellObject.AddComponent<SpraySpellInstance>()
        };

        spellInstance.SetSpellData(template);

        return spellInstance;
    }

    public static Color GetElementColor(MagicElement element, List<ColorMap> colorMap)
    {
        foreach (var map in colorMap)
        {
            if (map.element == element)
            {
                return map.color;
            }
        }
        return Color.black; // ����ɫ�Ǵ���
    }

    private static Color MixColors(Color c1, Color c2)
    {
        return new Color((c1.r + c2.r) / 2f, (c1.g + c2.g) / 2f, (c1.b + c2.b) / 2f, 1f);
    }
    #endregion
}