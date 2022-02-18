using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatformPicker : MonoBehaviour
{
    public PlatformType type;


    private void Awake()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        gameObject.SetActive(type != PlatformType.PC);

#else
        gameObject.SetActive(type != PlatformType.Phone);
#endif
    }
}

public enum PlatformType
{
    All,
    Phone,
    PC
}
