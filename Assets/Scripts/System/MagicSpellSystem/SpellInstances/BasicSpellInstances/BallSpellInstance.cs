using PlayerSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpellInstance : BasicSpellInstance
{
    [Header("Å×Éä²ÎÊý")]
    private float projectileSpeed = 150f;

    private bool hasExploded = false;

    protected override void Start()
    {
        base.Start();
    }

    protected override void ExecuteSpell()
    {
        PlaySpellEffect(GetCastPos());
        StartCoroutine(ProjectileMovement());
    }


    protected override void PlaySpellEffect(Vector3 position)
    {
        if (spellData.spellPrefab != null)
        {
            transform.rotation = Quaternion.LookRotation((targetPosition - transform.position).normalized);

            GameObject effect = Instantiate(spellData.spellPrefab, position, transform.rotation);

            VFXColorHelper.ApplyColorToVFX(effect, spellData.spellColor);

            if (spellData.castType != CastType.Channel) Destroy(effect, 5f);
        }
    }



    private IEnumerator ProjectileMovement()
    {
        Vector3 direction = (targetPosition - transform.position).normalized;

        while (!hasExploded)
        {
            float moveDistance = projectileSpeed * Time.deltaTime;
            transform.position += direction * moveDistance;

            RaycastHit hit;
            if (Physics.Raycast(transform.position, direction, out hit, moveDistance, ~0, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.gameObject != this.gameObject)
                {
                    ExplodeAtPosition(hit.point);
                    yield break;
                }
                else
                {
                    Debug.Log(hit.collider.gameObject.name);
                }
            }

            yield return null;
        }

        Destroy(gameObject, 5f);
    }

    private void ExplodeAtPosition(Vector3 position)
    {
        if (hasExploded) return;
        hasExploded = true;

        Collider[] colliders = Physics.OverlapSphere(position, spellData.damageRadius, enemyLayerMask);

        List<Entity> targets = GetValidTargets(colliders);

        foreach (var target in targets)
        {
            Debug.Log(target.name);
            DealDamage(target);
        }

        Destroy(gameObject, 5f);
    }

    private void OnDrawGizmos()
    {
        if (hasExploded && spellData != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, spellData.damageRadius);
        }
    }

}