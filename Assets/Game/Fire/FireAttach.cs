using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireAttach : MonoBehaviour {

    [Range(0.0f, 1.0f)]
    /// chance to spread a fire
    public float spreadChance = 1.0f;
    [Range(0.0f, 1.0f)]
    /// if spread chance is lower than the value the object will be removed
    public float minChance = 0.15f;

    [Range(0.0f, 1.0f)]
    public float chanceDamping = 0.95f;

    [Space]
    public GameObject spreadPrefab;
    public float refreshSpreadChance = 0.125f;
    public float myRefreash = 0.125f;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Random.value > spreadChance || !enabled)
            return;

        var hp = collision.gameObject.GetComponent<HealthController>();
        if (!hp)
            return;

        refresh(myRefreash);

        var attach = collision.gameObject.GetComponentInChildren<FireAttach>();
        if (attach)
            attach.refresh(refreshSpreadChance);
        else
        {
            Instantiate(spreadPrefab, collision.transform);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (Random.value > spreadChance || ! enabled)
            return;

        var hp = collision.gameObject.GetComponent<HealthController>();
        if (!hp)
            return;

        refresh(myRefreash);

        var attach = collision.gameObject.GetComponentInChildren<FireAttach>();
        if (attach)
            attach.refresh(refreshSpreadChance);
        else
        {
            Instantiate(spreadPrefab, collision.transform);
        }
    }

    private void FixedUpdate()
    {
        spreadChance *= chanceDamping;
        if (spreadChance < minChance)
            Destroy(gameObject);
    }


    public void refresh(float v)
    {
        spreadChance += v;
    }

}
