using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapCamera : MonoBehaviour
{
    public GameObject camera1;
    public GameObject camera2;

    public KeyCode swapKey = KeyCode.Space;
    void Update()
    {
        if (Input.GetKeyDown(swapKey))
        {
            bool isCamera1Active = camera1.activeSelf;
            camera1.SetActive(!isCamera1Active);
            camera2.SetActive(isCamera1Active);
        }


    }
}
