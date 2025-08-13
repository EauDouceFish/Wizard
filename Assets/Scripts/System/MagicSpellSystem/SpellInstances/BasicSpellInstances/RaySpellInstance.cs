using System.Collections;
using UnityEngine;

public class RaySpellInstance : BasicSpellInstance
{
    [SerializeField] private float damageInterval = 0.125f; // 伤害触发频率

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

        // 如果射线是打到Entity身上，就Hurt对方
        if (Physics.Raycast(startPosition, direction, out RaycastHit hit, maxDistance))
        {
            Entity entity = hit.collider.GetComponent<Entity>();
            if (entity != null && entity != caster)
            {
                DealDamage(entity);
                //if (((1 << hit.collider.gameObject.layer) & enemyLayerMask) != 0)
                //{
    //                友军伤害
                //}
            }

            // 用实际命中距离绘制射线
            Debug.DrawRay(startPosition, direction * hit.distance, Color.red, 0.1f);
        }
        else
        {
            // 没有命中任何物体，绘制完整射线
            Debug.DrawRay(startPosition, direction * maxDistance, Color.green, 0.1f);
        }
    }
}