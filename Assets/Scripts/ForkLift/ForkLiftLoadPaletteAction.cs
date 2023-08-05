using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ForkLiftLoadPaletteAction : ForkLiftBaseAction
{

    private ForkLiftFork fork;
    private NavMeshAgent agent;

    private Palette targetPalette = null;
    private Palette.ApproachPositions? targetPaletteApproachPositions = null;

    private enum State
    {
        None,
        LongApproach,
        RotationAdjust,
        NearApproach,
        Loading,
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
                case State.Loading:
                    HandleLoadingState();
                    break;
                case State.Withdrawal:
                    HandleWithdrawalState();
                    break;
            }
        }
    }

    private void HandleLongApproachState()
    {
        if (this.targetPaletteApproachPositions == null)
        {
            return;
        }
        float minDistanceDiff = 0.1f;
        Vector3 targetPosition = ((Palette.ApproachPositions)this.targetPaletteApproachPositions).longApproachPosition;
        if (GameUtils.Distance2d(targetPosition, transform.position) < minDistanceDiff)
        {
            this.agent.enabled = false;
            this.state = State.RotationAdjust;
        }
    }

    private void HandleRotationAdjustState()
    {
        if (this.targetPaletteApproachPositions == null)
        {
            return;
        }
        Vector3 targetPosition = ((Palette.ApproachPositions)this.targetPaletteApproachPositions).nearApproachPosition;
        if (HandleRotationToward(targetPosition))
        {
            this.state = State.NearApproach;
        }
    }

    private void HandleNearApproachState()
    {
        if (this.targetPaletteApproachPositions == null)
        {
            return;
        }
        Vector3 targetPosition = ((Palette.ApproachPositions)this.targetPaletteApproachPositions).nearApproachPosition;
        if (HandleMoveToward(targetPosition))
        {
            this.state = State.Loading;
        }
    }

    private void HandleLoadingState()
    {
        this.fork.LoadPalette(this.targetPalette);
        this.state = State.Withdrawal;
        this.fork.AdjustForkHeightToWithdrawal();
    }

    private void HandleWithdrawalState()
    {
        if (this.targetPaletteApproachPositions == null)
        {
            return;
        }
        Vector3 targetPosition = ((Palette.ApproachPositions)this.targetPaletteApproachPositions).longApproachPosition;
        if (HandleMoveToward(targetPosition))
        {
            this.state = State.None;
            this.agent.enabled = true;
            this.fork.AdjustForkHeightToTransport();
        }
    }

    public void LoadPalette(Palette palette)
    {
        if (!palette.CanBeTransported())
        {
            return;
        }
        Palette.ApproachPositions? paletteApproachPositions = palette.GetForkLiftApproachPositions(this.forkLift);
        if (paletteApproachPositions == null)
        {
            return;
        }
        this.targetPalette = palette;
        this.targetPaletteApproachPositions = paletteApproachPositions;
        palette.TransportByForkLift(this.forkLift);
        this.state = State.LongApproach;
        this.agent.SetDestination(((Palette.ApproachPositions)this.targetPaletteApproachPositions).longApproachPosition);
        this.fork.AdjustForkHeightToPalette(this.targetPalette);
    }

    public override bool CanDeactivate()
    {
        return state == State.None || state == State.LongApproach;
    }

    public override void Deactivate()
    {
        if (!CanDeactivate())
        {
            return;
        }
        if (this.state == State.LongApproach)
        {
            this.targetPalette.CancelTransportByForkLift();
        }
        this.targetPalette = null;
        this.targetPaletteApproachPositions = null;
        this.state = State.None;
    }

}
