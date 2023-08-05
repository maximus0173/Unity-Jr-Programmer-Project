using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForkLiftSelectionIndicator : MonoBehaviour
{

    [SerializeField] private ForkLift forkLift;
    [SerializeField] private GameObject selectionIndicator;

    private void Start()
    {
        HandleSelection();
        if (ForkLiftsManager.Instance != null)
        {
            ForkLiftsManager.Instance.OnSelectedForkLiftChanged += OnSelectedForkLiftChanged;
        }
    }

    private void OnDestroy()
    {
        if (ForkLiftsManager.Instance != null)
        {
            ForkLiftsManager.Instance.OnSelectedForkLiftChanged -= OnSelectedForkLiftChanged;
        }
    }

    private void OnSelectedForkLiftChanged(object sender, System.EventArgs e)
    {
        this.HandleSelection();
    }

    private void HandleSelection()
    {
        if (ForkLiftsManager.Instance != null && ForkLiftsManager.Instance.IsForkLiftSelected(this.forkLift))
        {
            this.selectionIndicator.SetActive(true);
            return;
        }
        this.selectionIndicator.SetActive(false);
    }

}
