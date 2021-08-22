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
            foreach (var chunk in chunks) {
                DestroyImmediate(chunk);
            }
            chunks.Clear();
            
            // Create quadtree
            QuadTree qt = new QuadTree(Vector2Int.zero, 512);
            // Insert player into quadtree
            qt.Insert(Vector2.zero);

            // Generate new chunks
            foreach (var node in qt.GetChildNodes()) {
                // Generate chunk with size and resolution
                var map = mapGen.GenerateMap(node.Value.bounds.width, node.Key);
                var meshData = MeshGenerator.GenerateMesh(map, mapGen.heightMultiplier, 64);
                var chunk = new Chunk(node.Key, meshData);
                chunk.DisplayChunk(mapGen.display.meshRenderer.sharedMaterial);
                
                chunks.Add(chunk.meshObject);
            }
        }
    }
}
