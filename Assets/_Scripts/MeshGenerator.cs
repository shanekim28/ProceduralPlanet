using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator {
    public static MeshData GenerateMesh(float[,] heightMap, float heightFactor, int resolution) {
        // Dimensions of mesh
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        
        // Offset
        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1) / 2f;

        MeshData meshData = new MeshData(width, height);
        int vertexIndex = 0;
        
        int resStep = (width - 1) / resolution;
        int verticesPerLine = (width - 1) / resStep + 1;

        for (int col = 0; col < height; col += resStep) {
            for (int row = 0; row < width; row += resStep) {
                // Create vertex with height and set UVs
                meshData.vertices[vertexIndex] = new Vector3(topLeftX + row, heightMap[row, col] * 
                heightFactor, topLeftZ - col);
                meshData.uvs[vertexIndex] = new Vector2(row / (float) width, col / (float) height);

                // Add triangles within bounds
                if (row < width - 1 && col < height - 1) {
                    meshData.AddTriangle(vertexIndex, vertexIndex + verticesPerLine + 1, vertexIndex + verticesPerLine);
                    meshData.AddTriangle(vertexIndex, vertexIndex + 1, vertexIndex + verticesPerLine + 1);
                }
                
                vertexIndex++;
            }
        }

        return meshData;
    }
}

/// <summary>
/// Stores triangles, vertices, and uvs for a mesh
/// </summary>
public class MeshData {
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;

    int triangleIndex;

    public MeshData(int meshWidth, int meshHeight) {
        vertices = new Vector3[meshWidth * meshHeight];
        uvs = new Vector2[meshWidth * meshHeight];
        triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
    }

    public void AddTriangle(int v1, int v2, int v3) {
        triangles[triangleIndex] = v1;
        triangles[triangleIndex + 1] = v2;
        triangles[triangleIndex + 2] = v3;
        
        triangleIndex += 3;
    }

    public Mesh CreateMesh() {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        return mesh;
    }
}