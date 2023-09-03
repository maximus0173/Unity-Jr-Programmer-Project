using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TruckShippingManager))]
public class TruckShippingManagerEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TruckShippingManager truckShippingManager = (TruckShippingManager)target;
        if (GUILayout.Button("Generate shipments"))
        {
            truckShippingManager.Generate();
        }
    }

}
