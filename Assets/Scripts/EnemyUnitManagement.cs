using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyUnitManagement : MonoBehaviour
{
    // Start is called before the first frame update
    public Game Manager;
    private List<Unit> _playerTroops=new();
    private List<Vector2> _playerTroopPositions = new();
    public GameObject TempRenderObject;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Dictionary<Unit, List<Unit>> groups = GetUnitGroups();
            HashSet<List<Unit>> processed = new();

            foreach (KeyValuePair<Unit, List<Unit>> pair in groups)
            {
                if (!processed.Contains(pair.Value))
                {
                    GetNewComposition(pair.Value);
                    processed.Add(pair.Value);
                }
            }
            Debug.Log("DONE");
        }
    }
    private List<Unit> CheckPlayerTroops()
    {
        List<Unit> playerTroops=new();
        foreach(Unit unit in Manager.Units)
        {
            if (unit.Team == "Player")
            {
                if (!unit) continue;
                playerTroops.Add(unit);
            }
        }
        return playerTroops;
    }
    public List<float> GetScores()
    {
        Dictionary<Unit, List<Unit>> enemies = GetUnitGroups();
        List<float> scores=new();
        foreach(KeyValuePair<Unit, List<Unit>> pair in enemies)
        {
            scores.Add((pair.Key.transform.position.y + pair.Value.Count + 14f) * 0.12f);
        }
        return scores;
    }
    private void RenderScores()
    {
        List<Unit> enemies = CheckPlayerTroops();
        List<float> scores = GetScores();
        for (int i = 0; i < enemies.Count; i++)
        {
            Debug.Log(scores[i]);
            GameObject render = Instantiate(TempRenderObject);
            render.transform.position = enemies[i].transform.position;
            render.transform.localScale = new Vector2(scores[i], scores[i]);
        }
    }
    private Dictionary<Unit, List<Unit>> GetUnitGroups()
    {
        List<Unit> enemies = CheckPlayerTroops();
        Dictionary<Unit, List<Unit>> enemyDensity = new();
        HashSet<Unit> visited = new();

        foreach (Unit e in enemies)
        {
            List<Unit> group = new();
            if (visited.Contains(e)) continue;
            Queue<Unit> queue = new();
            queue.Enqueue(e);
            while (queue.Count > 0)
            {
                Unit current = queue.Dequeue();
                if (visited.Contains(current)) continue;
                visited.Add(current);
                group.Add(current);

                Collider2D[] hits = Physics2D.OverlapCircleAll(current.transform.position, HexData.InnerRadius * 3f);
                foreach (Collider2D hit in hits)
                {
                    GameObject enemyObject = hit.gameObject;
                    if (hit.gameObject.CompareTag("Enemy"))
                    {
                        Unit unit = enemyObject.GetComponent<Unit>();

                        if (unit.Team == "Player" && visited.Contains(unit) == false)
                        {
                            queue.Enqueue(unit);
                        }
                    }
                }
            }

            if (group.Count > 0)
            {
                foreach (Unit member in group)
                {
                    if (enemyDensity.ContainsKey(member) == false)
                    {
                        enemyDensity[member] = group;
                    }
                }
            }
            //int count = 0;
            //Collider2D[] hits = Physics2D.OverlapCircleAll(e.transform.position, HexData.InnerRadius * 4f);
            //foreach (Collider2D hit in hits)
            //{
            //    GameObject enemyObject = hit.gameObject;
            //    if (hit.gameObject.CompareTag("Enemy"))
            //    {
            //        Enemy enemy = enemyObject.GetComponent<Enemy>();

            //        if (enemy.OnPlayerTeam == true)
            //        {
            //            if (collided.Contains(enemy) == false)
            //            {
            //                collided.Add(enemy);
            //                count++;
            //            }
            //        }
            //    }
            //}
            //if (count == 0) continue;
            //enemyDensity.Add(e, count);

        }
        return enemyDensity;
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
        composition[2]+= Mathf.FloorToInt(unitValues[1] / 2.5f);
        //an archer for every floor(2x/5) knights
        composition[1]+= Mathf.FloorToInt(unitValues[1] *1.5f);
        composition[0] += Mathf.FloorToInt(unitValues[0] * 1.5f);
        //maybe economic consideration to comp[1]
        if (unitValues[2] >= 2)
        {
            //maybe add economic consideration here up above^^
            composition[1] += Mathf.FloorToInt(unitValues[2]*1.5f);
        }
        else composition[0] += Mathf.RoundToInt(unitValues[2] * 1.5f);
        foreach (int i in composition)
        {
            Debug.Log(i);
        }
        return composition;
    }
}
