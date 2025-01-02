using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyBrain : MonoBehaviour
{
    // Start is called before the first frame update
    public EnemyUnitManagement UnitManagement;
    public Game Manager;
    private int _economyWeight;
    private int _militaryWeight;
    public List<HashSet<Unit>> Groups;
    // Groups is a list of lists of enemies.
    private List<HexCell> _currentlyScouting;
    // tandem with ResourceGroup.
    public HashSet<Unit> ResourceGroup = new();
    // ResourceGroup is list of scouts.

    private Dictionary<Village,Unit> _villages = new();

    //maybe predefine at start and then start Think();
    
    private Dictionary<string, List<Unit>> playerZoneUnits = new();
    private Dictionary<string, List<Unit>> enemyZoneUnits = new();

    private IEnumerator Think()
    {
        yield return new WaitForFixedUpdate();
        Expand();

        playerZoneUnits = UnitManagement.GetUnitDistribution("Player");
        enemyZoneUnits = UnitManagement.GetUnitDistribution("Enemy");

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
            Expand();
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
            foreach (HashSet<Unit> subgroup in Groups)
            {
                if (subgroup.Contains(unit))
                {
                    subgroup.Remove(unit);
                    // do something - maybe train another up.
                    // also reevaluate weights and composition
                }
            }
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
        foreach(Village v in Manager.TotalVillages) 
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
        if (Manager.PlayerUnits.Count == 0) // Attack threshold
        {
            if (Manager.EnemyCoins > 0) // and rateofgrowth > ? !!!
            {
                Unit newAttacker = Game.EnemySpawn.PlaceTroop("Knight"); // !!!
                // do by zone
            }
        }

        if (Manager.PlayerUnits.Count > Manager.EnemyUnits.Count - ResourceGroup.Count)
        {

            if (playerZoneUnits["Middle"].Count / enemyZoneUnits["Middle"].Count > 1.5f)
            {
                Composition missing = UnitManagement.GetMissingComposition(enemyZoneUnits["Middle"], playerZoneUnits["Middle"]);
                // get missing composition units and train.
            }
            else if (playerZoneUnits["Left"].Count / enemyZoneUnits["Left"].Count > 1.5f)
            {
                // get missing composition units and train.
            }
            else if (playerZoneUnits["Right"].Count / enemyZoneUnits["Right"].Count > 1.5f)
            {
                // check resources for threshold
                // get missing composition units and train.
            }
        }
        // WEIGH MOST IMPORTANT SECTIONS and defend 2/3 of them.


        // other use of resources - build army to attack player or defend.
        // PURPOSE: create group to counter player group or attack.

        // check player strength and if existing groups
        // THEN: If group, make counter group
        //       if no group, train group of (knights?) to attack
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
}
