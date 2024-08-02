using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Spawner : MonoBehaviour
{
    public GameObject[] EnemyPrefabs;
    public readonly static string[][] Waves = new string[][] {
        new string[] { "S,2","W,0.5","S,2","W,0.5","S,2","W,0.5","S,2","W,0.5","S,2","W,0.5","S,2","W,0.5","S,0","W,0.5","S,0","W,0.5","S,0","W,0.5","S,0","W,0.5","S,0","W,0.5","S,0","W,0.5","S,0"},
        new string[] { "S,0", "W,2.5", "S,0"},
        new string[] { "S,0", "W,2.5", "S,0", "W,2.5", "S,0", "W,2.5", "S,0" },
        new string[] { "S,0", "W,1", "S,0", "W,1", "S,0", "W,1", "S,0","W,1", "S,0", "W,1", "S,0" },
        new string[] { "S,0", "W,1.5", "S,0", "W,1.5", "B,0", "W,1.5", "S,0", "W,2.5", "S,0" },
        new string[] { "S,1", "W,2.5","S,0", "W,1.5", "S,0", "W,1.5", "S,0", "W,1.5", "S,0","W,2.5","S,1"},
        new string[] { "S,1", "W,1","S,0", "W,1", "S,1", "W,1", "S,1", "W,1", "S,0","W,1","S,1"}

    };
    private readonly int[] WaveLengths = new int[] {
        10,2,4,5,6,5,7,8,9,6,10
    };
    public static int EnemiesRemaining;
    private void Awake()
    {
        GameControl.StartWave.AddListener(HandleWaveStart);
    }
    public IEnumerator SpawnWave(int index)
    {
        foreach (string obj in Waves[index])
        {
            string[] splitWord = obj.Split(',');
            switch (splitWord[0])
            {
                case "S":
                    {
                        int spawnCoord = -Random.Range(3, 5);
                        GameObject enemyObject = Instantiate(EnemyPrefabs[int.Parse(splitWord[1])]);
                        Enemy enemy = enemyObject.GetComponent<Enemy>();
                        enemy.transform.position = new Vector3(0f, 9f, 0);
                        enemy.Position = new Vector2Int(spawnCoord, 7);
                        enemy.FollowPath(3, -6);
                    }
                    //Spawn and splitWord[1] is the enemy index.
                    break;
                case "B":
                    {
                        int difficulty = int.Parse(splitWord[1]);
                        int spawnCoord = -Random.Range(3, 5);
                        GameObject enemyObject = Instantiate(EnemyPrefabs[EnemyPrefabs.Length-1]);//Set to last EnemyPrefab. (will be Boss Prefab)
                        Enemy enemy = enemyObject.GetComponent<Enemy>();
                        enemy.Health = 5 * Mathf.Pow(difficulty + 2, 2) + 5; //using quadratic equation 5(x+2)^2+5. Will ensure slight exponential scaling to keep difficulty.
                        enemy.Type = "Boss " + difficulty.ToString();
                        enemy.transform.position = new Vector3(0f, 9f, 0);
                        enemy.Position = new Vector2Int(spawnCoord, 7);
                        enemy.FollowPath(3, -6);
                    }
                    break;
                    //handle Boss Event
                case "W":
                    yield return new WaitForSeconds(float.Parse(splitWord[1]));
                    break;
            }
        }
        yield break;
    }
    public IEnumerator WaitUntilWaveEnd()
    {
        EnemiesRemaining = WaveLengths[GameControl.CurrentWave];
        yield return new WaitUntil(() => EnemiesRemaining == 0);
        yield return new WaitForSeconds(1.5f);//cooldown for smoothness
        Debug.Log("Wave Finished!");
        GameControl.InWave = false;
        GameControl.CurrentWave++;
        //add UI indicators to make it easier.
    }
    private void HandleWaveStart()
    {
        StartCoroutine(SpawnWave(GameControl.CurrentWave));
        StartCoroutine(WaitUntilWaveEnd());
        // will add method at END of wave to add the progress etc.
    }
    //private List<string[]> WaveGeneration()
    //{
    //    List<string[]> waves = new();
    //    for (int waveNum = 0; waveNum < 40; waveNum++)
    //    {
    //        List<string> waveData = new();
    //        int basicEnemies = 1 + (waveNum / 2);
    //        int speedEnemies = 0 +waveNum/5
    //    }
    //    return null;
    //}
    // Waves will be spawned in with an array, with numbers for enemy type and other symbols for cd periods.
}
