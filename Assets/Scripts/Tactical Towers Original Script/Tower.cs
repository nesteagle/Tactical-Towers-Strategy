using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public float Range; //will be by Hexes (1 range=radius of 1 hex.)
    public float AttackCooldown;
    public float AttackPower;
    public Enemy TargetEnemy;
    public string TowerType;
    private GameControl _control;
    private void Awake()
    {
        _control = GameObject.Find("Control").GetComponent<GameControl>();
    }
    public float DamagePerSecond
    {
        get
        {
            return AttackPower / AttackCooldown;
        }
    }
}