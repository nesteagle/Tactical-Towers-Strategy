using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Enemy : MonoBehaviour
{
    public float LocalHealth;
    public float Health
    {
        get
        {
            return LocalHealth;
        }
        set
        {
            LocalHealth = value;
            StartCoroutine(HandleDeath());
        }
    }
    public float Speed; //speed between tile movement.
    public Vector2Int Position; //closest tile (or make them spawn on said tile)
    public string Type;//useful when extending class
    public int ID;
    private List<HexCell> _hexPath;
    public bool Moving = false;
    public bool Attacking = false;
    public float Cooldown; //when extending class set custom cooldowns.
    public float Damage;
    public bool OnPlayerTeam;
    private Vector2Int _objectivePosition;
    private HexMap _map;
    public Coroutine ActiveRoutine;
    private void Awake()
    {
        _map = GameObject.Find("HexGrid").GetComponent<HexMap>();
    }
    public void InitializePosition(Vector2Int tilePosition)
    {
        HexMap hexMap = GameObject.Find("HexGrid").GetComponent<HexMap>();
        Vector3 pos = hexMap.ReturnHex(tilePosition.x, tilePosition.y).transform.position;
        transform.position = new Vector3(pos.x, pos.y, 0);
    }
    public bool FollowPath(int objTileX, int objTileY)
    {
        Vector3Int finalPosition;
        _hexPath = Pathfinding.FindPath(_map.ReturnHex(Position.x, Position.y), _map.ReturnHex(objTileX, objTileY));
        //if (_hexPath[^1].Occupied) finalPosition = _hexPath[^2].Position;
        //else finalPosition = _hexPath[^1].Position;
        if (_hexPath.Count != 0)
        {
            finalPosition = _hexPath[^1].Position;
            StartCoroutine(PathToTarget(finalPosition.x, finalPosition.y));
            return true;
        }
        else
        {
            Debug.Log("CANNOT PATH");
            return false;
        }
    }
    private IEnumerator PathToTarget(int objTileX, int objTileY)
    {
        //use recursion?
        List<HexCell> cellsToClear = new();
        Moving = true;
        while (Position.x != objTileX || Position.y != objTileY)
        {
            _hexPath = Pathfinding.FindPath(_map.ReturnHex(Position.x, Position.y), _map.ReturnHex(objTileX, objTileY));
            for (int i=_hexPath.Count-1;i>=0 ;i--)
            {
                if (_hexPath[i].Occupied == true)
                {
                    _hexPath.RemoveAt(i);
                }
                else break;
            }
            if (_hexPath.Count<=1) {
                Debug.Log("end");
                Moving = false;
                yield break; 
            }
            foreach (HexCell cell in _hexPath)
            {
                cell.GetComponent<SpriteRenderer>().color = new Color(0.4f, 0.7f, 0.4f);
                cellsToClear.Add(cell);
            }
            _hexPath[0].Occupied = false;
            //if (_map.ReturnHex(objTileX, objTileY).Occupied)
            //{
            //    Debug.Log("DONE");
            //    Moving = false;
            //    yield break;
            //}
            _hexPath[1].Occupied = true;
            yield return StartCoroutine(MovePosition(transform.position, _hexPath[1].transform.position, Speed));
            Position = new Vector2Int(_hexPath[1].Position.x, _hexPath[1].Position.y);
            foreach (HexCell cell in cellsToClear)
            {
                ResetColors(cell);
            }
        }
        Moving = false;
    }
    private IEnumerator MovePosition(Vector3 pos1, Vector3 pos2, float speed)
    {
        float i = 0;
        yield return new WaitForSeconds(0.2f);
        while (i < 1f)
        {
            i += Time.deltaTime * speed;
            transform.localPosition = Vector3.Lerp(pos1, new Vector3(pos2.x,pos2.y), i);
            yield return new WaitForEndOfFrame();
        }
    }
    private IEnumerator HandleDeath()
    {
        if (Health <= 0f)
        {
            yield return new WaitForSeconds(0.15f);
            //play animation!
            _map.ReturnHex(Position.x, Position.y).Occupied = false;
            Destroy(gameObject);
        }
    }
    //private void FinishedPath()
    //{
    //    //Remove objective HP, if too low lose menu.
    //    GameControl.health -= Damage;
    //    Spawner.EnemiesRemaining--;
    //    Destroy(gameObject);
    //}
    private void ResetColors(HexCell c)
    {
        //will be obsolete after icons are added.
        switch (c.TerrainType)
        {
            case "Forest":
                c.GetComponent<SpriteRenderer>().color = Color.green;
                break;
            case "Mountain":
                c.GetComponent<SpriteRenderer>().color = Color.cyan;
                break;
            case "Spawn":
                c.GetComponent<SpriteRenderer>().color = Color.blue;
                break;
            case "Control":
                c.GetComponent<SpriteRenderer>().color = Color.yellow;
                break;
            default:
                c.GetComponent<SpriteRenderer>().color = new Color(0.9f, 0.9f, 0.9f);
                break;
        }
    }
}
