using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public Transform player;

    [Range(0.0f,10.0f)]
	public float learpFactor = 10.0f;
    [Range(0.0f, 1.0f)]
    public float mouseLerpFactor = 1.0f;

    public float maxMouseOffset = 3.0f;

    Vector3 initialOffsetPosition;
	float initialOffsetRotation;
	float initialOffsetScale;

    Vector3 lastMousePosition;


	// Use this for initialization
	void Start()
    {
        initialOffsetPosition = transform.position - player.position;
		initialOffsetRotation = transform.rotation.eulerAngles.z;
		initialOffsetScale = Camera.main.orthographicSize;
	}

    // Update is called once per frame
    void LateUpdate()
    {
        float offsetScale = initialOffsetScale + shakeScaleInfluence;

        if (player)
		{
            lastMousePosition = Vector3.Lerp( lastMousePosition, Camera.main.ScreenToWorldPoint(Input.mousePosition), mouseLerpFactor);

            Vector3 v = (lastMousePosition - player.position);
            v.y /= Camera.main.aspect;
            float vLength = v.magnitude;
            v /= vLength;
            vLength = Mathf.Clamp(vLength, 0.0f, maxMouseOffset);
            //lastMousePosition = player.position + v;
            v.y *= Camera.main.aspect;
            Vector3 middlePos = player.position + v*vLength;


            transform.position = transform.position + ( middlePos - transform.position) * learpFactor * Time.deltaTime
				+ (Vector3)shakePositionInfluence;
			transform.position = new Vector3(transform.position.x, transform.position.y, initialOffsetPosition.z);
		}

		transform.rotation = Quaternion.Euler(0, 0, initialOffsetRotation + shakeRotationInfluence);
		Camera.main.orthographicSize = offsetScale;
	}
	private void FixedUpdate()
	{
		shakePositionInfluence *= shakePositionDamping;
		shakeRotationInfluence *= shakeRotationDamping;
		shakeScaleInfluence *= shakeScaleDamping;
	}


	/// Screan shakes
	/// 
	[Range(0.0f, 1.0f)]
	public float shakePositionDamping;
	Vector2 shakePositionInfluence;
	public void shakePosition(Vector2 shakePower)
	{
	    shakePositionInfluence += shakePower;
	}

	[Range(0.0f, 1.0f)]
	public float shakeRotationDamping;
	float shakeRotationInfluence;
	public void shakeRotation(float shakePower)
	{
        shakeRotationInfluence += shakePower;
	}

	[Range(0.0f, 1.0f)]
	public float shakeScaleDamping;
	float shakeScaleInfluence;
	public void shakeScale(float shakePower)
	{
		shakeScaleInfluence += shakePower;
	}


}

