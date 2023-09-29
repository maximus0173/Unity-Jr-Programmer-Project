using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TruckShippingManager : MonoBehaviour
{

    [System.Serializable]
    public struct Shipping
    {
        public Truck truck;
        public Pallet[] pallets;
        public int timeInSeconds;
    }

    public static TruckShippingManager Instance { get; private set; }

    [SerializeField] private Canvas canvas;

    [SerializeField] private Text shipmentText;

    [SerializeField] private PalletTime palletTimePrefab;

    [SerializeField] private List<Shipping> shipments = new List<Shipping>();

    private float elapsedTime = 0f;

    private int currentShippingIndex = -1;

    private List<PalletTime> uiPalleteTimes = new List<PalletTime>();

    public int ElapsedTime { get => (int)this.elapsedTime; }

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
        if (this.shipments.Count > 0)
        {
            GameManager.Instance.StartNextTruckShippment();
        }
    }

    private void Update()
    {
        this.elapsedTime += Time.deltaTime;
    }

    public void Generate()
    {
        Shipping newShipping = new Shipping();
        newShipping.timeInSeconds = 60;
        List<Pallet> pallets = GameObject.FindObjectsOfType<Pallet>().ToList();
        foreach (Shipping shipping in this.shipments)
        {
            foreach (Pallet shippingPallet in shipping.pallets)
            {
                pallets.Remove(shippingPallet);
            }
            newShipping.truck = shipping.truck;
            newShipping.timeInSeconds = shipping.timeInSeconds + 60;
        }
        newShipping.pallets = new Pallet[3];
        for (int i = 0; i < 3; i++)
        {
            int p = Random.Range(0, pallets.Count - 1);
            Pallet pallet = pallets[p];
            newShipping.pallets[i] = pallet;
            pallets.Remove(pallet);
        }
        this.shipments.Add(newShipping);
    }

    private void StartShippment(int shippingIndex)
    {
        this.currentShippingIndex = shippingIndex;
        TrucksManager.Instance.DoArrive();
        UpdateUi();
    }

    public void StartNextShippment()
    {
        if (this.currentShippingIndex < 0 && this.shipments.Count > 0)
        {
            StartShippment(0);
            return;
        }
        if (this.currentShippingIndex + 1 >= this.shipments.Count)
        {
            return;
        }
        StartShippment(this.currentShippingIndex + 1);
        this.UpdateShipment();
    }

    public void EndCurrentShippment()
    {
        TrucksManager.Instance.DoDeparture();
    }

    private void UpdateUi()
    {
        foreach (PalletTime palletTime in this.uiPalleteTimes)
        {
            Destroy(palletTime.gameObject);
        }
        this.uiPalleteTimes.Clear();
        int i = this.currentShippingIndex;
        int p = 1;
        while (i < this.shipments.Count && p <= 3)
        {
            Shipping shipping = this.shipments[i];
            CreateUiPalletTimeForShipping(p, shipping);
            i++;
            p++;
        }
    }

    private void CreateUiPalletTimeForShipping(int priority, Shipping shipping)
    {
        foreach (Pallet pallet in shipping.pallets)
        {
            GameObject palletTimeGo = Instantiate(this.palletTimePrefab.gameObject);
            RectTransform palletTimeTransform = palletTimeGo.GetComponent<RectTransform>();
            palletTimeTransform.SetParent(this.canvas.transform);
            PalletTime palletTime = palletTimeGo.GetComponent<PalletTime>();
            palletTime.FollowPallet(priority, pallet, shipping.timeInSeconds);
            this.uiPalleteTimes.Add(palletTime);
        }
    }

    public bool IsPalletToShipping(IPallet pallet)
    {
        if (this.currentShippingIndex < 0 || this.currentShippingIndex >= this.shipments.Count)
        {
            return false;
        }
        Shipping shipping = this.shipments[this.currentShippingIndex];
        return shipping.pallets.Contains(pallet);
    }

    private void UpdateShipment()
    {
        this.shipmentText.text = "Shipment: " + (this.currentShippingIndex + 1);
    }

}
