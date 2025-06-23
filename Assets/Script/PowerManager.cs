using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class PowerManager : MonoBehaviour
{
    public static PowerManager Instance { get; private set; }

    public int totalPower; // total power produced
    public int powerUsage; // total power consumed

    [SerializeField] private Image sliderFill;
    [SerializeField] private Slider powerSlider;
    [SerializeField] private TextMeshProUGUI powerText;

    public AudioClip powerAddedClip;
    public AudioClip powerInsufficientClip;

    private AudioSource powerAudioSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        powerAudioSource = gameObject.AddComponent<AudioSource>();
        powerAudioSource.volume = 0.1f;
    }

    public void AddPower(int amount)
    {
        PlayPowerAddedSound();
        totalPower += amount;
        UpdatePowerUI();
        Debug.Log("AddPower");
    }
    public void ConsumePower(int amount)
    {
        powerUsage += amount;
        UpdatePowerUI();
        Debug.Log("ConsumePower");

        if (IsInsufficientPower())
        {
            PlayPowerInsufficientSound();
        }
    }
    public void RemovePower(int amount)
    {
        totalPower -= amount;
        UpdatePowerUI();
        Debug.Log("RemovePower");

        if (IsInsufficientPower())
        {
            PlayPowerInsufficientSound();
        }
    }
    public void ReleasePower(int amount)
    {
        powerUsage -= amount;
        UpdatePowerUI();
        Debug.Log("ReleasePower");
    }

    private bool IsInsufficientPower()
    {
        return (totalPower - powerUsage) <= 0;
    }

    private void UpdatePowerUI()
    {
        int availablePower = totalPower - powerUsage;
        if (availablePower > 0)
        {
            sliderFill.gameObject.SetActive(true);
        }
        else
        {
            sliderFill.gameObject.SetActive(false);
        }

        if (powerSlider != null)
        {
            powerSlider.maxValue = totalPower;
            powerSlider.value = totalPower - powerUsage;
        }

        if (powerText != null)
        {
            powerText.text = $"{totalPower - powerUsage}/{totalPower}";
        }
    }

    public int CalculateAvailablePower()
    {
        return totalPower - powerUsage;
    }

    public void PlayPowerAddedSound()
    {
        Debug.Log("play PlayPowerAddedSound");
        powerAudioSource.PlayOneShot(powerAddedClip);
    }

    public void PlayPowerInsufficientSound()
    {
        if (powerAudioSource && powerInsufficientClip)
        {
            Debug.Log("play PlayPowerInsufficientSound");
            powerAudioSource.PlayOneShot(powerInsufficientClip);
        }
        else
        {
            Debug.Log("missing PlayPowerInsufficientSound or PlayPowerAddedSound");
        }
    }
}
