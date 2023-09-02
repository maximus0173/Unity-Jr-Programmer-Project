using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rack : MonoBehaviour, IRack
{

    [SerializeField] private BoxCollider mountCollider;
    
    [SerializeField] private Transform[] forkliftApproachTransforms;
    [SerializeField] private Transform[] forkliftLoadingTransforms;

    [SerializeField] private GameObject selectedMarker;

    private IPallet mountedPalette;
    private IForklift reservedForForkliftUnloadPallet = null;
    private bool selectedMarkerShowed = false;

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

    private void Update()
    {
        if (this.selectedMarker.activeSelf && !this.selectedMarkerShowed)
        {
            this.selectedMarkerShowed = true;
        }
        else if (this.selectedMarkerShowed)
        {
            this.selectedMarker.SetActive(false);
        }
    }

    private void InitialPaletteMounts()
    {
        RaycastHit[] hits = Physics.BoxCastAll(this.mountCollider.bounds.center, this.mountCollider.bounds.size / 2, transform.forward);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.TryGetComponent<IPallet>(out IPallet pallet))
            {
                MountPallet(pallet);
            }
        }
    }

    public bool CanUnloadPaletteFromForklift(IForklift forklift)
    {
        if (this.mountedPalette != null)
        {
            return false;
        }
        if (this.reservedForForkliftUnloadPallet != null && this.reservedForForkliftUnloadPallet != forklift)
        {
            return false;
        }
        return true;
    }

    public void ReserveForUnloadPaletteFromForklift(IForklift forklift)
    {
        if (!CanUnloadPaletteFromForklift(forklift))
        {
            throw new System.Exception("Cannot reserve for unload pallet from forklift");
        }
        this.reservedForForkliftUnloadPallet = forklift;
    }

    public void MountPallet(IPallet pallet)
    {
        float maxDistance = 0.5f;
        if (this.mountedPalette == null
            && Vector3.Distance(pallet.Position, transform.position) < maxDistance)
        {
            this.mountedPalette = pallet;
            this.mountCollider.enabled = false;
            GameManager.Instance.MountPaletteToRack(pallet, this);
        }
    }

    public void UnmountPalette(IPallet pallet)
    {
        if (this.mountedPalette == pallet)
        {
            this.mountedPalette = null;
            this.mountCollider.enabled = true;
            GameManager.Instance.UnmountPalletFromRack(pallet, this);
        }
    }

    public void CompleteReservationForUnloadPaletteFromForklift(IForklift forklift)
    {
        if (this.reservedForForkliftUnloadPallet != forklift)
        {
            throw new System.Exception("Cannot complete reservation for unload pallet from forklift");
        }
        this.reservedForForkliftUnloadPallet = null;
    }

    public void ShowSelectedMarker()
    {
        this.selectedMarkerShowed = false;
        this.selectedMarker.SetActive(true);
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
