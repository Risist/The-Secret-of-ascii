using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReAnim;
using System;

public class BowArrowLoadManager : MonoBehaviour {

    /// assume there are max 3 arrows
    public Transform[] arrowPositions;
    public GameObject bullet;
    public Transform[] spawnPositions;

    public bool HasAmmo()
    {
        foreach (var it in arrowPositions)
            if (it.gameObject.activeInHierarchy)
                return true;
        return false;
    }
    public bool CanAddAmmo()
    {
        foreach (var it in arrowPositions)
            if (!it.gameObject.activeInHierarchy)
                return true;
        return false;
    }
    public void AddAmmmo()
    {
        if (!CanAddAmmo())
            return;

        if (!arrowPositions[0].gameObject.activeInHierarchy)
            arrowPositions[0].gameObject.SetActive(true);
        else
        {
            arrowPositions[0].gameObject.SetActive(false);
            arrowPositions[1].gameObject.SetActive(true);
            arrowPositions[2].gameObject.SetActive(true);
        }
    }
    public void UseAmmmo()
    {
        if (!HasAmmo())
            return;
        
        arrowPositions[0].gameObject.SetActive(false);
        arrowPositions[1].gameObject.SetActive(false);
        arrowPositions[2].gameObject.SetActive(false);
    }

    public void SpawnBullets(float normalizedTime)
    {
        /// normalTime to use later on for increasing efectiveness of shoot

        for (int i = 0; i < 3; ++i)
        {
            if (arrowPositions[i].gameObject.activeInHierarchy)
            {
                Instantiate(bullet, spawnPositions[i].position, spawnPositions[i].rotation);
            }
        }
    }
}

namespace Character
{
    public class CStateLoadArrow : StateComponent
    {
        public CStateLoadArrow(float _loadTime)
        {
            loadTime = _loadTime;
        }
        public float loadTime;
        BowArrowLoadManager manager;
        bool added = true;

        public override void Init()
        {
            manager = controller.GetComponentInChildren<BowArrowLoadManager>();
        }
        
        public override bool CanEnter()
        {
            return manager.CanAddAmmo();
        }
        public override void InitPlayback(StateTransition transition)
        {
            added = false;
        }
        public override void OnAnimationUpdate(AnimatorStateInfo stateInfo)
        {
            if(!added && stateInfo.normalizedTime > loadTime)
            {
                manager.AddAmmmo();
                added = true;
            }
        }
    }

    public class CStateUseArrows : CStateCanUseArrows
    {
        public CStateUseArrows(float _loadTime)
        {
            loadTime = _loadTime;
        }
        public float loadTime;
        bool used = true;
        
        public override void InitPlayback(StateTransition transition)
        {
            used = false;
        }
        public override void OnAnimationUpdate(AnimatorStateInfo stateInfo)
        {
            if (!used && stateInfo.normalizedTime > loadTime)
            {
                manager.SpawnBullets(stateInfo.normalizedTime);
                manager.UseAmmmo();
                used = true;
            }
        }
    }

    public class CStateCanUseArrows : StateComponent
    {
        protected BowArrowLoadManager manager;
        
        public override bool CanEnter()
        {
            return manager.HasAmmo();
        }
        public override void Init()
        {
            manager = controller.GetComponentInChildren<BowArrowLoadManager>();
        }
    }
}
