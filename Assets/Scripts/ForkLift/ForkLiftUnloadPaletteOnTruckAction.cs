using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ForkLiftUnloadPaletteOnTruckAction : ForkLiftBaseAction
{

    private ForkLiftFork fork;
    private NavMeshAgent agent;

    private Truck targetTruck = null;
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
        this.fork = this.forkLift.GetComponentInChildren<ForkLiftFork>();
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
        float minDistanceDiff = 0.1f;
        Vector3 targetPosition = ((Truck.ForkliftUnloadPositions)this.targetTruckUnloadPositions).approachPosition;
        if (GameUtils.Distance2d(targetPosition, transform.position) < minDistanceDiff)
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
        }
    }

    public void UnloadPaletteOnTruck(Truck truck)
    {
        Truck.ForkliftUnloadPositions? truckUnloadPositions = truck.GetForkLiftUnloadPositions(this.forkLift);
        if (truckUnloadPositions == null)
        {
            return;
        }
        this.targetTruck = truck;
        this.targetTruckUnloadPositions = (Truck.ForkliftUnloadPositions)truckUnloadPositions;
        this.state = State.LongApproach;
        this.agent.SetDestination(((Truck.ForkliftUnloadPositions)this.targetTruckUnloadPositions).approachPosition);

    }

}
