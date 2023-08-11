using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ForkliftUnloadPaletteOnTruckAction : ForkliftBaseAction
{

    private ForkliftFork fork;
    private NavMeshAgent agent;

    private ITruck targetTruck = null;
    private Truck.ForkliftUnloadPositions? targetTruckUnloadPositions;

    private enum State
    {
        None,
        LongApproach,
        RotationAdjust,
        NearApproach,
        Unloading,
        Withdrawal
    }

    private State state = State.None;

    protected override void Awake()
    {
        base.Awake();
        this.fork = this.forklift.GetComponentInChildren<ForkliftFork>();
        this.agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (this.state != State.None)
        {
            switch (this.state)
            {
                case State.LongApproach:
                    HandleLongApproachState();
                    break;
                case State.RotationAdjust:
                    HandleRotationAdjustState();
                    break;
                case State.NearApproach:
                    HandleNearApproachState();
                    break;
                case State.Unloading:
                    HandleUnloadingState();
                    break;
                case State.Withdrawal:
                    HandleWithdrawalState();
                    break;
            }
        }
    }

    private void HandleLongApproachState()
    {
        if (this.targetTruckUnloadPositions == null)
        {
            return;
        }
        Vector3 targetPosition = ((Truck.ForkliftUnloadPositions)this.targetTruckUnloadPositions).approachPosition;
        if (HandleMoveToward(targetPosition))
        {
            this.agent.enabled = false;
            this.state = State.RotationAdjust;
        }
    }

    private void HandleRotationAdjustState()
    {
        if (this.targetTruckUnloadPositions == null)
        {
            return;
        }
        Vector3 targetPosition = ((Truck.ForkliftUnloadPositions)this.targetTruckUnloadPositions).palettePosition;
        if (HandleRotationToward(targetPosition))
        {
            this.state = State.NearApproach;
        }
    }

    private void HandleNearApproachState()
    {
        if (this.targetTruckUnloadPositions == null)
        {
            return;
        }
        Vector3 targetPosition = ((Truck.ForkliftUnloadPositions)this.targetTruckUnloadPositions).palettePosition;
        if (HandleMoveToward(targetPosition))
        {
            this.state = State.Unloading;
        }
    }

    private void HandleUnloadingState()
    {
        this.fork.AdjustForkHeightToUnload();
        if (this.fork.ForkHeight < 0.1f)
        {
            GameManager.Instance.UnloadPaletteFromForkliftToTruck(this.forklift, this.targetTruck);
            this.fork.UnloadPalette();
            this.state = State.Withdrawal;
        }
    }

    private void HandleWithdrawalState()
    {
        if (this.targetTruckUnloadPositions == null)
        {
            return;
        }
        Vector3 targetPosition = ((Truck.ForkliftUnloadPositions)this.targetTruckUnloadPositions).approachPosition;
        if (HandleMoveToward(targetPosition))
        {
            this.state = State.None;
            this.agent.enabled = true;
            GameManager.Instance.CompleteReserveationOfTruckToUnloadPaletteFromForklift(this.forklift, this.targetTruck);
            this.targetTruck = null;
            this.targetTruckUnloadPositions = null;
            CompleteAction();
        }
    }

    public void UnloadPaletteOnTruck(ITruck truck)
    {
        Truck.ForkliftUnloadPositions? truckUnloadPositions = truck.GetForkliftUnloadPositions(this.forklift);
        if (truckUnloadPositions == null)
        {
            return;
        }
        this.targetTruck = truck;
        this.targetTruckUnloadPositions = (Truck.ForkliftUnloadPositions)truckUnloadPositions;
        this.state = State.LongApproach;
        this.agent.SetDestination(((Truck.ForkliftUnloadPositions)this.targetTruckUnloadPositions).approachPosition);
    }

    public override bool CanDeactivate()
    {
        switch (this.state)
        {
            case State.None:
            case State.LongApproach:
            case State.RotationAdjust:
                return true;
        }
        return false;
    }

    public override void Deactivate()
    {
        if (!CanDeactivate())
        {
            return;
        }
        if (this.targetTruck != null)
        {
            GameManager.Instance.CancelMoveForkliftToUnloadPaletteToTruck(this.forklift, this.targetTruck);
        }
        this.agent.enabled = true;
        this.targetTruck = null;
        this.targetTruckUnloadPositions = null;
        this.state = State.None;
    }

}
