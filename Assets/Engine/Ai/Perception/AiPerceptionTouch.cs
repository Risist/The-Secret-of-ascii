using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiPerceptionTouch : AiPerceptionBase
{
    public Timer tInsert;
    public float minimalSpeed;
    public float predictionScale = 1f;

    private void Start()
    {
        var rb = GetComponentInParent<Rigidbody2D>();
        if (rb)
        {
            var obj = rb.gameObject.AddComponent<AiPerceptionTouchReal>();
            obj.tInsert = tInsert;
            obj.minimalSpeed = minimalSpeed;
            obj.memoryTime = memoryTime;
            obj.shadeTime = shadeTime;
            obj.matureTime = matureTime;
            obj.predictionScale = predictionScale;
        }
    }
}

public class AiPerceptionTouchReal : AiPerceptionBase
{
    public Timer tInsert;
    public float minimalSpeed;
    public float predictionScale = 1f;

    private void Start()
    {
        holder = GetComponentInChildren<AiPerceptionHolder>();
        myUnit = GetComponent<AiPerceiveUnit>();
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (!tInsert.IsReady() || !collision.rigidbody)
            return;

        if (collision.relativeVelocity.sqrMagnitude >= minimalSpeed * minimalSpeed)
        {
            holder.InsertToMemory(EMemoryEvent.ENoise_Touch, collision.rigidbody.position, -collision.relativeVelocity, memoryTime, matureTime, shadeTime, priority);
            tInsert.Restart();
        }
    }
}