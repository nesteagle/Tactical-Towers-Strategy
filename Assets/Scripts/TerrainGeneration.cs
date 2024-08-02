using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGeneration : MonoBehaviour
{
    //name is outdated but whole script will be deleted eventually. Just copy over CreateTroop function
    public HexMap Map;
    public GameObject[] troopPrefab;
    public GameObject TESTOBJECT;
    public GameObject[] rangeDetectPrefab;
    private void Start()
    {
        //CreateTerrain(Map);
        CreateTroop(0,0,"Knight");
        CreateTroop(3,-3, "Scout");
        CreateTroop(-3,-3,"Archer");
    }
    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        GetIntersectingTile(_e1, _e2);
    //    }
    //}
    //private void CreateTerrain(HexMap map)
    //{
    //    foreach(HexCell cell in map.Cells)
    //    {
    //        if (Random.Range(1, 12) == 1)
    //        {
    //            cell.GetComponent<SpriteRenderer>().color = Color.red;
    //            cell.Weight = 4;
    //            continue;
    //        }
    //        if (Random.Range(1, 8) == 1)
    //        {
    //            cell.GetComponent<SpriteRenderer>().color = Color.green;
    //            cell.Weight = 2;
    //            continue;
    //        }
    //        if (Random.Range(1, 3) == 1)
    //        {
    //            cell.GetComponent<SpriteRenderer>().color = Color.cyan;
    //            cell.Weight = 100;
    //            continue;
    //            //Forest Generation?
    //        }
    //    }
    //}
    private void CreateMountainRange(int size, HexCell origin)
    {

    }
    private void CreateForestRange(int size, HexCell origin)
    {

    }
    public void CreateTroop(int posX, int posY, string type)
    {
        GameObject enemyObject;
        Enemy enemy=null;
        switch (type)
        {
            case "Knight":
                enemyObject = Instantiate(troopPrefab[0]);
                enemy = enemyObject.GetComponent<Enemy>();
                Instantiate(rangeDetectPrefab[0], enemy.transform);
                break;
            case "Archer":
                enemyObject = Instantiate(troopPrefab[1]);
                enemy = enemyObject.GetComponent<Enemy>();
                Instantiate(rangeDetectPrefab[1], enemy.transform);
                break;
            case "Scout":
                enemyObject = Instantiate(troopPrefab[2]);
                enemy = enemyObject.GetComponent<Enemy>();
                Instantiate(rangeDetectPrefab[0], enemy.transform);
                break;
        }
        enemy.Type = type;
        enemy.Position = new Vector2Int(posX,posY);
        enemy.InitializePosition(enemy.Position);
    }
    //private void GetIntersectingTile(Enemy enemy1, Enemy enemy2)
    //{
    //    HexCell intersectingTile;
    //    //WHERE enemy1 will be enemy which script is running on.
    //    Vector3 displacement;
    //    Vector3 position1 = enemy1.transform.position;
    //    Vector3 position2 = enemy2.transform.position;
    //    displacement = new Vector3((position2.x - position1.x) / 2 + position1.x, (position2.y - position1.y) / 2 + position1.y);
    //    GameObject t = Instantiate(TESTOBJECT);//debug
    //    t.transform.position = displacement;//debug
    //    Collider2D hit = Physics2D.OverlapCircle(displacement, 0.2f);
    //    //Physics2D.OverlapCircle(displacement, 0.5f);
    //    if (hit == null) return;
    //    Debug.Log("HAS COL");
    //    if (hit.gameObject.GetComponent<HexCell>())
    //    {
    //        Debug.Log("HAS CELL");
    //        intersectingTile = hit.gameObject.GetComponent<HexCell>();
    //        Debug.Log(intersectingTile);
    //    }
    //}
}
