using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReAnim;
using System;

namespace Character
{

    /// Applies force to character (aka dash)
    public class CStateMotor : StateComponent
    {
        public CStateMotor(Vector2 _force, float _ignoreDirectionRotation) { force = _force; ignoreDirectionRotation = _ignoreDirectionRotation; }
        public CStateMotor(Vector2 _force, float _ignoreDirectionRotation, Period _period) { force = _force; period = _period; ignoreDirectionRotation = _ignoreDirectionRotation; }
        public Period period = new Period(0f,1f);
        public Vector2 force;
        public float ignoreDirectionRotation;
        Vector2 directionUp;
        Vector2 directionRight;

        public override void OnAnimationBeggin(AnimatorStateInfo stateInfo)
        {
            if (controller.GetInput().GetDirectionInput().sqrMagnitude < ignoreDirectionRotation * ignoreDirectionRotation)
            {
                directionUp = controller.transform.up;
                directionRight = controller.transform.right;
            }else
            {
                directionUp = controller.GetInput().GetDirectionInput().normalized;
                directionRight = new Vector2(-directionUp.y, directionUp.x);
            }
        }
        public override void OnAnimationUpdate(AnimatorStateInfo stateInfo)
        {
            if (state == controller.GetCurrentState())
            {
                if (period.IsIn(stateInfo.normalizedTime))
                  
                    controller.GetBody().AddForce(
                        directionUp * force.y +
                        directionRight * force.x
                    );
            }
        }

    }

    /// prevents character from rotation towards direction axies during playback
    public class CStateBlockRotation : StateComponent
    {
        public CStateBlockRotation( float _maxTime = 1.0f)
        {
            maxTime = _maxTime;
        }
        public float maxTime;
        public override void OnAnimationBeggin(AnimatorStateInfo stateInfo)
        {
            controller.GetMovement().rotateToDirection = false;
        }
        public override void OnAnimationUpdate(AnimatorStateInfo stateInfo)
        {
            if (stateInfo.normalizedTime > maxTime)
                controller.GetMovement().rotateToDirection = true; 
        }

    }
    /// prevents character from moving towards direction axies during playback
    public class CStateBlockMovement : StateComponent
    {
        public CStateBlockMovement(float _maxTime = 1.0f)
        {
            maxTime = _maxTime;
        }
        public float maxTime;
        public override void OnAnimationBeggin(AnimatorStateInfo stateInfo)
        {
            controller.GetMovement().moveToDirection = false;
        }
        public override void OnAnimationUpdate(AnimatorStateInfo stateInfo)
        {
            if (stateInfo.normalizedTime > maxTime)
                controller.GetMovement().moveToDirection = true;
        }

    }

    /// Spawns prefab
    public class CStateSpawn : StateComponent
    {
        public CStateSpawn(int _prefabId, Vector2 _positionOffset, float _rotationOffset = 0)
        {
            prefabId = _prefabId;
            positionOffset = _positionOffset;
            rotationOffset = _rotationOffset;
        }
        public CStateSpawn(int _prefabId, float _rotationOffset = 0)
        {
            prefabId = _prefabId;
            positionOffset = Vector2.zero;
            rotationOffset = _rotationOffset;
        }
        public CStateSpawn(int _prefabId, Period _spawnPeriod, Vector2 _positionOffset, float _rotationOffset = 0)
        {
            spawnPeriod = _spawnPeriod;
            prefabId = _prefabId;
            positionOffset = _positionOffset;
            rotationOffset = _rotationOffset;
        }
        public CStateSpawn(int _prefabId, Period _spawnPeriod, float _rotationOffset = 0)
        {
            prefabId = _prefabId;
            positionOffset = Vector2.zero;
            rotationOffset = _rotationOffset;
            spawnPeriod = _spawnPeriod;
        }
        public int prefabId;
        public Period spawnPeriod = new Period(0f, 1f);
        public Vector2 positionOffset;
        public float rotationOffset;
        bool toSpawn = false;

        public override void OnAnimationBeggin(AnimatorStateInfo stateInfo)
        {
            toSpawn = true;
        }
        public override void OnAnimationUpdate(AnimatorStateInfo stateInfo)
        {
            if (toSpawn && stateInfo.normalizedTime >= spawnPeriod.min && stateInfo.normalizedTime <= spawnPeriod.max)//&& spawnPeriod.IsIn(stateInfo.normalizedTime))
            {
                toSpawn = false;
                controller.SpawnPrefab(prefabId, positionOffset, rotationOffset);
            }
        }
    }
}