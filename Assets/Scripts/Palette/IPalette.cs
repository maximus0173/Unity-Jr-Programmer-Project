using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPalette
{

    public bool IsMountedToRack { get; }

    public bool IsPendingTransport { get; }

    public bool IsTransportedByForklift { get; }

    public Vector3 Position { get; }

    public Palette.ApproachPositions? GetForkliftApproachPositions(IForklift forklift);

}
