using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CloseOnA : MonoBehaviour
{
    float waitDelay = 0f;

    public void OnEnable()
    {
        waitDelay = Time.time + 2f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > waitDelay && Input.GetKeyDown(KeyCode.Space))
        {
            gameObject.SetActive(false);
        }
    }
}
