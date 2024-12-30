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
    public List<Unit> Suppliers = new();
    public List<Coroutine> ActiveRoutines = new();
    private Dictionary<Unit, HexCell> _adjacentSpawns = new();

    public Unit SpawnSupplyUnit(HexCell baseCell, bool onPlayerTeam)
    {
        GameObject unitObject;
        unitObject = Instantiate(SupplyPrefab);
        Unit unit = unitObject.GetComponent<Unit>();
        unit.Type = "Supply";
        unit.TilePosition = new Vector2Int(baseCell.Position.x, baseCell.Position.y);
        Vector3 pos = baseCell.transform.position;
        unit.transform.position = new Vector3(pos.x, pos.y, 0);
        if (onPlayerTeam) unit.Team = "Player";
        else unit.Team = "Enemy";
        Suppliers.Add(unit);
        unit.State = "Rest";
        return unit;
        //StartCoroutine(SupplyVillage(enemy, villageCell, baseCell));
    }
    private void CheckSupplyPoints(Unit e)
    {
        foreach (HexCell c in _spawn.AdjacentTiles)
        {
            if (!_adjacentSpawns.ContainsValue(c) && !_adjacentSpawns.ContainsKey(e))
            {
                _adjacentSpawns.Add(e, c);
            }
        }
    }
    public IEnumerator SupplyVillage(Unit supplier, HexCell villageCell, HexCell baseCell)
    {
        if (!_spawn)
        {
            if (supplier.Team == "Player") _spawn = Manager.PlayerSpawnCell;
            else _spawn = Manager.EnemySpawnCell;
        }
        while (supplier)
        {
            CheckSupplyPoints(supplier);
            yield return new WaitForSeconds(2f);

            yield return StartCoroutine(MoveWhenValid(supplier, villageCell, true));
            yield return new WaitForSeconds(2f);
            yield return StartCoroutine(MoveWhenValid(supplier, _adjacentSpawns[supplier], false));
            yield return new WaitForSeconds(1f);

            if (supplier.Team == "Player")
                Manager.PlayerCoins += 3;
            else Manager.EnemyCoins += 3;
            _adjacentSpawns.Remove(supplier);
        }
    }
    private IEnumerator MoveWhenValid(Unit supplier, HexCell destination, bool endAdjacent)
    {
        if (endAdjacent)
        {
            while (!AdjacentToDest(supplier, destination))
            {
                yield return new WaitUntil(() => AdjacentToDest(supplier, destination) || supplier.State == "Rest");
                supplier.MoveTo(destination);
            }
        }
        else
        {
            while (!AtDest(supplier, destination))
            {
                yield return new WaitUntil(() => AtDest(supplier, destination) || supplier.State == "Rest");
                supplier.MoveTo(destination);
            }
        }
        supplier.State = "Rest";
    }
    private bool AdjacentToDest(Unit supplier, HexCell destination)
    {
        return destination.AdjacentTiles.Contains(Game.Map.ReturnHex(supplier.TilePosition.x, supplier.TilePosition.y));
    }

    private bool AtDest(Unit supplier, HexCell destination)
    {
        return destination == Game.Map.ReturnHex(supplier.TilePosition.x, supplier.TilePosition.y);
    }
}
