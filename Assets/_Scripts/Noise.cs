using System;
using System.Collections;
using System.Collections.Generic;
using SimplexNoise;
using UnityEngine;
using Random = System.Random;

public static class Noise {
    public static float[,] GenerateBaseMap(int mapWidth, int mapHeight, Vector2 offset, float tileSize) {
        float[,] baseMap = new float[mapWidth, mapHeight];

        // Loop through each pixel in current map
        for (int col = 0; col < mapHeight; col++) {
            for (int row = 0; row < mapWidth; row++) {
                // Get pixel's nearest tile
                var nearestTile = new Vector2(Mathf.Floor((offset.x + row) / tileSize), Mathf.Floor((offset.y + col) / tileSize));
                var center = RandomVector(nearestTile);
                var pixelPosInTile = new Vector2((offset.x + row) / tileSize, (offset.y + col) / tileSize) - nearestTile;

                var diff = center - pixelPosInTile;

                baseMap[row, col] = 1 - diff.magnitude;
            }
        }

        // Declare min dist
        // For each surrounding tile
            // Get neighbor index
            // Hash tile to get pseudorandom offset
            // difference = neighbor + random - current pixel
            // distance = x * x + y * y
            // Store min distance
        // Set basemap color

        /*        
        var rp = RandomVector(new Vector2(32, 32)) * mapWidth;
        Debug.Log(rp);
        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;
        
        for (int col = 0; col < mapHeight; col++) {
            for (int row = 0; row < mapWidth; row++) {
                float minDist = float.MaxValue;
                
                var sampleX = row - halfWidth + offset.x;
                var sampleY = col - halfHeight - offset.y;
                
                var dist = Vector2.Distance(rp, new Vector2(sampleX, sampleY));
                minDist = Mathf.Min(minDist, dist / mapWidth);

                baseMap[row, col] += minDist;
            }
        }
        
        */

        return baseMap;
    }
    
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, Vector2 offset, float scale, int octaves, float 
    persistence, float lacunarity, float multiplier, int seed) {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        OpenSimplexNoise simplexNoise = new OpenSimplexNoise(seed);

        float amplitude = 1;
        float frequency = 1;
        
        // Minimum scale
        if (scale <= 0) {
            scale = 0.0001f;
        }

        // Save max and min values of height
        float maxNoiseHeight = 0;

        // Save half of width and height
        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        for (int i = 0; i < octaves; i++) {
            maxNoiseHeight += amplitude;
            amplitude *= persistence;
        }

        // Loop through each vertex
        for (int col = 0; col < mapHeight; col++) {
            for (int row = 0; row < mapWidth; row++) {
                amplitude = 1;
                frequency = 1;
                float noiseHeight = 0;
                
                // Apply fractional brownian motion
                for (int i = 0; i < octaves; i++) {
                    float sampleX = (row - halfWidth + offset.x) / scale * frequency;
                    float sampleY = (col - halfHeight - offset.y) / scale * frequency;
                    
                    // Get noise and adjust range between -1 and 1
                    float noiseVal = (float) simplexNoise.Evaluate(sampleX, sampleY) * 2 - 1;
                    
                    noiseHeight += noiseVal * amplitude;
                    
                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                // Set noise height
                noiseMap[row, col] = noiseHeight;
            }
        }

        // Normalize noise values
        for (int col = 0; col < mapHeight; col++) {
            for (int row = 0; row < mapWidth; row++) {
                float normalizedHeight = (noiseMap[row, col] + 1) / (2f * maxNoiseHeight);
                noiseMap[row, col] = Mathf.Pow(normalizedHeight, multiplier);
            }
        }

        return noiseMap;
    }

    /// <summary>
    /// Generates a pseudorandom 2D vector based on a 2D vector seed between 0 and 1
    /// </summary>
    /// <param name="seed"></param>
    /// <returns></returns>
    private static Vector2 RandomVector(Vector2 seed) {
        seed = new Vector2(
                           Vector2.Dot(seed, new Vector2(127.1f, 311.7f)),
                           Vector2.Dot(seed, new Vector2(269.5f, 183.3f))
                           );
        var x = new Vector2(Mathf.Sin(seed.x), Mathf.Sin(seed.y));
        return new Vector2(x.x - Mathf.Floor(x.x), x.y - Mathf.Floor(x.y));
    }
}
