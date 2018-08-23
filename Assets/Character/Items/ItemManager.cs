using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "URe/Item", order = 1)]
public class Item : ScriptableObject
{
    public GameObject prefab;
    public string itemName;
    public string description;
}

public class ItemManager : MonoBehaviour {
	



}
