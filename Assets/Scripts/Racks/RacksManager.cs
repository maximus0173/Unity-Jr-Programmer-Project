using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacksManager : MonoBehaviour
{

    public static RacksManager Instance { get; private set; }

    private List<Rack> allRacks = new List<Rack>();

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
            UserControl.Instance.OnMouseHover += UserControl_OnMouseHover;
            UserControl.Instance.OnMouseClick += UserControl_OnMouseClicked;
        }
    }

    private void OnDestroy()
    {
        if (UserControl.Instance != null)
        {
            UserControl.Instance.OnMouseHover -= UserControl_OnMouseHover;
            UserControl.Instance.OnMouseClick -= UserControl_OnMouseClicked;
        }
    }


    public void RegisterRack(Rack rack)
    {
        if (this.allRacks.Contains(rack))
        {
            return;
        }
        this.allRacks.Add(rack);
    }

    public void UnregisterRack(Rack rack)
    {
        if (!this.allRacks.Contains(rack))
        {
            return;
        }
        this.allRacks.Remove(rack);
    }

    private void UserControl_OnMouseHover(object sender, UserControl.OnMouseHoverEventArgs e)
    {
        if (ForkliftsManager.Instance != null && UIMainScene.Instance != null)
        {
            if (ForkliftsManager.Instance.SelectedForklift != null
                && ForkliftsManager.Instance.SelectedForklift.HasPalette
                && ForkliftsManager.Instance.SelectedForklift.CanMove()
                && e.MouseHoveredObject.TryGetComponent<IRack>(out IRack hoveredRack)
                && GameManager.Instance.CanUnloadPaletteFromForkliftToRack(ForkliftsManager.Instance.SelectedForklift, hoveredRack))
            {
                UIMainScene.Instance.ShowUnloadOnRackHoverText(hoveredRack.Position);
                hoveredRack.ShowSelectedMarker();
            }
            else
            {
                UIMainScene.Instance.HideUnloadOnRackHoverText();
            }
        }
    }

    private void UserControl_OnMouseClicked(object sender, UserControl.OnMouseClickEventArgs e)
    {
        if (ForkliftsManager.Instance != null)
        {
            if (e.IsLeftMouseButtonClicked && ForkliftsManager.Instance.SelectedForklift != null
                && ForkliftsManager.Instance.SelectedForklift.HasPalette
                && ForkliftsManager.Instance.SelectedForklift.CanMove()
                && e.MouseClickedObject.TryGetComponent<IRack>(out IRack clickedRack)
                && GameManager.Instance.CanUnloadPaletteFromForkliftToRack(ForkliftsManager.Instance.SelectedForklift, clickedRack))
            {
                GameManager.Instance.MoveForkliftToUnloadPaletteToRack(ForkliftsManager.Instance.SelectedForklift, clickedRack);
            }
        }
    }


}
