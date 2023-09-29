using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TrucksManager : MonoBehaviour
{

    public static TrucksManager Instance { get; private set; }

    [SerializeField] private Truck truck;
    [SerializeField] private PlayableDirector truckArriveTimeline;
    [SerializeField] private PlayableDirector truckDepartureTimeline;

    private bool isDriving = false;

    public bool IsDriving { get => this.isDriving; }

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
        this.truckArriveTimeline.stopped += TruckArriveTimeline_stopped;
        this.truckDepartureTimeline.stopped += TruckDepartureTimeline_stopped;
        if (UserControl.Instance != null)
        {
            UserControl.Instance.OnMouseHover += UserControl_OnMouseHover;
            UserControl.Instance.OnMouseClick += UserControl_OnMouseClicked;
        }
    }

    private void OnDestroy()
    {
        this.truckArriveTimeline.stopped -= TruckArriveTimeline_stopped;
        this.truckDepartureTimeline.stopped -= TruckDepartureTimeline_stopped;
        if (UserControl.Instance != null)
        {
            UserControl.Instance.OnMouseHover -= UserControl_OnMouseHover;
            UserControl.Instance.OnMouseClick -= UserControl_OnMouseClicked;
        }
    }

    private void UserControl_OnMouseHover(object sender, UserControl.OnMouseHoverEventArgs e)
    {
        if (ForkliftsManager.Instance != null && UIMainScene.Instance != null)
        {
            if (ForkliftsManager.Instance.SelectedForklift != null
                && ForkliftsManager.Instance.SelectedForklift.HasPalette
                && ForkliftsManager.Instance.SelectedForklift.CanMove()
                && e.MouseHoveredObject.TryGetComponent<ITruck>(out ITruck hoveredTruck)
                && GameManager.Instance.CanUnloadPaletteFromForkliftToTruck(ForkliftsManager.Instance.SelectedForklift, hoveredTruck))
            {
                UIMainScene.Instance.ShowUnloadOnTruckHoverText(hoveredTruck.InfoPosition);
            }
            else
            {
                UIMainScene.Instance.HideUnloadOnTruckHoverText();
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
                && e.MouseClickedObject.TryGetComponent<ITruck>(out ITruck clickedTruck)
                && GameManager.Instance.CanUnloadPaletteFromForkliftToTruck(ForkliftsManager.Instance.SelectedForklift, clickedTruck))
            {
                GameManager.Instance.MoveForkliftToUnloadPaletteToTruck(ForkliftsManager.Instance.SelectedForklift, clickedTruck);
            }
        }
    }

    private void TruckArriveTimeline_stopped(PlayableDirector timeline)
    {
        this.isDriving = false;
    }

    private void TruckDepartureTimeline_stopped(PlayableDirector timeline)
    {
        this.isDriving = false;
        this.truck.ClearPalletes();
        GameManager.Instance.StartNextTruckShippment();
    }

    public void DoArrive()
    {
        this.isDriving = true;
        this.truckArriveTimeline.Play();
    }

    public void DoDeparture()
    {
        this.isDriving = true;
        this.truckDepartureTimeline.Play();
    }

}
