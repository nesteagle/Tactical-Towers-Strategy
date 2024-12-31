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
    // Update is called once per frame
    private IEnumerator Think()
    {
        yield return new WaitForEndOfFrame();
        Expand();
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







    private void CalculateEconomyWeight()
    {
        _economyWeight = Mathf.Abs((Manager.PlayerVillages.Count - Manager.EnemyVillages.Count) * 15);
    }
    private void CalculateMilitaryWeight()
    {
        float score = 0;
        foreach (float f in UnitManagement.GetScores())
        {
            score += f;
        }
        _militaryWeight = Mathf.RoundToInt(score * 1.3f);
    }

}
