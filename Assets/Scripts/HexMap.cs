using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public static class HexData
{
    public const float OuterRadius = 0.675f;
    public const float InnerRadius = OuterRadius * 0.866025404f; //sqrt(3)/2 = 0.58456714755 roughly
    public const float CellDiameter = InnerRadius * 2f;
    // 2* 0.58456714755 + x*2*0.58456714755 for exact tile radius.
}

/* public static Vector3[] Corners = {
        new Vector3(0f, 0f, OuterRadius),
        new Vector3(InnerRadius, 0f, 0.5f * OuterRadius),
        new Vector3(InnerRadius, 0f, -0.5f * OuterRadius),
        new Vector3(0f, 0f, -OuterRadius),
        new Vector3(-InnerRadius, 0f, -0.5f * OuterRadius),
        new Vector3(-InnerRadius, 0f, 0.5f * OuterRadius)
}; */
public class HexMap : MonoBehaviour
{
    private readonly int _size = 13;
    private int _cellNumber;
    public GameObject CellPrefab;

    public HexCell[] Cells;
    private Dictionary<(int x, int y), HexCell> _cellDictionary = new();

    private readonly int _chanceEmpty = 12;
    private readonly int _chanceForest = 6;
    private readonly int _chanceMountain = 2;
    private int _totalMountains;
    public GameObject ControlPrefab;
    public GameObject SpawnPrefab;
    public Game Manager;

    void Awake() // Unity definition for when scene begins to load
    {
        _cellNumber = 3 * _size * (_size - 1) + 1;
        // this code gets the total amount of hexagons using
        // 3n(n-1)+1
        // this function gets the # of hexagons per n rings
        // n=1 returns 1, n=2 returns 7, etc.
        Cells = new HexCell[_cellNumber];
        GenerateGrid(0, 0);
        GenerateMap();
    }
    void GenerateGrid(int centerX, int centerY)
    {
        GenerateCells(centerX, centerY);
        GenerateAdjacentTiles();
    }

    void GenerateCells(int centerX, int centerY)
    {
        for (int column = -_size + 1, i = 0; column < _size; column++)
        {
            int smaller = Mathf.Max(-_size + 1, -column - _size + 1);
            int larger = Mathf.Min(_size - 1, -column + _size - 1);

            for (int row = smaller; row <= larger; row++)
            {
                int x = centerX - column;
                int y = centerY - row;
                CreateCell(x, y, i++);
            }
        }
    }

