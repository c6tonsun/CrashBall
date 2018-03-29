using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunBall : Ball {

    private void Start()
    {
        canBeMagneted = false;
    }
    
    protected new void FixedUpdate() 
    {
        base.FixedUpdate();
        Rb.velocity = Rb.velocity.normalized * minSpeed * 2;
    }    
    protected new void OnCollisionStay(Collision collision)
    {
        base.OnCollisionStay(collision);
        Player player = collision.collider.GetComponentInParent<Player>();
        if (player == null)
            return;

        if (player.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            player.movement.stunTimer = 0f;
        }
    }
}
