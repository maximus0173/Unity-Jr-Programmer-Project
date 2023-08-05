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
        if (ForkLiftsManager.Instance != null && UIMainScene.Instance != null)
        {
            if (ForkLiftsManager.Instance.SelectedForkLift != null
                && e.MouseHoveredObject.TryGetComponent<Palette>(out Palette hoveredPalette)
                && hoveredPalette.CanBeTransported()
                && ForkLiftsManager.Instance.SelectedForkLift.CanLoadPalette(hoveredPalette))
            {
                UIMainScene.Instance.ShowLoadHoverText(hoveredPalette.transform.position);
            }
            else
            {
                UIMainScene.Instance.HideLoadHoverText();
            }
        }
    }

    private void UserControl_OnMouseClicked(object sender, UserControl.OnMouseClickEventArgs e)
    {
        if (ForkLiftsManager.Instance != null)
        {
            if (e.IsLeftMouseButtonClicked && ForkLiftsManager.Instance.SelectedForkLift != null
                && e.MouseClickedObject.TryGetComponent<Palette>(out Palette clickedPalette)
                && clickedPalette.CanBeTransported()
                && ForkLiftsManager.Instance.SelectedForkLift.CanLoadPalette(clickedPalette))
            {
                ForkLiftsManager.Instance.SelectedForkLift.LoadPalette(clickedPalette);
            }
        }
    }

}
