using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRack
{

    public Vector3 Position { get; }

    public Quaternion Rotation { get; }

    public Rack.ApproachPositions? GetForkliftApproachPositions(IForklift forklift);

    public void ShowSelectedMarker();

}
