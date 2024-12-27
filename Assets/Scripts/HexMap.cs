using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public static class HexData
{
    public const float OuterRadius = 0.675f;
    public const float InnerRadius = OuterRadius * 0.866025404f; //sqrt(3)/2 = 0.58456714755 roughly
    public const float CellDiameter = InnerRadius * 2f;
    // 2* 0.58456714755 + x*2*0.58456714755 for exact tile radius.
}

//public static Vector3[] Corners = {
//		new Vector3(0f, 0f, OuterRadius),
//		new Vector3(InnerRadius, 0f, 0.5f * OuterRadius),
//		new Vector3(InnerRadius, 0f, -0.5f * OuterRadius),
//		new Vector3(0f, 0f, -OuterRadius),
//		new Vector3(-InnerRadius, 0f, -0.5f * OuterRadius),
//		new Vector3(-InnerRadius, 0f, 0.5f * OuterRadius)
//	};
public class HexMap : MonoBehaviour
{
    private readonly int _size = 13;
    private int _cellNumber;
    public GameObject CellPrefab;
    public HexCell[] Cells;
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
        for (int column = -_size + 1, j = 0; column < _size; column++)
        {
            int smaller = Mathf.Max(-_size + 1, -column - _size + 1);
            int larger = Mathf.Min(_size - 1, -column + _size - 1);

            for (int row = smaller; row <= larger; row++)
            {
                int x = centerX - column;
                int y = centerY - row;
                AddAdjacent(x, y, j++);
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
        for (int i = 0; i < Random.Range(1, 4); i++)
        {
            int randomIndex2 = Random.Range(1, _cellNumber / 2);
            GenerateMountainRange(Random.Range(1, _cellNumber / 2), Random.Range(6, 11));
            Debug.Log("Generated at " + Cells[randomIndex2].name);
        }
        //Debug.Log(_totalMountains);
        for (int i = 0; i < 3; i++) GenerateControl(Random.Range(1, _cellNumber / 2));
        GenerateSpawns(5, -10);
        //Implement team system
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
        foreach (HexCell cell in Cells[index].AdjacentTiles)
        {
            if (!cell) continue;
            if (cell.TerrainType == "Mountain") continue;
            if (Random.Range(0, 3) == 0)
            {
                if (mountainNumber < 1) return;
                _totalMountains++;
                Debug.Log("Generated MT at " + cell);
                cell.TerrainType = "Mountain";
                Cells[_cellNumber - 1 - cell.index].TerrainType = "Mountain";
                GenerateMountainRange(cell.index, mountainNumber / 2);
            }
        }
        //if (mountainNumber < 1) return;
        //int nextNumber = mountainNumber;
        ////Cells[index].TerrainType = "Mountain";
        ////Cells[symmetricalIndex].TerrainType = "Mountain";
        //foreach (HexCell cell in Cells[index].AdjacentTiles) {
        //	nextNumber--;
        //	Debug.Log("made mountain " + nextNumber);
        //	Cells[index].TerrainType = "Mountain";
        //	Cells[symmetricalIndex].TerrainType = "Mountain";
        //	if (cell) GenerateMountainRange(cell.index, mountainNumber/2);
        //};
    }
    void GenerateControl(int index)
    {
        int symmetricalIndex = _cellNumber - 1 - index;
        if (Cells[index].TerrainType == "Forest" || Cells[index].TerrainType == "Mountain")
        {
            GenerateControl(Random.Range(1, _cellNumber / 2));
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
            GenerateSpawns(Random.Range(0, 10), Random.Range(-11, -6));
        }
        else
        {
            potentialSpawn.TerrainType = "Spawn";
            Cells[_cellNumber - 1 - potentialSpawn.index].TerrainType = "Spawn";
            Manager.PlayerSpawnCell = potentialSpawn;
            Manager.EnemySpawnCell = Cells[_cellNumber - 1 - potentialSpawn.index];
            GameObject spawn = Instantiate(SpawnPrefab, potentialSpawn.transform);
            spawn.GetComponent<Spawn>().IsPlayerSpawn = true;
            spawn = Instantiate(SpawnPrefab, Cells[_cellNumber - 1 - potentialSpawn.index].transform);
            spawn.GetComponent<Spawn>().IsPlayerSpawn = false;
        }
    }
    //void GenerateControlPoints(int controlPointIndex)
    //{
    //	int symmetricalIndex = _cellNumber - 1 - controlPointIndex;
    //	Vector2Int basePosition = new Vector2Int(Cells[controlPointIndex].Position.x, Cells[controlPointIndex].Position.y);
    //       for (int i = 0; i < 14; i++)
    //       {
    //           HexCell cell = ReturnHex(basePosition.x + Random.Range(-2, 3), basePosition.y + Random.Range(-2, 3));
    //           if (cell.Weight == 0)
    //           {
    //               Cells[controlPointIndex].TerrainType = "Mountain";
    //               Cells[symmetricalIndex].TerrainType = "Mountain";
    //           }
    //       }
    //       Cells[controlPointIndex].TerrainType = "Spawn";
    //	Cells[symmetricalIndex].TerrainType = "Spawn";
    //}
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
        cell.transform.SetParent(transform, false);
        // Unity function to keep cell relative to its' game object
        cell.transform.localPosition = position;
        cell.name = new Vector2Int(x, y).ToString();
        // A debug measure: simple way to see which cell is which.
        cell.index = i;
    }
    void AddAdjacent(int x, int y, int i)
    {
        HexCell cell = Cells[i];
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

    public HexCell ReturnHex(int x, int y)
    {
        for (int i = 0; i < _cellNumber; i++)//3 * size * (size - 1) + 1)
        {
            if (new Vector2Int(Cells[i].Position.x, Cells[i].Position.y) == new Vector2Int(x, y))
            {
                return Cells[i];
            }
        }
        return null;
    }
}