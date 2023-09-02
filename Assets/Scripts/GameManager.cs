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

    public bool CanLoadPaletteToForklift(IPallet pallet, IForklift forklift)
    {
        if (pallet == null || forklift == null)
        {
            return false;
        }
        Pallet rootPalette = pallet as Pallet;
        Forklift rootForklift = forklift as Forklift;
        if (!rootPalette.CanBeTransported())
        {
            return false;
        }
        if (!rootForklift.CanLoadPalette(pallet))
        {
            return false;
        }
        return true;
    }

    public void MoveForkliftToLoadPalette(IPallet pallet, IForklift forklift)
    {
        if (!CanLoadPaletteToForklift(pallet, forklift))
        {
            throw new System.Exception("Cannot load pallet to forklift");
        }
        Pallet rootPalette = pallet as Pallet;
        Forklift rootForklift = forklift as Forklift;
        rootForklift.MoveToLoadPalette(pallet);
        rootPalette.PendingTransportByForklift(forklift);
    }

    public void CancelMoveForkliftToLoadPalette(IPallet pallet, IForklift forklift)
    {
        if (!pallet.IsPendingTransport)
        {
            throw new System.Exception("Cannot cancel pending transport pallet by forklift");
        }
        Pallet rootPalette = pallet as Pallet;
        Forklift rootForklift = forklift as Forklift;
        rootPalette.CancelPendingTransportByForklift();
    }

    public void LoadPaletteToForklift(IPallet pallet, IForklift forklift)
    {
        if (!pallet.IsPendingTransport)
        {
            throw new System.Exception("Cannot load pallet to forklift because pallet is not pending to transport");
        }
        Pallet rootPalette = pallet as Pallet;
        Forklift rootForklift = forklift as Forklift;
        if (rootPalette.IsMountedToRack)
        {
            IRack rack = rootPalette.MountedToRack;
            Rack rootRack = rack as Rack;
            rootPalette.UnmountFromRack();
            rootRack.UnmountPalette(pallet);
        }
        rootPalette.LoadToForklift(forklift);
    }

    public bool CanUnloadPaletteFromForkliftToTruck(IForklift forklift, ITruck truck)
    {
        if (!forklift.HasPalette)
        {
            return false;
        }
        IPallet pallet = forklift.LoadedPallet;
        Pallet rootPalette = forklift.LoadedPallet as Pallet;
        Forklift rootForklift = forklift as Forklift;
        Truck rootTruck = truck as Truck;
        if (!rootPalette.IsTransportedByForklift)
        {
            return false;
        }
        if (!rootForklift.HasPalette || rootForklift.LoadedPallet != pallet)
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
            throw new System.Exception("Cannot move forklift to unload pallet to truck");
        }
        IPallet pallet = forklift.LoadedPallet;
        Pallet rootPallet = forklift.LoadedPallet as Pallet;
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
            throw new System.Exception("Cannot unload pallet from forklift to truck");
        }
        IPallet pallet = forklift.LoadedPallet;
        Pallet rootPalette = forklift.LoadedPallet as Pallet;
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

    public void MountPaletteToRack(IPallet pallet, IRack rack)
    {
        Pallet rootPallet = pallet as Pallet;
        Rack rootRack = rack as Rack;
        rootPallet.MountToRack(rack);
        rootRack.MountPallet(pallet);
    }

    public void UnmountPalletFromRack(IPallet pallet, IRack rack)
    {
        Pallet rootPallet = pallet as Pallet;
        Rack rootRack = rack as Rack;
        rootPallet.UnmountFromRack();
        rootRack.UnmountPalette(pallet);
    }

    public bool CanUnloadPaletteFromForkliftToRack(IForklift forklift, IRack rack)
    {
        if (!forklift.HasPalette)
        {
            return false;
        }
        IPallet pallet = forklift.LoadedPallet;
        Pallet rootPalette = forklift.LoadedPallet as Pallet;
        Forklift rootForklift = forklift as Forklift;
        Rack rootRack = rack as Rack;
        if (!rootPalette.IsTransportedByForklift)
        {
            return false;
        }
        if (!rootForklift.HasPalette || rootForklift.LoadedPallet != pallet)
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
            throw new System.Exception("Cannot move forklift to unload pallet to truck");
        }
        IPallet pallet = forklift.LoadedPallet;
        Pallet rootPalette = forklift.LoadedPallet as Pallet;
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
            throw new System.Exception("Cannot unload pallet from forklift to rack");
        }
        IPallet pallet = forklift.LoadedPallet;
        Pallet rootPalette = forklift.LoadedPallet as Pallet;
        Forklift rootForklift = forklift as Forklift;
        Rack rootRack = rack as Rack;
        MountPaletteToRack(pallet, rack);
        rootPalette.UnloadFromForklift();
    }

    public void CompleteReserveationOfRackToUnloadPaletteFromForklift(IForklift forklift, IRack rack)
    {
        Rack rootRack = rack as Rack;
        rootRack.CompleteReservationForUnloadPaletteFromForklift(forklift);
    }

}
