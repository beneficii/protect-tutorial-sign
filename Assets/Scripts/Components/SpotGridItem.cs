using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpotGridItem : MonoBehaviour
{
    Vector2Int Pos => new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));

    public static event System.Action<GameObject> OnAppear;

    private void Start()
    {
        MySpots.current.taken.Add(Pos);
        OnAppear?.Invoke(gameObject);

    }

    public void Remove()
    {
        tag = "Destroyed";
        MySpots.current.taken.Remove(Pos);
        Destroy(gameObject);
    }
}
