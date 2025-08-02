using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy prefabs to choose from")]
    public List<GameObject> enemyPrefabs;

    [Header("Fixed spawn points in the scene")]
    public List<Transform> spawnPoints;

    [Header("Chance settings (0 to 1, e.g. 0.5 = 50%)")]
    public float spawnChance = 0.5f;

    // Track spawned enemies if you want to reset them later
    private List<GameObject> spawnedEnemies = new List<GameObject>();

    public void RerollSpawnPoints()
    {
        foreach (Transform point in spawnPoints)
        {
            // Optional: Clear enemy if one already exists at this point
            Collider[] nearby = Physics.OverlapSphere(point.position, 0.1f);
            foreach (Collider col in nearby)
            {
                if (col.CompareTag("Enemy"))  // tag your enemies
                {
                    Destroy(col.gameObject);
                }
            }

            // 50% chance to spawn
            if (Random.value <= spawnChance)
            {
                GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
                GameObject enemy = Instantiate(prefab, point.position, Quaternion.identity);
                enemy.tag = "Target"; // Ensure tagging for clean-up
                spawnedEnemies.Add(enemy);
            }
        }
    }
}
