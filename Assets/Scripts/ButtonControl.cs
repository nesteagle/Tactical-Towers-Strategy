using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonControl : MonoBehaviour
{
    public Canvas DeployMenu;
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
    public void CloseMenu()
    {
        DeployMenu.enabled = false;
        Game.PlayerSpawn.MenuOpened = false;
    }
}
