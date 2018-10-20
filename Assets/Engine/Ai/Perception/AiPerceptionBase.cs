using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryItem
{
    public Timer remainedTime;
    public AiPerceiveUnit unit;
    public float lastDistance;
}

public abstract class AiPerceptionBase : MonoBehaviour
{
    /// how long the target is remembered
	public float memoryTime = 5.0f;
    public Timer timerPerformSearch = new Timer(0.5f);

    [System.NonSerialized] List<MemoryItem> memory = new List<MemoryItem>();

    public MemoryItem SearchInMemory(AiFraction fraction, AiFraction.Attitude attitude)
    {
        foreach(var it in memory)
            if(it.unit && it.unit.fraction)
                if(it.unit.fraction.GetAttitude(fraction) == attitude)
                    return it;
        return null;
    }
    public MemoryItem SearchInMemory(AiFraction fraction, string fractionName)
    {
        foreach (var it in memory)
            if (it.unit && it.unit.fraction)
                if (it.unit.fraction.fractionName == fractionName)
                return it;
        return null;
    }
    protected bool InsertToMemory(AiPerceiveUnit unit, float distance)
    {
        if (!unit.memoriable)
            return false;

        foreach (var itMemory in memory)
            if (itMemory.unit == unit)
            {
                itMemory.remainedTime.restart();
                itMemory.lastDistance = distance;
                return false;
            }

        var memoryItem = new MemoryItem();
        memoryItem.unit = unit;
        memoryItem.remainedTime = new Timer(memoryTime);
        memoryItem.lastDistance = distance;

        memory.Add(memoryItem);
        return true;
    }
    protected void PerformClear()
    {
        for (int i = 0; i < memory.Count; ++i)
            if (memory[i].remainedTime.isReady())
            {
                memory.RemoveAt(i);
                --i;
            }
    }
    protected void SortMemory()
    {
        memory.Sort(delegate (MemoryItem item1, MemoryItem item2) { return item1.lastDistance.CompareTo(item2.lastDistance); });
    }
}