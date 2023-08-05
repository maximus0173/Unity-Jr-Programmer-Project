using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Palette : MonoBehaviour
{

    [SerializeField]
    private List<Transform> forkLiftApproachTransforms = new List<Transform>();
    [SerializeField]
    private List<Transform> forkLiftLoadingTransforms = new List<Transform>();

    private ForkLift transportedByForkLift;

    private void Start()
    {
        if (PalettesManager.Instance != null)
        {
            PalettesManager.Instance.RegisterPalette(this);
        }
    }

    private void OnDestroy()
    {
        if (PalettesManager.Instance != null)
        {
            PalettesManager.Instance.UnregisterPalette(this);
        }
    }

    public bool CanBeTransported()
    {
        return this.transportedByForkLift == null;
    }

    public void TransportByForkLift(ForkLift forkLift)
    {
        this.transportedByForkLift = forkLift;
    }

    public void CancelTransportByForkLift()
    {
        this.transportedByForkLift = null;
    }

    public void dropToFloor()
    {
        this.transportedByForkLift = null;
    }

    public void dropToRack()
    {
        this.transportedByForkLift = null;
    }

    public ApproachPositions? GetForkLiftApproachPositions(ForkLift forkLift)
    {
        Transform longApproachTransform = null;
        float approachDistance = -1;
        foreach (Transform t in this.forkLiftApproachTransforms)
        {
            float d = Vector3.Distance(t.position, forkLift.transform.position);
            if (longApproachTransform == null || d < approachDistance)
            {
                longApproachTransform = t;
                approachDistance = d;
            }
        }
        if (longApproachTransform == null)
        {
            return null;
        }
        Transform nearApproachTransform = null;
        approachDistance = -1;
        foreach (Transform t in this.forkLiftLoadingTransforms)
        {
            float d = Vector3.Distance(t.position, longApproachTransform.position);
            if (nearApproachTransform == null || d < approachDistance)
            {
                nearApproachTransform = t;
                approachDistance = d;
            }
        }
        if (nearApproachTransform == null)
        {
            return null;
        }
        return new ApproachPositions()
        {
            longApproachPosition = longApproachTransform.position,
            nearApproachPosition = nearApproachTransform.position
        };
    }

    public struct ApproachPositions
    {
        public Vector3 longApproachPosition;
        public Vector3 nearApproachPosition;
    }

}
