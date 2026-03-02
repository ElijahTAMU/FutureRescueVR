using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public int TimeSeconds;
    float timeRemaining = 0;
    public TextMeshProUGUI ClockText;

    bool countingDown = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (countingDown)
        {
            timeRemaining -= Time.deltaTime;

            int minutes = (int)(timeRemaining / 60);
            int seconds = (int)(timeRemaining - (minutes * 60));

            ClockText.text = minutes + ":" + seconds;
        }
    }

    public void ResetTimer()
    {
        timeRemaining = TimeSeconds;

        int minutes = (int)(timeRemaining / 60);
        int seconds = (int)(timeRemaining - (minutes * 60));
        ClockText.text = minutes + ":" + seconds;

    }

    public void StartTimer()
    {
        timeRemaining = TimeSeconds;
        countingDown = true;
    }

    public void StopTimer()
    {
        countingDown = false;
    }
}
