using UnityEngine;
using TMPro;

using System;

public class TimerScript : MonoBehaviour
{
    public float time = 60f;
    public TMP_Text timerText;
    // Update is called once per frame
    void Update()
    {
        if(time > 0f)
        {
          time -= Time.deltaTime;
          timerText.text = Math.Ceiling(time).ToString();  
        }
        else
        {
            timerText.text = "Time Over";
            Time.timeScale = 0f; //freeze scene
        }
        
    }
}
