using System.Collections;
using System.Collections.Generic;
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
    void Start()
    {
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

    public IEnumerator SupplyVillage(Enemy enemy, HexCell villageCell,HexCell baseCell)
    {

        while (enemy)
        {
            yield return new WaitForSeconds(2f);
            Debug.Log("starting");
            if (enemy.FollowPath(villageCell.Position.x, villageCell.Position.y) == false)
            {
                yield return new WaitUntil(() => enemy.FollowPath(villageCell.Position.x, villageCell.Position.y) == true);
            }
            yield return new WaitUntil(() => Vector2.Distance(enemy.transform.position,villageCell.transform.position) <=HexData.CellDiameter);
            Debug.Log("there now");
            yield return new WaitForSeconds(2f);
            enemy.FollowPath(baseCell.Position.x, baseCell.Position.y);
            yield return new WaitUntil(() => Vector2.Distance(enemy.transform.position, baseCell.transform.position) <= HexData.CellDiameter);
            Debug.Log("done route");
            yield return new WaitForSeconds(1f);
            if (enemy.OnPlayerTeam == true)
            {
                Manager.PlayerCoins += 2;//maybe change amount
            }
            else
            {
                Manager.EnemyCoins += 2;
            }
        }
    }
    // Update is called once per frame

}
