using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReAnim;
using System;

namespace Character
{

    /// component which instantly rotates character towards direction axies
    public class CStateInitDirection : StateComponent
    {
        public override void OnAnimationBeggin(AnimatorStateInfo stateInfo)
        {
            controller.GetMovement().ApplyRotationToDirection();
        }
    }

    /// component which smoothly rotates character towards direction axies
    public class CStateInitDirectionSmooth : StateComponent
    {
        public CStateInitDirectionSmooth(float _rotationSpeed, float _minimalDirectionInput = 0 ) {
            rotationSpeed = _rotationSpeed;
            period = new Period(0f,1f);
            minimalDirectionInput = _minimalDirectionInput;
        }
        public CStateInitDirectionSmooth(float _rotationSpeed, float _minimalDirectionInput, Period _period) {
            rotationSpeed = _rotationSpeed;
            period = _period;
            minimalDirectionInput = _minimalDirectionInput;
        }
        public CStateInitDirectionSmooth(float _rotationSpeed, float _minimalDirectionInput, Period _period, Period _periodTrackDir)
        {
            rotationSpeed = _rotationSpeed;
            period = _period;
            minimalDirectionInput = _minimalDirectionInput;
            periodTrackDir = _periodTrackDir;
        }
        public float rotationSpeed;
        public float minimalDirectionInput;
        public Period period;
        public Period periodTrackDir = new Period(0f,0f);
        protected float desiredRotation;
        protected bool changeRotation;
        protected Transform tr;

        public override void Init()
        {
            tr = controller.transform.Find("modelCharacter");
        }

        public override void OnAnimationBeggin(AnimatorStateInfo stateInfo)
        {
            Vector2 mouseDir = controller.GetInput().GetDirectionInput();
            float minDir = minimalDirectionInput * controller.GetInput().minimalDirectionInputStrength;
            changeRotation = mouseDir.sqrMagnitude > minDir * minDir;
            if (changeRotation)
            {
                desiredRotation = Vector2.Angle(Vector2.up, mouseDir) * (mouseDir.x > 0 ? -1 : 1);
                controller.GetInput().SetLastInput(mouseDir);
            }
        }
        public override void OnAnimationUpdate(AnimatorStateInfo stateInfo)
        {
            if (changeRotation && period.IsIn(stateInfo.normalizedTime))
            {
                if (periodTrackDir.IsIn(stateInfo.normalizedTime))
                    OnAnimationBeggin(stateInfo);

                var body = controller.GetBody();
                float trRot = tr.rotation.eulerAngles.z;
                float trDiff = body.rotation - trRot;
                body.rotation = Mathf.LerpAngle(body.rotation, desiredRotation, rotationSpeed);
            }
        }
    }

    public class CStateInitDirectionUpdate : StateComponent
    {
        public CStateInitDirectionUpdate(float _rotationSpeed, float _minimalDirectionInput = 0, float _maxRotDiff = float.PositiveInfinity)
        {
            rotationSpeed = _rotationSpeed;
            period = new Period(0f, 1f);
            minimalDirectionInput = _minimalDirectionInput;
            maxRotDiff = _maxRotDiff;
        }
        public CStateInitDirectionUpdate(float _rotationSpeed, float _minimalDirectionInput, float _maxRotDiff, Period _period)
        {
            rotationSpeed = _rotationSpeed;
            period = _period;
            minimalDirectionInput = _minimalDirectionInput;
            maxRotDiff = _maxRotDiff;
        }
        public float rotationSpeed;
        public float minimalDirectionInput;
        public float maxRotDiff;
        public Period period;

        Vector2 mouseDir;

        public override void OnAnimationUpdate(AnimatorStateInfo stateInfo)
        {
            Vector2 _mouseDir = controller.GetInput().GetDirectionInput();
            float minDir = minimalDirectionInput * controller.GetInput().minimalDirectionInputStrength;
            bool changeRotation = _mouseDir.sqrMagnitude > minDir * minDir;
            if (changeRotation)
            {
                float desiredRotation = Vector2.Angle(Vector2.up, _mouseDir) * (_mouseDir.x > 0 ? -1 : 1);
                mouseDir = _mouseDir;

                if(period.IsIn(stateInfo.normalizedTime))
                {
                    var body = controller.GetBody();

                    float angle = Mathf.LerpAngle(body.rotation, desiredRotation, rotationSpeed);
                    //float diff = Mathf.DeltaAngle(body.rotation, angle);
                    float diff = -body.rotation + angle;

                    body.rotation = body.rotation + Mathf.Clamp(diff, -maxRotDiff, maxRotDiff);
                }
            }
        }

