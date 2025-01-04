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
    public UnitComposition Composition;
    public Game Manager;

    public HashSet<Unit> ResourceGroup = new();
    // ResourceGroup is list of scouts.

    private Dictionary<Village, Unit> _villages = new();

    //maybe predefine at start and then start Think();

    public Dictionary<string, List<Unit>> PlayerZoneUnits = new()
    {
        { "Left", new List<Unit>() },
        { "Middle", new List<Unit>() },
        { "Right", new List<Unit>() }
    };
    public Dictionary<string, List<Unit>> EnemyZoneUnits = new()
    {
        { "Left", new List<Unit>() },
        { "Middle", new List<Unit>() },
        { "Right", new List<Unit>() }
    };

    private string _defensePriority = null;
    private Queue<string> _unitQueue = new();

    private IEnumerator Think()
    {
        yield return new WaitForFixedUpdate();
        Expand();

        PlayerZoneUnits = GetUnitDistribution("Player");
        EnemyZoneUnits = GetUnitDistribution("Enemy");

        //CalculateEconomyWeight();
        //CalculateMilitaryWeight();

        //Debug.Log("ECONOMYWEIGHT" + _economyWeight);
        //Debug.Log("MILITARYWEIGHT" + _militaryWeight);

        //int randomValue = Random.Range(1, _economyWeight + _militaryWeight + 1);
        //Debug.Log(randomValue);
        //if (randomValue <= _economyWeight)
        //{
        //    Expand();
        //}
        //else
        //{
        //    //Military
        //    //train troops and set assign target groups using GetUnitGroups and GetNewComposition

        //}
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Expand();
            BuildArmy();
            //StartCoroutine(Think());
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
            EnemyZoneUnits[GetZone(unit.transform.position.x)].Remove(unit);
        }
    }

    private void ScoutVillage(Unit unit)
    {
        List<Village> villages = Manager.TotalVillages.OrderBy(v => Mathf.Abs(Vector2.Distance(v.transform.position, unit.transform.position))).ToList();
        foreach (Village v in villages)
        {
            if (v.Control <= -1f || v.Control > 0.9f) continue;
            if (_villages.ContainsKey(v)) continue;
            _villages.Add(v, unit);
            unit.MoveTo(v.Cell);
            return;
        }
    }
    private void Expand()
    {
        // If # of scouts < max, create one. Otherwise, send them to explore.
        if (!Manager.EnemySpawnCell.Occupied && ResourceGroup.Count < 5)
        {
            Unit newScout = Game.EnemySpawn.PlaceTroop("Scout");
            ResourceGroup.Add(newScout);
        }
        foreach (Village v in Manager.TotalVillages)
        {
            if (v.Control <= -1f) _villages.Remove(v);
        }
        foreach (Unit scout in ResourceGroup)
        {
            if (_villages.ContainsValue(scout)) continue;
            Debug.Log(scout.State);
            if (scout.State == "Rest") ScoutVillage(scout);
        }
    }

    private void BuildArmy()
    {
        if (Manager.PlayerUnits.Count == 0 && Manager.EnemyCoins > 0)
        {
            Unit newAttacker = Game.EnemySpawn.PlaceTroop("Knight");
        }

        if (Manager.PlayerUnits.Count > Manager.EnemyUnits.Count - ResourceGroup.Count)
        {
            int midDiff = PlayerZoneUnits["Middle"].Count - EnemyZoneUnits["Middle"].Count;
            int leftDiff = PlayerZoneUnits["Left"].Count - EnemyZoneUnits["Left"].Count;
            int rightDiff = PlayerZoneUnits["Right"].Count - EnemyZoneUnits["Right"].Count;

            switch (_defensePriority)
            {
                case null:
                    _defensePriority = midDiff > leftDiff ? midDiff > rightDiff ? "Middle" : "Right" : leftDiff > rightDiff ? "Left" : "Right";
                    break;
                case "Left":
                    if (leftDiff <= 0)
                    {
                        _defensePriority = rightDiff > midDiff ? "Right" : "Middle";
                    }
                    else if (2 * PlayerZoneUnits["Left"].Count > 3 * EnemyZoneUnits["Left"].Count)
                    {
                        Composition missing = Composition.GetMissingComposition(EnemyZoneUnits["Left"], PlayerZoneUnits["Left"]);
                        _unitQueue = Composition.CompositionToUnitQueue(missing);
                    }
                    break;
                case "Right":
                    if (rightDiff <= 0)
                    {
                        _defensePriority = leftDiff > midDiff ? "Left" : "Middle";
                    }
                    else if (2 * PlayerZoneUnits["Right"].Count > 3 * EnemyZoneUnits["Right"].Count)
                    {
                        Composition missing = Composition.GetMissingComposition(EnemyZoneUnits["Right"], PlayerZoneUnits["Right"]);
                        _unitQueue = Composition.CompositionToUnitQueue(missing);
                    }
                    break;
                case "Middle":
                    if (midDiff <= 0)
                    {
                        _defensePriority = leftDiff > rightDiff ? "Left" : "Right";
                    }
                    else if (2 * PlayerZoneUnits["Middle"].Count > 3 * EnemyZoneUnits["Middle"].Count)
                    {
                        Composition missing = Composition.GetMissingComposition(EnemyZoneUnits["Middle"], PlayerZoneUnits["Middle"]);
                        _unitQueue = Composition.CompositionToUnitQueue(missing);
                    }
                    break;
            }

            if (_unitQueue.Count > 0)
            {
                Unit unit = Game.EnemySpawn.PlaceTroop(_unitQueue.Peek());
                StartCoroutine(MoveWhenSpawned(unit, _defensePriority));
                _unitQueue.Dequeue();
            }
        }
    }
    private IEnumerator MoveWhenSpawned(Unit unit, string targetZone)
    {
        yield return new WaitUntil(() => unit.State == "Rest");
        switch (targetZone)
        {
            case "Middle":
                unit.MoveTo(Game.Map.ReturnHex(-3, 6));
                break;
            case "Left":
                unit.MoveTo(Game.Map.ReturnHex(-7, 7));
                break;
            case "Right":
                unit.MoveTo(Game.Map.ReturnHex(0, 7));
                break;
        }
    }
    private void UpdateGroupPurpose()
    {
        // several cases:
        // evaluate player strength and decide to attack or defend.
        // evaluate player position and decide next movement.
        // special cases: 
        // player near control point
        // player massing troops in back

        // Group has State "Attack" "Defend"
        // Group has HexCell Target - target position
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
}
