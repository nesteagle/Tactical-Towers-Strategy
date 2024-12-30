using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonControl : MonoBehaviour
{
    //public Spawn Deploy;

    //private void Awake()
    //{
    //    Deploy = Game.PlayerSpawn;
    //}
    public void PlaceScout()
    {
        Game.PlayerSpawn.PlaceTroop("Scout");
    }
    public void PlaceArcher()
    {
        Game.PlayerSpawn.PlaceTroop("Archer");
    }
    public void PlaceKnight()
    {
        Game.PlayerSpawn.PlaceTroop("Knight");
    }
    //private IEnumerator WaitForLoad()
    //{
    //    yield return new WaitForSeconds(0.2f);
    //    Deploy = Game.PlayerSpawn;
    //}
}
