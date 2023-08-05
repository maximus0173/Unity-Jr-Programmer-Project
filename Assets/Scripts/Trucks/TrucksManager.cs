using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrucksManager : MonoBehaviour
{

    public static TrucksManager Instance { get; private set; }

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

    private void UserControl_OnMouseHover(object sender, UserControl.OnMouseHoverEventArgs e)
    {
        if (ForkLiftsManager.Instance != null && UIMainScene.Instance != null)
        {
            if (ForkLiftsManager.Instance.SelectedForkLift != null
                && ForkLiftsManager.Instance.SelectedForkLift.HasPalette
                && ForkLiftsManager.Instance.SelectedForkLift.CanMove()
                && e.MouseHoveredObject.TryGetComponent<Truck>(out Truck hoveredTruck)
                && ForkLiftsManager.Instance.SelectedForkLift.CanUnloadPaletteOnTruck(hoveredTruck))
            {
                UIMainScene.Instance.ShowUnloadHoverText(hoveredTruck.InfoPosition);
            }
            else
            {
                UIMainScene.Instance.HideUnloadHoverText();
            }
        }
    }

    private void UserControl_OnMouseClicked(object sender, UserControl.OnMouseClickEventArgs e)
    {
        if (ForkLiftsManager.Instance != null)
        {
            if (e.IsLeftMouseButtonClicked && ForkLiftsManager.Instance.SelectedForkLift != null
                && ForkLiftsManager.Instance.SelectedForkLift.HasPalette
                && ForkLiftsManager.Instance.SelectedForkLift.CanMove()
                && e.MouseClickedObject.TryGetComponent<Truck>(out Truck clickedTruck)
                && ForkLiftsManager.Instance.SelectedForkLift.CanUnloadPaletteOnTruck(clickedTruck))
            {
                ForkLiftsManager.Instance.SelectedForkLift.UnloadPaletteOnTruck(clickedTruck);
            }
        }
    }

}
