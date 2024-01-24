using UnityEngine;
using UnityEditor;

public class CubeCreatorAndSaver : MonoBehaviour
{
    [MenuItem("Custom/Create Cube Mesh")]
    static void CreateAndSaveCubeMesh()
    {
        // Create a new cube mesh
        Mesh cubeMesh = CreateCubeMesh();

        // Save the cube mesh as an asset
        SaveMeshAsset(cubeMesh, "CubeMesh");
    }

    static Mesh CreateCubeMesh()
    {
        Mesh mesh = new Mesh();

        // Define vertices for a cube
        Vector3[] vertices = new Vector3[]
        {
            // Bottom face
            new Vector3(0, 0, 0),
            new Vector3(1, 0, 0),
            new Vector3(1, 0, 1),
            new Vector3(0, 0, 1),

            // Top face
            new Vector3(0, 1, 0),
            new Vector3(1, 1, 0),
            new Vector3(1, 1, 1),
            new Vector3(0, 1, 1),
        };

        // Define triangles for the cube
        int[] triangles = new int[]
        {
            // Bottom face
            0, 2, 1,
            0, 3, 2,

            // Top face
            4, 5, 6,
            4, 6, 7,

            // Side faces
            0, 1, 5,
            0, 5, 4,
            1, 2, 6,
            1, 6, 5,
            2, 3, 7,
            2, 7, 6,
            3, 0, 4,
            3, 4, 7,
        };

        // Set mesh properties
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        // You can set other properties like normals, UVs, etc. if needed

        return mesh;
    }

    static void SaveMeshAsset(Mesh mesh, string assetName)
    {
        // Create a new asset and save the mesh
        string path = "Assets/Meshes/" + assetName + (int)Random.Range(0, 10000) + ".asset";
        AssetDatabase.CreateAsset(mesh, path);
        AssetDatabase.SaveAssets();
    }
}