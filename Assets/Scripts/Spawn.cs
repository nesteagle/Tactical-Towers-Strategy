using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.EventSystems.EventTrigger;

public class Spawn : MonoBehaviour
{
    public GameObject[] UnitPrefabs;
    public GameObject[] RangeDetectorPrefab;
    public RectTransform DeployTransform;
    public Canvas DeployMenu;
    private HexCell _cell;
    public bool MenuOpened;
    public bool IsPlayerSpawn;
    private Game _manager;
    public Queue<Unit> Actions = new();
    private int _index;
    private bool _spawning; //make functional eventually
    private void Start()
    {
        _cell = GetComponentInParent<HexCell>();
        _manager = GameObject.Find("Control").GetComponent<Game>();
        DeployMenu = GameObject.Find("UnitDeploy").GetComponent<Canvas>();
        DeployTransform = GameObject.Find("UnitDeploy").GetComponentInChildren<RectTransform>();
        StartCoroutine(UpdateResources());
        StartCoroutine(ManageActions());
        //_deployCanvas = GameObject.Find("UnitDeploy").GetComponent<Canvas>();
    }
    private void OnMouseDown()
    {
        if (MenuOpened)
        {
            MenuOpened = false;
            DeployMenu.enabled = false;
        }
        else
        {
            DeployMenu.enabled = true;
            DeployTransform.position = _cell.transform.position + new Vector3(3f, 0);
            MenuOpened = true;
            // add a method in DeployMenu to close itself.
        }
    }
    private IEnumerator UpdateResources()
    {
        while (this)
        {
            yield return new WaitForSeconds(4);
            if (IsPlayerSpawn == true)
            {
                _manager.PlayerCoins++;
            }
            else _manager.EnemyCoins++;
        }
    }
    public Unit PlaceTroop(string type)
    {
        _index++;
        int cost;
        switch (type)
        {
            case "Scout":
                cost = 2;
                break;
            case "Knight":
                cost = 3;
                break;
            case "Archer":
                cost = 5;
                break;
            default:
                cost = int.MaxValue;
                break;
        }
        if (IsPlayerSpawn)
        {
            if (_manager.PlayerCoins < cost) return null;
            _manager.PlayerCoins -= cost;
        }
        else if (_manager.EnemyCoins < cost) return null;
        _manager.EnemyCoins -= cost;

        Unit toPlace = SpawnUnit(type, _index);
        Actions.Enqueue(toPlace);
        return toPlace; //!!!
    }
    private IEnumerator ManageActions()
    {
        while (_manager)//game running
                        // Add game state.
        {
            if (Actions.Count > 0)
            {
                Unit toPlace = Actions.Peek();
                yield return new WaitUntil(() => _cell.Occupied == false);
                switch (toPlace.Type)
                {
                    case "Scout":
                        yield return new WaitForSeconds(1f);
                        break;
                    case "Knight":
                        yield return new WaitForSeconds(1.5f);
                        break;
                    case "Archer":
                        yield return new WaitForSeconds(2.5f);
                        break;
                    default:
                        break;
                }
                ;
                //play animation[CheckUnit(Actions.Peek().Split(" ")[0])];
                //play animation (when made)
                toPlace.TilePosition = new Vector2Int(_cell.Position.x, _cell.Position.y);
                Vector3 pos = _cell.transform.position;
                toPlace.transform.position = new Vector3(pos.x, pos.y, 0);
                _cell.Occupied = true;
                toPlace.State = "Rest";
                Debug.Log(toPlace.Team + toPlace.State);
                Actions.Dequeue();
            }
            yield return new WaitForFixedUpdate();
        }
        yield break;
    }
    public Unit SpawnUnit(string type, int id)
    {
        GameObject unitObject;
        Unit unit = null;
        switch (type)
        {
            case "Knight":
                unitObject = Instantiate(UnitPrefabs[0]);
                unit = unitObject.GetComponent<Unit>();
                Instantiate(RangeDetectorPrefab[0], unit.transform);
                break;
            case "Scout":
                unitObject = Instantiate(UnitPrefabs[1]);
                unit = unitObject.GetComponent<Unit>();
                Instantiate(RangeDetectorPrefab[0], unit.transform);
                break;
            case "Archer":
                unitObject = Instantiate(UnitPrefabs[2]);
                unit = unitObject.GetComponent<Unit>();
                Instantiate(RangeDetectorPrefab[1], unit.transform);
                break;
        }
        unit.Type = type;
        if (IsPlayerSpawn) unit.Team = "Player";
        else { 
            unit.Team = "Enemy";
            unit.GetComponent<SelectPath>().enabled = false;
        }
        unit.State = "Moving";
        unit.transform.position = new Vector3(0, -100);

        _manager.AddUnit(unit);
        return unit;
    }
}
