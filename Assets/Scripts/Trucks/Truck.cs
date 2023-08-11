using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Truck : MonoBehaviour, ITruck
{

    [SerializeField]
    private Transform forkliftApproachTransform;

    [SerializeField]
    private List<Transform> paletteTransforms = new List<Transform>();

    [SerializeField]
    private Transform infoTransform;

    [SerializeField]
    private float forkliftMaxHeight = 2.5f;

    private List<IPalette> loadedPalettes = new List<IPalette>();

    public Vector3 InfoPosition { get => this.infoTransform.position; }

    public float ForkliftMaxHeight { get => this.forkliftMaxHeight; }

    public IForklift reservedForUnloadPaletteFromForklift = null;

    public bool CanUnloadPaletteFromForklift(IForklift forklift)
    {
        if (this.reservedForUnloadPaletteFromForklift != null && this.reservedForUnloadPaletteFromForklift != forklift)
        {
            return false;
        }
        if (forklift.Height > this.forkliftMaxHeight)
        {
            return false;
        }
        if (this.loadedPalettes.Count >= this.paletteTransforms.Count)
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
        this.reservedForUnloadPaletteFromForklift = forklift;
    }

    public void UnloadPaletteFromForklift(IForklift forklift)
    {
        if (!CanUnloadPaletteFromForklift(forklift))
        {
            throw new System.Exception("Cannot unload palette from forklift");
        }
        this.loadedPalettes.Add(forklift.LoadedPalette);
    }

    public void CompleteReservationForUnloadPaletteFromForklift(IForklift forklift)
    {
        if (this.reservedForUnloadPaletteFromForklift != forklift)
        {
            throw new System.Exception("Cannot complete reservation for unload palette from forklift");
        }
        this.reservedForUnloadPaletteFromForklift = null;
    }

    public ForkliftUnloadPositions? GetForkliftUnloadPositions(IForklift forklift)
    {
        int index = this.loadedPalettes.Count;
        if (index >= this.paletteTransforms.Count)
        {
            return null;
        }
        return new ForkliftUnloadPositions()
        {
            approachPosition = this.forkliftApproachTransform.position,
            palettePosition = this.paletteTransforms[index].position
        };
    }

    public struct ForkliftUnloadPositions
    {
        public Vector3 approachPosition;
        public Vector3 palettePosition;
    }

}
