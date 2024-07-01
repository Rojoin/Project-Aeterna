using UnityEngine;

public class SwTimer : SwBasic
{
    public float totalTime;
    public bool playOnStart;
    private float time = 0;

    void OnEnable()
    {
        if (!playOnStart)
        {
            RestartTimer();
        }
        else
        {
            time = totalTime;
        }
    }

    private void Update()
    {
        time += Time.deltaTime;

        if (time >= totalTime)
        {
            TimerEnd();
        }

    }

    void TimerEnd()
    {
        OnPlayAudio();
        RestartTimer();
    }

    private void RestartTimer()
    {
        time = 0;
    }
}