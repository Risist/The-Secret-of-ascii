using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// types of memory events
public enum EMemoryEvent
{
    /// All potential targets of assault
    EEnemy,
    /// potential targets of friendly actions
    EAlly,
    /// uncertain attraction sources
    /// ai should not take immediatelly actions
    /// instead just walk towards the source of attraction and check what is going on
    /// works only when not on combat
    ENoise, 
    /// Todo other types
    ECount
}
/// An structure to hold data about event that happened nearby
public class MemoryEvent
{
    public MinimalTimer remainedTime;
    /// how long it takes to start considering the data as valid
    public float matureTime;
    /// how long the data will be taken as valid
    public float knowledgeTime;
    /// how long the data will be kept in memory
    public float shadeTime;


    /// position of the event source at record time
    public Vector2 exactPosition;
    public Vector2 direction;

    
    /// a value which can be used to tweak sorting.
    /// target fittness depends both on distance and importance value ( the lower the better)
    public float importance = 1f;

    /// unit responsible for this event
    /// if not null remainedTime is refreshed if trying to add new event with the same unit
    /// otherwise always adds the event
    public AiPerceiveUnit unit;
    public bool hadUnit;

    /// distance which target could possibly travel from the start of the event assuming same speed
    public float elapsedDistance
    {
        get
        {
            return direction.magnitude * remainedTime.ElapsedTime();
        }
    }

    /// fully computed position predicted by this event
    /// comes up with uncertainity area, and linear interpolation of current position by direction and speed of the event
    public Vector2 position
    {
        get
        {
            return exactPosition + direction * remainedTime.ElapsedTime();
        }
    }
}

public class AiPerceptionHolder : MonoBehaviour
{
    public Timer tPerformClear;

    #region EventMemory
    [System.NonSerialized] protected List<MemoryEvent>[] eventMemory = new List<MemoryEvent>[(int)EMemoryEvent.ECount];
    /// dirty bit kind of optimalisation
    /// sorts memory only when needed
    bool[] anyEventAdded = new bool[(int)EMemoryEvent.ECount];


    public bool InsertToMemory(AiPerceiveUnit unit, EMemoryEvent eventType, Vector2 position, Vector2 direction,
        float remainTime = 1f, float matureTime = 0f, float shadeTime = 0f, float importance =1f)
    {
        if (!unit.memoriable)
            return false;

        int id = (int)eventType;
        var mem = eventMemory[id];

        /// search if the unit is recorded in our memory
        /// if so then update it
        if(unit)
            foreach (var itMemory in mem)
                if (itMemory.unit == unit)
                {
                    /// time data
                    /// 

                    /// If information is mature - result should be mature too 
                    /// 
                    if (itMemory.remainedTime.IsReady(itMemory.matureTime))
                    {
                        itMemory.matureTime = 0;
                        itMemory.remainedTime.Restart();
                    }
                    else
                        itMemory.matureTime = matureTime;
                    itMemory.knowledgeTime = remainTime;
                    itMemory.shadeTime = shadeTime;

                    /// spatial data
                    itMemory.exactPosition = position;
                    itMemory.direction = direction;

                    itMemory.importance = importance;
                    /// list is not sorted
                    anyEventAdded[id] = true;
                    return false;
                }

        /// otherwise insert new item
        MemoryEvent item = new MemoryEvent();
        item.unit = unit;
        item.hadUnit = unit != null;

        /// spatial data
        item.exactPosition = position;
        item.direction = direction;

        /// time data
        item.remainedTime = new Timer();
        item.remainedTime.Restart();
        item.matureTime = matureTime;
        item.knowledgeTime = remainTime;
        item.shadeTime = shadeTime;

        item.importance = importance;

        mem.Add(item);
        /// list is not sorted
        anyEventAdded[id] = true;
        return true;
    }
    /// auto compute direction of unit
    public bool InsertToMemory(AiPerceiveUnit unit, EMemoryEvent eventType, Vector2 position, float predictionScale, 
        float remainTime = 1f, float matureTime = 0f, float shadeTime = 0f, float importance = 1f)
    {
        if (!unit.memoriable)
            return false;

        int id = (int)eventType;
        var mem = eventMemory[id];

        /// search if the unit is recorded in our memory
        /// if so then update it
        if(unit)
            foreach (var itMemory in mem)
                if (itMemory.unit == unit )
                {
                    /// spatial data
                    if (itMemory.remainedTime.ElapsedTime() > 3 * float.Epsilon)
                        itMemory.direction = (position - itMemory.exactPosition)*(predictionScale/itMemory.remainedTime.ElapsedTime()); /// auto compute direction
                    /// else keep last value... dunno what to do in case of such a small time step
                    
                    itMemory.exactPosition = position;


                    /// time data
                    itMemory.remainedTime.Restart();
                    itMemory.matureTime = 0f;

                    /// list is not sorted
                    anyEventAdded[id] = true;
                    return false;
                }

        /// otherwise insert new item
        MemoryEvent item = new MemoryEvent();
        item.unit = unit;
        item.hadUnit = unit != null;

        /// spatial data
        item.exactPosition = position;
        item.direction = Vector2.zero;

        /// time data
        item.remainedTime = new Timer();
        item.remainedTime.Restart();
        item.matureTime = matureTime;
        item.knowledgeTime = remainTime;
        item.shadeTime = shadeTime;

        item.importance = importance;

        mem.Add(item);
        /// list is not sorted
        anyEventAdded[id] = true;
        return true;
    }
    public void InsertToMemory(EMemoryEvent eventType, Vector2 position, Vector2 direction, 
        float remainTime = 1f, float matureTime = 0f, float shadeTime =0f, float importance = 1f)
    {
        int id = (int)eventType;
        var mem = eventMemory[id];

        MemoryEvent item = new MemoryEvent();
        /// no unit/ unknown unit responsible for the event
        item.unit = null;/// otherwise insert new item
        item.hadUnit = false;

        /// spatial data
        item.exactPosition = position;
        item.direction = direction;

        /// time data
        item.remainedTime = new Timer();
        item.remainedTime.Restart();
        item.matureTime = matureTime;
        item.knowledgeTime = remainTime;
        item.shadeTime = shadeTime;

        item.importance = importance;

        mem.Add(item);
        /// list is not sorted
        anyEventAdded[id] = true;
    }


