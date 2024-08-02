using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class Pathfinding : MonoBehaviour
{
    public static List<HexCell> FindPath(HexCell startPoint, HexCell endPoint)
    {
        List<HexCell> openPathTiles = new List<HexCell>();
        List<HexCell> closedPathTiles = new List<HexCell>();

        HexCell currentTile = startPoint;

        currentTile.G = 0;
        currentTile.H = GetEstimatedPathCost(startPoint.Position, endPoint.Position);
        openPathTiles.Add(currentTile);

        while (openPathTiles.Count != 0)
        {
            openPathTiles = openPathTiles.OrderBy(x => x.F).ThenByDescending(x => x.G).ToList();
            currentTile = openPathTiles[0];

            openPathTiles.Remove(currentTile);
            closedPathTiles.Add(currentTile);

            if (currentTile == endPoint)
            {
                break;
            }

            foreach (HexCell adjacentTile in currentTile.AdjacentTiles)
            {
                if (adjacentTile == null|| closedPathTiles.Contains(adjacentTile) || adjacentTile.Occupied )
                {
                    continue;
                }

                int g = currentTile.G + 1 + adjacentTile.Weight; // Consider the weight between the current tile and the adjacent tile.

                if (!(openPathTiles.Contains(adjacentTile)))
                {
                    adjacentTile.G = g;
                    adjacentTile.H = GetEstimatedPathCost(adjacentTile.Position, endPoint.Position);
                    openPathTiles.Add(adjacentTile);
                }
                else if (adjacentTile.G > g)
                {
                    adjacentTile.G = g;
                }
            }
        }

        List<HexCell> finalPathTiles = new List<HexCell>();

        //if (closedPathTiles.Contains(endPoint))
        //{
        //    currentTile = endPoint;
        //    finalPathTiles.Add(currentTile);

        //    for (int i = endPoint.G - 1; i >= 0; i--)
        //    {
        //        if (closedPathTiles.Find(x => x.G == i && currentTile.AdjacentTiles.Contains(x))) currentTile = closedPathTiles.Find(x => x.G == i && currentTile.AdjacentTiles.Contains(x));
        //        else return new List<HexCell>();
        //        finalPathTiles.Add(currentTile);
        //    }

        //    finalPathTiles.Reverse();
        //}
        HexCell cTile = endPoint;
        //int tileNum = 0;
        //while (cTile.Occupied)
        //{
        //    tileNum++;
        //    cTile = closedPathTiles[tileNum];
        //}

        while (cTile != startPoint)
        {
            finalPathTiles.Add(cTile);
            HexCell nextTile = null;
            int lowestG = int.MaxValue;

            if (!cTile) {
                return new List<HexCell>();}
            foreach (HexCell adjacentTile in cTile.AdjacentTiles)
            {
                if (adjacentTile == null || !closedPathTiles.Contains(adjacentTile))
                {
                    continue;
                }
                if (adjacentTile.G < lowestG)
                {
                    lowestG = adjacentTile.G;
                    nextTile = adjacentTile;
                }
            }

            cTile = nextTile;
        }
        finalPathTiles.Add(startPoint);
        finalPathTiles.Reverse();
        return finalPathTiles;
    }

    //public static List<HexCell> FindPath(HexCell startPoint, HexCell endPoint)
    //{
    //    List<HexCell> openPathTiles = new List<HexCell>();
    //    List<HexCell> closedPathTiles = new List<HexCell>();

    //    HexCell currentTile = startPoint;

    //    currentTile.G = 0;
    //    currentTile.H = GetEstimatedPathCost(startPoint.Position, endPoint.Position);
    //    openPathTiles.Add(currentTile);

    //    while (openPathTiles.Count != 0)
    //    {
    //        openPathTiles = openPathTiles.OrderBy(x => x.F).ThenByDescending(x => x.G).ToList();
    //        currentTile = openPathTiles[0];

    //        openPathTiles.Remove(currentTile);
    //        closedPathTiles.Add(currentTile);

    //        int g = currentTile.G + 1;

    //        if (currentTile == endPoint)
    //        {
    //            break;
    //        }

    //        foreach (HexCell adjacentTile in currentTile.AdjacentTiles)
    //        {

    //            if (adjacentTile == null)
    //            {
    //                continue;
    //            }
    //            if (adjacentTile.Occupied)
    //            {
    //                continue;
    //            }

    //            if (closedPathTiles.Contains(adjacentTile))
    //            {
    //                continue;
    //            }

    //            if (!(openPathTiles.Contains(adjacentTile)))
    //            {
    //                adjacentTile.G = g;
    //                adjacentTile.H = GetEstimatedPathCost(adjacentTile.Position, endPoint.Position);
    //                openPathTiles.Add(adjacentTile);

    //            }
    //            else if (adjacentTile.F > g + adjacentTile.H)
    //            {
    //                adjacentTile.G = g;
    //            }
    //        }
    //    }

    //    List<HexCell> finalPathTiles = new List<HexCell>();

    //    if (closedPathTiles.Contains(endPoint))
    //    {
    //        currentTile = endPoint;
    //        finalPathTiles.Add(currentTile);

    //        for (int i = endPoint.G - 1; i >= 0; i--)
    //        {
    //            currentTile = closedPathTiles.Find(x => x.G == i && currentTile.AdjacentTiles.Contains(x));
    //            finalPathTiles.Add(currentTile);
    //        }

    //        finalPathTiles.Reverse();
    //    }
    //    return finalPathTiles;
    //}
    protected static int GetEstimatedPathCost(Vector3Int startPosition, Vector3Int targetPosition)
    {
        return Mathf.Max(Mathf.Abs(startPosition.z - targetPosition.z), Mathf.Max(Mathf.Abs(startPosition.x - targetPosition.x), Mathf.Abs(startPosition.y - targetPosition.y)));
        //return Mathf.Max(Mathf.Abs(startPosition.x - targetPosition.x), Mathf.Max(Mathf.Abs(startPosition.y - targetPosition.y)));
        //, Mathf.Abs(startPosition.z - targetPosition.z)));

    }
}