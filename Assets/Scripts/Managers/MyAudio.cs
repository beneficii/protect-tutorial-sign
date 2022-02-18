using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MyAudio : MonoBehaviour
{
    public static MyAudio current;

    public AudioSource source;
    public List<AudioClip> clips;

    private void Awake()
    {
        current = this;
    }

    public void Play(AudioClip clip)
    {
        if (!clip) return;

        source.PlayOneShot(clip);
    }

    public void Play(Sounds sound)
    {
#if UNITY_WEBGL
        switch (sound) //webgl can't handle some sounds
        {
            case Sounds.Select: return;
        }
#endif

        int idx = (int)sound - 1;
        if (idx < 0) return;

        var clip = clips[idx];
        source.PlayOneShot(clip);
    }
}

public enum Sounds
{
    None,
    AttackWall,
    BigError,
    BuildingUpgrade,
    enemyDead,
    EnemyHit,
    Error,
    Hammer,
    PickupItem,
    PlaceBuilding,
    PlaceItem,
    pljak,
    Potion,
    Select,
    WallDead,
    Select2,
    PlaceBlock,
    Victory,
    Lose
}