    public List<MemoryEvent> GetMemoryEventList(EMemoryEvent eventType)
    {
        return eventMemory[(int)eventType];
    }
    public MemoryEvent SearchInMemory(EMemoryEvent eventType)
    {
        int id = (int)eventType;
        if (eventMemory[id].Count == 0)
            return null;

        if (anyEventAdded[id])
        {
            SortMemory(eventType);
            anyEventAdded[id] = false;
        }

        var e = eventMemory[id][0];
        if(e.remainedTime.IsReady(e.matureTime))
            return eventMemory[id][0];

        return null;
    }

    void SortMemory(EMemoryEvent eventType)
    {
        eventMemory[(int)eventType].Sort(
            delegate (MemoryEvent item1, MemoryEvent item2) 
            {
                if (item1.remainedTime.IsReady(item1.knowledgeTime))
                {
                    if (!item2.remainedTime.IsReady(item2.knowledgeTime))
                        return 1;
                }
                else if (item2.remainedTime.IsReady(item2.knowledgeTime))
                    return -1;


                if (item1.unit != null)
                {
                    if (item2.unit == null)
                        return 1;
                }
                else if (item2.unit != null)
                    return -1;
                
                if (item1.remainedTime.IsReady(item1.matureTime))
                {
                    if (!item2.remainedTime.IsReady(item2.matureTime))
                        return 1;
                }
                else if (item2.remainedTime.IsReady(item2.matureTime))
                    return -1;

                float item1Distance = ((Vector2)transform.position - item1.exactPosition).sqrMagnitude * item1.importance;
                float item2Distance = ((Vector2)transform.position - item2.exactPosition).sqrMagnitude * item2.importance;
                return item1Distance.CompareTo(item2Distance);
            });
    }
    #endregion EventMemory


    void PerformClear()
    {
        for (int j = 0; j < (int)EMemoryEvent.ECount; ++j)
        {
            var evMem = eventMemory[j];
            for (int i = 0; i < evMem.Count; ++i)
                if (evMem[i].remainedTime.IsReady(evMem[i].shadeTime) || 
                    (evMem[i].hadUnit && evMem[i].unit == null)
                    )
                {
                    evMem.RemoveAt(i);
                    --i;
                }
        }
    }

    private void Start()
    {
        for (int i = 0; i < (int)EMemoryEvent.ECount; ++i)
            eventMemory[i] = new List<MemoryEvent>();
    }
    private void Update()
    {
        if (tPerformClear.IsReadyRestart())
            PerformClear();
    }
}