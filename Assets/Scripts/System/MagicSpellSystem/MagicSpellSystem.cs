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


    // 当前玩家念的咒语
    List<MagicElement> currentElements = new();
    // 咒语颜色映射
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

    #region 外部方法

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
    /// 根据咒语,获取要释放的法术实例
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
            //Debug.Log($"咒语中包含如下元素：{string.Join(", ", elements)}");
            BasicSpellInstance basicSpellInstance = CreateBasicSpellInstanceFromData(basicSpellData);

            return basicSpellInstance;
        }
    }

    #endregion

    #region 内部实现

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
    /// 根据元素组合确定法术类型
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
        else // 2+ 种元素
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
    /// 获取基础的法术数据模板
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
    /// 通过传入的模板和元素列表，修改模板默认的值
    /// </summary>
    private BasicSpellBaseData ModifySpellWithElements(BasicSpellBaseData template, List<MagicElement> elements)
    {
        BasicSpellBaseData modifiedSpell = ScriptableObject.Instantiate(template);
        // 修改特效颜色
        ModifySpellColor(modifiedSpell, elements);

        // 计算伤害
        float damageMultiplier = CalculateElementDamageNumMultiplier(elements);
        float originalDamage = modifiedSpell.baseDamage;
        modifiedSpell.baseDamage *= damageMultiplier;
        Debug.Log($"总共有:{elements.Count}个元素，法术基础伤害变为 {originalDamage} x {damageMultiplier} = {modifiedSpell.baseDamage} ");

        // 给对方上元素状态
        return modifiedSpell;
    }

    // 染色逻辑如下
    // 1.先除去Nature、Rock，检测是否有反应
    // 有反应：调色
    // 火+水
    // 冰+水
       
    // 2.无反应：那么所有元素检测为孤立元素，直接修改粒子特效颜色。每种元素单独给对方上Buff，试图反应
    // 粒子特效：有冰火水，就按照冰/火/水颜色。如果没有冰火水，就按照Nature、岩石颜色，不然就没颜色
    private void ModifySpellColor(BasicSpellBaseData template, List<MagicElement> elements)
    {
        // 1. 去除Nature和Rock
        List<MagicElement> mainElements = elements.Where(e => e == MagicElement.Fire || e == MagicElement.Water || e == MagicElement.Ice).ToList();

        // 2. 检查元素是否反应
        bool hasFire = mainElements.Contains(MagicElement.Fire);
        bool hasWater = mainElements.Contains(MagicElement.Water);
        bool hasIce = mainElements.Contains(MagicElement.Ice);

        if (hasFire && hasWater) // 火+水
        {
            template.spellColor = MixColors(GetElementColor(MagicElement.Fire, colorMap), GetElementColor(MagicElement.Water, colorMap));
        }
        else if (hasIce && hasWater) // 冰+水
        {
            template.spellColor = MixColors(GetElementColor(MagicElement.Ice, colorMap), GetElementColor(MagicElement.Water, colorMap));
        }
        else if (mainElements.Count > 0) // 有冰/火/水其中之一，按照元素颜色染色
        {
            // 按单元素染色
            template.spellColor = GetElementColor(mainElements[0], colorMap);
            // 给目标上对应Buff
        }
        else // 没有冰火水，检查Nature和Rock去染色
        {
            if (elements.Contains(MagicElement.Nature))
                template.spellColor = GetElementColor(MagicElement.Nature, colorMap);
            else if (elements.Contains(MagicElement.Rock))
                template.spellColor = GetElementColor(MagicElement.Rock, colorMap);
            else
                template.spellColor = Color.black; // 无色
        }
    }

    /// <summary>
    /// 计算伤害倍率，元素越多倍率越高
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
        return Color.black; // 此颜色是错误
    }

    private static Color MixColors(Color c1, Color c2)
    {
        return new Color((c1.r + c2.r) / 2f, (c1.g + c2.g) / 2f, (c1.b + c2.b) / 2f, 1f);
    }
    #endregion
}