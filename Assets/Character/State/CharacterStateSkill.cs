using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReAnim;
using System;

namespace Character
{

    public class CStateInitDirection : StateComponent
    {
        public override void OnAnimationBeggin(AnimatorStateInfo stateInfo)
        {
            controller.GetMovement().ApplyRotationToDirection();
        }
    }
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
        public float rotationSpeed;
        public float minimalDirectionInput;
        public Period period;
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
                var body = controller.GetBody();
                body.rotation = Mathf.LerpAngle(body.rotation, desiredRotation, rotationSpeed);
            }
        }
    }
}


