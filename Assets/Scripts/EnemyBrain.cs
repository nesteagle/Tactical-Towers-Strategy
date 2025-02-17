using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyBrain : MonoBehaviour
{
    // Start is called before the first frame update
    public EnemyUnitManagement UnitManagement;
    public EnemyUnitPositioning UnitPositioning;
    public EnemyMilitaryBrain Military;
    public UnitComposition Composition;
    public Game Manager;

    public HashSet<Unit> ResourceGroup = new();
    // ResourceGroup is list of scouts.

    public Dictionary<Village, Unit> VillageAndScout = new();

    //maybe predefine at start and then start Think();

    public Dictionary<int, List<Unit>> PlayerZoneDistribution = new()
    {
        { 0, new List<Unit>() },
        { 1, new List<Unit>() },
        { 2, new List<Unit>() }
    };
    public Dictionary<int, List<Unit>> EnemyZoneDistribution = new()
    {
        { 0, new List<Unit>() },
        { 1, new List<Unit>() },
        { 2, new List<Unit>() }
    };

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Expand();

            //UnitManagement.AssignAllTargets(Manager.EnemyUnits);
            Expand();
            UpdateZoneDistributions();
            Military.Think();
        }
    }

    public void HandleDeath(Unit unit)
    {
        if (ResourceGroup.Contains(unit))
        {
            ResourceGroup.Remove(unit);
            // do something - maybe train another up.
        }
        else
        {
            EnemyZoneDistribution[GetZone(unit.transform.position.x)].Remove(unit);
        }
        if (UnitManagement.TargetAndDefenders.ContainsKey(unit))
        {
            UnitManagement.TargetAndDefenders.Remove(unit);
        }
    }

    private void ScoutVillage(Unit unit)
    {
        List<Village> villages = Manager.TotalVillages.OrderBy(v => Mathf.Abs(Vector2.Distance(v.transform.position, unit.transform.position))).ToList();
        for (int i = 0; i < 2; i++) // nearby 2 villages.
        {
            if (Military.NetPowerScores[GetZone(villages[i].transform.position.x)] <= -1)
            {
                // ASK FOR REINFORCEMENTS.
            }
            if (VillageAndScout.ContainsKey(villages[i])) continue;
            VillageAndScout.Add(villages[i], unit);
            unit.MoveTo(villages[i].Cell);
            return;
        }

        // THEN, evaluate remaining villages - accompany with troop push.

        foreach (Village v in villages)
        {
            int zoneScore = Military.NetPowerScores[GetZone(v.transform.position.x)];
            if (zoneScore <= -1)
            {
                // ASK FOR REINFORCEMENTS.
            } else if (zoneScore >= 2)
            {
                // PUSH with military in area
                //if (v.Control <= -1f || v.Control > 0.9f) continue;
                if (VillageAndScout.ContainsKey(v)) continue;
                VillageAndScout.Add(v, unit);
                unit.MoveTo(v.Cell);
                return;
            }
        }
    }
    private void Expand()
    {
        // If # of scouts < max, create one. Otherwise, send them to explore.
        if (!Manager.EnemySpawnCell.Occupied && ResourceGroup.Count < 3)
        {
            Unit newScout = Game.EnemySpawn.PlaceTroop("Scout");
            ResourceGroup.Add(newScout);
        }
        foreach (Village v in Manager.TotalVillages)
        {
            if (v.Control <= -1f) VillageAndScout.Remove(v);
        }
        foreach (Unit scout in ResourceGroup)
        {
            if (VillageAndScout.ContainsValue(scout)) continue;
            Debug.Log(scout.State);
            if (scout.State == "Rest") ScoutVillage(scout);
        }
    }

    public int GetEconomyScore()
    {
        int score = 0;
        score += (Manager.EnemyVillages.Count - Manager.PlayerVillages.Count) * 5;

        int moneyDifference = Manager.EnemyCoins - Manager.PlayerCoins;

        score += moneyDifference / 3;

        return score;
    }
    
    public void UpdateZoneDistributions()
    {
        PlayerZoneDistribution = GetUnitDistribution("Player");
        EnemyZoneDistribution = GetUnitDistribution("Enemy");
    }

    public Dictionary<int, List<Unit>> GetUnitDistribution(string team)
    {
        Dictionary<int, List<Unit>> zoneDistribution = new()
        {
            { 0, new List<Unit>() },
            { 1, new List<Unit>() },
            { 2, new List<Unit>() }
        };
        // Directions are oriented to player view.

        foreach (Unit unit in team == "Player" ? Manager.PlayerUnits : Manager.EnemyUnits)
        {
            if (ResourceGroup.Contains(unit)) continue;

            int zoneKey = GetZone(unit.transform.position.x);
            zoneDistribution[zoneKey].Add(unit);
        }

        return zoneDistribution;
    }

    public int GetZone(float x)
    {
        float leftBound = -7f * HexData.InnerRadius; //approx 4.1
        float rightBound = 7f * HexData.InnerRadius;

        // Horizontal Zones (Left, Middle, Right)
        int zone = x < leftBound ? 0 : x > rightBound ? 1 : 2;
        return zone;
    }
}
