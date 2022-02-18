using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MySpots : MonoBehaviour
{
    public Vector2Int corner1;
    public Vector2Int corner2 = Vector2Int.one;

    public static MySpots current;
    public HashSet<Vector2Int> taken = new HashSet<Vector2Int>();

    private void Awake()
    {
        current = this;
    }

    public bool CanUse(Vector2Int pos)
    {
        if (!InRange(pos.x, corner1.x, corner2.x)) return false;
        if (!InRange(pos.y, corner1.y, corner2.y)) return false;
        if (taken.Contains(pos)) return false;

        return true;
    }

    public Vector2Int RandomPos()
    {
        // +/- 1 to avoid tooltip overlap
        int x = Random.Range(corner1.x, corner2.x); 
        int y = Random.Range(corner1.y+1, corner2.y-2);

        return new Vector2Int(x, y);
    }

    public Vector2Int RandomFreeSpot(int tries = 200)
    {
        Vector2Int pos = new Vector2Int();
        for (int i = 0; i < tries; i++)
        {
            pos = RandomPos();
            if (CanUse(pos)) return pos;
        }

        Debug.LogWarning("Could not get free spot");

        return pos;
    }


    static bool InRange(int value, int min, int max) => min <= value && value <= max;

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 1, 0, 0.5f);
        var radius = new Vector2(corner2.x - corner1.x, corner2.y - corner1.y) * 0.5f;
        var center = new Vector2(corner2.x - radius.x, corner2.y - radius.y);

        Gizmos.DrawCube(center, new Vector3(radius.x * 2, radius.y * 2, 1));
    }
}
