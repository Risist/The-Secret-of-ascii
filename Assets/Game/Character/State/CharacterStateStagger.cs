using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReAnim;

public class CharacterStateStagger : MonoBehaviour
{
    Rigidbody2D body;
    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }
    
    public Character.CStateDamage state;

    public Character.CStateWallStagger wallState;
    public void OnReceiveDamage(HealthController.DamageData data)
    {
        if(state != null)
            state.OnReceiveDamage(data);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(wallState != null && body)
        {
            float velocityDiffSq;
            var otherBody = collision.collider.GetComponent<Rigidbody2D>();
            if (otherBody)
            {
                velocityDiffSq = (body.velocity - otherBody.velocity).sqrMagnitude;

                if (velocityDiffSq > wallState.minVelocity * wallState.minVelocity)
                {
                    float angle = Vector2.SignedAngle(transform.up, collision.collider.transform.position - transform.position);
                    wallState.React(angle);

                }
            }
            else
            {
                velocityDiffSq = body.velocity.sqrMagnitude;

                if (velocityDiffSq > wallState.minVelocity * wallState.minVelocity)
                {
                    float angle = Vector2.SignedAngle(transform.up, collision.collider.transform.position - transform.position);
                    wallState.React(angle);

                }
            }

        }
    }

}


namespace Character
{
    /// allows to transition if character have been damaged and has enough pains
    /// TODO: maybe a little refractor sometime
    public class CStateDamage: StateComponent
    {
        public CStateDamage(float _minimumDamage=0, float _damageAccumulatorChange = 0) { minimumDamage = _minimumDamage; damageAccumulatorChange = _damageAccumulatorChange; }
        public float minimumDamage;
        public float damageAccumulatorChange;

        bool damaged = false;
        bool atAnim = false;

        public void OnReceiveDamage(HealthController.DamageData data)
        {
            if(!atAnim && -controller.GetPainAccumulator() > minimumDamage)
            {
                damaged = true;
            }
        }
        public override bool CanEnter() { return damaged; }

        public override void Init()
        {
            var characterStagger = controller.GetComponent<CharacterStateStagger>();
            if (!characterStagger)
                characterStagger = controller.gameObject.AddComponent<CharacterStateStagger>();

            characterStagger.state = this;
        }

        public override void InitPlayback(StateTransition transition)
        {
            damaged = false;
            atAnim = true;
            controller.ModifyPainAccumulator(damageAccumulatorChange);
        }
        public override void OnAnimationEnd(AnimatorStateInfo stateInfo)
        {
            atAnim = false;
            damaged = false;
        }
    }
    /// Changes saved activation time of given cds
    public class CStateCdReduce : StateComponent
    {
        public CStateCdReduce() { }
        public CStateCdReduce(CdRestartStruct[] _cdRestart) { cdRestart = _cdRestart; }
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

        public override void OnAnimationEnd(AnimatorStateInfo stateInfo)
        {
            foreach (var it in cdRestart)
            {
                var tim = controller.GetCdTimer(it.cdId);
                tim.actualTime = Mathf.Clamp(tim.actualTime, 0, Time.time - tim.cd);
                tim.actualTime += it.cdOffset;
            }
        }
    }

    /// Intention: when thrown onto wall then animation should play
    /// TODO fix it to start actually work as intended :D
    public class CStateWallStagger : StateComponent
    {
        public CStateWallStagger(float _minVelocity, string[] _animCodes)
        {
            minVelocity = _minVelocity;
            animCodes = _animCodes;
        }


        public string[] animCodes;

        public float minVelocity;
        int enter = -1;

        public override bool CanEnter() { return enter != -1; }

        public override void Init()
        {
            base.Init();
            var characterStagger = controller.GetComponent<CharacterStateStagger>();
            if (!characterStagger)
                characterStagger = controller.gameObject.AddComponent<CharacterStateStagger>();

            characterStagger.wallState = this;
        }

        public void React(float angle)
        {
            var v = Mathf.DeltaAngle(angle, 0);
            if (v > 25f && v < 175f)
                enter = 1;
            else if (v > -175f && v < -25f)
                enter = 2;
            else
                enter = 0;
        }

        public override void InitPlayback(StateTransition transition)
        {
            if(enter == -1 || enter > animCodes.Length)
                Debug.Log("enterInit = " + enter);
            controller.PlayAnimation(animCodes[enter]);
        }

        public override void OnAnimationEnd(AnimatorStateInfo stateInfo)
        {
            foreach (var it in animCodes)
                controller.ResetAnimation(it);
            enter = -1;
        }


    }

    /// Does screan shake if DamageEventShake component is in character
    public class CStateDamageShake : StateComponent
    {
        DamageEventShake shake;
        bool bApplied = false;
        public override void Init()
        {
            shake = controller.GetComponentInChildren<DamageEventShake>();
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


    public class CStateDeflect : StateComponent
    {
        string deflectPath;
        public CStateDeflect(string _deflectPath, string _animCodeLeft, string _animCodeRight)
        {
            deflectPath = _deflectPath;
            animCodeLeft = _animCodeLeft;
            animCodeRight = _animCodeRight;
        }
        public override void Init()
        {
            var tr = controller.transform.Find(deflectPath);
            if(tr)
                area = tr.GetComponent<DeflectionArea>();
            else
                Debug.LogError("wrong deflect area path");
            if (!area)
                Debug.LogError("not find deflect");
        }

        public override bool CanEnter()
        {
            return area.GetAnim() != 0;
        }

        int anim;
        public override void InitPlayback(StateTransition transition)
        {
            anim = area.GetAnimReset();
            //controller.PlayAnimation(anim == 1 ? animCodeRight : animCodeLeft);
            controller.PlayAnimation(Random.value > 0.5f ? animCodeRight: animCodeLeft);
        }

        public override void OnAnimationBeggin(AnimatorStateInfo stateInfo)
        {
            //controller.ResetAnimation(anim == 1 ? animCodeRight : animCodeLeft);
            controller.ResetAnimation(animCodeRight);
            controller.ResetAnimation(animCodeLeft);
        }

        public override void FinishPlayback()
        {
            //controller.ResetAnimation(anim == 1 ? animCodeRight : animCodeLeft);
            controller.ResetAnimation(animCodeRight);
            controller.ResetAnimation(animCodeLeft);
        }

        public DeflectionArea area;
        public string animCodeLeft;
        public string animCodeRight;
    }

}
