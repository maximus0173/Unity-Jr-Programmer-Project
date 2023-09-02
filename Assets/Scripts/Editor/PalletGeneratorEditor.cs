using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PalletsGenerator))]
public class PalletGeneratorEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PalletsGenerator palletsGenerator = (PalletsGenerator)target;
        if (GUILayout.Button("Generate pallets"))
        {
            palletsGenerator.Generate();
        }
    }

}
