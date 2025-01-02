using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyUnitManagement : MonoBehaviour
{
    // Start is called before the first frame update
    public Game Manager;
    private List<Unit> _playerTroops = new();
    private List<Vector2> _playerTroopPositions = new();
    public GameObject TempRenderObject;

    public Dictionary<string, List<Unit>> GetUnitDistribution(string team)
    {
        Dictionary<string, List<Unit>> zoneDistribution = new()
    {
        { "Left", new List<Unit>() },
        { "Middle", new List<Unit>() },
        { "Right", new List<Unit>() }
    };
        // Directions are oriented to player view.

        float leftBound = -7f * HexData.InnerRadius;
        float rightBound = 7f * HexData.InnerRadius;

        foreach (Unit unit in team == "Player" ? Manager.PlayerUnits: Manager.EnemyUnits)
        {
            // Horizontal Zones (Left, Middle, Right)
            string zone = unit.transform.position.x < leftBound ? "Left" :
                                    unit.transform.position.x > rightBound ? "Right" : "Middle";
            string zoneKey = zone;
            zoneDistribution[zoneKey].Add(unit);
        }

        return zoneDistribution;
    }

    private (string State, float Score) EvaluateZone(string zone)
    {
        // Zone must be one of: "Left", "Middle", "Right"
        List<Unit> playerZoneUnits = GetUnitDistribution("Player")[zone];
        List<Unit> enemyZoneUnits = GetUnitDistribution("Enemy")[zone];

        float averageY = 0f;
        if (playerZoneUnits.Count == 0 || enemyZoneUnits.Count / playerZoneUnits.Count > 3) return ("ATTACK", 5f); // adjust weight system?
        foreach (Unit u in playerZoneUnits)
        {
            float y = u.transform.position.y;
            if (y > 6 * HexData.InnerRadius)
            {
                return ("DEF", 100f); // change to more creative name; DEF-B currently means enemy incursion.
            }
            averageY += y;
        }
        if (averageY / playerZoneUnits.Count <= -12 * HexData.InnerRadius) // CONSTANT !!!
        {
            return ("BUILDUP", playerZoneUnits.Count); // Player is building up troops in the back.
        }
        return ("NORMAL", playerZoneUnits.Count);
    }

    private int[] GetNewComposition(List<Unit> group)
    {
        int[] unitValues = new int[] { 0, 0, 0 };
        int[] composition = new int[3];
        //0 is scout, 1 is knight, 2 is archer
        //knight>scout, archer>knight, scout>archer
        Debug.Log(group.Count);
        foreach (Unit u in group)
        {
            switch (u.Type)
            {
                case "Archer":
                    unitValues[2]++;
                    break;
                case "Knight":
                    unitValues[1]++;
                    break;
                case "Scout":
                    unitValues[0]++;
                    break;
            }
        }
        composition[2] += Mathf.FloorToInt(unitValues[1] / 2.5f);
        //an archer for every floor(2x/5) knights
        composition[1] += Mathf.FloorToInt(unitValues[1] * 1.5f);
        composition[0] += Mathf.FloorToInt(unitValues[0] * 1.5f);
        //maybe economic consideration to comp[1]
        if (unitValues[2] >= 2)
        {
            //maybe add economic consideration here up above^^
            composition[1] += Mathf.FloorToInt(unitValues[2] * 1.5f);
        }
        else composition[0] += Mathf.RoundToInt(unitValues[2] * 1.5f);
        foreach (int i in composition)
        {
            Debug.Log(i);
        }
        return composition;
    }
}
