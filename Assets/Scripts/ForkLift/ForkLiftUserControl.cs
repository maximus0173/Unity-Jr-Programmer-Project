using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForkLiftUserControl : MonoBehaviour, IUserInteraction
{

    private ForkLift forkLift;

    private void Awake()
    {
        this.forkLift = GetComponent<ForkLift>();
    }

    public void OnLeftMouseButtonClicked(Vector3 position)
    {
        if (this.forkLift == null || ForkLiftsManager.Instance == null)
        {
            return;
        }
        ForkLiftsManager.Instance.SelectForkLift(this.forkLift);
    }

    public void OnRightMouseButtonClicked(Vector3 position)
    {
        if (this.forkLift == null || ForkLiftsManager.Instance == null)
        {
            return;
        }
        if (this.forkLift.IsSelected)
        {
            this.forkLift.MoveToPosition(position);
        }
    }

}
