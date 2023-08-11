using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITruck
{

    public Vector3 InfoPosition { get; }

    public float ForkliftMaxHeight { get; }

    public Truck.ForkliftUnloadPositions? GetForkliftUnloadPositions(IForklift forklift);

}
