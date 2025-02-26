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
    public List<Village> TotalVillages = new();

    public HexCell PlayerSpawnCell;
    public HexCell EnemySpawnCell;

    public static float PLAYER_SPAWN_Y;
    public static float ENEMY_SPAWN_Y;

    public static EnemyBrain EnemyBrain;
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
        EnemyBrain = GetComponent<EnemyBrain>();
        yield return StartCoroutine(Map.GenerateGrid(0, 0));
        yield return StartCoroutine(Map.GenerateMap());
        PlayerSpawn = PlayerSpawnCell.GetComponentInChildren<Spawn>();
        EnemySpawn = EnemySpawnCell.GetComponentInChildren<Spawn>();
        PLAYER_SPAWN_Y = PlayerSpawnCell.transform.position.y;
        ENEMY_SPAWN_Y = EnemySpawnCell.transform.position.y;
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
                EnemyBrain.HandleDeath(toRemove);
                EnemyUnits.Remove(toRemove);
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

    public static string IndexToZone(int index)
    {
        switch (index)
        {
            case 0:
                return "Left";
            case 1:
                return "Middle";
            default:
                return "Right";
        }
    }

    public static int ZoneToIndex(string name)
    {

        switch (name)
        {
            case "Left":
                return 0;
            case "Middle":
                return 1;
            default:
                return 2;
        }
    }
}
//TO DO FEATURES:

//unit upkeep +cheaper initial cost
//villages resource etc
//supply lines-could be cool