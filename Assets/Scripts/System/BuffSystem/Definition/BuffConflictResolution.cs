using UnityEngine;

/// <summary>
/// ������ͬ��Buff����ʱ��Ĵ���ʽ
/// </summary>
public enum BuffConflictResolution
{
    /// <summary>
    /// �ϲ�Ϊһ��buff������
    /// </summary>
    [InspectorName("�ϲ�����")]
    Combine,
    
    /// <summary>
    /// �������ڣ�Ч���������໥Ӱ��
    /// </summary>
    [InspectorName("��������")]
    Separate,

    /// <summary>
    /// ���߸���ǰ��
    /// </summary>
    [InspectorName("����ǰ��")]
    Cover,
}
