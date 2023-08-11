using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rack : MonoBehaviour, IRack
{

    [SerializeField]
    private BoxCollider mountCollider;
    
    [SerializeField]
    private Transform[] forkliftApproachTransforms;
    [SerializeField]
    private Transform[] forkliftLoadingTransforms;

    private IPalette mountedPalette;
    private IForklift reservedForForkliftUnloadPalette = null;

    public Vector3 Position { get => transform.position; }

    public Quaternion Rotation { get => transform.rotation; }

    private void Start()
    {
        if (RacksManager.Instance != null)
        {
            RacksManager.Instance.RegisterRack(this);
        }
        InitialPaletteMounts();
    }

    private void OnDestroy()
    {
        if (RacksManager.Instance != null)
        {
            RacksManager.Instance.UnregisterRack(this);
        }
    }

    private void InitialPaletteMounts()
    {
        RaycastHit[] hits = Physics.BoxCastAll(this.mountCollider.bounds.center, this.mountCollider.bounds.extents / 2, transform.forward);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.TryGetComponent<IPalette>(out IPalette palette))
            {
                GameManager.Instance.MountPaletteToRack(palette, this);
            }
        }
    }

    public bool CanUnloadPaletteFromForklift(IForklift forklift)
    {
        if (this.mountedPalette != null)
        {
            return false;
        }
        if (this.reservedForForkliftUnloadPalette != null && this.reservedForForkliftUnloadPalette != forklift)
        {
            return false;
        }
        return true;
    }

    public void ReserveForUnloadPaletteFromForklift(IForklift forklift)
    {
        if (!CanUnloadPaletteFromForklift(forklift))
        {
            throw new System.Exception("Cannot reserve for unload palette from forklift");
        }
        this.reservedForForkliftUnloadPalette = forklift;
    }

    public void MountPalette(IPalette palette)
    {
        float maxDistance = 0.5f;
        if (this.mountedPalette == null
            && Vector3.Distance(palette.Position, transform.position) < maxDistance)
        {
            this.mountedPalette = palette;
            this.mountCollider.enabled = false;
            GameManager.Instance.MountPaletteToRack(palette, this);
        }
    }

    public void UnmountPalette(IPalette palette)
    {
        if (this.mountedPalette == palette)
        {
            this.mountedPalette = null;
            this.mountCollider.enabled = true;
            GameManager.Instance.UnmountPaletteFromRack(palette, this);
        }
    }

    public void CompleteReservationForUnloadPaletteFromForklift(IForklift forklift)
    {
        if (this.reservedForForkliftUnloadPalette != forklift)
        {
            throw new System.Exception("Cannot complete reservation for unload palette from forklift");
        }
        this.reservedForForkliftUnloadPalette = null;
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
