using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReAnim;

public class WallDashMarker : EnviroObcjectDashInteract
{
    public float minNormalDot;
    public float destinationOffset;
    
    public override bool CanUse(Vector2 normal, Vector2 rayDirection)
    {
        return Vector2.Dot(normal.normalized, rayDirection.normalized) > minNormalDot;
    }
    public override Vector2 GetDestinationVector(Vector2 travellerPosition, Vector2 normal)
    {
        return (Vector2)transform.position + normal * destinationOffset;
    }

    public override void InitJump(Rigidbody2D other)
    {
        other.simulated = false;
    }
    public override void UpdateJump(Transform tr, Vector2 initPosition, Vector2 finalPosition, float animationNormalizedTime)
    {
        tr.position = Vector2.Lerp(initPosition, finalPosition, animationNormalizedTime);
    }
    public override void OutJump(Rigidbody2D other)
    {
        other.simulated = true;
        //tr.position = finalPosition;
    }

}


public abstract class EnviroObcjectDashInteract : CharacterUiMarker
{
    public abstract bool CanUse(Vector2 normal, Vector2 rayDirection);
    public abstract Vector2 GetDestinationVector(Vector2 travellerPosition, Vector2 normal);

    public abstract void InitJump(Rigidbody2D other);
    public abstract void UpdateJump(Transform tr, Vector2 initPosition, Vector2 finalPosition, float animationNormalizedTime);
    public abstract void OutJump(Rigidbody2D other);
}

namespace Character
{
    public class CStateDashThroughWall : StateComponent
    {
        public CStateDashThroughWall(float _rayInitialDistance, float _rayLength, float _raySeparation, float _minPeriod, float _alphaScale)
        {
            rayInitialDistance = _rayInitialDistance;
            rayLength = _rayLength;
            raySeparation = _raySeparation;
            minPeriod = _minPeriod;
            alphaScale = _alphaScale;
        }

        public float rayInitialDistance;
        public float rayLength;
        public float raySeparation;
        /// at which part of animation (in normalized time) the character should come to destination?
        public float minPeriod;
        public float alphaScale;

        EnviroObcjectDashInteract marker;
        EnviroObcjectDashInteract lastMarker;
        Vector2 normal;

        Vector2 initPosition;
        Vector2 finalPosition;
        CharacterUiIndicator indicator;

        public override void Init()
        {
            indicator = controller.GetComponentInChildren<CharacterUiIndicator>();
        }

        public override bool CanEnter()
        {
            if(indicator.environmentIndicators[1].use && indicator.environmentIndicators[1].hit.distance < rayLength)
            {
                marker = indicator.environmentIndicators[1].hit.collider.GetComponent<WallDashMarker>();
                normal = indicator.environmentIndicators[1].hit.normal;
                return true;
            }
            return false;
        }

        bool final = false;

        public override void InitPlayback(StateTransition transition)
        {
            lastMarker = marker;
            lastMarker.InitJump(controller.GetBody());
            initPosition = controller.transform.position;
            finalPosition = lastMarker.GetDestinationVector(initPosition, normal);


            Vector2 toFinal = -(initPosition - finalPosition).normalized;
            controller.GetMovement().ApplyExternalRotationI(toFinal);
            final = false;
        }
        public override void OnAnimationUpdate(AnimatorStateInfo stateInfo)
        {
            float period = stateInfo.normalizedTime;

            if (alphaScale != 0)
                period = Mathf.Clamp01(stateInfo.normalizedTime / alphaScale);

            if (!final)
            {
                if (stateInfo.normalizedTime > minPeriod)
                {
                    final = true;
                    lastMarker.OutJump(controller.GetBody());
                    Debug.Log(stateInfo.normalizedTime);
                    controller.GetMovement().StopCurrentMovement();

                    return;
                }
                else
                {
                    Vector2 toFinal = -(initPosition - finalPosition).normalized;
                    controller.GetMovement().ApplyExternalRotationI(toFinal);
                    lastMarker.UpdateJump(controller.transform, initPosition, finalPosition, period);
                }
            }
        }
    }
}
