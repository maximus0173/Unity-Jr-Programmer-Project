using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPallet
{

    public bool IsMountedToRack { get; }

    public bool IsPendingTransport { get; }

    public bool IsTransportedByForklift { get; }

    public Vector3 Position { get; }

    public Pallet.ApproachPositions? GetForkliftApproachPositions(IForklift forklift);

}
