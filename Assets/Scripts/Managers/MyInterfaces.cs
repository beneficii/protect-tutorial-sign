using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MyInterfaces : MonoBehaviour
{
    public static bool IsAlive(object obj) => (bool)(obj as UnityEngine.Object);
}
