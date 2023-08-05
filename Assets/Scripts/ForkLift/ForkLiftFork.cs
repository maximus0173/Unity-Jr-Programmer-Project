using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForkLiftFork : MonoBehaviour
{

    private ForkLift forkLift;

    [SerializeField] private float maxHeight = 2f;
    private float speed = 1f;

    private float targetForkHeight = 0f;
    private float currentVelocity;

    private Palette loadedPalette = null;

    public float ForkHeight { get => transform.localPosition.y; }
    public float ForkMaxHeight { get => this.maxHeight; }

    public bool HasPalette { get => this.loadedPalette != null; }

    private void Awake()
    {
        this.forkLift = GetComponentInParent<ForkLift>();
    }

    private void Update()
    {
        float newHeight = Mathf.SmoothDamp(ForkHeight, this.targetForkHeight, ref currentVelocity, speed);
        transform.localPosition = new Vector3(transform.localPosition.x, newHeight, transform.localPosition.z);
    }

    public bool CanLoadPalette(Palette palette)
    {
        if (this.loadedPalette != null || !palette.CanBeTransported())
        {
            return false;
        }
        float paletteHeight = transform.InverseTransformPoint(palette.transform.position).y;
        return this.maxHeight >= paletteHeight;
    }

    public void AdjustForkHeightToPalette(Palette palette)
    {
        this.targetForkHeight = transform.InverseTransformPoint(palette.transform.position).y;
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

    public void LoadPalette(Palette palette)
    {
        this.loadedPalette = palette;
        this.loadedPalette.transform.parent = transform;
    }

    public void UnloadPalette()
    {
        this.loadedPalette.transform.parent = null;
        this.loadedPalette = null;
    }

}
