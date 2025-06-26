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

    // <<< 1. 在这里添加一个公共变量，用于链接破坏特效的Prefab >>>
    [Header("Effects")]
    public GameObject destructionEffectPrefab;

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

            // <<< 2. 在销毁对象前，实例化破坏特效 >>>
            if (destructionEffectPrefab != null)
            {
                Instantiate(destructionEffectPrefab, transform.position, transform.rotation);
            }
            // <<< ----------------------------------- >>>

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
        // --- Add this English diagnostic line! ---
        Debug.Log($"[DIAGNOSTIC] {name} took {damage} damage. Health remaining: {constHealth} / {constMaxHealth}");
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
