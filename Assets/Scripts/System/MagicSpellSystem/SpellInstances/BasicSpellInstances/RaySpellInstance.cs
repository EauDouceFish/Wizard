using System.Collections;
using UnityEngine;

public class RaySpellInstance : BasicSpellInstance
{
    [SerializeField] private float damageInterval = 0.125f; // �˺�����Ƶ��

    private Coroutine channelCoroutine;

    protected override void ExecuteSpell()
    {
        if (spellData.castType == CastType.Channel)
        {
            channelCoroutine = StartCoroutine(ChannelRayAttack());
        }
        else
        {
            ExecuteRayAttack();
            PlaySpellEffect(transform.position);
        }
    }

    private IEnumerator ChannelRayAttack()
    {
        PlaySpellEffect(transform.position);

        while (true)
        {
            ExecuteRayAttack();
            yield return new WaitForSeconds(damageInterval);
        }
    }


    private void OnDestroy()
    {
        if (channelCoroutine != null)
        {
            StopCoroutine(channelCoroutine);
        }
    }

    private void ExecuteRayAttack()
    {
        Vector3 startPosition = transform.position;
        Vector3 direction = castDirection.normalized;
        float maxDistance = spellData.castRange;

        // ��������Ǵ�Entity���ϣ���Hurt�Է�
        if (Physics.Raycast(startPosition, direction, out RaycastHit hit, maxDistance))
        {
            Entity entity = hit.collider.GetComponent<Entity>();
            if (entity != null && entity != caster)
            {
                DealDamage(entity);
                //if (((1 << hit.collider.gameObject.layer) & enemyLayerMask) != 0)
                //{
    //                �Ѿ��˺�
                //}
            }

            // ��ʵ�����о����������
            Debug.DrawRay(startPosition, direction * hit.distance, Color.red, 0.1f);
        }
        else
        {
            // û�������κ����壬������������
            Debug.DrawRay(startPosition, direction * maxDistance, Color.green, 0.1f);
        }
    }
}