using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public GameObject troopPrefab;
    public GameObject rangeDetectPrefab;
    public HexCell deploymentCell;
    public HexMap map;
    public void CreateTroop(int posX, int posY)
    {
        GameObject enemyObject = Instantiate(troopPrefab);
        Enemy enemy = enemyObject.GetComponent<Enemy>();
        enemy.Position = new Vector2Int(posX, posY);
        enemy.InitializePosition(enemy.Position);
        Instantiate(rangeDetectPrefab, enemy.transform);
        map.ReturnHex(posX, posY).Occupied = true;
    }
}
