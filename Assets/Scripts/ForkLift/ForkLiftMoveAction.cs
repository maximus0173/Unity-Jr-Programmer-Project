using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ForkliftMoveAction : ForkliftBaseAction
{

    private NavMeshAgent agent;
    private Vector3? targetPosition = null;

    protected override void Awake()
    {
        base.Awake();
        this.agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        HandleMoveToTarget();
    }

    private void HandleMoveToTarget()
    {
        if (this.targetPosition == null)
        {
            return;
        }
        float minDistanceDiff = 0.1f;
        if (Vector3.Distance((Vector3)this.targetPosition, transform.position) < minDistanceDiff)
        {
            this.targetPosition = null;
        }
    }

    public void MoveToPosition(Vector3 position)
    {
        Vector3 targetPosition = position;
        this.targetPosition = targetPosition;
        this.agent.SetDestination(targetPosition);
    }

    public override bool CanDeactivate()
    {
        return true;
    }

    public override void Deactivate()
    {
        if (!CanDeactivate())
        {
            return;
        }
        this.targetPosition = null;
        this.agent.SetDestination(transform.position);
    }

}
