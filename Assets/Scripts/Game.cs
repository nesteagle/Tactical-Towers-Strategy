using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using System.Runtime.Remoting.Channels;
using Unity.VisualScripting;

public class Game : MonoBehaviour
{
    public int PlayerCoins;
    public int EnemyCoins;
    public HashSet<Unit> Units = new();
    public HashSet<Unit> PlayerUnits = new();
    public HashSet<Unit> EnemyUnits = new();
    //private Dictionary<Unit, int> _unitDictionary = new();

    public List<HexCell> PlayerVillages = new();
    public List<HexCell> EnemyVillages = new();
    public List<HexCell> TotalVillages = new();

    public HexCell PlayerSpawnCell;
    public HexCell EnemySpawnCell;
    public static Spawn PlayerSpawn;
    public static Spawn EnemySpawn;
    public static HexMap Map;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadGameResources());
    }
    private IEnumerator LoadGameResources()
    {
        yield return new WaitUntil(() => GameObject.Find("HexGrid").GetComponent<HexMap>() != null);
        Map = GameObject.Find("HexGrid").GetComponent<HexMap>();
        PlayerSpawn = PlayerSpawnCell.GetComponentInChildren<Spawn>();
        EnemySpawn = EnemySpawnCell.GetComponentInChildren<Spawn>();
    }

    public HashSet<Unit> RemoveUnit(Unit toRemove)
    {
        if (toRemove != null)
        {
            Units.Remove(toRemove);
            if (toRemove.Team == "Player")
            {
                PlayerUnits.Remove(toRemove);
            }
            else
            {
                EnemyUnits.Remove(toRemove);
                // Call event to EnemyBrain
            }
        }
        return Units;
    }
    public HashSet<Unit> AddUnit(Unit toAdd)
    {
        if (toAdd != null)
        {
            Units.Add(toAdd);
            if (toAdd.Team == "Player")
            {
                PlayerUnits.Add(toAdd);
            }
            else
            {
                EnemyUnits.Add(toAdd);
                // Call event to EnemyBrain
            }
        }
        return Units;
    }
}
//TO DO FEATURES:

//unit upkeep +cheaper initial cost
//villages resource etc
//supply lines-could be cool