        public override void OnAnimationEnd(AnimatorStateInfo stateInfo)
        {
            //if(mouseDir.sqrMagnitude > minimalDirectionInput*minimalDirectionInput)
                controller.GetInput().SetLastInput(mouseDir);
        }
    }
    public class CStateReflectedDirection : CStateInitDirectionSmooth
    {
        public CStateReflectedDirection(float _rayInitialDistance, float _rayLength, float _rotationSpeed, float _minimalDirectionInput = 0)
            : base(_rotationSpeed, _minimalDirectionInput)
        {
            rayLength = _rayLength;
            rayInitialDistance = _rayInitialDistance;
        }
        public CStateReflectedDirection(float _rayInitialDistance, float _rayLength, float _rotationSpeed, float _minimalDirectionInput, Period _period)
            : base(_rotationSpeed, _minimalDirectionInput, _period)
        {
            rayLength = _rayLength;
            rayInitialDistance = _rayInitialDistance;
        }
        public CStateReflectedDirection(float _rayInitialDistance, float _rayLength, float _rotationSpeed, float _minimalDirectionInput, Period _period, Period _periodTrackDir)
            : base(_rotationSpeed, _minimalDirectionInput, _period, _periodTrackDir)
        {
            rayLength = _rayLength;
            rayInitialDistance = _rayInitialDistance;
        }
        public float rayInitialDistance;
        public float rayLength;
        public float raySeparation;
        public CStateReflectedDirection SetRaySeparation(float s)
        {
            raySeparation = s;
            return this;
        }


        CharacterUiIndicator indicator;
        Vector2 lastMouseDir;

        public override void Init()
        {
            base.Init();
            indicator = controller.GetComponentInChildren<CharacterUiIndicator>();
            Debug.Assert(indicator);
        }

