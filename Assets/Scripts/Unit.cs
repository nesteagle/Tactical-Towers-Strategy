using System.Collections;
using System.Collections.Generic;
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

    [SerializeField]
    public const float Speed = 1f;
    // Speed is movement speed.

    [SerializeField]
    public const float AttackDamage = 1f;
    // AttackDamage is the damage the enemy does per attack.

    [SerializeField]
    public const float Cooldown = 1f;
    // Cooldown is the total time between attack - 1/3 to charge and 2/3 of waiting cooldown.

    public Vector2Int TilePosition;
    // Position is the current tile position of the unit.

    public string State = "Rest";
    // State is one of "Rest" "Attacking" "Moving"; current status of unit.

    public string Type;
    // Type is one of "Scout" "Knight" "Archer"; unit type.

    public string Team;
    // Team is one of "Player" "Enemy"; team the unit is fighting for.

    public List<HexCell> CheckPath(int objectiveX, int objectiveY)
    {
        List<HexCell> path = Pathfinding.FindPath(Game.Map.ReturnHex(TilePosition.x, TilePosition.y), Game.Map.ReturnHex(objectiveX, objectiveY));
        return path;
        // path is:
        // null - if not pathable
        // List<HexCell> - if successful
    }

    public void MoveTo(int objectiveX, int objectiveY)
    {
        List<HexCell> path = CheckPath(objectiveX, objectiveY);
        if (State == "Moving" && path != null)
        {
            // coroutine for moving.
        }
    }
    private IEnumerator MoveCoroutine(int objectiveX, int objectiveY)
    {
        while (TilePosition.x != objectiveX && TilePosition.y != objectiveY)
        {
            List<HexCell> path = CheckPath(objectiveX, objectiveY);
            if (path == null)
            {
                State = "Rest";
                yield return new WaitUntil(() => path != null);
            }

            State = "Moving";
            yield return StartCoroutine(MovePosition(transform.localPosition, path[1].transform.localPosition, Speed));
            TilePosition = new Vector2Int(path[1].Position.x, path[1].Position.y);
            path[0].ResetColor();
            path[1].ResetColor();
        }
        /// !!!

        State = "Rest";
        yield break;
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

    public Unit HandleDeath()
    {
        // Removes unit from scene, stops any relying functions calling instance.
        // !!!
        return this;
    }
}
