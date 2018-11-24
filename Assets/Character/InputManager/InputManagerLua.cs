using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp.Interpreter;

[MoonSharpUserData]
class LuaPerceptionProxy
{
    AiPerceptionBase perception;
    

}
[MoonSharpUserData]
class LuaOutputProxy
{
    InputManagerLua input;

}

public class InputManagerLua : InputManagerBase
{
    public string fileName;
    Script script = new Script();
    DynValue updateFunction;

    #region Setable input data
    public bool[] input = new bool[4] { false, false, false, false };
    public Vector2 positionInput;
    public Vector2 directionInput;
    #endregion Setable input data


    private void Start()
    {
        UserData.RegisterAssembly();
        UserData.RegisterType<Vector2>();
        UserData.RegisterType<Quaternion>();

        script.Globals["perception"] = new LuaPerceptionProxy();
        script.Globals["output"] = new LuaOutputProxy();
        script.DoFile(fileName);
        updateFunction = script.Globals.Get("Update");

    }

    new private void Update()
    {
        base.Update();
        script.Call(updateFunction);
    }

    public override Vector2 GetDirectionInput()
    {
        return Vector2.zero;
    }

    public override Vector2 GetPositionInput()
    {
        return Vector2.zero;
    }

    public override bool IsInputDown(int id)
    {
        return false;
    }

    public override bool IsInputPressed(int id)
    {
        return false;
    }

    public override bool IsInputUp(int id)
    {
        return false;
    }
}
