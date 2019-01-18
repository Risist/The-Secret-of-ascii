using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CthuluMind : MonoBehaviour {

    public GameObject aim;
    public float maxTorque;
    [Range(0.0f, 1.0f)]
    public float averageFactor = 0;
    [Range(0.0f, 1.0f)]
    public float followFactor = 0;

    public float followFactorIgnoreDifference = 5.0f;
    public float followScale = 0.1f;

    float averageTorque;
    Rigidbody2D rb;
    AiPerceptionHolder perception;
    AiFraction fraction;

    void Start ()
    {
        rb = GetComponent<Rigidbody2D>();
        perception = GetComponent<AiPerceptionHolder>();
        fraction = GetComponent<AiFraction>();
    }

    void Update()
    {
        /*var enemy = perception.SearchInMemory(fraction, AiFraction.Attitude.enemy);
        if(enemy != null)
        {
            aim = enemy.unit.gameObject;
        }*/
    }

    void FixedUpdate () {
        float followDir = 0;
        if(aim)
        {
            Vector2 toFollow = transform.position - aim.transform.position;
            followDir = Quaternion.FromToRotation(Vector2.down, toFollow).eulerAngles.z;

            if (Mathf.Abs(followDir) < followFactorIgnoreDifference)
                return;
        }

        followDir -= transform.rotation.eulerAngles.z;
        var _followFactor = aim ? followScale : 0;

        averageTorque = averageTorque * averageFactor + Random.Range(-maxTorque, maxTorque) * (1 - averageFactor);
        rb.AddTorque(followDir * _followFactor * followScale + averageTorque * (1 - _followFactor));
    }
}
