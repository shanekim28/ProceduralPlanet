using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChunkManager : MonoBehaviour {
    public Material meshMaterial;
    
    public static MapGenerator mapGenerator;
    public Transform player;

    public int chunkViewDist = 3;
    public int chunkSize = 64;
    public int chunkResolution = 64;
    
    private Dictionary<Vector2, Chunk> chunks = new Dictionary<Vector2, Chunk>();
    
    // Start is called before the first frame update
    void Start() {
        mapGenerator = GetComponent<MapGenerator>();
    }

    // Update is called once per frame
    void Update() {
        // Get player's current chunk
        var currentChunk = GetChunk(player.transform.position);

        List<Vector2> visibleChunks = new List<Vector2>();

        // Get visible chunks
        for (int col = -chunkViewDist; col <= chunkViewDist; col++) {
            for (int row = -chunkViewDist; row <= chunkViewDist; row++) {
                visibleChunks.Add(new Vector2(currentChunk.x + row ,currentChunk.y + col));
            }
        }
        
        // Get difference of chunks
        var difference = visibleChunks.Except(chunks.Keys);

        foreach (var chunkKey in difference) {
            if (chunks.ContainsKey(chunkKey)) {
                continue;
            }
            
            chunks.Add(chunkKey, GenerateChunk(chunkKey));
        }
    }

    private Vector2 GetChunk(Vector3 position) {
        float chunkX = Mathf.FloorToInt((position.x + chunkSize / 2f) / (chunkSize));
        float chunkZ = Mathf.FloorToInt((position.z + chunkSize / 2f) / (chunkSize));

        return new Vector2(chunkX, chunkZ);
    }

    /// <summary>
    /// Generates a chunk at a given chunk key
    /// </summary>
    /// <param name="chunkKey">Chunk key</param>
    /// <returns>Chunk object containing mesh</returns>
    private Chunk GenerateChunk(Vector2 chunkKey) {
        // Generate heightmap for chunk
        var heightMap = mapGenerator.GenerateMap(chunkSize, chunkKey);
        // Generate mesh for chunk
        var meshData = MeshGenerator.GenerateMesh(heightMap, mapGenerator.heightMultiplier, chunkResolution);
        
        return new Chunk(chunkKey, meshData, chunkSize, meshMaterial);
    }

    class Chunk {
        GameObject meshObject;
        Vector2 position;

        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;
        private MeshData meshData;

        public Chunk(Vector2 position, MeshData meshData, int chunkSize, Material meshMaterial) {
            this.meshData = meshData;
            this.position = position * chunkSize;
            this.meshData = meshData;
            Vector3 posv3 = new Vector3(this.position.x, 0, this.position.y);

            meshObject = new GameObject($"Chunk {position.x} {position.y}");
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            
            // TODO: Multithread this
            meshFilter.sharedMesh = meshData.CreateMesh();
            meshRenderer.sharedMaterial = meshMaterial;
            // TODO: Set texture
            
            meshObject.transform.position = posv3;
        }

        public void UpdateChunk() {
            
        }
    }
}