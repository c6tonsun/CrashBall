using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunBall : Ball {

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
