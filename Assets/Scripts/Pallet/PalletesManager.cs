using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PalletesManager : MonoBehaviour
{

    public static PalletesManager Instance { get; private set; }

    private List<Pallet> allPallets = new List<Pallet>();

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

    public void RegisterPalette(Pallet pallet)
    {
        if (this.allPallets.Contains(pallet))
        {
            return;
        }
        this.allPallets.Add(pallet);
    }

    public void UnregisterPalette(Pallet pallet)
    {
        if (!this.allPallets.Contains(pallet))
        {
            return;
        }
        this.allPallets.Remove(pallet);
    }

    private void UserControl_OnMouseHover(object sender, UserControl.OnMouseHoverEventArgs e)
    {
        if (ForkliftsManager.Instance != null && UIMainScene.Instance != null)
        {
            if (ForkliftsManager.Instance.SelectedForklift != null
                && e.MouseHoveredObject.TryGetComponent<IPallet>(out IPallet hoveredPalette)
                && GameManager.Instance.CanLoadPaletteToForklift(hoveredPalette, ForkliftsManager.Instance.SelectedForklift))
            {
                UIMainScene.Instance.ShowLoadHoverText(hoveredPalette.Position);
            }
            else
            {
                UIMainScene.Instance.HideLoadHoverText();
            }
        }
    }

    private void UserControl_OnMouseClicked(object sender, UserControl.OnMouseClickEventArgs e)
    {
        if (ForkliftsManager.Instance != null)
        {
            if (e.IsLeftMouseButtonClicked && ForkliftsManager.Instance.SelectedForklift != null
                && e.MouseClickedObject.TryGetComponent<IPallet>(out IPallet clickedPalette)
                && GameManager.Instance.CanLoadPaletteToForklift(clickedPalette, ForkliftsManager.Instance.SelectedForklift))
            {
                GameManager.Instance.MoveForkliftToLoadPalette(clickedPalette, ForkliftsManager.Instance.SelectedForklift);
            }
        }
    }

}
