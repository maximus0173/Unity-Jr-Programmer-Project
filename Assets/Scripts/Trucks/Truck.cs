using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Truck : MonoBehaviour, ITruck
{

    [SerializeField]
    private Transform forkliftApproachTransform;

    [SerializeField]
    private List<Transform> palletTransforms = new List<Transform>();

    [SerializeField]
    private Transform infoTransform;

    [SerializeField]
    private float forkliftMaxHeight = 2.5f;

    private List<IPallet> loadedPalettes = new List<IPallet>();

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
        if (this.loadedPalettes.Count >= this.palletTransforms.Count)
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
        this.reservedForUnloadPaletteFromForklift = forklift;
    }

    public void UnloadPaletteFromForklift(IForklift forklift)
    {
        if (!CanUnloadPaletteFromForklift(forklift))
        {
            throw new System.Exception("Cannot unload pallet from forklift");
        }
        this.loadedPalettes.Add(forklift.LoadedPallet);
    }

    public void CompleteReservationForUnloadPaletteFromForklift(IForklift forklift)
    {
        if (this.reservedForUnloadPaletteFromForklift != forklift)
        {
            throw new System.Exception("Cannot complete reservation for unload pallet from forklift");
        }
        this.reservedForUnloadPaletteFromForklift = null;
    }

    public ForkliftUnloadPositions? GetForkliftUnloadPositions(IForklift forklift)
    {
        int index = this.loadedPalettes.Count;
        if (index >= this.palletTransforms.Count)
        {
            return null;
        }
        return new ForkliftUnloadPositions()
        {
            approachPosition = this.forkliftApproachTransform.position,
            palletPosition = this.palletTransforms[index].position
        };
    }

    public struct ForkliftUnloadPositions
    {
        public Vector3 approachPosition;
        public Vector3 palletPosition;
    }

}
