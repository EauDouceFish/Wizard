/// <summary>
/// ��������:���������������
/// </summary>
public enum SpellType
{
    BasicSpell,
    SpecialSpell,
}

/// <summary>
/// ���������ͷŷ�ʽ���������ָ���Ԫ����Ͻ��б仯
/// </summary>
public enum BasicSpellType
{
    Spray,      // ���� (��һ��/ˮ/��/..)
    Ball,       // ���� (����+��ʯ)
    Ray,        // ���� (����+��Ȼ)
    Vine,       // ���� (��Ȼ+��ʯ)
    Buff,       // ���� (���Լ�����)
}

/// <summary>
/// �����ͷ�����
/// </summary>
public enum CastType
{
    Instant,        // ˲������
    Channel,        // ��������ʩ��
    Chant,          // ��������
}
