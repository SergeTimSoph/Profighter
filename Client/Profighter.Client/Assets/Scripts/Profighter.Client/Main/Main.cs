using System;
using Profighter.Client.UI;
using UnityEngine;

public class Main : MonoBehaviour
{
    [SerializeField]
    private

    void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.targetFrameRate = 60;

        DeviceScreenInfo.Setup();
    }

    private void Start()
    {


    }
}
