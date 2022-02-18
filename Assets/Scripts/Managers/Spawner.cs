using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    public List<Transform> checkPoints = new List<Transform>();

    public void Spawn(Enemy prefab)
    {
        var instance = Instantiate(prefab, transform.position, Quaternion.identity);

        if (instance.TryGetComponent<Enemy>(out var enemy))
        {
            foreach (var item in checkPoints)
            {
                enemy.checkPoints.Enqueue(item.position);
            }
        }
    }
}
