using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IForklift
{

    public bool IsSelected { get; }

    public bool HasPalette { get; }

    public IPallet LoadedPallet { get; }

    public float Height { get; }

    public Vector3 Position { get; }

    public Transform ForkPaletteHandle { get; }

    public bool CanMove();

    public void MoveToPosition(Vector3 position);

}
