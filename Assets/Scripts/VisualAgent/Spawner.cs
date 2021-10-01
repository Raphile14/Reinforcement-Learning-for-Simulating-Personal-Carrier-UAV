using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    float spawnEvery = 3f;
    public GameObject obstaclePrefab;
    float[] acceptableSpawnpoints = new float[] { -2.5f, 0f, 2.5f };

    void Start()
    {
        StartCoroutine(SpawnObstacle(spawnEvery));
    }

    private IEnumerator SpawnObstacle(float spawnEvery)
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnEvery);
            GameObject obstacle = Instantiate(obstaclePrefab, this.transform);
            obstacle.transform.localPosition = new Vector3(acceptableSpawnpoints[Random.Range(0, acceptableSpawnpoints.Length)], 0.5f, 20f);
        }
    }
}
