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
    public int tileSize = 33;
    public int chunkResolution = 64;
    
    private Dictionary<Vector2Int, Chunk> chunks = new Dictionary<Vector2Int, Chunk>();

    private Dictionary<Vector2Int, Chunk> chunkPool = new Dictionary<Vector2Int, Chunk>();
    
    // Start is called before the first frame update
    void Start() {
        mapGenerator = GetComponent<MapGenerator>();
    }

    // Update is called once per frame
    void Update() {
        var playerPos = player.transform.position;
        var playerChunkPos = new Vector2Int(Mathf.RoundToInt(playerPos.x / chunkSize), 
                                            Mathf.RoundToInt(playerPos.z / chunkSize));

        // TODO: Chunk pool
        for (int i = -chunkViewDist; i <= chunkViewDist; i++) {
            for (int j = -chunkViewDist; j <= chunkViewDist; j++) {
                var offset = new Vector2Int(i, j);
                if (chunks.ContainsKey(playerChunkPos + offset)) continue;

                var chunk = GenerateChunk(playerChunkPos + offset, chunkSize);
                chunk.DisplayChunk(meshMaterial);
                chunks.Add(playerChunkPos + offset, chunk);
            }
        }
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
        var heightMap = mapGenerator.GenerateMap(size, Vector2.Scale(chunkKey * size, new Vector2(1, -1)), tileSize);
        // Generate mesh for chunk
        var meshData = MeshGenerator.GenerateMesh(heightMap, mapGenerator.heightMultiplier, chunkSize);

        return new Chunk(chunkKey * size, meshData);
    }
    
    private Vector2 GetChunk(Vector3 position) {
        float chunkX = Mathf.FloorToInt((position.x + chunkSize / 2f) / (chunkSize));
        float chunkZ = Mathf.FloorToInt((position.z + chunkSize / 2f) / (chunkSize));

        return new Vector2(chunkX, chunkZ);
    }

    public class Chunk {
        public GameObject meshObject;
        Vector2 position;

        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;
        public MeshData meshData;

        public Chunk(Vector2 position, MeshData meshData) {
            this.meshData = meshData;
            this.position = position;
            this.meshData = meshData;
        }

        public void DisplayChunk(Material meshMaterial) {
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