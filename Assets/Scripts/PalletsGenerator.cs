using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class PalletsGenerator : MonoBehaviour
{

    [SerializeField] private GameObject palletePrefab;

    [SerializeField] private int palletsCount = 30;

    public void Generate()
    {
#if UNITY_EDITOR
        int loopCount = 10000;
        while (transform.childCount > 0 && loopCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
            loopCount++;
        }

        List<Rack> racks = GameObject.FindObjectsOfType<Rack>().ToList();
        int actualPalletsCount = this.palletsCount;
        if (racks.Count < actualPalletsCount)
        {
            actualPalletsCount = racks.Count;
        }
        for (int i = 0; i < actualPalletsCount; i++)
        {
            int rackIndex = Random.Range(0, racks.Count - 1);
            Rack rack = racks[rackIndex];
            GameObject pallete = PrefabUtility.InstantiatePrefab(this.palletePrefab, gameObject.transform) as GameObject;
            pallete.transform.position = rack.Position;
            pallete.transform.rotation = rack.Rotation;
            racks.Remove(rack);
        }
#endif
    }

}
