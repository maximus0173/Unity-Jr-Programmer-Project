using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckShippingManager : MonoBehaviour
{

    [System.Serializable]
    public struct Shipping
    {
        public IPallet[] pallets;
        public int timeInSeconds;
    }

    public static TruckShippingManager Instance { get; private set; }

    [SerializeField] List<Shipping> shipments = new List<Shipping>();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Generate()
    {

    }

}
