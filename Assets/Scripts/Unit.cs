using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using UnityEngine;
using UnityEngine.UIElements;

public class Unit : MonoBehaviour
{
    public float LocalHealth;
    public float Health
    {
        get
        {
            return LocalHealth;
        }
        set
        {
            LocalHealth = value;
            // Check alive or death state.
        }
    }
    // Health is float of enemy health.

    public float Speed = 1f;
    // Speed is movement speed.

    public float AttackDamage = 1f;
    // AttackDamage is the damage the enemy does per attack.

    public float Cooldown = 1f;
    // Cooldown is the total time between attack - 1/3 to charge and 2/3 of waiting cooldown.

    public Vector2Int TilePosition;
    // Position is the current tile position of the unit.

    public string State = "Rest";
    // State is one of "Rest" "Attacking" "Moving"; current status of unit.

    public string Type;
    // Type is one of "Scout" "Knight" "Archer"; unit type.

    public string Team;
    // Team is one of "Player" "Enemy"; team the unit is fighting for.

    public List<HexCell> CheckPath(HexCell destination)
    {
        List<HexCell> path = Pathfinding.FindPath(Game.Map.ReturnHex(TilePosition.x, TilePosition.y), destination);
        return path;
        // path is:
        // null - if not pathable
        // List<HexCell> - if successful
    }

    public void MoveTo(HexCell destination)
    {
        List<HexCell> path = CheckPath(destination);
        if (State == "Rest" && path != null)
        {
            StartCoroutine(MoveCoroutine(destination));
        }
    }
    private IEnumerator MoveCoroutine(HexCell destination)
    {
        while (TilePosition.x != destination.Position.x || TilePosition.y != destination.Position.y)
        {
            List<HexCell> path = CheckPath(destination);
            if (path == null)
            {
                State = "Rest";
                yield return new WaitUntil(() => path != null);
            }
            if (path.Count == 1)
            {
                State = "Rest";
                yield break;
            }
            State = "Moving";

            yield return StartCoroutine(MovePosition(transform.localPosition, path[1].transform.localPosition, Speed));
            TilePosition = new Vector2Int(path[1].Position.x, path[1].Position.y);
            path[0].ResetColor();
            path[1].ResetColor();
            path[0].Occupied = false;
            path[1].Occupied = true;
        }

        State = "Rest";
    }
    private IEnumerator MovePosition(Vector3 pos1, Vector3 pos2, float speed)
    {
        float i = 0;
        yield return new WaitForSeconds(0.15f);
        while (i < 1f)
        {
            i += Time.deltaTime * speed;
            transform.localPosition = Vector3.Lerp(pos1, new Vector3(pos2.x, pos2.y), i);
            yield return new WaitForEndOfFrame();
        }
    }

    public void HandleDeath()
    {
        // Removes unit from scene, stops any relying functions calling instance.
        // !!!
        StopAllCoroutines();
        GameObject.Find("Control").GetComponent<Game>().RemoveUnit(this);
        Destroy(gameObject);
    }
}
