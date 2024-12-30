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
            Obstructed = false;
            switch (_terrain)
            {
                case "Forest":
                    Weight = 2;
                    Obstructed = false;
                    break;
                case "Mountain":
                    Weight = 2;
                    Obstructed = true;
                    break;
                case "Control":
                    Obstructed = true;
                    break;
                default:
                    break;
            }
            ResetColor();
        }
    }
    public Vector3Int Position;// REMEMBER THAT Z= -x-y
    public List<HexCell> AdjacentTiles = new();
    public bool Obstructed = false;
    // Obstructed is boolean based on terrain type.
    public bool Occupied = false;
    // Occupied is boolean based on whether a unit is on the tile.
    public int Weight = 0;
    public GameObject PathVisualizer;

    public void ResetColor()
    {
        switch (TerrainType)
        {
            case "Forest":
                GetComponent<SpriteRenderer>().color = Color.green;
                break;
            case "Mountain":
                GetComponent<SpriteRenderer>().color = Color.cyan;
                break;
            case "Spawn":
                GetComponent<SpriteRenderer>().color = Color.blue;
                break;
            case "Control":
                GetComponent<SpriteRenderer>().color = Color.yellow;
                break;
            default:
                GetComponent<SpriteRenderer>().color = new Color(0.9f, 0.9f, 0.9f);
                break;
        }
    }
}
