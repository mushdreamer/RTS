using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; set; }

    [Header("Infantry")]
    private AudioSource infantryAttackChannel;
    public AudioClip infantryAttackClip;

    [Header("Unit")]
    public int poolSize = 2;
    private int unitCurrentPoolIndex = 0;
    private int constructionCurrentPoolIndex = 0;

    private AudioSource[] unitVoiceChannelPool;

    public AudioClip[] unitSelectionSounds;
    public AudioClip[] unitCommandSounds;

    [Header("Buildings")]
    private AudioSource destructionBuildingChannel;
    private AudioSource[] constructionBuildingChannelPool;
    private AudioSource extraBuildingChannel;

    public AudioClip sellingSound;
    public AudioClip buildingConstructionSound;
    public AudioClip buildingDestructionSound;

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

        infantryAttackChannel = gameObject.AddComponent<AudioSource>();

        destructionBuildingChannel = gameObject.AddComponent<AudioSource>();

        constructionBuildingChannelPool = new AudioSource[3];

        for (int i = 0; i < 3; i++)
        {
            constructionBuildingChannelPool[i] = gameObject.AddComponent<AudioSource>();
        }

        extraBuildingChannel = gameObject.AddComponent<AudioSource>();

        // create array of audio sources
        unitVoiceChannelPool = new AudioSource[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            unitVoiceChannelPool[i] = gameObject.AddComponent<AudioSource>();
        }
    }

    public void PlayInfantryAttackSound()
    {
        if(infantryAttackChannel.isPlaying == false)
        {
            infantryAttackChannel.PlayOneShot(infantryAttackClip);
        }
    }
    public void PlayBuildingSellingSound()
    {
        if (extraBuildingChannel != null && extraBuildingChannel.isPlaying == false)
        {
            extraBuildingChannel.PlayOneShot(sellingSound);
        }
    }
    public void PlayBuildingConstructionSound()
    {
        Debug.Log("PlayBuildingConstructionSound");
        constructionBuildingChannelPool[constructionCurrentPoolIndex].PlayOneShot(buildingConstructionSound);

        unitCurrentPoolIndex = (constructionCurrentPoolIndex + 1) % poolSize;
    }
    public void PlayBuildingDestructionSound()
    {
        //future when we have more sound
        //public void PlayBuildingDestructionSound(BuildingType type)
        /*switch (type)
        {
            case BuildingType.CommandCenter:
                destructionBuildingChannel.PlayOneShot(commandCenterdestructionSound);
                break;
            case BuildingType.PowerPlant:
                destructionBuildingChannel.PlayOneShot(powerPlantdestructionSound);
                break;
        }*/
        if (destructionBuildingChannel.isPlaying == false)
        {
            destructionBuildingChannel.PlayOneShot(buildingDestructionSound);
        }
    }
    public void PlayUnitSelectionSound()
    {
        AudioClip randomClip = unitSelectionSounds[Random.Range(0, unitSelectionSounds.Length)];

        unitVoiceChannelPool[unitCurrentPoolIndex].PlayOneShot(randomClip);

        unitCurrentPoolIndex = (unitCurrentPoolIndex + 1) % poolSize;
    }
    public void PlayUnitCommandSound()
    {
        AudioClip randomClip = unitCommandSounds[Random.Range(0, unitCommandSounds.Length)];

        unitVoiceChannelPool[unitCurrentPoolIndex].PlayOneShot(randomClip);

        unitCurrentPoolIndex = (unitCurrentPoolIndex + 1) % poolSize;
    }
}
