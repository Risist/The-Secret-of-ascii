using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReAi
{
    public enum EAwarenessLevel
    {
        EChill,
        ESearching,
        EBattle
    }
}

public class AiController : InputManagerBase
{
    bool[] input = new bool[4];
    Vector2 positionInput;
    Vector2 directionInput;

    public override Vector2 GetDirectionInput(){ return directionInput; }
    public override Vector2 GetPositionInput() { return positionInput; }
    public override bool IsInputDown(int id) { return input[id]; }
    public override bool IsInputPressed(int id) { return input[id]; }
    public override bool IsInputUp(int id) { return input[id]; }


    new private void Update()
    {
        base.Update();
        ChillBehaviour();   
    }

    #region Chill
    public float tChangeAimMin, tChangeAimMax;
    public float minDir;
    public float maxDir;
    Timer tChangeAim = new Timer();

    void ChillBehaviour()
    {
        if(tChangeAim.isReadyRestart())
        {
            tChangeAim.cd = Random.Range(tChangeAimMin, tChangeAimMax);
            positionInput = Random.insideUnitCircle * Random.Range(minDir, maxDir);
        }
    }
    #endregion Chill
}
