using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Setting : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(GameObject.Find("UnityManager"));

        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Screen.SetResolution(450, 800, false);
    }
}
