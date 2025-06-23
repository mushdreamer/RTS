using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class Constructable : MonoBehaviour, IDamageable
{
    private float constHealth;
    public float constMaxHealth;

    public HealthTracker healthTracker;

    public bool isEnemy = false;

    NavMeshObstacle obstacle;

    public BuildingType buildingType;

    public Vector3 buildPosition;

    public bool inPreviewMode;

    private void Start()
    {
        constHealth = constMaxHealth;
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        healthTracker.UpdateSliderValue(constHealth, constMaxHealth);

        if (constHealth <= 0)
        {
            ResourceManager.Instance.UpdateBuildingChanged(buildingType, false, buildPosition);

            SoundManager.Instance.PlayBuildingDestructionSound();//future when we have more sounds //SoundManager.Instance.PlayBuildingDestructionSound(buildingType);

            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (inPreviewMode == false)
        {
            if (constHealth > 0 && buildPosition != Vector3.zero)
            {
                ResourceManager.Instance.SellBuilding(buildingType);
            }
        }
        
    }

    public void TakeDamage(int damage)
    {
        constHealth -= damage;
        UpdateHealthUI();
    }

    public void ConstructableWasPlaced(Vector3 position)
    {
        buildPosition = position;

        inPreviewMode = false;

        healthTracker.gameObject.SetActive(true);
        ActivateObstacle();

        if (isEnemy)
        {
            gameObject.tag = "Enemy";
        }

        if (GetComponent<PowerUser>() != null)
        {
            GetComponent<PowerUser>().PowerOn();
        }
    }

    private void ActivateObstacle()
    {
        if (isEnemy)
        {
            gameObject.tag = "Enemy";
        }

        obstacle = GetComponentInChildren<NavMeshObstacle>();
        obstacle.enabled = true;
    }
}
