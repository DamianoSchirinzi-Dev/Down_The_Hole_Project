using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{

    public float slowdownFactor = 0.05f;
    
    public void DoSlowMoTime()
    {
        Time.timeScale = slowdownFactor;
    }

    public void StopSlowMoTime()
    {
        Time.timeScale = 1;
    }
}
