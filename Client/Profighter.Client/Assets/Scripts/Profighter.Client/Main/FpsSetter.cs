using UnityEngine;

public class FpsSetter : MonoBehaviour
{
    void Start()
    {
        Application.targetFrameRate = 60;
    }
}
