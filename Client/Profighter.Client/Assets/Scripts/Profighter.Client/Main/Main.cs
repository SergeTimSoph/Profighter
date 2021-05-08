using UnityEngine;

public class Main : MonoBehaviour
{
    void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.targetFrameRate = 60;
    }
}
