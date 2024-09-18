using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public int PlayerCoins;
    public int EnemyCoins;
    public List<Enemy> Enemies=new();
    public List<HexCell> PlayerVillages=new();
    public List<HexCell> EnemyVillages = new();
    public List<HexCell> TotalVillages = new();
    public List<Enemy> PlayerEnemies = new();
    public List<Enemy> EnemyEnemies = new();
    public HexCell PlayerSpawnCell;
    public HexCell EnemySpawnCell;
    public Spawn PlayerSpawn;
    public Spawn EnemySpawn;
    // Start is called before the first frame update
    void Start()
    {
        PlayerSpawn = PlayerSpawnCell.GetComponentInChildren<Spawn>();
        EnemySpawn = EnemySpawnCell.GetComponentInChildren<Spawn>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
//TO DO FEATURES:

//unit upkeep +cheaper initial cost
//villages resource etc
//supply lines-could be cool