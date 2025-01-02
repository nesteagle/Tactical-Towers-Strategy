using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Composition
{
    // Composition is army composition, consists of 3 integers representing the number of each unit type.
    public int Scouts;
    public int Knights;
    public int Archers;
    public Composition(int scouts, int knights, int archers)
    {
        Scouts = scouts; Knights = knights; Archers = archers;
    }
}

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

        foreach (Unit unit in team == "Player" ? Manager.PlayerUnits : Manager.EnemyUnits)
        {
            // if () continue; // if ResourceGroup?

            // Horizontal Zones (Left, Middle, Right)
            string zone = unit.transform.position.x < leftBound ? "Left" :
                                    unit.transform.position.x > rightBound ? "Right" : "Middle";
            string zoneKey = zone;
            zoneDistribution[zoneKey].Add(unit);
        }

        return zoneDistribution;
    }

    public (string State, float Score) EvaluateZone(string zone)
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
                return ("DEFEND", 100f); // change to more creative name - currently representing enemy incursion
            }
            averageY += y;
        }
        if (averageY / playerZoneUnits.Count <= -12 * HexData.InnerRadius) // CONSTANT !!!
        {
            return ("BUILDUP", playerZoneUnits.Count); // Player is building up troops in the back.
        }
        return ("NORMAL", playerZoneUnits.Count);
    }

    public Composition GroupToComposition(List<Unit> group)
    {
        int scoutCount = 0;
        int knightCount = 0;
        int archerCount = 0;

        foreach (Unit unit in group)
        {
            switch (unit.Type)
            {
                case "Archer":
                    archerCount++;
                    break;
                case "Knight":
                    knightCount++;
                    break;
                case "Scout":
                    scoutCount++;
                    break;
            }
        }
        return new Composition(scoutCount, knightCount, archerCount);
    }
    public Composition GetNewComposition(List<Unit> group)
    {
        int scoutCount = 0;
        int knightCount = 0;
        int archerCount = 0;

        Composition composition = GroupToComposition(group);

        const float archerToKnightRatio = 2.5f;
        const float knightMultiplier = 1.5f;
        const float scoutMultiplier = 1.5f;

        archerCount = Mathf.FloorToInt(composition.Knights / archerToKnightRatio);
        knightCount = Mathf.FloorToInt(composition.Knights * knightMultiplier);
        scoutCount = Mathf.FloorToInt(composition.Archers * scoutMultiplier);

        if (composition.Scouts >= 2)
        {
            knightCount += Mathf.FloorToInt(composition.Scouts * knightMultiplier);
        }
        else
        {
            scoutCount += composition.Scouts;
        }
        return new Composition(scoutCount, knightCount, archerCount);
    }
    public Composition GetMissingComposition(Composition curr, Composition total)
    {
        return new Composition(total.Scouts - curr.Scouts, total.Knights - curr.Knights, total.Archers - curr.Archers);
    }
}
