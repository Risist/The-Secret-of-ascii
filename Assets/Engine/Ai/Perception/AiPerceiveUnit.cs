using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Only GameObjects with this component can be perceived by Ai
 * composes all the data agents need to react to the perceived object
 */
public class AiPerceiveUnit : MonoBehaviour
{
	/// <summary>
	/// modifies how far the agents will perceive this unit
	/// </summary>
	public float distanceModificator = 1.0f;
    public float transparencyLevel = 1.0f;
	public bool blocksVision = true;
    public bool memoriable = true;

	/// <summary>
	///  references to useful data
	/// </summary>
	public AiFraction fraction;

	protected void Start()
	{
		if (!fraction)
			fraction = GetComponent<AiFraction>();
	}
}
