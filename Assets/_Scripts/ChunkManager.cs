using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Scripts;
using UnityEngine;

public class ChunkManager : MonoBehaviour {
    public Material meshMaterial;
    
    public static MapGenerator mapGenerator;
    public Transform player;

    public int chunkViewDist = 3;
    public int chunkSize = 64;
    public int chunkResolution = 64;
    
    private Dictionary<Vector2Int, Chunk> chunks = new Dictionary<Vector2Int, Chunk>();
    
    // Start is called before the first frame update
    void Start() {
        mapGenerator = GetComponent<MapGenerator>();
    }

    // Update is called once per frame
    void Update() {
        // Create quadtree
        QuadTree qt = new QuadTree(Vector2Int.zero, 512);
        // Insert player into quadtree
        qt.Insert(new Vector2(player.transform.position.x, player.transform.position.z));

        var updatedChunks = new Dictionary<Vector2Int, Chunk>();
        // Get all visible chunks
        var visibleChunks = qt.GetChildNodes();
        
        // Set intersection of current chunks and visible chunks
        var existingChunks = chunks.Keys.Intersect(visibleChunks.Keys);
        var newChunks = visibleChunks.Keys.Except(chunks.Keys);
        // Old, non-visible chunks
        var oldChunks = chunks.Keys.Except(visibleChunks.Keys);

        // TODO: Object pool
        foreach (var oldChunk in oldChunks) {
            Destroy(chunks[oldChunk].meshObject);
        }

        // Start off with chunks we already have
        foreach (var chunkKey in existingChunks) {
            updatedChunks.Add(chunkKey, chunks[chunkKey]);
        }
        
        // Generate new chunks
        foreach (var chunkKey in newChunks) {
            // Generate chunk with size and resolution
            updatedChunks.Add(chunkKey, GenerateChunk(chunkKey, visibleChunks[chunkKey].bounds.width));
        }

        chunks = updatedChunks;
    }

    /// <summary>
    /// Generates a chunk at a given chunk key
    /// </summary>
    /// <param name="chunkKey">Chunk key</param>
    /// <param name="size">Size of chunk</param>
    /// <returns>Chunk object containing mesh</returns>
    private Chunk GenerateChunk(Vector2 chunkKey, int size) {
        // TODO: Variable chunk resolution based on chunk size
        // Generate heightmap for chunk
        var heightMap = mapGenerator.GenerateMap(size, chunkKey);
        // Generate mesh for chunk
        var meshData = MeshGenerator.GenerateMesh(heightMap, mapGenerator.heightMultiplier, QuadTree.minNodeSize);
        
        return new Chunk(chunkKey, meshData, chunkSize, meshMaterial);
    }
    
    private Vector2 GetChunk(Vector3 position) {
        float chunkX = Mathf.FloorToInt((position.x + chunkSize / 2f) / (chunkSize));
        float chunkZ = Mathf.FloorToInt((position.z + chunkSize / 2f) / (chunkSize));

        return new Vector2(chunkX, chunkZ);
    }

    class Chunk {
        public GameObject meshObject;
        Vector2 position;

        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;
        private MeshData meshData;

        public Chunk(Vector2 position, MeshData meshData, int chunkSize, Material meshMaterial) {
            this.meshData = meshData;
            this.position = position;
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