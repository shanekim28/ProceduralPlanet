using System;
using System.Collections;
using System.Collections.Generic;
using SimplexNoise;
using UnityEngine;

public static class Noise {
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, float scale) {
        SimplexNoise.Noise.Seed = 123456789;
        
        if (scale <= 0) {
            scale = 0.0001f;
        }

        return SimplexNoise.Noise.Calc2D(mapWidth, mapHeight, scale);
    }
}