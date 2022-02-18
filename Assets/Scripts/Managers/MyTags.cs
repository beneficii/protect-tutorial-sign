using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MyTags : MonoBehaviour
{
    public static IEnumerable<GameObject> GetItems(string tag)
    {
        foreach (var item in GameObject.FindGameObjectsWithTag(tag))
        {
            yield return item.transform.parent.gameObject;
        }
    }
}
