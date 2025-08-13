using PlayerSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineSpellInstance : BasicSpellInstance
{
    protected override void ExecuteSpell()
    {
        Vector3 startPos = caster.transform.position + Vector3.up;

        PlaySpellEffect(startPos);
        ExecuteVineAttack();

        Destroy(gameObject, 3f);
    }

    private void ExecuteVineAttack()
    {
        Vector3 rectangleCenter = caster.transform.position + castDirection * spellData.castRange * 0.5f;
        Vector3 boxSize = new Vector3(spellData.spellWidth, spellData.spellHeight, spellData.castRange);
        Quaternion boxRotation = Quaternion.LookRotation(castDirection, Vector3.up);

        // 可视化,生成一个BoxCollider（Trigger）
        GameObject boxVis = new GameObject("VineSpell_OverlapBox_Vis");
        boxVis.transform.SetParent(transform);
        boxVis.transform.position = rectangleCenter;
        boxVis.transform.rotation = boxRotation;
        BoxCollider boxCollider = boxVis.AddComponent<BoxCollider>();
        boxCollider.size = boxSize;
        boxCollider.isTrigger = true;

        Destroy(boxVis, 2f);

        Collider[] colliders = Physics.OverlapBox(rectangleCenter, boxSize * 0.5f, boxRotation, enemyLayerMask);
        List<Entity> targets = GetValidTargets(colliders);

        foreach (var target in targets)
        {
            //Debug.Log($"{target.name}");
            DealDamage(target);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (caster == null || spellData == null) return;

        Vector3 start = caster.transform.position + Vector3.up;
        Vector3 dir = castDirection.normalized;
        float length = spellData.castRange;
        float width = spellData.spellWidth;

        Vector3 right = Vector3.Cross(Vector3.up, dir).normalized;
        Vector3 leftStart = start - right * (width / 2f);
        Vector3 rightStart = start + right * (width / 2f);

        Vector3 leftEnd = leftStart + dir * length;
        Vector3 rightEnd = rightStart + dir * length;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(leftStart, leftEnd);
        Gizmos.DrawLine(rightStart, rightEnd);
    }
#endif
}