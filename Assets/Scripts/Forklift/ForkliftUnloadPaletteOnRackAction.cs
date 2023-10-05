using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ForkliftUnloadPaletteOnRackAction : ForkliftBaseAction
{

    private ForkliftFork fork;
    private NavMeshAgent agent;

    private IRack targetRack = null;
    private Rack.ApproachPositions? targetRackApproachPositions;

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
        if (this.targetRackApproachPositions == null)
        {
            return;
        }
        Vector3 targetPosition = ((Rack.ApproachPositions)this.targetRackApproachPositions).longApproachPosition;
        if (HandleMoveToward(targetPosition))
        {
            this.agent.enabled = false;
            this.state = State.RotationAdjust;
        }
    }

    private void HandleRotationAdjustState()
    {
        if (this.targetRackApproachPositions == null)
        {
            return;
        }
        Vector3 targetPosition = ((Rack.ApproachPositions)this.targetRackApproachPositions).nearApproachPosition;
        if (HandleRotationToward(targetPosition) && this.fork.IsForkHeightAdjustedToRack(this.targetRack))
        {
            this.state = State.NearApproach;
        }
    }

    private void HandleNearApproachState()
    {
        if (this.targetRackApproachPositions == null)
        {
            return;
        }
        Vector3 targetPosition = ((Rack.ApproachPositions)this.targetRackApproachPositions).nearApproachPosition;
        if (HandleMoveToward(targetPosition))
        {
            this.state = State.Unloading;
        }
    }

    private void HandleUnloadingState()
    {
        GameManager.Instance.UnloadPaletteFromForkliftToRack(this.forklift, this.targetRack);
        this.fork.UnloadPalette();
        this.state = State.Withdrawal;
    }

    private void HandleWithdrawalState()
    {
        if (this.targetRackApproachPositions == null)
        {
            return;
        }
        Vector3 targetPosition = ((Rack.ApproachPositions)this.targetRackApproachPositions).longApproachPosition;
        if (HandleMoveToward(targetPosition))
        {
            this.state = State.None;
            this.agent.enabled = true;
            GameManager.Instance.CompleteReserveationOfRackToUnloadPaletteFromForklift(this.forklift, this.targetRack);
            this.targetRack = null;
            this.targetRackApproachPositions = null;
            CompleteAction();
        }
    }

    public void UnloadPaletteOnRack(IRack rack)
    {
        Rack.ApproachPositions? approachPositions = rack.GetForkliftApproachPositions(this.forklift);
        if (approachPositions == null || !this.agent.enabled)
        {
            return;
        }
        this.agent.SetDestination(((Rack.ApproachPositions)approachPositions).longApproachPosition);
        this.targetRack = rack;
        this.targetRackApproachPositions = (Rack.ApproachPositions)approachPositions;
        this.state = State.LongApproach;
        this.fork.AdjustForkHeightToRack(this.targetRack);
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
        if (this.targetRack != null)
        {
            GameManager.Instance.CancelMoveForkliftToUnloadPaletteToRack(this.forklift, this.targetRack);
        }
        this.agent.enabled = true;
        this.targetRack = null;
        this.targetRackApproachPositions = null;
        this.state = State.None;
    }

}