    void GenerateAdjacentTiles()
    {
        for (int i = 0; i < _cellNumber; i++)
        {
            HexCell cell = Cells[i];
            int x = cell.Position.x;
            int y = cell.Position.y;

            if (y % 2 == 0)
            {
                cell.AdjacentTiles.Add(ReturnHex(x + 1, y - 1));
                cell.AdjacentTiles.Add(ReturnHex(x, y - 1));
                cell.AdjacentTiles.Add(ReturnHex(x - 1, y));
                cell.AdjacentTiles.Add(ReturnHex(x + 1, y));
                cell.AdjacentTiles.Add(ReturnHex(x, y + 1));
                cell.AdjacentTiles.Add(ReturnHex(x - 1, y + 1));
            }
            else
            {
                cell.AdjacentTiles.Add(ReturnHex(x, y - 1));
                cell.AdjacentTiles.Add(ReturnHex(x + 1, y - 1));
                cell.AdjacentTiles.Add(ReturnHex(x + 1, y));
                cell.AdjacentTiles.Add(ReturnHex(x - 1, y));
                cell.AdjacentTiles.Add(ReturnHex(x - 1, y + 1));
                cell.AdjacentTiles.Add(ReturnHex(x, y + 1));
            }
        }
    }
    void GenerateMap()
    {
        int randomIndex = RandomCentralCell(_cellNumber / 2);

        for (int i = 0; i < _cellNumber / 2; i++)
        {
            int symmetricalIndex = _cellNumber - 1 - i;
            GenerateTerrain(Cells[i]);
        }
        for (int i = 0; i < Random.Range(0, 3); i++)
        {
            GenerateMountainRange(Random.Range(0, _cellNumber / 2), 8);
            Debug.Log("Generated range");
        }
        for (int i = 0; i < 3; i++) GenerateControl(Random.Range(0, _cellNumber / 3));
        GenerateSpawns(5, -10);
    }
    void GenerateTerrain(HexCell cell)
    {
        int symmetricalIndex = _cellNumber - 1 - cell.index;
        int randomVal = Random.Range(1, _chanceEmpty + _chanceForest + _chanceMountain + 1);
        if (randomVal <= _chanceEmpty) return;
        if (randomVal <= _chanceEmpty + _chanceForest)
        {
            cell.TerrainType = "Forest";
            Cells[symmetricalIndex].TerrainType = "Forest";
        }
        else
        {
            cell.TerrainType = "Mountain";
            Cells[symmetricalIndex].TerrainType = "Mountain";
        }
    }
    int RandomCentralCell(int max)
    {
        while (true)
        {
            int randomIndex = Random.Range(1, max);
            if (Mathf.Abs(Cells[randomIndex].transform.position.x) < 7 && Mathf.Abs(Cells[randomIndex].transform.position.x) > 3 && Mathf.Abs(Cells[randomIndex].transform.position.y) < 4)
            {
                return randomIndex;
            }
        }
    }
    void GenerateMountainRange(int index, int mountainNumber)
    {
        Queue<HexCell> queue = new Queue<HexCell>();
        queue.Enqueue(Cells[index]);

        while (queue.Count > 0)
        {
            HexCell cell = queue.Dequeue();

            if (cell.TerrainType == "Mountain")
                continue;

            if (mountainNumber < 1)
                return;

            _totalMountains++;
            cell.TerrainType = "Mountain";
            Cells[_cellNumber - 1 - cell.index].TerrainType = "Mountain";

            foreach (HexCell adjacentCell in cell.AdjacentTiles)
            {
                if (adjacentCell && adjacentCell.TerrainType != "Mountain" && Random.Range(0, 100) < 34)
                {
                    mountainNumber--;
                    queue.Enqueue(adjacentCell);
                }
            }
        }
    }
    void GenerateControl(int index)
    {
        int symmetricalIndex = _cellNumber - 1 - index;
        if (Cells[index] == null || Cells[index].AdjacentTiles.All(cell => cell.TerrainType == "Mountain"))
        {
            GenerateControl(Random.Range(0, _cellNumber / 3));
        }
        else
        {
            Cells[index].TerrainType = "Control";
            Cells[symmetricalIndex].TerrainType = "Control";
            Instantiate(ControlPrefab, Cells[index].transform);
            Instantiate(ControlPrefab, Cells[symmetricalIndex].transform);
        }
    }
    void GenerateSpawns(int x, int y)
    {
        HexCell potentialSpawn = ReturnHex(x, y);
        bool isOccupied = false;
        foreach (HexCell cell in potentialSpawn.AdjacentTiles)
        {
            if (cell.TerrainType == "Mountain" || cell.TerrainType == "Control")
            {
                isOccupied = true;
            }

        }
        if (isOccupied)
        {
            GenerateSpawns(Random.Range(3, 7), Random.Range(-11, -9));
        }
        else
        {
            potentialSpawn.TerrainType = "Spawn";
            Cells[_cellNumber - 1 - potentialSpawn.index].TerrainType = "Spawn";
            Manager.PlayerSpawnCell = potentialSpawn;
            Manager.EnemySpawnCell = Cells[_cellNumber - 1 - potentialSpawn.index];
            GameObject spawn = Instantiate(SpawnPrefab, potentialSpawn.transform);
            spawn.GetComponent<Spawn>().IsPlayerSpawn = true;
            GameObject enemySpawn = Instantiate(SpawnPrefab, Cells[_cellNumber - 1 - potentialSpawn.index].transform);
            enemySpawn.GetComponent<Spawn>().IsPlayerSpawn = false;
        }
    }
    void CreateCell(int x, int y, int i)
    {
        Vector3 position; // Game object's position on screen
        position.x = x * (HexData.InnerRadius * 2f) + (y * HexData.InnerRadius);
        position.z = 10f; // sorting order on 2d screen
        position.y = y * (HexData.OuterRadius * 1.5f);
        // HexData is used to align the cells from their axial coordinates
        // to their on-screen ones.
        GameObject cellObj = Instantiate(CellPrefab);
        // Unity function to create Game object.
        HexCell cell = Cells[i] = cellObj.GetComponent<HexCell>();
        //We need to access the instantiated object's class properties.
        cell.Position = new Vector3Int(x, y, -x - y);
        _cellDictionary[(cell.Position.x, cell.Position.y)] = cell;
        // Assign cell to dictionary.
        cell.transform.SetParent(transform, false);
        // Unity function to keep cell relative to its' game object
        cell.transform.localPosition = position;
        cell.name = new Vector2Int(x, y).ToString();
        // A debug measure: simple way to see which cell is which.
        cell.index = i;
    }

    public HexCell ReturnHex(int x, int y)
    {
        if (_cellDictionary.TryGetValue((x, y), out HexCell cell))
        {
            return cell;
        }
        return null;
    }
}