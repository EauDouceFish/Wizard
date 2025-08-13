using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpraySpellInstance : BasicSpellInstance
{
    [Header("扇形攻击参数")]
    [SerializeField] private float sprayAngle = 40f;

    private Coroutine coroutine;

    protected override void ExecuteSpell()
    {
        if (spellData.castType == CastType.Channel)
        {
            coroutine = StartCoroutine(ChannelSprayAttack());
        }
        else
        {
            ExecuteSprayAttack();
            PlaySpellEffect(transform.position);
        }
    }

    private IEnumerator ChannelSprayAttack()
    {
        PlaySpellEffect(transform.position);

        while (true)
        {
            ExecuteSprayAttack();
            yield return new WaitForSeconds(spellData.damageInterval);
        }
    }

    private void ExecuteSprayAttack()
    {
        Vector3 direction = castDirection.normalized;
        float angleRange = spellData.castRange;

        Collider[] colliders = Physics.OverlapSphere(transform.position, angleRange, enemyLayerMask);
        List<Entity> validTargets = GetValidTargets(colliders);

        // 只对扇形范围造成伤害
        foreach (var target in validTargets)
        {
            Vector3 toTarget = (target.transform.position - transform.position).normalized;

            if (IsInHorizontalAngleRange(direction, toTarget, sprayAngle) &&
                IsInVerticalRange(target.transform.position))
            {
                DealDamage(target);
            }
        }
    }

    private void OnDestroy()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
    }
}