using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{

    public float slowdownFactor = 0.05f;
    public bool isInSlowMo = false;
    
    public void DoSlowMoTime()
    {
        Time.timeScale = slowdownFactor;
        isInSlowMo = true;
    }

    public void StopSlowMoTime()
    {
        Time.timeScale = 1;
        isInSlowMo = false;
    }
}
