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

        destroyAfter.restart();
        body.AddForce(transform.up * initialForce);
    }

    private void Update()
    {
        if (turnOnCollisionAfter.isReady())
        {
            GetComponent<PointEffector2D>().enabled = true;
        }
        if (destroyAfter.isReady())
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        {/// Rotate towards direction of movement
            Vector2 velocity = body.velocity;
            float angle = Vector2.Angle(Vector2.up, velocity) * (velocity.x > 0 ? -1 : 1);
            body.rotation = angle;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        {/// Rotate towards direction of movement
            Vector2 velocity = body.velocity;
            float angle = Vector2.Angle(Vector2.up, velocity) * (velocity.x > 0 ? -1 : 1);
            body.rotation = angle;
        }


        HealthController healthController = collision.gameObject.GetComponent<HealthController>();
        if (healthController != null)
        {
            healthController.DealDamage(damageDealed, null);
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
