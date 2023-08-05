using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Truck : MonoBehaviour
{

    [SerializeField]
    private Transform forkLiftApproachTransform;

    [SerializeField]
    private List<Transform> paletteTransforms = new List<Transform>();

    [SerializeField]
    private Transform infoTransform;

    [SerializeField]
    private float forkLiftMaxHeight = 2.5f;

    private List<Palette> loadedPalettes = new List<Palette>();

    public Vector3 InfoPosition { get => this.infoTransform.position; }

    public ForkliftUnloadPositions? GetForkLiftUnloadPositions(ForkLift forkLift)
    {
        return new ForkliftUnloadPositions()
        {
            approachPosition = this.forkLiftApproachTransform.position,
            palettePosition = this.paletteTransforms[0].position
        };
    }

    public struct ForkliftUnloadPositions
    {
        public Vector3 approachPosition;
        public Vector3 palettePosition;
    }

}
