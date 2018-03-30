using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunBall : Ball {

    private void Start()
    {
        canBeMagneted = false;
    }
    
    protected void FixedUpdate() 
    {
        base.FixedUpdate();
        Rb.velocity = Rb.velocity.normalized * minSpeed * 2;
    }

    private void OnCollisionEnter(Collision collision)
    {
        StunPlayer(collision.collider.GetComponentInParent<Player>());
    }

    protected new void OnCollisionStay(Collision collision)
    {
        base.OnCollisionStay(collision);
        StunPlayer(collision.collider.GetComponentInParent<Player>());
    }

    private void StunPlayer(Player player)
    {
        if (player == null)
            return;

        if (player.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            player.movement.stunTimer = 0f;
        }
    }
}
