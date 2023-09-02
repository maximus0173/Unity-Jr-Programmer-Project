using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ForkliftLoadPaletteAction : ForkliftBaseAction
{

    private ForkliftFork fork;
    private NavMeshAgent agent;

    private IPallet targetPalette = null;
    private Pallet.ApproachPositions? targetPaletteApproachPositions = null;

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
        Vector3 targetPosition = ((Pallet.ApproachPositions)this.targetPaletteApproachPositions).longApproachPosition;
        if (HandleMoveToward(targetPosition))
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
        Vector3 targetPosition = ((Pallet.ApproachPositions)this.targetPaletteApproachPositions).nearApproachPosition;
        if (HandleRotationToward(targetPosition) && this.fork.IsForkHeightAdjustedToPalette(this.targetPalette))
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
        Vector3 targetPosition = ((Pallet.ApproachPositions)this.targetPaletteApproachPositions).nearApproachPosition;
        if (HandleMoveToward(targetPosition))
        {
            this.state = State.Loading;
        }
    }

    private void HandleLoadingState()
    {
        GameManager.Instance.LoadPaletteToForklift(this.targetPalette, this.forklift);
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
        Vector3 targetPosition = ((Pallet.ApproachPositions)this.targetPaletteApproachPositions).longApproachPosition;
        if (HandleMoveToward(targetPosition))
        {
            this.state = State.None;
            this.targetPalette = null;
            this.targetPaletteApproachPositions = null;
            this.agent.enabled = true;
            this.fork.AdjustForkHeightToTransport();
            CompleteAction();
        }
    }

    public void LoadPalette(IPallet pallet)
    {
        Pallet.ApproachPositions? palletApproachPositions = pallet.GetForkliftApproachPositions(this.forklift);
        if (palletApproachPositions == null)
        {
            return;
        }
        this.targetPalette = pallet;
        this.targetPaletteApproachPositions = palletApproachPositions;
        this.state = State.LongApproach;
        this.agent.SetDestination(((Pallet.ApproachPositions)this.targetPaletteApproachPositions).longApproachPosition);
        this.fork.AdjustForkHeightToPalette(this.targetPalette);
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
        if (this.targetPalette != null)
        {
            GameManager.Instance.CancelMoveForkliftToLoadPalette(this.targetPalette, this.forklift);
        }
        this.agent.enabled = true;
        this.targetPalette = null;
        this.targetPaletteApproachPositions = null;
        this.state = State.None;
    }

}
