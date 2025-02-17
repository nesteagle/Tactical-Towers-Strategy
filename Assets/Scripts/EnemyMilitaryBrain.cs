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
    // Zone evaluation      (DONE?)

    // - Power comparison   (DONE)
    // - - Through zone evaluation     (DONE)

    // - Position comparison   (DONE)
    // - - Updated through unit move events    TODO 

    // - use power and position comparisons to get strengths and weaknesses.    TODO

    // - Unit creation
    // - - refinement TODO

    // - Unit repuporsement based on unit types and position  TODO
    // - - GetClosestCounterUnit     TODO

    // - Unit movement
    // - - refinement TODO
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

    public List<int> NetPowerScores = new() { 0, 0, 0 };
    public List<int> NetPositionScores = new() { 0, 0, 0 };
    // TODO: handle player position scores to work with HandleMovement observer, make them destination position but weighted less.

    private Queue<string> _unitQueue = new();
    private int _economyScore;

    private static Dictionary<(int, int), (int x, int y)> _advancePositions = new()
    {
        // (zone, position): (0,0) represents a leftward defensive position.
        { (0, 0), (-7, 7) },
        { (1, 0), (-4, 8) }, // represents a defensive position
        { (2, 0), (0, 7) },

        { (0,1), (-8, 4) },
        { (1, 1), (-2, 4) }, // represents the default position
        { (2, 1), (4, 4) },

        { (0, 2), (-7, -1) },
        { (1, 2), (0, 0) }, // represents an slightly aggressive position
        { (2, 2), (8, -1) },

        { (0, 3), (-3, -5) },
        { (1, 3), (1, -2) }, // represents an aggressive position
        { (2, 3), (8, -5) },

        { (0, 4), (5, -10) },
        { (1, 4), (5, -10) }, // represents attacking player spawn.
        { (2, 4), (5, -10) },
    };

    // Evaluates scores and acts based on their values
    public void Think()
    {
        _economyScore = Brain.GetEconomyScore();
        Debug.Log(_economyScore);
        UpdateScores();
        int netScore = GetNetScore();
        // different playstyles based on net score.
        //if (netScore >= 5)
        //{
        //    EvaluatePlayerMistakes();

        //    // aggressive playstyle
        //    // - focus on attacking player units
        //    // - focus on unit creation
        //    // - focus on unit movement
        //}
        //else if (netScore <= -1)
        //{
        //    // defensive playstyle
        //    // - focus on defending enemy units
        //    // - focus on unit repurposement
        //    // - focus on unit movement
        //}
        //else
        //{
        //    // balanced playstyle
        //    // - focus on unit creation
        //    // - focus on unit movement
        //    // will try to bring netScore up to 3.
        //}

        for (int i = 0; i < 3; i++)
        {
            foreach (Unit u in Brain.EnemyZoneDistribution[i])
            {
                if (u.State == "Rest")
                {
                    int targetScore = GetNetZoneScores()[i];
                    int destIndex = GetDestinationByScore(targetScore);
                    u.MoveTo(Game.Map.ReturnHex(_advancePositions[(i, destIndex)].x,
                        _advancePositions[(i, destIndex)].y));
                }
            }

            BuildArmy();
        }
    }

    public void BuildArmy()
    {
        int weakestZone = GetNthWeakestZone(0);
        Debug.Log(weakestZone);
        if (GetNetZoneScores()[weakestZone] <= -1)
        {
            // BUILD TROOP

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
        else if (Manager.EnemyCoins >= 8)
        { // if coins not >=8, too risky to build troops.
          // good economy, evaluate netScore and act.

            Unit unit = Game.EnemySpawn.PlaceTroop("Knight");
            if (unit == null)
            {
                // doesn't have enough money, failed.
                return;
            };
            StartCoroutine(MoveWhenSpawned(unit, weakestZone));
        }
    }

    private IEnumerator MoveWhenSpawned(Unit unit, int targetZone)
    {
        _offsetPowerScores[targetZone] += 1;
        yield return new WaitUntil(() => unit.State == "Rest");
        switch (targetZone)
        {
            case 0:
                unit.MoveTo(FindAvailableCell(unit, Game.Map.ReturnHex(-8, 4), new List<HexCell>()));
                break;
            case 2:
                unit.MoveTo(FindAvailableCell(unit, Game.Map.ReturnHex(4, 4), new List<HexCell>()));
                break;
            default:
                unit.MoveTo(FindAvailableCell(unit, Game.Map.ReturnHex(-2, 4), new List<HexCell>()));
                break;
        }
        yield return new WaitUntil(() => unit.State == "Rest");
        _offsetPowerScores[targetZone] -= 1;
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
        Dictionary<int, List<Unit>> playerUnitDistribution = Brain.PlayerZoneDistribution;
        Dictionary<int, List<Unit>> enemyUnitDistribution = Brain.EnemyZoneDistribution;

        EnemyPowerScores = new()
        {
            enemyUnitDistribution[0].Count,
            enemyUnitDistribution[1].Count,
            enemyUnitDistribution[2].Count
        };
        PlayerPowerScores = new()
        {
            playerUnitDistribution[0].Count,
            playerUnitDistribution[1].Count,
            playerUnitDistribution[2].Count
        };

        EnemyPositionScores = new()
        {
            GetZonePositionScore(enemyUnitDistribution, 0),
            GetZonePositionScore(enemyUnitDistribution, 1),
            GetZonePositionScore(enemyUnitDistribution, 2)
        };

        PlayerPositionScores = new()
        {
            GetZonePositionScore(playerUnitDistribution, 0),
            GetZonePositionScore(playerUnitDistribution, 1),
            GetZonePositionScore(playerUnitDistribution, 2)
        };
        NetPowerScores = new()
        {
            EnemyPowerScores[0] - PlayerPowerScores[0] + _offsetPowerScores[0],
            EnemyPowerScores[1] - PlayerPowerScores[1] + _offsetPowerScores[1],
            EnemyPowerScores[2] - PlayerPowerScores[2] + _offsetPowerScores[2]
        };
        NetPositionScores = new()
        {
            // position score is slightly less weighted than power score.
            // Position score is influenced by:
            // player groupings
            // units in forests
            EnemyPositionScores[0] - PlayerPositionScores[0],
            EnemyPositionScores[1] - PlayerPositionScores[1],
            EnemyPositionScores[2] - PlayerPositionScores[2]
        };
    }

    public int GetZonePositionScore(Dictionary<int, List<Unit>> distribution, int zone)
    {
        float positionScore = 0;
        foreach (Unit u in distribution[zone])
        {
            positionScore += Mathf.Abs(Game.ENEMY_SPAWN_Y - u.transform.position.y) / 12f; // every 12 tiles down is "worth" 1 unit.
            if (u.State != "Training" &&
                u.CurrentTile.TerrainType == "Forest")
            {
                positionScore = positionScore * 1.3f;
            }
            // TODO: does player movement increase position score by a little bit?
        }
        return Mathf.RoundToInt(positionScore);
    }

    public List<int> GetNetZoneScores()
    {
        List<int> netScores = new()
        {
            NetPowerScores[0] + NetPositionScores[0],
            NetPowerScores[1] + NetPositionScores[1],
            NetPowerScores[2] + NetPositionScores[2]
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

    // Returns the nth weakest zone
    public int GetNthWeakestZone(int n)
    {
        List<int> netTotalScores = GetNetZoneScores();
        List<int> sortedScores = new(netTotalScores);
        sortedScores.Sort();

        int nthWeakestScore = sortedScores[n];
        int nthWeakestZoneIndex = netTotalScores.IndexOf(nthWeakestScore);

        return nthWeakestZoneIndex;
    }

    private int GetDestinationByScore(int targetScore)
    {
        if (targetScore <= -2) return 0;
        if (targetScore >= 6) return 4;
        else if (targetScore >= 4) return 3;
        else if (targetScore >= 2) return 2;
        else return 1;
    }

    // Evaluates strongest zone and tries to find player mistakes, and capitalizes on them.
    public void EvaluatePlayerMistakes()
    {
        int strongestZone = GetNthWeakestZone(2);
        if (NetPowerScores[strongestZone] >= 4)
        {
            foreach (Unit unit in Brain.PlayerZoneDistribution[strongestZone])
            {
                unit.MoveTo(FindAvailableCell(unit, Game.Map.ReturnHex(-8, 4), new List<HexCell>()));
            }
            // player has a strong zone, try to find mistakes.
            // - player has a weak zone, try to capitalize on it.
            // - - move units to that zone
            // - - create units to that zone
            // - - repurpose units to that zone
        }
    }
}