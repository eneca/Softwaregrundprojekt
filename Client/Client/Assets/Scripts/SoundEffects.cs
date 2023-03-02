using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SoundEffects : MonoBehaviour
{
    public GameObject musicPlayerGameobject;

    private AudioSource musicPlayer;
    private AudioSource effectsPlayer;
    private AudioClip[] sounds = new AudioClip[7];


    // Start is called before the first frame update
    void Start()
    {
        musicPlayer = musicPlayerGameobject.GetComponent<AudioSource>();
        effectsPlayer = gameObject.GetComponent<AudioSource>();
        InitialiseSounds();
    }

    //load sound effects in array
    private void InitialiseSounds()
    {
        sounds[0] = Resources.Load<AudioClip>("Sounds/loadingMusic");
        sounds[1] = Resources.Load<AudioClip>("Sounds/Oof");
        sounds[2] = Resources.Load<AudioClip>("Sounds/utini");
        sounds[3] = Resources.Load<AudioClip>("Sounds/wingardiumLeviosa");
        sounds[4] = Resources.Load<AudioClip>("Sounds/roar");
        sounds[5] = Resources.Load<AudioClip>("Sounds/boomheadshot");
        sounds[6] = Resources.Load<AudioClip>("Sounds/wololo");
    }

    //load sound settings from file
    private float LoadVolumenEffects()
    {
        string destination = Application.persistentDataPath + "/volumenEffectes.dat";
        FileStream file;
        float returnSound;
        if (File.Exists(destination))
        {
            file = File.OpenRead(destination);
            BinaryFormatter bf = new BinaryFormatter();
            returnSound = (float)bf.Deserialize(file);
            file.Close();
        }
        else
        {
            return 0.5f;
        }
        return returnSound;
    }

    //load musik settings from file
    private float LoadVolumenMusic()
    {
        string destination = Application.persistentDataPath + "/volumenMusic.dat";
        FileStream file;
        float returnSound;
        if (File.Exists(destination))
        {
            file = File.OpenRead(destination);
            BinaryFormatter bf = new BinaryFormatter();
            returnSound = (float)bf.Deserialize(file);
            file.Close();
        }
        else
        {
            return 0.5f;
        }
        return returnSound;
    }

    //Play Audio Clip
    //
    //      0: Loading Sound
    //      1: Oof
    //      2: utini
    //      3: wingardium leviosaaaaaaaa
    //      4: roar
    //      5: boomheadshot
    //      6: wololo
    //
    private void PlayAudioClip(AudioClip sound)
    {
        effectsPlayer.PlayOneShot(sound, LoadVolumenEffects());
    }

    //Play Sound
    public void PlaySound(int index)
    {
        effectsPlayer.volume = LoadVolumenEffects();
        PlayAudioClip(sounds[index]);
    }

    //Play Loading Musik
    public void StartLoadingMusic()
    {
        musicPlayer.Pause();
        effectsPlayer.volume = LoadVolumenEffects();
        effectsPlayer.loop = true;
        effectsPlayer.clip = sounds[0];
        effectsPlayer.Play();
    }
    //Stop Loading Musik
    public void StoppLoadingMusic()
    {
        effectsPlayer.Stop();
        effectsPlayer.loop = false;
        musicPlayer.UnPause();
    }
}
