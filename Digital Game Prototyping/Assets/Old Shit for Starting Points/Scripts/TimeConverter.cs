using UnityEngine;

public class TimeConverter //: MonoBehaviour
{
    void Awake()
    {
        
    }

    public string SecondsToDigitalDisplay(float sec)
    {
        return SecondsToDigitalDisplay(Mathf.RoundToInt(sec));
    }

    public string SecondsToDigitalDisplay(int sec)
    {
        string time = "";
        int min = 0;

        if (sec < 60)
        {
            time = "00:";

            if (sec < 10)
            {
                time = time + "0";
            }

            time = time + sec;
        }
        else
        {
            while (sec >= 60)
            {
                sec -= 60;
                min++;
            }

            if (min > 9)
            {
                time = time + min;
            }
            else
            {
                time = time + "0";
                time = time + min;
            }

            time = time + ":";

            if (sec > 9)
            {
                time = time + sec;
            }
            else
            {
                time = time + "0";
                time = time + sec;
            }
        }

        

        return time;
    }
}
