using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "LTRO-1/Input/Schema" )]
public class InputSchema : ScriptableObject
{
    public List<DirectionKey> directionKeys;
    public KeyCode keyA;
    public KeyCode keyB;
    public KeyCode keySelect;
}
