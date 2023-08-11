using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForkliftUserControl : MonoBehaviour, IUserInteraction
{

    private Forklift forklift;

    private void Awake()
    {
        this.forklift = GetComponent<Forklift>();
    }

    public void OnLeftMouseButtonClicked(Vector3 position)
    {
        if (this.forklift == null || ForkliftsManager.Instance == null)
        {
            return;
        }
        ForkliftsManager.Instance.SelectForklift(this.forklift);
    }

    public void OnRightMouseButtonClicked(Vector3 position)
    {
        if (this.forklift == null || ForkliftsManager.Instance == null)
        {
            return;
        }
        if (this.forklift.IsSelected)
        {
            this.forklift.MoveToPosition(position);
        }
    }

}
