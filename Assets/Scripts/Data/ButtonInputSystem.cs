using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ButtonInputSystem : MonoBehaviour
{
    public InputData input;

    public InputSchema schema1;
    public InputSchema schema2;

    private void Awake()
    {
        input.ResetAll(schema1.keyA.ToString(), schema1.keyB.ToString(), schema1.keySelect.ToString());
    }

    void Update()
    {
        Vector2Int direction = Vector2Int.zero;

        foreach (var item in schema1.directionKeys)
        {
            if (Input.GetKey(item.key))
            {
                direction += item.delta;
            }
        }

        //Debug.Log($"direction: {direction}");

        bool a = Input.GetKey(schema1.keyA);
        bool b = Input.GetKey(schema1.keyB);
        
        bool select = Input.GetKey(schema1.keySelect);

        bool a2 = false;
        bool b2 = false;
        bool select2 = false;

        if (schema2)
        {
            if(direction == Vector2Int.zero)
            {
                foreach (var item in schema2.directionKeys)
                {
                    if (Input.GetKey(item.key))
                    {
                        direction += item.delta;
                    }
                }
            }

            a2 = Input.GetKey(schema2.keyA);
            b2 = Input.GetKey(schema2.keyB);
            select2 = Input.GetKey(schema2.keySelect);
        }

        // check for axis. easy hack to detect more inputs.
        var axisDirection = new Vector2Int(
            Mathf.RoundToInt(Input.GetAxisRaw("Horizontal")),
            Mathf.RoundToInt(Input.GetAxisRaw("Vertical"))
            );

        if (axisDirection != Vector2Int.zero) direction = axisDirection;

        if (a || a2 || b || b2) direction = Vector2Int.zero;

        input.SetInputs(direction, a || a2, b || b2, select || select2);
    }
}

[System.Serializable]
public class DirectionKey
{
    public KeyCode key;
    public Vector2Int delta;
}