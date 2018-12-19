using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// An structure to hold data about perception items which are fully known to an agent
public class MemoryItem
{
    public Timer remainedTime;
    public AiPerceiveUnit unit;
    public float lastDistance;
}


public enum EMemoryEvent
{
    ENoise,
    EDamage,
    /// Todo other types
    ECount
}
/// An structure to hold data about event that happened nearby
public class MemoryEvent
{
    public Timer remainedTime;
    public Vector2 position
    {
        set
        {
            _position = value;
        }
        get
        {
            return _position + Random.insideUnitCircle * uncertainityRadius;
        }
    }
    Vector2 _position;

    public float uncertainityRadius;
    public Vector2 GetExactPosition()
    {
        return _position;
    }
}

public class AiPerceptionHolder : MonoBehaviour
{
    #region UnitMemory
    /// how long the target is remembered
    public Timer tPerformClear;
	public float memoryTime = 5.0f;
    [System.NonSerialized] protected List<MemoryItem> memory = new List<MemoryItem>();

    bool anyAdded = false;

    public MemoryItem SearchInMemory(AiFraction fraction, AiFraction.Attitude attitude)
    {
        if (anyAdded)
        {
            SortMemory();
            anyAdded = false;
        }

        foreach (var it in memory)
            if (it.unit && it.unit.fraction)
                if (it.unit.fraction.GetAttitude(fraction) == attitude)
                    return it;
        return null;
    }
    public MemoryItem SearchInMemory(string fractionName)
    {
        if (anyAdded)
        {
            SortMemory();
            anyAdded = false;
        }

        foreach (var it in memory)
            if (it.unit && it.unit.fraction)
                if (it.unit.fraction.fractionName == fractionName)
                    return it;
        return null;
    }
    public bool InsertToMemory(AiPerceiveUnit unit, float distance)
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
        anyAdded = true;
        return true;
    }
    void PerformClear()
    {
        for (int i = 0; i < memory.Count; ++i)
            if (memory[i].remainedTime.isReady())
            {
                memory.RemoveAt(i);
                --i;
            }

        for (int j = 0; j < (int)EMemoryEvent.ECount; ++j)
        {
            var evMem = eventMemory[j];
            for (int i = 0; i < evMem.Count; ++i)
                if (evMem[i].remainedTime.isReady())
                {
                    evMem.RemoveAt(i);
                    --i;
                }
        }
    }
    void SortMemory()
    {
        memory.Sort(delegate (MemoryItem item1, MemoryItem item2) { return item1.lastDistance.CompareTo(item2.lastDistance); });
    }

    #endregion UnitMemory

    #region EventMemory
    public float eventMemoryTime = 1.0f;
    [System.NonSerialized] protected List<MemoryEvent>[] eventMemory = new List<MemoryEvent>[(int)EMemoryEvent.ECount];

    public void InsertToMemory(EMemoryEvent eventType, Vector2 position, float uncertainityRadius = 0f)
    {
        MemoryEvent item = new MemoryEvent();
        item.position = position;
        item.uncertainityRadius = uncertainityRadius;
        eventMemory[(int)eventType].Add(item);
    }
    public List<MemoryEvent> GetMemoryEvent(EMemoryEvent eventType)
    {
        return eventMemory[(int)eventType];
    }

    bool[] anyEventAdded = new bool[(int)EMemoryEvent.ECount];
    void SortMemory(EMemoryEvent eventType)
    {
        eventMemory[(int)eventType].Sort(
            delegate (MemoryEvent item1, MemoryEvent item2) 
            {
                float item1Distance = ((Vector2)transform.position - item1.GetExactPosition()).sqrMagnitude;
                float item2Distance = ((Vector2)transform.position - item2.GetExactPosition()).sqrMagnitude;
                return item1Distance.CompareTo(item2Distance);
            });
    }
    #endregion EventMemory

    private void Start()
    {
        for (int i = 0; i < (int)EMemoryEvent.ECount; ++i)
            eventMemory[i] = new List<MemoryEvent>();
    }
    private void Update()
    {
        if (tPerformClear.isReadyRestart())
            PerformClear();
    }
}