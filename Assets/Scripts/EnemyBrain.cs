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

    public Dictionary<string, List<Unit>> PlayerZoneDistribution = new()
    {
        { "Left", new List<Unit>() },
        { "Middle", new List<Unit>() },
        { "Right", new List<Unit>() }
    };
    public Dictionary<string, List<Unit>> EnemyZoneDistribution = new()
    {
        { "Left", new List<Unit>() },
        { "Middle", new List<Unit>() },
        { "Right", new List<Unit>() }
    };

    private IEnumerator Think()
    {
        yield return new WaitForFixedUpdate();
        Expand();
        UpdateZoneDistributions();
    }

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
        foreach (Village v in villages)
        {
            if (v.Control <= -1f || v.Control > 0.9f) continue;
            if (VillageAndScout.ContainsKey(v)) continue;
            VillageAndScout.Add(v, unit);
            unit.MoveTo(v.Cell);
            return;
        }
    }
    private void Expand()
    {
        // If # of scouts < max, create one. Otherwise, send them to explore.
        if (!Manager.EnemySpawnCell.Occupied && ResourceGroup.Count < 4)
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

    public Dictionary<string, List<Unit>> GetUnitDistribution(string team)
    {
        Dictionary<string, List<Unit>> zoneDistribution = new()
        {
            { "Left", new List<Unit>() },
            { "Middle", new List<Unit>() },
            { "Right", new List<Unit>() }
        };
        // Directions are oriented to player view.

        foreach (Unit unit in team == "Player" ? Manager.PlayerUnits : Manager.EnemyUnits)
        {
            if (ResourceGroup.Contains(unit)) continue;

            string zoneKey = GetZone(unit.transform.position.x);
            zoneDistribution[zoneKey].Add(unit);
        }

        return zoneDistribution;
    }

    public string GetZone(float x)
    {
        float leftBound = -7f * HexData.InnerRadius; //approx 4.1
        float rightBound = 7f * HexData.InnerRadius;

        // Horizontal Zones (Left, Middle, Right)
        string zone = x < leftBound ? "Left" :
                        x > rightBound ? "Right" : "Middle";
        return zone;
    }
}
