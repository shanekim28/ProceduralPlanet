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
    public float detailPower = 1;
    public float ridgePower = 1;
    public int seed;

    [Space(10)]
    public bool autoGenerate = false;

    public float[,] GenerateMap(int chunkSize, Vector2 offset, int tileSize) {
        var baseMap = Noise.GenerateBaseMap(chunkSize + 1, chunkSize + 1, offset, ridgePower, tileSize);
        var noiseMap = Noise.GenerateNoiseMap(chunkSize + 1, chunkSize + 1, offset, noiseScale, octaves, 
        persistence, lacunarity, detailPower, seed);

        float[,] res = new float[chunkSize + 1, chunkSize + 1];
        for (int i= 0; i< chunkSize + 1; i++) {
            for (int j = 0; j < chunkSize + 1; j++) {
                res[j, i] = (baseMap[j, i] + noiseMap[j, i]) * heightMultiplier;
                //res[j, i] = noiseMap[j, i];
                //res[j, i] = baseMap[j, i];
            }
        }

        return res;
    }

    public void DisplayMap(float[,] noiseMap, Vector2 offset) {
        display.DrawNoiseMap(noiseMap);
    }

    private void OnValidate() {
        if (lacunarity < 1) lacunarity = 1;
        if (octaves < 1) octaves = 1;
        if (detailPower < 0) detailPower = 0;
        if (heightMultiplier < 0) heightMultiplier = 0;
    }
}