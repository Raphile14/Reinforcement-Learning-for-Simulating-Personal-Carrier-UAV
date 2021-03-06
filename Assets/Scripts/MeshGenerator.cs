using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    NavMeshSurface surface;
    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;
    MeshCollider meshCollider;
    public GameObject treeContainer;
    public GameObject[] treePrefabs;
    GameObject[] treeObjects;
    public int maxTrees = 30;
    public int minTrees = 20;
    int countTrees = 0;
    public int xSize = 100;
    public int zSize = 100;
    int layerMask;
    bool instantiatedShape = false;
    bool instantiatedMesh = false;
    bool instantiatedTrees = false;
    bool instantiatedNavMesh = false;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        meshCollider = GetComponent<MeshCollider>();
        surface = GetComponent<NavMeshSurface>();
        layerMask = LayerMask.GetMask("GroundMesh");
    }

    public void CreateShape()
    {
        if (instantiatedShape) return;
        int xSeed = Random.Range(-1000, 1000);
        int zSeed = Random.Range(-1000, 1000);
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = Mathf.PerlinNoise((x + xSeed) * .1f, (z + zSeed) * .1f) * 4f;
                vertices[i] = new Vector3(x - (xSize / 2), y, z - (zSize / 2));
                i++;
            }
        }

        triangles = new int[xSize * zSize * 6];
        int vert = 0;
        int tris = 0;
        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris] = vert;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }
        instantiatedShape = true;
    }

    public void UpdateMesh()
    {
        if (instantiatedMesh) return;
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        meshCollider.sharedMesh = mesh;

        mesh.RecalculateNormals();
        instantiatedMesh = true;
    }

    public void UpdateNavMesh()
    {
        if (instantiatedNavMesh) return;
        surface.BuildNavMesh();
        instantiatedNavMesh = true;
    }

    public void GenerateTrees()
    {
        if (instantiatedTrees) return;
        countTrees = Random.Range(minTrees, maxTrees);
        float maxX = mesh.bounds.extents.x;
        float maxY = mesh.bounds.extents.y;        
        float maxZ = mesh.bounds.extents.z;

        // Delete Current Trees
        if (treeObjects != null)
        {
            for (int i = 0; i < treeObjects.Length; i++)
            {
                // Destroy(treeObjects[i]);
                Vector3 spawnArea = CheckArea(maxX, maxY, maxZ);
                treeObjects[i].transform.localPosition = spawnArea;
                treeObjects[i].transform.rotation = Quaternion.Euler(0, Random.Range(0, 180), 0);
            }
        }
        else
        {
            // Create Trees
            treeObjects = new GameObject[countTrees];
            for (int i = 0; i < countTrees; i++)
            {
                Vector3 spawnArea = CheckArea(maxX, maxY, maxZ);
                treeObjects[i] = Instantiate(treePrefabs[Random.Range(0, treePrefabs.Length - 1)], treeContainer.transform);
                treeObjects[i].transform.localPosition = spawnArea;
                treeObjects[i].transform.rotation = Quaternion.Euler(0, Random.Range(0, 180), 0);
            }
        }
        instantiatedTrees = true;
    }

    // Randomize Locations
    // Check area if tree is already there
    public Vector3 CheckArea(float maxX, float maxY, float maxZ)
    {
        float xCoord = Random.Range(-maxX, maxX);
        float zCoord = Random.Range(-maxZ, maxZ);

        if (xCoord < 10 && xCoord > -10)
        {
            if (xCoord >= 0 && xCoord < 10) xCoord += 10;
            else xCoord -= 10;
        }
        if (zCoord < 10 && zCoord > -10)
        {
            if (zCoord >= 0 && zCoord < 10) zCoord += 10;
            else zCoord -= 10;
        }

        RaycastHit hit;
        if (Physics.Raycast(new Vector3(xCoord, maxY, zCoord), Vector3.down, out hit, 20f, layerMask))
        {
            return hit.point;
        } 
        return CheckArea(maxX, maxY, maxZ);
        
    }
}
