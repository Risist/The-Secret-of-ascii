using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReAnim;
using System;

namespace Character
{
    /// TODO component for resource drain & requirements

    /// TODO component midify animation play speed relative to given resource state
    public class CStateReduceAnimatorSpeed : StateComponent
    {
        public int resourceId;
        public float energySpeedScale;
    }

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
        float desiredRotation;
        bool changeRotation;

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
                body.rotation = Mathf.LerpAngle(body.rotation, desiredRotation, rotationSpeed);
            }
        }
    }

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
}


