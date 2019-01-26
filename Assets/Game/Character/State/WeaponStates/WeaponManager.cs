using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ReAnim;
using Character;


public class WeaponManager : MonoBehaviour
{
    CharacterStateController controller;
    WeaponBase currentWeapon;

    private void Start()
    {
        controller = GetComponent<CharacterStateController>();
    }

    [Serializable]
    public struct Weapon
    {
        [Serializable]
        public struct SpawnObject
        {
            public Transform parent;
            public GameObject prefab;
        }
        public SpawnObject[] objects;
    }
    public Weapon[] weapons;

    public void ChangeWeapon(int id)
    {
        currentWeapon.CleanUpWeapon(controller);
        currentWeapon = WeaponBase.GetNewWeapon(id);
        Debug.Assert(currentWeapon != null );
        currentWeapon.InitWeapon(controller);

        /// clean up objects on model
        /// (weapon, skill ect, any game object which differs )
        /// 



        /// spawn new objects onto model
    }
}
