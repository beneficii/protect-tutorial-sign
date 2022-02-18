using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WarZone : MonoBehaviour
{
    public static WarZone current;

    public static IAttackable GetClosestAttackable(Vector3 source, bool enemyTeam, float maxDistance = float.MaxValue, bool needsHp = false)
    {
        IAttackable closest = null;
        float minDistance = maxDistance;

        foreach (var item in MyTags.GetItems("Attackable"))
        {
            if (item.transform.position == source) continue; //don't find self (for reasons)
            var iItem = item.GetComponent<IAttackable>();
            if (needsHp && !iItem.HasHp) continue;


            if (iItem == null || iItem.IsEnemyTeam != enemyTeam || !iItem.IsAlive) continue;

            float distance = Vector2.Distance(source, item.transform.position);
            if(distance < minDistance) 
            {
                minDistance = distance;
                closest = iItem;
            }
        }

        return closest;
    }

    public static IAttackable GetFarestAttackable(Vector3 source, bool enemyTeam, float maxDistance = float.MaxValue, bool needsHp = false)
    {
        IAttackable best = null;
        float bestDistance = float.MinValue;

        foreach (var item in MyTags.GetItems("Attackable"))
        {
            if (item.transform.position == source) continue; //don't find self (for reasons)

            var iItem = item.GetComponent<IAttackable>();
            if (needsHp && !iItem.HasHp) continue;

            if (iItem == null || iItem.IsEnemyTeam != enemyTeam || !iItem.IsAlive) continue;

            float distance = Vector2.Distance(source, item.transform.position);
            if (distance > bestDistance && distance <= maxDistance)
            {
                bestDistance = distance;
                best = iItem;
            }
        }

        return best;
    }

    public static List<IAttackable> GetAttackables(Vector3 source, bool enemyTeam, float radius)
    {
        List<IAttackable> result = new List<IAttackable>();

        foreach (var item in MyTags.GetItems("Attackable"))
        {
            var iItem = item.GetComponent<IAttackable>();

            if (iItem == null || iItem.IsEnemyTeam != enemyTeam || !iItem.IsAlive) continue;

            float distance = Vector2.Distance(source, item.transform.position);
            if (distance < radius)
            {
                result.Add(iItem);
            }
        }

        return result;
    }
}
