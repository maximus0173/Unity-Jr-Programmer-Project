using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ForkliftBaseAction : MonoBehaviour
{

    protected Forklift forklift;

    public event System.EventHandler OnCompleteAction;

    protected virtual void Awake()
    {
        this.forklift = GetComponent<Forklift>();
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
        if (GameUtils.Distance2d(targetPosition, this.forklift.transform.position) < minDistanceDiff)
        {
            return true;
        }
        return false;
    }

    protected bool HandleRotationToward(Vector3 targetPosition)
    {
        float rotationSpeed = 1f;
        Vector3 targetDirection = GameUtils.Direction2d(this.forklift.transform.position, targetPosition);
        Vector3 newDirection = Vector3.RotateTowards(this.forklift.transform.forward, targetDirection, rotationSpeed * Time.deltaTime, 0.0f);
        this.forklift.transform.rotation = Quaternion.LookRotation(newDirection);
        float minRotationDiff = 2f;
        if (Mathf.Abs(Vector3.SignedAngle(targetDirection, newDirection, Vector3.up)) < minRotationDiff)
        {
            this.forklift.transform.rotation = Quaternion.LookRotation(targetDirection);
            return true;
        }
        return false;
    }

    protected bool HandleMoveToward(Vector3 targetPosition)
    {
        targetPosition.y = this.forklift.transform.position.y;
        Vector3 newPosition = Vector3.MoveTowards(this.forklift.transform.position, targetPosition, 1f * Time.deltaTime);
        this.forklift.transform.position = newPosition;
        if (HandleMoveDestinationReached(targetPosition))
        {
            return true;
        }
        return false;
    }

    protected void CompleteAction()
    {
        this.OnCompleteAction?.Invoke(this, System.EventArgs.Empty);
    }

}
