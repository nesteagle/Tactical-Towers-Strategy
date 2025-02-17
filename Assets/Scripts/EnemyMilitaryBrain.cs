using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using UnityEngine;

public class EnemyMilitaryBrain : MonoBehaviour
{
    public EnemyBrain Brain;
    public UnitComposition Composition;
    public Game Manager;
    public EnemyUnitManagement UnitManagement;
    // TODO:
    // Needed methods:
    // Zone evaluation
    // - Power comparison
    // - - Through zone evaluation
    // - Position comparison
    // - - Updated through unit move events
    // - use power and position comparisons to get strengths and weaknesses.
    // - Unit creation
    // - Unit repuporsement based on unit types and position
    // - - GetClosestCounterUnit
    // - Unit movement
    // Unit control method ImprovePositioning()

    // Position score: score based on unit position and type of terrain they are in.
    // To increase: place troops in forests and mountains, and group up units.
    // Is also marginally impacted by y position - nearer to enemy = slightly higher score / unit.

    // Power score: raw power score. Used to offset position score if no other position improvements are possible.

    public List<int> EnemyPowerScores = new() { 0, 0, 0 };
    public List<int> EnemyPositionScores = new() { 0, 0, 0 };
    private List<int> _offsetPowerScores = new() { 0, 0, 0 };

    public List<int> PlayerPowerScores = new() { 0, 0, 0 };
    public List<int> PlayerPositionScores = new() { 0, 0, 0 };

    private Queue<string> _unitQueue = new();
    private int _economyScore;

    // Evaluates scores and acts based on their values
    public void Think()
    {
        _economyScore = Brain.GetEconomyScore();
        Debug.Log(_economyScore);
        UpdateScores();
        int netScore = GetNetScore();
        // different playstyles based on net score.
        if (netScore >= 5)
        {
            //EvaluatePlayerMistakes();

            // aggressive playstyle
            // - focus on attacking player units
            // - focus on unit creation
            // - focus on unit movement
        }
        else if (netScore <= -1)
        {
            // defensive playstyle
            // - focus on defending enemy units
            // - focus on unit repurposement
            // - focus on unit movement
        }
        else
        {
            // balanced playstyle
            // - focus on unit creation
            // - focus on unit movement
            // will try to bring netScore up to 3.
        }
        BuildArmy();
    }

    public void BuildArmy()
    {
        string weakestZone = ZoneIndexToName(GetWeakestZone());
        Debug.Log(weakestZone);
        if (GetNetZoneScores()[ZoneNameToIndex(weakestZone)]<=-1)
        {
            // BUILD TROOP

            Debug.Log("ZONE IS WEAK");
            
            Composition missing = Composition.GetMissingComposition(Brain.EnemyZoneDistribution[weakestZone], Brain.PlayerZoneDistribution[weakestZone]);
            _unitQueue = Composition.CompositionToUnitQueue(missing);

            if (_unitQueue.Count > 0)
            {
                Unit unit = Game.EnemySpawn.PlaceTroop(_unitQueue.Peek());
                _unitQueue.Dequeue();
                if (unit == null)
                {
                    // doesn't have enough money, failed.
                    return;
                };
                StartCoroutine(MoveWhenSpawned(unit, weakestZone));
                // ASSIGN TARGET !!!
            }
        }
        else if (Manager.EnemyCoins >= 8) { // if coins not >=8, too risky to build troops.
            // good economy, evaluate netScore and act.
            
            Debug.Log("STRONG ECONOMY");

            Unit unit = Game.EnemySpawn.PlaceTroop("Knight");
            if (unit == null)
            {
                // doesn't have enough money, failed.
                return;
            };
            StartCoroutine(MoveWhenSpawned(unit, weakestZone));
        }
    }

    private IEnumerator MoveWhenSpawned(Unit unit, string targetZone)
    {
        _offsetPowerScores[ZoneNameToIndex(targetZone)] += 1;
        yield return new WaitUntil(() => unit.State == "Rest");
        switch (targetZone)
        {
            case "Left":
                unit.MoveTo(FindAvailableCell(unit, Game.Map.ReturnHex(-8, 4), new List<HexCell>()));
                break;
            case "Right":
                unit.MoveTo(FindAvailableCell(unit, Game.Map.ReturnHex(4, 4), new List<HexCell>()));
                break;
            default:
                unit.MoveTo(FindAvailableCell(unit, Game.Map.ReturnHex(-2, 4), new List<HexCell>()));
                break;
        }
        yield return new WaitUntil(() => unit.State == "Rest");
        _offsetPowerScores[ZoneNameToIndex(targetZone)] -= 1;
    }
    private HexCell FindAvailableCell(Unit unit, HexCell cell, List<HexCell> visited)
    {
        if (unit.CheckPath(cell) != null) return cell;
        else
        {
            foreach (HexCell c in cell.AdjacentTiles)
            {
                if (visited.Contains(c) || c == null) continue;
                visited.Add(c);
                HexCell tryCell = FindAvailableCell(unit, c, visited);
                if (tryCell != null) return tryCell;
            }
            return null;
        }
    }

