using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitDeploy : MonoBehaviour
{
    public RectTransform DeployTransform;
    public Canvas DeployMenu;
    public GameObject[] UnitPrefabs;
    public GameObject[] RangeDetectorPrefab;
    //private Game _manager;
    //private Spawn _spawn;
    private HexCell _selected=null;
    //add queue system here
    //private void Start()
    //{
    //    _manager = GameObject.Find("Control").GetComponent<Game>();
    //    _spawn = _manager.PlayerSpawn;
    //}
    public void OpenDeployMenu(HexCell cell)
    {
        DeployMenu.enabled = true;
        //return button feature to close.
        DeployTransform.position = cell.transform.position+new Vector3(3f,0);
        _selected = cell;
    }
    public IEnumerator PlaceTroop(string type)
    {
        if (_selected.Occupied) yield break;
        //if (resources<number) return;
        //all checks will be here.
        //maybe add method to place on adjacent tiles.
        CreateTroop(_selected.Position.x, _selected.Position.y, type);
        _selected.Occupied = true;
        DeployMenu.enabled = false;
    }
    //public void CreateTroop(int index, int posX, int posY) OLD METHOD 
    //{
    //    Debug.Log("PLACED Unit #" + index);
    //    GameObject enemyObject = Instantiate(UnitPrefabs[0]);//[index]
    //    Enemy enemy = enemyObject.GetComponent<Enemy>();
    //    enemy.Position = new Vector2Int(posX, posY);
    //    enemy.InitializePosition(enemy.Position);
    //    Instantiate(RangeDetectorPrefab, enemy.transform);
    //    enemy.OnPlayerTeam = true;
    //} 
    public void CreateTroop(int posX, int posY, string type)
    {
        GameObject enemyObject;
        Enemy enemy = null;
        switch (type)
        {
            case "Knight":
                enemyObject = Instantiate(UnitPrefabs[0]);
                enemy = enemyObject.GetComponent<Enemy>();
                Instantiate(RangeDetectorPrefab[0], enemy.transform);
                break;
            case "Scout":
                enemyObject = Instantiate(UnitPrefabs[1]);
                enemy = enemyObject.GetComponent<Enemy>();
                Instantiate(RangeDetectorPrefab[0], enemy.transform);
                break;
            case "Archer":
                enemyObject = Instantiate(UnitPrefabs[2]);
                enemy = enemyObject.GetComponent<Enemy>();
                Instantiate(RangeDetectorPrefab[1], enemy.transform);
                break;
        }
        enemy.Type = type;
        enemy.Position = new Vector2Int(posX, posY);
        enemy.InitializePosition(enemy.Position);
        enemy.OnPlayerTeam = true;
    }
}
