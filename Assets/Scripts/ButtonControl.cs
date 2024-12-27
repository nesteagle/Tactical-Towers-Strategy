using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonControl : MonoBehaviour
{
    public Spawn Deploy;

    private void Start()
    {
        StartCoroutine(WaitForLoad());
    }
    public void PlaceScout()
    {
        Deploy.PlaceTroop("Scout");
    }
    public void PlaceArcher()
    {
       Deploy.PlaceTroop("Archer");
    }
    public void PlaceKnight()
    {
        Deploy.PlaceTroop("Knight");
    }
    private IEnumerator WaitForLoad()
    {
        yield return new WaitForSeconds(0.2f);
        Deploy = GameObject.Find("Control").GetComponent<Game>().PlayerSpawn;
    }
}
