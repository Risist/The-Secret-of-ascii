using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Helper class to randomise and id from given set with different probabilites for evry oportunity 
 * 
 */

[System.Serializable]
public class RandomChance
{
    public RandomChance() { }
    public RandomChance(float[] _chances) { chances = _chances; }
    public float[] chances = null;

    public int GetRandedId()
    {
        float sum = 0;
        foreach (float it in chances)
            sum += it;

        if (sum == 0)
            return -1;

        float randed = Random.Range(0, sum);

        float lastSum = 0;
        for (int i = 0; i < chances.Length; ++i)
            if (randed >= lastSum && randed <= lastSum + chances[i])
            {
                return i;
            }
            else
            {
                lastSum += chances[i];
            }

        return -1;
    }
}

[System.Serializable]
public class RandomChanceList
{
    public RandomChanceList() { }
    public RandomChanceList(List<float> _chances) { chances = _chances; }
    public List<float> chances = null;

    public int GetRandedId()
    {
        float sum = 0;
        for(int i = 0; i < chances.Count; ++i)
            sum += chances[i];

        if (sum == 0)
            return -1;

        float randed = Random.Range(0, sum);

        float lastSum = 0;
        for (int i = 0; i < chances.Count; ++i)
            if (randed >= lastSum && randed <= lastSum + chances[i])
            {
                return i;
            }
            else
            {
                lastSum += chances[i];
            }

        return -1;
    }
}

