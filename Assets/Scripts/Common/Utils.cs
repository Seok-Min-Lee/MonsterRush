using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Utils
{
    public static List<T> Shuffle<T>(IEnumerable<T> samples)
    {
        List<T> shuffled = samples.ToList();

        for (int i = shuffled.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);

            T temp = shuffled[j];
            shuffled[j] = shuffled[i];
            shuffled[i] = temp;
        }

        return shuffled;
    }
    public static string FormatTimeToHHmmss(float value)
    {
        int hours = (int)(value / 3600);
        int minutes = (int)((value % 3600) / 60);
        int seconds = (int)(value % 60);

        return $"{hours:00}:{minutes:00}:{seconds:00}";
    }
    public static string FormatTimeTommss(float value)
    {
        int minutes = (int)(value / 60);
        int seconds = (int)(value % 60);

        return $"{minutes:00}:{seconds:00}";
    }
}
