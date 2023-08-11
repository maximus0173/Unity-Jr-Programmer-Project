using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Forklift : MonoBehaviour, IForklift
{

    private ForkliftFork fork;
    private NavMeshAgent agent;

    private ForkliftMoveAction moveAction;
    private ForkliftLoadPaletteAction loadPaletteAction;
    private ForkliftUnloadPaletteOnTruckAction unloadPaletteOnTruckAction;
    private ForkliftUnloadPaletteOnRackAction unloadPaletteOnRackAction;
    private ForkliftBaseAction activeAction;

    public bool IsSelected
    {
        get
        {
            if (ForkliftsManager.Instance != null)
            {
                return ForkliftsManager.Instance.IsForkliftSelected(this);
            }
            return false;
        }
    }

    public bool HasPalette { get => this.fork.HasPalette; }

    public IPalette LoadedPalette { get => this.fork.LoadedPalette; }

    public float Height { get => this.fork.ForkMaxHeight; }

    public Vector3 Position { get => transform.position; }

    public Transform ForkPaletteHandle { get => this.fork.PaletteHandle; }

    private void Start()
    {
        this.fork = this.GetComponentInChildren<ForkliftFork>();
        this.agent = GetComponent<NavMeshAgent>();
        this.moveAction = GetComponent<ForkliftMoveAction>();
        this.loadPaletteAction = GetComponent<ForkliftLoadPaletteAction>();
        this.unloadPaletteOnTruckAction = GetComponent<ForkliftUnloadPaletteOnTruckAction>();
        this.unloadPaletteOnRackAction = GetComponent<ForkliftUnloadPaletteOnRackAction>();
        this.activeAction = this.moveAction;
        this.loadPaletteAction.OnCompleteAction += OnCompleteAction;
        this.unloadPaletteOnTruckAction.OnCompleteAction += OnCompleteAction;
        this.unloadPaletteOnRackAction.OnCompleteAction += OnCompleteAction;
        if (ForkliftsManager.Instance != null)
        {
            ForkliftsManager.Instance.RegisterForklift(this);
        }
    }

    private void OnDestroy()
    {
        if (ForkliftsManager.Instance != null)
        {
            ForkliftsManager.Instance.UnregisterForklift(this);
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

    public bool CanLoadPalette(IPalette palette)
    {
        return this.activeAction == this.moveAction && this.fork.CanLoadPalette(palette);
    }

    public void MoveToLoadPalette(IPalette palette)
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

    public bool CanUnloadPaletteOnTruck(ITruck truck)
    {
        if (!this.HasPalette)
        {
            return false;
        }
        if (truck.ForkliftMaxHeight < this.Height)
        {
            return false;
        }
        return true;
    }

    public void MoveToUnloadPaletteOnTruck(ITruck truck)
    {
        if (this.activeAction == this.moveAction)
        {
            this.moveAction.Deactivate();
        }
        ActivateAction(this.unloadPaletteOnTruckAction);
        this.unloadPaletteOnTruckAction.UnloadPaletteOnTruck(truck);
    }

    public bool CanUnloadPaletteOnRack(IRack rack)
    {
        if (!this.HasPalette)
        {
            return false;
        }
        if (rack.Position.y > this.Height)
        {
            return false;
        }
        return true;
    }

    public void MoveToUnloadPaletteOnRack(IRack rack)
    {
        if (this.activeAction == this.moveAction)
        {
            this.moveAction.Deactivate();
        }
        ActivateAction(this.unloadPaletteOnRackAction);
        this.unloadPaletteOnRackAction.UnloadPaletteOnRack(rack);
    }

    private void ActivateAction(ForkliftBaseAction action)
    {
        this.activeAction = action;
    }

    private void OnCompleteAction(object sender, EventArgs e)
    {
        this.activeAction.Deactivate();
        ActivateAction(this.moveAction);
    }

}