        public override bool CanEnter()
        {
            return indicator.environmentIndicators[0].use && indicator.environmentIndicators[0].rayDistance < rayLength;
        }
        public override void OnAnimationBeggin(AnimatorStateInfo stateInfo)
        {
            Vector2 mouseDir = -indicator.environmentIndicators[0].hit.normal;//lastMouseDir - 2*Vector2.Dot(lastMouseDir, lastHit.normal) * lastHit.normal;
            float minDir = minimalDirectionInput * controller.GetInput().minimalDirectionInputStrength;
            changeRotation = mouseDir.sqrMagnitude > minDir * minDir;
            if (changeRotation)
            {
                desiredRotation = Vector2.Angle(Vector2.up, mouseDir) * (mouseDir.x > 0 ? -1 : 1);
                controller.GetInput().SetLastInput(mouseDir);
            }
        }
    }
    /*public class CStateReflectedDirection : CStateInitDirectionSmooth
    {
        public CStateReflectedDirection(float _rayInitialDistance, float _rayLength, float _rotationSpeed, float _minimalDirectionInput = 0)
            : base( _rotationSpeed, _minimalDirectionInput)
        {
            rayLength = _rayLength;
            rayInitialDistance = _rayInitialDistance;
        }
        public CStateReflectedDirection(float _rayInitialDistance, float _rayLength, float _rotationSpeed, float _minimalDirectionInput, Period _period)
            : base(_rotationSpeed, _minimalDirectionInput, _period)
        {
            rayLength = _rayLength;
            rayInitialDistance = _rayInitialDistance;
        }
        public CStateReflectedDirection(float _rayInitialDistance, float _rayLength, float _rotationSpeed, float _minimalDirectionInput, Period _period, Period _periodTrackDir)
            : base(_rotationSpeed, _minimalDirectionInput, _period, _periodTrackDir)
        {
            rayLength = _rayLength;
            rayInitialDistance = _rayInitialDistance;
        }
        public float rayInitialDistance;
        public float rayLength;
        public float raySeparation;
        public CStateReflectedDirection SetRaySeparation(float s)
        {
            raySeparation = s;
            return this;    
        }


        RaycastHit2D lastHit;
        Vector2 lastMouseDir;
        public override bool CanEnter()
        {
            Vector2 mouseDir = controller.GetInput().GetDirectionInput().normalized;
            RaycastHit2D hit = Physics2D.Raycast(
                (Vector2)controller.transform.position + mouseDir*rayInitialDistance,
                mouseDir, rayLength
            );

            if(hit.collider&& !hit.collider.isTrigger)
            {
                Debug.Assert(hit.collider.gameObject != controller.gameObject);
                lastMouseDir = -mouseDir;
                lastHit = hit;
                return true;
            }

            hit = Physics2D.Raycast(
                (Vector2)controller.transform.position + mouseDir * rayInitialDistance + new Vector2(-mouseDir.y, mouseDir.x)*raySeparation,
                mouseDir, rayLength
            );

            if (hit.collider && !hit.collider.isTrigger)
            {
                Debug.Assert(hit.collider.gameObject != controller.gameObject);
                lastMouseDir = -mouseDir;
                lastHit = hit;
                return true;
            }

            hit = Physics2D.Raycast(
                (Vector2)controller.transform.position + mouseDir * rayInitialDistance - new Vector2(-mouseDir.y, mouseDir.x) * raySeparation,
                mouseDir, rayLength
            );

            if (hit.collider && !hit.collider.isTrigger)
            {
                Debug.Assert(hit.collider.gameObject != controller.gameObject);
                lastMouseDir = -mouseDir;
                lastHit = hit;
                return true;
            }

            return false;
        }
        public override void OnAnimationBeggin(AnimatorStateInfo stateInfo)
        {
            Vector2 mouseDir = -lastHit.normal;//lastMouseDir - 2*Vector2.Dot(lastMouseDir, lastHit.normal) * lastHit.normal;
            float minDir = minimalDirectionInput * controller.GetInput().minimalDirectionInputStrength;
            changeRotation = mouseDir.sqrMagnitude > minDir * minDir;
            if (changeRotation)
            {
                desiredRotation = Vector2.Angle(Vector2.up, mouseDir) * (mouseDir.x > 0 ? -1 : 1);
                controller.GetInput().SetLastInput(mouseDir);
            }
        }
    }*/



    /// detects if an colision is in front of the character
    public class CStateCollisionInFront : StateComponent
    {
        public CStateCollisionInFront(float _radius, Vector2 _offset)
        {
            radius = _radius;
            offset = _offset;
        }
        // radius of a trigger circle
        public float radius;
        // offset in local coordinates
        public Vector2 offset;

        public override bool CanEnter()
        {
            Vector2 point = controller.transform.position;
            point += (Vector2)controller.transform.up * offset.y;
            point += (Vector2)controller.transform.right * offset.x;
            var c = Physics2D.OverlapCircleAll(point, radius);

            foreach (var it in c)
                if (!it.isTrigger && it.gameObject != controller.gameObject)
                    return true;
            return false;
            /*Debug.DrawLine(point, point + (Vector2)controller.transform.up * radius, Color.red, 0.1f);
            Debug.DrawLine(point, point - (Vector2)controller.transform.up * radius, Color.red, 0.1f);

            Debug.DrawLine(point, point + (Vector2)controller.transform.right * radius, Color.red, 0.1f);
            Debug.DrawLine(point, point - (Vector2)controller.transform.right * radius, Color.red, 0.1f);*/
            //return c && !c.isTrigger && c.gameObject != controller.gameObject;
        }
    }

    public class CStateEnergy : StateComponent
    {
        public enum Mode
        {
            EAll,
            ECheckOnly,
            EConsumeOnly
        }
        public int resourceId;
        public Mode mode;
        public float cost;

        public CStateEnergy(int _resorceId, float _cost, Mode _mode = Mode.EAll)
        {
            resourceId = _resorceId;
            cost = _cost;
            mode = _mode;
        }

        public override bool CanEnter()
        {
            return mode == Mode.EConsumeOnly || controller.resources[resourceId].HasEnough(cost);
        }

        public override void InitPlayback(StateTransition transition)
        {
            if (mode == Mode.EConsumeOnly)
                controller.resources[resourceId].Spend(cost);
            if (mode != Mode.ECheckOnly)
                controller.resources[resourceId].Spend(cost);
        }
    }
}


