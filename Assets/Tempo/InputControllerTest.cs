using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputControllerTest : InputManagerBase {

    public bool[] input;
    float[] chances;
    public Vector2 positionInput;
    public Vector2 directionInput;

    public float followPositionPlayer;
    public float followDirectionPlayer;

    public override Vector2 GetPositionInput() { return followPositionPlayer > 0.2f ? positionInput : Vector2.zero; }

    public Transform player;
    AiPerceptionHolder perception;
    AiFraction fraction;
    private void Start()
    {
        perception = GetComponent<AiPerceptionHolder>();
        fraction = GetComponentInParent<AiFraction>();

        chances = new float[] {
            Random.Range(0.05f, 0.45f) * Random.Range(0.05f, 0.45f) ,
            Random.Range(0.05f, 0.45f) * Random.Range(0.05f, 0.45f) ,
            Random.Range(0.05f, 0.45f) * Random.Range(0.05f, 0.45f) ,
            Random.Range(0.05f, 0.45f) * Random.Range(0.05f, 0.45f) 
        };
        //chances = new float[] { 0.5f, 0.5f, 0.5f, 0.5f };
        //chances = new float[] { 0.6f, 0.9f, 0.6f, 0.75f };
        //chances = new float[] { 0.95f, 0.5f, 0.85f, 0.35f };
        Debug.Log(chances[0] + ", " + chances[1] + ", " + chances[2] + "," + chances[3]);

    }

    new private void Update()
    {
        base.Update();
        followDirectionPlayer *= 0.75f;
        followPositionPlayer *= 0.75f;

        if (!perception)
        {
            Debug.LogError("No PerceptionHolder in : " + gameObject);
            return;
        }

        var it = perception.SearchInMemory(EMemoryEvent.EEnemy);
        if (it == null )
            return;

        followDirectionPlayer += Random.Range(0f, 1f) * 0.15f;
        followPositionPlayer += Random.Range(0f, 1f) * 0.15f;

        directionInput = (it.position - (Vector2)transform.position) * followDirectionPlayer;
        positionInput  = (it.position - (Vector2)transform.position) * followPositionPlayer;
    }

    /// returns mouse position input (e.g. the direction character is targeting to, the direction character will shoot or strike)
    public override Vector2 GetDirectionInput()
    {
        return directionInput;
    }

    public override bool IsInputPressed(int id)
    {
        return input[id] && Random.value < chances[id] && followDirectionPlayer > 0.25f;
    }
    public override bool IsInputDown(int id)
    {
        return input[id] && Random.value < chances[id] && followDirectionPlayer > 0.25f;
    }
    public override bool IsInputUp(int id)
    {
        return input[id] && Random.value < chances[id] && followDirectionPlayer > 0.25f;
    }
}
