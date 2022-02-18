using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputKeySetup : MonoBehaviour
{
    public InputSchema schema;

    bool waitingInput = false;
    KeyCode lastKey = KeyCode.None;

    private void Update()
    {
        if (!waitingInput) return;
        foreach (var item in EnumUtil.GetValues<KeyCode>())
        {
            if(Input.GetKeyDown(item))
            {
                lastKey = item;
                waitingInput = false;
                return;
            }
        }
    }

    void SetKey(KeyCode code)
    {
        waitingInput = false;
    }

    public IEnumerator SetupRoutine()
    {
        yield break;
    }
}

