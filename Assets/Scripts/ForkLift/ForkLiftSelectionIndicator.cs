using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForkliftSelectionIndicator : MonoBehaviour
{

    [SerializeField] private Forklift forklift;
    [SerializeField] private GameObject selectionIndicator;

    private void Start()
    {
        HandleSelection();
        if (ForkliftsManager.Instance != null)
        {
            ForkliftsManager.Instance.OnSelectedForkliftChanged += OnSelectedForkliftChanged;
        }
    }

    private void OnDestroy()
    {
        if (ForkliftsManager.Instance != null)
        {
            ForkliftsManager.Instance.OnSelectedForkliftChanged -= OnSelectedForkliftChanged;
        }
    }

    private void OnSelectedForkliftChanged(object sender, System.EventArgs e)
    {
        this.HandleSelection();
    }

    private void HandleSelection()
    {
        if (ForkliftsManager.Instance != null && ForkliftsManager.Instance.IsForkliftSelected(this.forklift))
        {
            this.selectionIndicator.SetActive(true);
            return;
        }
        this.selectionIndicator.SetActive(false);
    }

}
