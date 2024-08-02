using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCell : MonoBehaviour
{
    public int G;
    public int H;
    public int F
    {
        get
        {
            return G + H;
        }
    }
    private string _terrain = "Empty";
    public int index;
    public string TerrainType
    {
        get
        {
            return _terrain;
        }
        set
        {
            _terrain = value;
            Weight = 1;
            Occupied = false;
            switch (_terrain)
            {
                case "Forest":
                    GetComponent<SpriteRenderer>().color = Color.green;
                    Weight = 2;
                    Occupied = false;
                    break;
                case "Mountain":
                    GetComponent<SpriteRenderer>().color = Color.cyan;
                    Weight = 2;
                    Occupied = true;
                    break;
                case "Swamp":
                    GetComponent<SpriteRenderer>().color = new Color(0.1725f,0.3f,0.231f);
                    Occupied = true;
                    break;
                case "Spawn":
                    GetComponent<SpriteRenderer>().color = Color.blue;
                    break;
                case "Control":
                    GetComponent<SpriteRenderer>().color = Color.yellow;
                    Occupied = true;
                    break;
                default:
                    GetComponent<SpriteRenderer>().color = new Color(0.9f, 0.9f, 0.9f);
                    break;
            }
        }
    }
    public Vector3Int Position;// REMEMBER THAT Z= -x-y
    public List<HexCell> AdjacentTiles = new List<HexCell>();
    public bool Occupied = false;
    public Tower Tower;
    public int Weight=0;
    public GameObject PathVisualizer;
}
