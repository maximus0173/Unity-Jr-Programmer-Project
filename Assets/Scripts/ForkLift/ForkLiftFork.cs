using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForkliftFork : MonoBehaviour
{

    private Forklift forklift;

    [SerializeField]
    private Transform paletteHandle;

    [SerializeField] private float maxHeight = 2f;
    private float speed = 1f;

    private float targetForkHeight = 0f;
    private float currentVelocity;

    private IPalette loadedPalette = null;

    public float ForkHeight { get => transform.localPosition.y; }
    public float ForkMaxHeight { get => this.maxHeight; }

    public bool HasPalette { get => this.loadedPalette != null; }

    public IPalette LoadedPalette { get => this.loadedPalette; }

    public Transform PaletteHandle { get => this.paletteHandle; }

    private void Awake()
    {
        this.forklift = GetComponentInParent<Forklift>();
    }

    private void Update()
    {
        float newHeight = Mathf.SmoothDamp(ForkHeight, this.targetForkHeight, ref currentVelocity, speed);
        transform.localPosition = new Vector3(transform.localPosition.x, newHeight, transform.localPosition.z);
    }

    public bool CanLoadPalette(IPalette palette)
    {
        if (this.loadedPalette != null)
        {
            return false;
        }
        float paletteHeight = this.forklift.transform.InverseTransformPoint(palette.Position).y;
        return this.maxHeight >= paletteHeight;
    }

    public bool IsForkHeightAdjustedToPalette(IPalette palette)
    {
        float minDistance = 0.1f;
        if (Mathf.Abs(transform.position.y - palette.Position.y) < minDistance)
        {
            return true;
        }
        return false;
    }

    public void AdjustForkHeightToPalette(IPalette palette)
    {
        this.targetForkHeight = this.forklift.transform.InverseTransformPoint(palette.Position).y;
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

    public void LoadPalette(IPalette palette)
    {
        this.loadedPalette = palette;

    }

    public void UnloadPalette()
    {
        this.loadedPalette = null;
    }

}
