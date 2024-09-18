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
    private List<Enemy> _playerTroops=new();
    private List<Vector2> _playerTroopPositions = new();
    public GameObject TempRenderObject;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Dictionary<Enemy, List<Enemy>> groups = GetUnitGroups();
            HashSet<List<Enemy>> processed = new();

            foreach (KeyValuePair<Enemy, List<Enemy>> pair in groups)
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
    private List<Enemy> CheckPlayerTroops()
    {
        List<Enemy> playerTroops=new();
        foreach(Enemy enemy in Manager.Enemies)
        {
            if (enemy.OnPlayerTeam == true)
            {
                if (!enemy) continue;
                playerTroops.Add(enemy);
            }
        }
        return playerTroops;
    }
    public List<float> GetScores()
    {
        Dictionary<Enemy, List<Enemy>> enemies = GetUnitGroups();
        List<float> scores=new();
        foreach(KeyValuePair<Enemy,List<Enemy>> pair in enemies)
        {
            scores.Add((pair.Key.transform.position.y + pair.Value.Count + 14f) * 0.12f);
        }
        return scores;
    }
    private void RenderScores()
    {
        List<Enemy> enemies = CheckPlayerTroops();
        List<float> scores = GetScores();
        for (int i = 0; i < enemies.Count; i++)
        {
            Debug.Log(scores[i]);
            GameObject render = Instantiate(TempRenderObject);
            render.transform.position = enemies[i].transform.position;
            render.transform.localScale = new Vector2(scores[i], scores[i]);
        }
    }
    private Dictionary<Enemy, List<Enemy>> GetUnitGroups()
    {
        List<Enemy> enemies = CheckPlayerTroops();
        Dictionary<Enemy, List<Enemy>> enemyDensity = new();
        HashSet<Enemy> visited = new();

        foreach (Enemy e in enemies)
        {
            List<Enemy> group = new();
            if (visited.Contains(e)) continue;
            Queue<Enemy> queue = new();
            queue.Enqueue(e);
            while (queue.Count > 0)
            {
                Enemy current = queue.Dequeue();
                if (visited.Contains(current)) continue;
                visited.Add(current);
                group.Add(current);

                Collider2D[] hits = Physics2D.OverlapCircleAll(current.transform.position, HexData.InnerRadius * 3f);
                foreach (Collider2D hit in hits)
                {
                    GameObject enemyObject = hit.gameObject;
                    if (hit.gameObject.CompareTag("Enemy"))
                    {
                        Enemy enemy = enemyObject.GetComponent<Enemy>();

                        if (enemy.OnPlayerTeam == true && visited.Contains(enemy) == false)
                        {
                            queue.Enqueue(enemy);
                        }
                    }
                }
            }

            if (group.Count > 0)
            {
                foreach (Enemy member in group)
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
    private int[] GetNewComposition(List<Enemy> group)
    {
        int[] unitValues = new int[] { 0, 0, 0 };
        int[] composition = new int[3];
        //0 is scout, 1 is knight, 2 is archer
        //knight>scout, archer>knight, scout>archer
        Debug.Log(group.Count);
        foreach (Enemy enemy in group)
        {
            switch (enemy.Type)
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
