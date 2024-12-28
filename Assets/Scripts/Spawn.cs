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
    private bool _menuOpened;
    public bool IsPlayerSpawn;
    private Game _manager;
    public Queue<string> Actions = new();
    private int _index;
    private bool _spawning; //make functional eventually
    private readonly float[] _cooldownTimes = new float[] { 1f, 1.5f, 3f };
    private readonly int[] _unitCosts = new int[] { 2, 3, 5 };
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
    //private void Update()
    //{
    //    if (Input.GetMouseButtonDown(0)){
    //        if (EventSystem.current.IsPointerOverGameObject()) return;
    //        Debug.Log("HI this is the spawn you clicked me!");
    //    }
    //}
    private void OnMouseDown()
    {
        Debug.Log("HI this is the spawn you clicked me!");
        if (_menuOpened)
        {
            _menuOpened = false;
            DeployMenu.enabled = false;
        }
        else
        {
            OpenDeployMenu(_cell);
            _menuOpened = true;
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
    public void OpenDeployMenu(HexCell cell)
    {
        DeployMenu.enabled = true;
        //return button feature to close.
        DeployTransform.position = cell.transform.position + new Vector3(3f, 0);
    }
    private int CheckUnit(string type)
    {
        switch (type)
        {
            case "Scout":
                return 0;
            case "Knight":
                return 1;
            case "Archer":
                return 2;
            default:
                return -1;
        }
    }
    public int PlaceTroop(string type)
    {
        _index++;
        if (IsPlayerSpawn)
        {
            if (_manager.PlayerCoins < _unitCosts[CheckUnit(type)]) return -1;
            _manager.PlayerCoins -= _unitCosts[CheckUnit(type)];
        }
        else if (_manager.EnemyCoins < _unitCosts[CheckUnit(type)]) return -1;
        _manager.EnemyCoins -= _unitCosts[CheckUnit(type)];
        //if (_cell.Occupied) yield break;
        //if (resources<number) return;
        //all checks will be here.
        //maybe add method to place on adjacent tiles.

        string id = type + " " + _index;
        Actions.Enqueue(id); 
        return _index;
    }
    private IEnumerator ManageActions()
    {
        while (_manager)//game running
        {
            if (Actions.Count > 0)
            {
                string[] splitData = Actions.Peek().Split(" ");
                yield return new WaitUntil(() => _cell.Occupied == false);
                yield return new WaitForSeconds(_cooldownTimes[CheckUnit(splitData[0])]);
                //play animation[CheckUnit(Actions.Peek().Split(" ")[0])];
                //play animation (when made)
                CreateUnit(_cell, splitData[0], int.Parse(splitData[1]));
                Actions.Dequeue();
                //now queue up the training thing
            }
            yield return new WaitForFixedUpdate();
        }
        yield break;
    }
    public void CreateUnit(HexCell cell, string type, int id)
    {
        GameObject unitObject;
        Enemy unit = null;
        switch (type)
        {
            case "Knight":
                unitObject = Instantiate(UnitPrefabs[0]);
                unit = unitObject.GetComponent<Enemy>();
                Instantiate(RangeDetectorPrefab[0], unit.transform);
                break;
            case "Scout":
                unitObject = Instantiate(UnitPrefabs[1]);
                unit = unitObject.GetComponent<Enemy>();
                Instantiate(RangeDetectorPrefab[0], unit.transform);
                break;
            case "Archer":
                unitObject = Instantiate(UnitPrefabs[2]);
                unit = unitObject.GetComponent<Enemy>();
                Instantiate(RangeDetectorPrefab[1], unit.transform);
                break;
        }
        unit.Type = type;
        unit.ID = id;
        unit.Position = new Vector2Int(cell.Position.x, cell.Position.y);
        Vector3 pos = Game.Map.ReturnHex(unit.Position.x, unit.Position.y).transform.position;
        unit.transform.position = new Vector3(pos.x, pos.y, 0);
        unit.OnPlayerTeam = IsPlayerSpawn;
        cell.Occupied = true;
        if (IsPlayerSpawn) _manager.PlayerEnemies.Add(unit);
        else _manager.EnemyEnemies.Add(unit);
    }
}
