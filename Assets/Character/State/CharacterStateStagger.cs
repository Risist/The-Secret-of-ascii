using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReAnim;
public class CharacterStateStagger : MonoBehaviour
{
    public Character.CStateStagger state;
    public void OnReceiveDamage(HealthController.DamageData data)
    {
        state.OnReceiveDamage(data);
    }
}
namespace Character
{
    
    public class CStateStagger : StateComponent
    {
        public CStateStagger() { }
        public CStateStagger( CdRestartStruct[] _cdRestart) { cdRestart = _cdRestart; }
        bool damaged = false;
        bool atAnim = false;

        public struct CdRestartStruct
        {
            public CdRestartStruct(int _cdId, float _cdOffset = 0)
            {
                cdId = _cdId;
                cdOffset = _cdOffset;
            }

            public int cdId;
            public float cdOffset;
        }
        public CdRestartStruct[] cdRestart;

        public override void Init()
        {
            base.Init();
            controller.gameObject.AddComponent<CharacterStateStagger>().state = this;
        }
        public void OnReceiveDamage(HealthController.DamageData data)
        {
            if (!atAnim && data.damage < 0)
            {
                damaged = true;
            }
        }
        public override bool CanEnter() { return damaged; }
        public override void InitPlayback(StateTransition transition)
        {
            damaged = false;
            atAnim = true;
        }
        public override void OnAnimationEnd(AnimatorStateInfo stateInfo)
        {
            atAnim = false;
            damaged = false;

            foreach (var it in cdRestart)
            {
                var tim = controller.GetCdTimer(it.cdId);
                tim.actualTime = Mathf.Clamp(tim.actualTime, 0, Time.time-tim.cd);
                tim.actualTime += it.cdOffset;
            }
        }
    }


    public class CStateDamageShake : StateComponent
    {
        DamageEventShake shake;
        bool bApplied = false;
        public override void Init()
        {
            shake = controller.GetComponent<DamageEventShake>();
        }
        public override void InitPlayback(StateTransition transition)
        {
            bApplied = false;
        }
        public override void OnAnimationUpdate(AnimatorStateInfo stateInfo)
        {
            if (!bApplied && shake && stateInfo.normalizedTime > 0.25f)
            {
                shake.shake();
                //Handheld.Vibrate();
                bApplied = true;
            }
        }
    }
}
