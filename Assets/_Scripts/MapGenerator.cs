using UnityEngine;

public class MapGenerator : MonoBehaviour {
    public MapDisplay display;
    
    [Space(10)]
    public float noiseScale;
    public int octaves;
    [Range(0f, 1f)]
    public float persistence;
    public float lacunarity;
    public float heightMultiplier;
    public float multiplier = 1;
    public int seed;

    [Space(10)]
    public bool autoGenerate = false;

    public float[,] GenerateMap(int chunkSize, Vector2 offset) {
        return Noise.GenerateNoiseMap(chunkSize + 1, chunkSize + 1, offset, noiseScale, octaves, 
        persistence, lacunarity, multiplier, seed);
    }

    public void DisplayMap(float[,] noiseMap, Vector2 offset) {
    }

    private void OnValidate() {
        if (lacunarity < 1) lacunarity = 1;
        if (octaves < 1) octaves = 1;
        if (multiplier < 0) multiplier = 0;
        if (heightMultiplier < 0) heightMultiplier = 0;
    }
}