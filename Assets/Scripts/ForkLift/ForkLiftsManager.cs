using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForkliftsManager : MonoBehaviour
{

    public static ForkliftsManager Instance { get; private set; }

    private List<Forklift> allForklifts = new List<Forklift>();
    private Forklift selectedForklift = null;

    public IForklift SelectedForklift { get => this.selectedForklift; }

    public event System.EventHandler OnSelectedForkliftChanged;

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

    public void RegisterForklift(Forklift forklift)
    {
        if (this.allForklifts.Contains(forklift))
        {
            return;
        }
        this.allForklifts.Add(forklift);
    }

    public void UnregisterForklift(Forklift forklift)
    {
        if (!this.allForklifts.Contains(forklift))
        {
            return;
        }
        this.allForklifts.Remove(forklift);
        if (this.selectedForklift == forklift)
        {
            this.selectedForklift = null;
            this.OnSelectedForkliftChanged?.Invoke(this, System.EventArgs.Empty);
        }
    }

    public bool IsForkliftSelected(Forklift forklift)
    {
        return this.selectedForklift == forklift;
    }

    public void SelectForklift(Forklift forklift)
    {
        if (this.selectedForklift == forklift)
        {
            return;
        }
        this.selectedForklift = forklift;
        this.OnSelectedForkliftChanged?.Invoke(this, System.EventArgs.Empty);
    }

    private void UserControl_OnMouseClicked(object sender, UserControl.OnMouseClickEventArgs e)
    {
        if (e.IsLeftMouseButtonClicked && e.MouseClickedObject.TryGetComponent<Forklift>(out Forklift clickedForklift))
        {
            SelectForklift(clickedForklift);
        }
        if (this.selectedForklift && e.IsLeftMouseButtonClicked && e.MouseClickedObject.CompareTag("Ground"))
        {
            this.selectedForklift.MoveToPosition(e.WorldPosition);
        }
    }

}
