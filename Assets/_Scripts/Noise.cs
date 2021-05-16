using System;
using System.Collections;
using System.Collections.Generic;
using SimplexNoise;
using UnityEngine;

public static class Noise {
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, Vector2 offset, float scale, int octaves, float 
    persistence,
     float lacunarity, float multiplier, int seed) {
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
}