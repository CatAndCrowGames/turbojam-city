using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] List<AudioClip> songs;
    AudioSource audioSource;

    static MusicPlayer instance;

    void Awake()
    {
        instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    public static void PlayRandomSongs()
    {
        instance.PlayRandom();
    }

    void PlayRandom()
    {
        StartCoroutine(PlaySongsRoutine());
    }

    IEnumerator PlaySongsRoutine()
    {
        while (true)
        {
            List<AudioClip> unplayedSongs = new(songs);

            while (unplayedSongs.Count > 0)
            {
                audioSource.Stop();
                AudioClip chosenSong = Helpers.RandomFromList(unplayedSongs);
                unplayedSongs.Remove(chosenSong);
                audioSource.clip = chosenSong;
                audioSource.Play();
                yield return new WaitForSeconds(chosenSong.length+1);
            }

            yield return new WaitForSeconds(10f);
        }
    }
}
