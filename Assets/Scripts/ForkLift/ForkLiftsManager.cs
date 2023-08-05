using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForkLiftsManager : MonoBehaviour
{

    public static ForkLiftsManager Instance { get; private set; }

    private List<ForkLift> allForkLifts = new List<ForkLift>();
    private ForkLift selectedForkLift = null;

    public ForkLift SelectedForkLift { get => this.selectedForkLift; }

    public event System.EventHandler OnSelectedForkLiftChanged;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (UserControl.Instance != null)
        {
            UserControl.Instance.OnMouseClick += UserControl_OnMouseClicked;
        }
    }

    private void OnDestroy()
    {
        if (UserControl.Instance != null)
        {
            UserControl.Instance.OnMouseClick -= UserControl_OnMouseClicked;
        }
    }

    public void RegisterForkLift(ForkLift forkLift)
    {
        if (this.allForkLifts.Contains(forkLift))
        {
            return;
        }
        this.allForkLifts.Add(forkLift);
    }

    public void UnregisterForkLift(ForkLift forkLift)
    {
        if (!this.allForkLifts.Contains(forkLift))
        {
            return;
        }
        this.allForkLifts.Remove(forkLift);
        if (this.selectedForkLift == forkLift)
        {
            this.selectedForkLift = null;
            this.OnSelectedForkLiftChanged?.Invoke(this, System.EventArgs.Empty);
        }
    }

    public bool IsForkLiftSelected(ForkLift forkLift)
    {
        return this.selectedForkLift == forkLift;
    }

    public void SelectForkLift(ForkLift forkLift)
    {
        if (this.selectedForkLift == forkLift)
        {
            return;
        }
        this.selectedForkLift = forkLift;
        this.OnSelectedForkLiftChanged?.Invoke(this, System.EventArgs.Empty);
    }

    private void UserControl_OnMouseClicked(object sender, UserControl.OnMouseClickEventArgs e)
    {
        if (e.IsLeftMouseButtonClicked && e.MouseClickedObject.TryGetComponent<ForkLift>(out ForkLift clickedForkLift))
        {
            SelectForkLift(clickedForkLift);
        }
        if (this.selectedForkLift && e.IsLeftMouseButtonClicked && e.MouseClickedObject.CompareTag("Ground"))
        {
            this.selectedForkLift.MoveToPosition(e.WorldPosition);
        }
    }

}
