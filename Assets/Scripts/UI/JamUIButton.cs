using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JamUIButton : MonoBehaviour
{
    public Collider2D colliderClickArea;
    public SpriteRenderer render;

    public bool Collides(Vector2 point)
    {
        return colliderClickArea.bounds.Contains(point);
    }

    public bool TestCollision(Vector2 point)
    {
        bool result = Collides(point);

        render.color = result ? Color.red : Color.white;

        return result;
    }

    public Vector2 GetDirection(Vector2 point)
    {
        //if (!Collides(point)) return Vector2.zero;

        return (point - (Vector2)transform.position).normalized;
    }
}
