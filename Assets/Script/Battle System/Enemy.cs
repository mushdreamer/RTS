using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health;
    internal void TakeDamage(int damageToInflict)
    {
        health -= damageToInflict;
    }

}
