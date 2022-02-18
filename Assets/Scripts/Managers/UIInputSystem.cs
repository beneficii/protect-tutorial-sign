using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIInputSystem : MonoBehaviour
{
    public static bool is8Way = false;

    public InputData data;

    public List<JamUIButton> buttons; //a, b, select
    public JamUIButton dPad;

    Camera cam;

    
    private void Awake()
    {
        data.ResetAll("A", "B", "Start");
        cam = Camera.main;
    }

    public Vector2Int Dir8Way(Vector2 delta) => new Vector2Int(Mathf.RoundToInt(delta.x), Mathf.RoundToInt(delta.y));
    public Vector2Int Dir4Way(Vector2 delta)
    {
        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
        {
            return new Vector2Int(Mathf.RoundToInt(delta.x), 0);
        }
        else
        {
            return new Vector2Int(0, Mathf.RoundToInt(delta.y));
        }
    }

    void Update()
    {
        int bCount = 3;
        var states = new bool[bCount];
        /*
        var pos = (Vector2)cam.ScreenToWorldPoint(Input.mousePosition);

        for (int i = 0; i < bCount; i++)
        {
            if(buttons[i].TestCollision(pos))
            {
                states[i] = true;
            }
        }*/

        bool detectedDPad = false;

        Vector2Int direction = new Vector2Int();
        foreach (var touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Ended && touch.phase == TouchPhase.Canceled) continue;
            var pos = (Vector2)cam.ScreenToWorldPoint(touch.position);

            for (int i = 0; i < bCount; i++)
            {
                if (buttons[i].Collides(pos))
                {
                    states[i] = true;
                }
            }

            if(!detectedDPad && dPad.Collides(pos))
            {
                var delta = dPad.GetDirection(pos);
                direction = is8Way? Dir8Way(delta):Dir4Way(delta);

                detectedDPad = true;
            }
        }


        data.SetInputs(direction, states[0], states[1], states[2]);
        if(data.useButton.State == KeyState.Down
            || data.hammerButton.State == KeyState.Down
            || data.selectButton.State == KeyState.Down)
        {
            AndroidManager.HapticFeedback();
        }
    }
}