    public void UpdateScores()
    {
        Brain.UpdateZoneDistributions();
        Dictionary<string, List<Unit>> playerUnitDistribution = Brain.PlayerZoneDistribution;
        Dictionary<string, List<Unit>> enemyUnitDistribution = Brain.EnemyZoneDistribution;

        EnemyPowerScores = new()
        {
            enemyUnitDistribution["Left"].Count,
            enemyUnitDistribution["Middle"].Count,
            enemyUnitDistribution["Right"].Count
        };
        PlayerPowerScores = new()
        {
            playerUnitDistribution["Left"].Count,
            playerUnitDistribution["Middle"].Count,
            playerUnitDistribution["Right"].Count
        };

        EnemyPositionScores = new()
        {
            GetZonePositionScore(enemyUnitDistribution, "Left"),
            GetZonePositionScore(enemyUnitDistribution, "Middle"),
            GetZonePositionScore(enemyUnitDistribution, "Right")
        };

        PlayerPositionScores = new()
        {
            GetZonePositionScore(playerUnitDistribution, "Left"),
            GetZonePositionScore(playerUnitDistribution, "Middle"),
            GetZonePositionScore(playerUnitDistribution, "Right")
        };
    }

    public int GetZonePositionScore(Dictionary<string, List<Unit>> distribution, string zone)
    {
        float positionScore = 0;
        foreach (Unit u in distribution[zone])
        {
            positionScore += Mathf.Abs(Game.ENEMY_SPAWN_Y - u.transform.position.y) / 12f; // every 12 tiles down is "worth" 1 unit.
            if (u.State != "Training" && u.CurrentTile.TerrainType == "Forest")
            {
                positionScore = positionScore * 1.3f;
            }
            // TODO: does player movement increase position score by a little bit?
        }
        return Mathf.RoundToInt(positionScore);
    }

    public List<int> GetNetZoneScores()
    {
        List<int> netPowerScores = GetNetPowerScores();
        List<int> netPositionScores = GetNetPositionScores();
        List<int> netScores = new()
        {
            netPowerScores[0] + netPositionScores[0],
            netPowerScores[1] + netPositionScores[1],
            netPowerScores[2] + netPositionScores[2]
        };
        return netScores;
    }

    public int GetNetScore()
    {
        List<int> netScores = GetNetZoneScores();
        //int netScore = netScores.Sum();
        //netScore += Brain.GetEconomyScore() / 4;
        return netScores.Sum();
    }

    // Calculates and formulates response for priority zone (zone with least net score)
    public int GetWeakestZone()
    {
        int weakestZoneIndex = 1; // default to middle
        List<int> netPowerScores = GetNetPowerScores();
        List<int> netPositionScores = GetNetPositionScores();
        for (int i = 0; i < 3; i++)
        {
            if (netPowerScores[i] + netPositionScores[i] <
                netPowerScores[weakestZoneIndex] + netPositionScores[weakestZoneIndex])
            {
                weakestZoneIndex = i;
            }
        }
        return weakestZoneIndex;
        // now we have index of weakest zone.
        // diagnose via netPowerScores[weakestZoneIndex] and netPositionScores[weakestZoneIndex] for the fix.
    }

    // Evaluates weakest zone and tries to find player mistakes, and capitalizes on them.
    public void EvaluatePlayerMistakes()
    {

    }

    public string ZoneIndexToName(int index)
    {
        switch (index)
        {
            case 0:
                return "Left";
            case 1:
                return "Middle";
            default:
                return "Right";
        }
    }

    public int ZoneNameToIndex(string name)
    {

       switch (name)
        {
            case "Left":
                return 0;
            case "Middle":
                return 1;
            default:
                return 2;
        }
    }

    public List<int> GetNetPositionScores()
    {
        // position score is slightly less weighted than power score.
        // Position score is influenced by:
        // player groupings
        // units in forests
        List<int> netPositionScores = new()
        {
            EnemyPositionScores[0] - PlayerPositionScores[0],
            EnemyPositionScores[1] - PlayerPositionScores[1],
            EnemyPositionScores[2] - PlayerPositionScores[2]
        };
        return netPositionScores;
    }

    public List<int> GetNetPowerScores()
    {
        List<int> netPowerScores = new()
        {
            EnemyPowerScores[0] - PlayerPowerScores[0] + _offsetPowerScores[0],
            EnemyPowerScores[1] - PlayerPowerScores[1] + _offsetPowerScores[1],
            EnemyPowerScores[2] - PlayerPowerScores[2] + _offsetPowerScores[2]
        };
        return netPowerScores;
    }
}