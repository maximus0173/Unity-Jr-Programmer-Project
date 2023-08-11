using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PalettesManager : MonoBehaviour
{

    public static PalettesManager Instance { get; private set; }

    private List<Palette> allPalettes = new List<Palette>();

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

    public void RegisterPalette(Palette palette)
    {
        if (this.allPalettes.Contains(palette))
        {
            return;
        }
        this.allPalettes.Add(palette);
    }

    public void UnregisterPalette(Palette palette)
    {
        if (!this.allPalettes.Contains(palette))
        {
            return;
        }
        this.allPalettes.Remove(palette);
    }

    private void UserControl_OnMouseHover(object sender, UserControl.OnMouseHoverEventArgs e)
    {
        if (ForkliftsManager.Instance != null && UIMainScene.Instance != null)
        {
            if (ForkliftsManager.Instance.SelectedForklift != null
                && e.MouseHoveredObject.TryGetComponent<IPalette>(out IPalette hoveredPalette)
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
                && e.MouseClickedObject.TryGetComponent<IPalette>(out IPalette clickedPalette)
                && GameManager.Instance.CanLoadPaletteToForklift(clickedPalette, ForkliftsManager.Instance.SelectedForklift))
            {
                GameManager.Instance.MoveForkliftToLoadPalette(clickedPalette, ForkliftsManager.Instance.SelectedForklift);
            }
        }
    }

}
