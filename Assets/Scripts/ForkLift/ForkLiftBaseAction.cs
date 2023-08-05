using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ForkLiftBaseAction : MonoBehaviour
{

    protected ForkLift forkLift;

    protected virtual void Awake()
    {
        this.forkLift = GetComponent<ForkLift>();
    }

    public virtual bool CanDeactivate()
    {
        return false;
    }

    public  virtual void Deactivate()
    {
    }

    protected bool HandleMoveDestinationReached(Vector3 targetPosition)
    {
        float minDistanceDiff = 0.1f;
        if (GameUtils.Distance2d(targetPosition, this.forkLift.transform.position) < minDistanceDiff)
        {
            return true;
        }
        return false;
    }

    protected bool HandleRotationToward(Vector3 targetPosition)
    {
        float rotationSpeed = 1f;
        Vector3 targetDirection = GameUtils.Direction2d(this.forkLift.transform.position, targetPosition);
        Vector3 newDirection = Vector3.RotateTowards(this.forkLift.transform.forward, targetDirection, rotationSpeed * Time.deltaTime, 0.0f);
        this.forkLift.transform.rotation = Quaternion.LookRotation(newDirection);
        float minRotationDiff = 2f;
        if (Mathf.Abs(Vector3.SignedAngle(targetDirection, newDirection, Vector3.up)) < minRotationDiff)
        {
            this.forkLift.transform.rotation = Quaternion.LookRotation(targetDirection);
            return true;
        }
        return false;
    }

    protected bool HandleMoveToward(Vector3 targetPosition)
    {
        targetPosition.y = this.forkLift.transform.position.y;
        Vector3 newPosition = Vector3.MoveTowards(this.forkLift.transform.position, targetPosition, 1f * Time.deltaTime);
        this.forkLift.transform.position = newPosition;
        if (HandleMoveDestinationReached(targetPosition))
        {
            return true;
        }
        return false;
    }

}
