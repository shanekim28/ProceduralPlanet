using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Scripts;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using Chunk = ChunkManager.Chunk;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor {
    private List<GameObject> chunks = new List<GameObject>();
    
    public override void OnInspectorGUI() {
        
        MapGenerator mapGen = (MapGenerator) target;

        DrawDefaultInspector();

        if (GUILayout.Button("Generate")) {
            var x = mapGen.GenerateMap(32, new Vector2(64, 0),  32);
            var y = MeshGenerator.GenerateMesh(x, 1, 32);
            mapGen.display.DrawMesh(y);
            mapGen.DisplayMap(x, Vector2.zero);
        }
    }
}
