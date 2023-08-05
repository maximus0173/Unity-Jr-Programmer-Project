using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ForkLift : MonoBehaviour
{

    private ForkLiftFork fork;
    private NavMeshAgent agent;

    private ForkLiftMoveAction moveAction;
    private ForkLiftLoadPaletteAction loadPaletteAction;
    private ForkLiftUnloadPaletteOnTruckAction unloadPaletteOnTruckAction;
    private ForkLiftBaseAction activeAction;

    public bool IsSelected
    {
        get
        {
            if (ForkLiftsManager.Instance != null)
            {
                return ForkLiftsManager.Instance.IsForkLiftSelected(this);
            }
            return false;
        }
    }

    public bool HasPalette { get => this.fork.HasPalette; }

    private void Start()
    {
        this.fork = this.GetComponentInChildren<ForkLiftFork>();
        this.agent = GetComponent<NavMeshAgent>();
        this.moveAction = GetComponent<ForkLiftMoveAction>();
        this.loadPaletteAction = GetComponent<ForkLiftLoadPaletteAction>();
        this.unloadPaletteOnTruckAction = GetComponent<ForkLiftUnloadPaletteOnTruckAction>();
        this.activeAction = this.moveAction;
        if (ForkLiftsManager.Instance != null)
        {
            ForkLiftsManager.Instance.RegisterForkLift(this);
        }
    }

    private void OnDestroy()
    {
        if (ForkLiftsManager.Instance != null)
        {
            ForkLiftsManager.Instance.UnregisterForkLift(this);
        }
    }

    public bool CanMove()
    {
        if (this.activeAction == this.moveAction)
        {
            return true;
        }
        else if (this.activeAction.CanDeactivate())
        {
            return true;
        }
        return false;
    }

    public void MoveToPosition(Vector3 position)
    {
        if (this.activeAction == this.moveAction)
        {
            this.moveAction.MoveToPosition(position);
        }
        else if (this.activeAction.CanDeactivate())
        {
            this.activeAction.Deactivate();
            ActivateAction(this.moveAction);
            this.moveAction.MoveToPosition(position);
        }
    }

    public bool CanLoadPalette(Palette palette)
    {
        return this.activeAction == this.moveAction && this.fork.CanLoadPalette(palette);
    }

    public void LoadPalette(Palette palette)
    {
        if (!CanLoadPalette(palette))
        {
            return;
        }
        if (this.activeAction == this.moveAction)
        {
            this.moveAction.Deactivate();
        }
        ActivateAction(this.loadPaletteAction);
        this.loadPaletteAction.LoadPalette(palette);
    }

    public bool CanUnloadPaletteOnTruck(Truck truck)
    {
        return true;
    }

    public void UnloadPaletteOnTruck(Truck truck)
    {
        if (this.activeAction == this.moveAction)
        {
            this.moveAction.Deactivate();
        }
        ActivateAction(this.unloadPaletteOnTruckAction);
        this.unloadPaletteOnTruckAction.UnloadPaletteOnTruck(truck);
    }

    private void ActivateAction(ForkLiftBaseAction action)
    {
        this.activeAction = action;
    }

}
