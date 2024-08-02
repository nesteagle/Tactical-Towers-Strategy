using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class GameControl : MonoBehaviour
{
    public static bool InWave=false;
    public static int CurrentWave=0;
    public static UnityEvent StartWave= new();
    public static int health = 100;
    private Spawner _spawnerComponent;
    private void Awake()
    {
        _spawnerComponent = GetComponent<Spawner>();
    }
    public static void BeginWave()
    {
        InWave = true;
        StartWave.Invoke();
    }
    // Add method for starting waves here.
    //
}
