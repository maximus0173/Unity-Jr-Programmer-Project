using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForkliftFork : MonoBehaviour
{

    private Forklift forklift;

    [SerializeField]
    private Transform palletHandle;

    [SerializeField] private float maxHeight = 2f;
    private float speed = 1f;

    private float targetForkHeight = 0f;
    private float currentVelocity;

    private IPallet loadedPallet = null;

    public float ForkHeight { get => transform.localPosition.y; }
    public float ForkMaxHeight { get => this.maxHeight; }

    public bool HasPalette { get => this.loadedPallet != null; }

    public IPallet LoadedPalette { get => this.loadedPallet; }

    public Transform PalletHandle { get => this.palletHandle; }

    private void Awake()
    {
        this.forklift = GetComponentInParent<Forklift>();
    }

    private void Update()
    {
        float newHeight = Mathf.SmoothDamp(ForkHeight, this.targetForkHeight, ref currentVelocity, speed);
        transform.localPosition = new Vector3(transform.localPosition.x, newHeight, transform.localPosition.z);
    }

    public bool CanLoadPalette(IPallet pallet)
    {
        if (this.loadedPallet != null)
        {
            return false;
        }
        float palletHeight = this.forklift.transform.InverseTransformPoint(pallet.Position).y;
        return this.maxHeight >= palletHeight;
    }

    public bool IsForkHeightAdjustedToPalette(IPallet pallet)
    {
        float minDistance = 0.1f;
        if (Mathf.Abs(transform.position.y - pallet.Position.y) < minDistance)
        {
            return true;
        }
        return false;
    }

    public void AdjustForkHeightToPalette(IPallet pallet)
    {
        this.targetForkHeight = this.forklift.transform.InverseTransformPoint(pallet.Position).y;
    }

    public bool IsForkHeightAdjustedToRack(IRack rack)
    {
        float minDistance = 0.1f;
        if (Mathf.Abs(transform.position.y - rack.Position.y) < minDistance)
        {
            return true;
        }
        return false;
    }


    public void AdjustForkHeightToRack(IRack rack)
    {
        this.targetForkHeight = this.forklift.transform.InverseTransformPoint(rack.Position).y;
    }

    public void AdjustForkHeightToWithdrawal()
    {
        float addForkHeight = 0.2f;
        this.targetForkHeight += addForkHeight;
    }

    public void AdjustForkHeightToUnload()
    {
        this.targetForkHeight = 0f;
    }

    public void AdjustForkHeightToTransport()
    {
        float transportForkHeight = 0.2f;
        this.targetForkHeight = transportForkHeight;
    }

    public void LoadPalette(IPallet pallet)
    {
        this.loadedPallet = pallet;

    }

    public void UnloadPalette()
    {
        this.loadedPallet = null;
    }

}
