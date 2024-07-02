using UnityEngine;

public class SwRandom : SwBasic
{
    public float minTime;
    public float maxTime;
    public bool playOnStart;
    private float time = 0;
    private float targetTime;

    void OnEnable()
    {
        if (!playOnStart)
        {
            RestartTimer();
        }
        else
        {
            targetTime = 0;
        }
    }

    private void Update()
    {
        time += Time.deltaTime;

        if (time >= targetTime)
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
        targetTime = Random.Range(minTime, maxTime);
    }
}