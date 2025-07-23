/// <summary>
/// 法术类型:基础咒语、特殊咒语
/// </summary>
public enum SpellType
{
    BasicSpell,
    SpecialSpell,
}

/// <summary>
/// 基础技能释放方式，后续表现根据元素组合进行变化
/// </summary>
public enum BasicSpellType
{
    Spray,      // 喷雾 (单一火/水/冰/..)
    Ball,       // 球体 (任意+岩石)
    Ray,        // 射线 (任意+自然)
    Vine,       // 藤蔓 (自然+岩石)
    Buff,       // 增益 (给自己增益)
}

/// <summary>
/// 法术释放类型
/// </summary>
public enum CastType
{
    Instant,        // 瞬发法术
    Channel,        // 蓄力持续施法
    Chant,          // 吟唱法术
}
