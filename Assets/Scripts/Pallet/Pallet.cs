using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pallet : MonoBehaviour, IPallet
{

    [SerializeField]
    private List<Transform> forkliftApproachTransforms = new List<Transform>();
    [SerializeField]
    private List<Transform> forkliftLoadingTransforms = new List<Transform>();

    private IRack mountedToRack;
    private IForklift pendingTransportByForklift;
    private IForklift transportedByForklift;

    public bool IsMountedToRack { get => this.mountedToRack != null; }

    public bool IsPendingTransport { get => this.pendingTransportByForklift != null; }

    public bool IsTransportedByForklift { get => this.transportedByForklift != null; }

    public IRack MountedToRack { get => this.mountedToRack; }

    public Vector3 Position { get => transform.position; }

    private void Start()
    {
        if (PalettesManager.Instance != null)
        {
            PalettesManager.Instance.RegisterPalette(this);
        }
    }

    private void OnDestroy()
    {
        if (PalettesManager.Instance != null)
        {
            PalettesManager.Instance.UnregisterPalette(this);
        }
    }

    public bool CanBeMountedToRack()
    {
        return this.mountedToRack == null;
    }

    public void MountToRack(IRack rack)
    {
        this.mountedToRack = rack;
        transform.position = rack.Position;
        transform.rotation = rack.Rotation;
    }

    public void UnmountFromRack()
    {
        if (this.mountedToRack == null)
        {
            return;
        }
        this.mountedToRack = null;
    }

    public bool CanBeTransported()
    {
        return this.pendingTransportByForklift == null && this.transportedByForklift == null;
    }

    public void PendingTransportByForklift(IForklift forklift)
    {
        if (!CanBeTransported())
        {
            return;
        }
        this.pendingTransportByForklift = forklift;
        this.transportedByForklift = null;
    }

    public void LoadToForklift(IForklift forklift)
    {
        if (this.pendingTransportByForklift != forklift)
        {
            return;
        }
        this.pendingTransportByForklift = null;
        this.transportedByForklift = forklift;
        transform.parent = forklift.ForkPaletteHandle;
        transform.localPosition = Vector3.zero;
    }

    public void CancelPendingTransportByForklift()
    {
        this.pendingTransportByForklift = null;
    }

    public void UnloadFromForklift()
    {
        if (!IsTransportedByForklift)
        {
            return;
        }
        transform.parent = null;
        this.transportedByForklift = null;
    }

    public ApproachPositions? GetForkliftApproachPositions(IForklift forklift)
    {
        Transform longApproachTransform = null;
        float approachDistance = -1;
        foreach (Transform t in this.forkliftApproachTransforms)
        {
            float d = Vector3.Distance(t.position, forklift.Position);
            if (longApproachTransform == null || d < approachDistance)
            {
                longApproachTransform = t;
                approachDistance = d;
            }
        }
        if (longApproachTransform == null)
        {
            return null;
        }
        Transform nearApproachTransform = null;
        approachDistance = -1;
        foreach (Transform t in this.forkliftLoadingTransforms)
        {
            float d = Vector3.Distance(t.position, longApproachTransform.position);
            if (nearApproachTransform == null || d < approachDistance)
            {
                nearApproachTransform = t;
                approachDistance = d;
            }
        }
        if (nearApproachTransform == null)
        {
            return null;
        }
        return new ApproachPositions()
        {
            longApproachPosition = longApproachTransform.position,
            nearApproachPosition = nearApproachTransform.position
        };
    }

    public struct ApproachPositions
    {
        public Vector3 longApproachPosition;
        public Vector3 nearApproachPosition;
    }

}
