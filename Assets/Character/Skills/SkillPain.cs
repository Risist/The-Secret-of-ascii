using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReAnim;

public class SkillPain : SkillBase {

    public string[] animCodes;
    bool damaged;
    

    public void OnReceiveDamage(HealthController.DamageData data) { if(cd.isReady()) damaged = true; }
    public override bool CanEnter() { return damaged; }

    public override void InitPlayback(Transition transition)
    {
        base.InitPlayback(transition);
        PlayAnimation(animCodes[Random.Range(0,animCodes.Length)]);
        damaged = false;
        skillManager.ResetInputBuffer();
    }
}
