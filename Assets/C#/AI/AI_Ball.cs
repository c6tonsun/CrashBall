using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AI_Ball : MonoBehaviour {

    Rigidbody BallRigidBody;

    private AI_KartMover kartMover;

    public LayerMask layerMask;

    [SerializeField]
    private Collider currentTargetGoal;

    public float TimeToGoal;
    public float GoalHitPoint;
    public int GoalTarget;

    private RaycastHit rayHit;


	// Use this for initialization
	void Start () {
        BallRigidBody = GetComponent<Rigidbody>();
        kartMover = FindObjectOfType<AI_KartMover>();
	}

    private void FixedUpdate()
    {
        Gizmos.color = Color.cyan;
        float MaxSpeed = BallRigidBody.velocity.magnitude * 5f;
        RaycastHit hit;
        float hitOneDistance = 0;
        float hitTwoDistance = 0;
        Physics.SphereCast(BallRigidBody.position, 0.7f, BallRigidBody.velocity, out hit, MaxSpeed, layerMask, QueryTriggerInteraction.Collide);
        Collider rayTarget = hit.collider;
        hitOneDistance = hit.distance;
        if (rayTarget != null)
        {
            if (rayTarget.GetComponent<AI_GoalData>() && rayTarget.isTrigger)
            {
                currentTargetGoal = rayTarget;
                rayHit = hit;
                TimeToGoal = hitOneDistance / BallRigidBody.velocity.magnitude;
            }
            else
            {
                Vector3 reflectedDir = Vector3.Reflect(BallRigidBody.velocity.normalized, hit.normal);
                reflectedDir.y = 0;
                Vector3 reflectpoint = hit.point;
                Physics.Raycast(reflectpoint, reflectedDir, out hit, 200, layerMask, QueryTriggerInteraction.Collide);
                hitTwoDistance = hit.distance;

                rayTarget = hit.collider;
                if (rayTarget != null && rayTarget.GetComponent<AI_GoalData>() && rayTarget.isTrigger)
                {
                    Gizmos.color = Color.magenta;
                    currentTargetGoal = rayTarget;
                    rayHit = hit;
                    TimeToGoal = (hitOneDistance + hitTwoDistance) / BallRigidBody.velocity.magnitude;
                }
                if (rayTarget == null || !rayTarget.GetComponent<AI_GoalData>())
                {
                    currentTargetGoal = null;
                    TimeToGoal = 99;
                }
            }
        }

        if (currentTargetGoal)
        {
            GoalTarget = (int)currentTargetGoal.GetComponent<AI_GoalData>().currentPlayer;
            GoalHitPoint = kartMover.CalculateGoalHitPoint(rayHit.point, GoalTarget);
        }
        else
        {
            GoalTarget = -1;
            GoalHitPoint = -1;
        }
    }

    private void OnDrawGizmos()
    {
        if (currentTargetGoal)
        {
            Gizmos.DrawSphere(rayHit.point, 0.5f);
        }
    }
}
