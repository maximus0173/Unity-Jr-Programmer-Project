using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public bool CanLoadPaletteToForklift(IPalette palette, IForklift forklift)
    {
        if (palette == null || forklift == null)
        {
            return false;
        }
        Palette rootPalette = palette as Palette;
        Forklift rootForklift = forklift as Forklift;
        if (!rootPalette.CanBeTransported())
        {
            return false;
        }
        if (!rootForklift.CanLoadPalette(palette))
        {
            return false;
        }
        return true;
    }

    public void MoveForkliftToLoadPalette(IPalette palette, IForklift forklift)
    {
        if (!CanLoadPaletteToForklift(palette, forklift))
        {
            throw new System.Exception("Cannot load palette to forklift");
        }
        Palette rootPalette = palette as Palette;
        Forklift rootForklift = forklift as Forklift;
        rootForklift.MoveToLoadPalette(palette);
        rootPalette.PendingTransportByForklift(forklift);
    }

    public void CancelMoveForkliftToLoadPalette(IPalette palette, IForklift forklift)
    {
        if (!palette.IsPendingTransport)
        {
            throw new System.Exception("Cannot cancel pending transport palette by forklift");
        }
        Palette rootPalette = palette as Palette;
        Forklift rootForklift = forklift as Forklift;
        rootPalette.CancelPendingTransportByForklift();
    }

    public void LoadPaletteToForklift(IPalette palette, IForklift forklift)
    {
        if (!palette.IsPendingTransport)
        {
            throw new System.Exception("Cannot load palette to forklift because palette is not pending to transport");
        }
        Palette rootPalette = palette as Palette;
        Forklift rootForklift = forklift as Forklift;
        if (rootPalette.IsMountedToRack)
        {
            IRack rack = rootPalette.MountedToRack;
            Rack rootRack = rack as Rack;
            rootPalette.UnmountFromRack();
            rootRack.UnmountPalette(palette);
        }
        rootPalette.LoadToForklift(forklift);
    }

    public bool CanUnloadPaletteFromForkliftToTruck(IForklift forklift, ITruck truck)
    {
        if (!forklift.HasPalette)
        {
            return false;
        }
        IPalette palette = forklift.LoadedPalette;
        Palette rootPalette = forklift.LoadedPalette as Palette;
        Forklift rootForklift = forklift as Forklift;
        Truck rootTruck = truck as Truck;
        if (!rootPalette.IsTransportedByForklift)
        {
            return false;
        }
        if (!rootForklift.HasPalette || rootForklift.LoadedPalette != palette)
        {
            return false;
        }
        if (!rootForklift.CanUnloadPaletteOnTruck(truck))
        {
            return false;
        }
        if (!rootTruck.CanUnloadPaletteFromForklift(forklift))
        {
            return false;
        }
        return true;
    }

    public void MoveForkliftToUnloadPaletteToTruck(IForklift forklift, ITruck truck)
    {
        if (!CanUnloadPaletteFromForkliftToTruck(forklift, truck))
        {
            throw new System.Exception("Cannot move forklift to unload palette to truck");
        }
        IPalette palette = forklift.LoadedPalette;
        Palette rootPalette = forklift.LoadedPalette as Palette;
        Forklift rootForklift = forklift as Forklift;
        Truck rootTruck = truck as Truck;
        rootForklift.MoveToUnloadPaletteOnTruck(truck);
        rootTruck.ReserveForUnloadPaletteFromForklift(forklift);
    }

    public void CancelMoveForkliftToUnloadPaletteToTruck(IForklift forklift, ITruck truck)
    {

    }

    public void UnloadPaletteFromForkliftToTruck(IForklift forklift, ITruck truck)
    {
        if (!CanUnloadPaletteFromForkliftToTruck(forklift, truck))
        {
            throw new System.Exception("Cannot unload palette from forklift to truck");
        }
        IPalette palette = forklift.LoadedPalette;
        Palette rootPalette = forklift.LoadedPalette as Palette;
        Forklift rootForklift = forklift as Forklift;
        Truck rootTruck = truck as Truck;
        rootPalette.UnloadFromForklift();
        rootTruck.UnloadPaletteFromForklift(forklift);
        rootPalette.transform.parent = rootTruck.transform;
    }

    public void CompleteReserveationOfTruckToUnloadPaletteFromForklift(IForklift forklift, ITruck truck)
    {
        Truck rootTruck = truck as Truck;
        rootTruck.CompleteReservationForUnloadPaletteFromForklift(forklift);
    }

    public void MountPaletteToRack(IPalette palette, IRack rack)
    {
        Palette rootPalette = palette as Palette;
        Rack rootRack = rack as Rack;
        rootPalette.MountToRack(rack);
        rootRack.MountPalette(palette);
    }

    public void UnmountPaletteFromRack(IPalette palette, IRack rack)
    {
        Palette rootPalette = palette as Palette;
        Rack rootRack = rack as Rack;
        rootPalette.UnmountFromRack();
        rootRack.UnmountPalette(palette);
    }

    public bool CanUnloadPaletteFromForkliftToRack(IForklift forklift, IRack rack)
    {
        if (!forklift.HasPalette)
        {
            return false;
        }
        IPalette palette = forklift.LoadedPalette;
        Palette rootPalette = forklift.LoadedPalette as Palette;
        Forklift rootForklift = forklift as Forklift;
        Rack rootRack = rack as Rack;
        if (!rootPalette.IsTransportedByForklift)
        {
            return false;
        }
        if (!rootForklift.HasPalette || rootForklift.LoadedPalette != palette)
        {
            return false;
        }
        if (!rootForklift.CanUnloadPaletteOnRack(rack))
        {
            return false;
        }
        if (!rootRack.CanUnloadPaletteFromForklift(forklift))
        {
            return false;
        }
        return true;
    }

    public void MoveForkliftToUnloadPaletteToRack(IForklift forklift, IRack rack)
    {
        if (!CanUnloadPaletteFromForkliftToRack(forklift, rack))
        {
            throw new System.Exception("Cannot move forklift to unload palette to truck");
        }
        IPalette palette = forklift.LoadedPalette;
        Palette rootPalette = forklift.LoadedPalette as Palette;
        Forklift rootForklift = forklift as Forklift;
        Rack rootRack = rack as Rack;
        rootForklift.MoveToUnloadPaletteOnRack(rack);
        rootRack.ReserveForUnloadPaletteFromForklift(forklift);
    }

    public void CancelMoveForkliftToUnloadPaletteToRack(IForklift forklift, IRack rack)
    {

    }

    public void UnloadPaletteFromForkliftToRack(IForklift forklift, IRack rack)
    {
        if (!CanUnloadPaletteFromForkliftToRack(forklift, rack))
        {
            throw new System.Exception("Cannot unload palette from forklift to rack");
        }
        IPalette palette = forklift.LoadedPalette;
        Palette rootPalette = forklift.LoadedPalette as Palette;
        Forklift rootForklift = forklift as Forklift;
        Rack rootRack = rack as Rack;
        MountPaletteToRack(palette, rack);
        rootPalette.UnloadFromForklift();
    }

    public void CompleteReserveationOfRackToUnloadPaletteFromForklift(IForklift forklift, IRack rack)
    {
        Rack rootRack = rack as Rack;
        rootRack.CompleteReservationForUnloadPaletteFromForklift(forklift);
    }

}
