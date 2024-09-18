using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using UnityEngine;

public class SupplyManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject SupplyPrefab;
    public HexCell TargetVillage;
    private HexCell _spawn;
    public Game Manager;
    public List<Enemy> Suppliers = new List<Enemy>();
    public List<Coroutine> ActiveRoutines = new List<Coroutine>();
    private Dictionary<Enemy, HexCell> _adjacentSpawns=new();
    void Start()
    {
        //StartCoroutine(WaitForLoad());
    }
    private IEnumerator WaitForLoad()
    {
        yield return new WaitForSeconds(1f);
        
    }

    public Enemy SpawnSupplyUnit(HexCell baseCell, bool onPlayerTeam)
    {
        GameObject enemyObject;
        enemyObject = Instantiate(SupplyPrefab);
        Enemy enemy = enemyObject.GetComponent<Enemy>();
        enemy.Type = "Supply";
        enemy.Position = new Vector2Int(baseCell.Position.x, baseCell.Position.y);
        enemy.InitializePosition(enemy.Position);
        enemy.OnPlayerTeam = onPlayerTeam;
        Suppliers.Add(enemy);
        return enemy;
        //StartCoroutine(SupplyVillage(enemy, villageCell, baseCell));
    }
    private void CheckSupplyPoints(Enemy e)
    {
        foreach (HexCell c in _spawn.AdjacentTiles)
        {
            if (!_adjacentSpawns.ContainsValue(c)&& !_adjacentSpawns.ContainsKey(e))
            {
                Debug.Log("added");
                _adjacentSpawns.Add(e, c);
            }
        }
    }
    public IEnumerator SupplyVillage(Enemy enemy, HexCell villageCell, HexCell baseCell)
    {
        if (!_spawn)
        {
            if (enemy.OnPlayerTeam) _spawn = Manager.PlayerSpawnCell;
            else _spawn = Manager.EnemySpawnCell;
        }
        while (enemy)
        {
            yield return new WaitForSeconds(2f);
            //Debug.Log("starting");
            CheckSupplyPoints(enemy);
            if (enemy.FollowPath(villageCell.Position.x, villageCell.Position.y) == false)
            {
                yield return new WaitUntil(() => enemy.FollowPath(villageCell.Position.x, villageCell.Position.y) == true);
            }// ONE FLAW: if path gets blocked mid-pathing, enemy will freeze.
            //while (Vector2.Distance(enemy.transform.position, villageCell.transform.position) >= HexData.CellDiameter + 0.05f)
            //{
            //    if (enemy.FollowPath(villageCell.Position.x, villageCell.Position.y) == false&&enemy.Moving==false)
            //    {
            //        yield return new WaitUntil(() => enemy.FollowPath(villageCell.Position.x, villageCell.Position.y) == true);
            //    }
            //    yield return new WaitForSeconds(1.5f);
            //}
            yield return new WaitUntil(() => Vector2.Distance(enemy.transform.position, villageCell.transform.position) <= HexData.CellDiameter + 0.05f);
            Debug.Log("there now");
            yield return new WaitForSeconds(2f);
            enemy.FollowPath(_adjacentSpawns[enemy].Position.x, _adjacentSpawns[enemy].Position.y);
            yield return new WaitUntil(() => Vector2.Distance(enemy.transform.position, _adjacentSpawns[enemy].transform.position) <= HexData.CellDiameter + 0.05f);
            //Debug.Log("done route");
            yield return new WaitForSeconds(1f);
            if (enemy.OnPlayerTeam == true)
            {
                Manager.PlayerCoins += 3;//maybe change amount
            }
            else
            {
                Manager.EnemyCoins += 3;
            }
            _adjacentSpawns.Remove(enemy);
        }
    }
}
