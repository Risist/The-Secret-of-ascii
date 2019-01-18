using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BulletController : MonoBehaviour {


    public float initialForce;
    public float force;
    [Range(0f,1f)] public float forceFallof = 1f;
    public float forceMinimal = 1f;
    public float damageDealed = -30f;
    public float painDealed = -30f;
    public bool removeOnHit;

    [Space]
    public Vector2 destructionOffset;
    public float destructionForce = -5f;

    [Space]
    public Timer destroyAfter;
    public Timer turnOnCollisionAfter;
    Rigidbody2D body;

	void Start () {
        body = GetComponent<Rigidbody2D>();
        body.constraints = RigidbodyConstraints2D.FreezeRotation;

        destroyAfter.Restart();
        body.AddForce(transform.up * initialForce);
    }

    private void Update()
    {
        /*if (turnOnCollisionAfter.IsReady())
        {
            GetComponent<PointEffector2D>().enabled = true;
        }*/
        if (destroyAfter.IsReady())
        {
            GetComponent<DeathEventPhysicsDestruction>().AlterDamageAccumulator(destructionForce);
            GetComponent<DeathEventPhysicsDestruction>().OnDeath(transform.position +
                transform.up * destructionOffset.y +
                transform.right * destructionOffset.x);
            Destroy(gameObject);
        }
    }



    private void FixedUpdate()
    {
        if (force > forceMinimal)
        {
            body.AddForce(transform.up * force);
            force *= forceFallof;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        {/// Rotate towards direction of movement
            Vector2 velocity = body.velocity;
            float angle = Vector2.SignedAngle(Vector2.up, velocity);
            body.rotation = angle;
        }


        HealthController healthController = collision.gameObject.GetComponent<HealthController>();
        if (healthController != null)
        {
            HealthController.DamageData damageData = new HealthController.DamageData();
            damageData.causer = gameObject;
            damageData.damage = damageDealed;
            damageData.pain = painDealed;
            damageData.position = transform.position;

            healthController.DealDamage(damageData);

            if (removeOnHit)
            {
                if (!collision.isTrigger)
                {
                    GetComponent<DeathEventPhysicsDestruction>().AlterDamageAccumulator(destructionForce);
                    GetComponent<DeathEventPhysicsDestruction>().OnDeath(transform.position  + 
                        transform.up    * destructionOffset.y + 
                        transform.right * destructionOffset.x);
                    Destroy(gameObject);
                }
            }
        }
    }
}
