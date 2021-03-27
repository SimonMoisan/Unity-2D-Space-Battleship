using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MusicType { MainTitle, Starmap, BattlezoneCalm, BattlezonHostile, BattlezoneAction }
public class AudioManager : MonoBehaviour
{
    public AudioClip actualAudioClip;
    public AudioClip nextAudioClip;
    public AudioSource mainAudioSource; //For music
    public AudioSource secondaryAudioSource; //For UI SFx
    public Animator animatorAudioSource;

    public AudioClip mainTitleMusic;
    public AudioClip[] starmapMusics;
    public AudioClip[] battlezoneCalmMusics;
    public AudioClip[] battlezoneHostileMusics;
    public AudioClip[] battlezoneActionMusics;

    static public AudioManager current;

    private void OnValidate()
    {
        current = this;
    }

    public IEnumerator musicSwitch(MusicType nextMusicType)
    {
        nextAudioClip = chooseNewMusic(nextMusicType);
        animatorAudioSource.Play("Transition-Start");
        yield return new WaitForSeconds(1.2f);
        mainAudioSource.clip = actualAudioClip;
        mainAudioSource.Play();
        animatorAudioSource.Play("Transition-End");
    }

    public void switchAudioClip()
    {
        actualAudioClip = nextAudioClip;
    }

    public AudioClip chooseNewMusic(MusicType nextMusicType)
    {
        AudioClip nextMusicToPlay = null;
        int index = 0;

        switch (nextMusicType)
        {
            case MusicType.MainTitle:
                nextMusicToPlay = mainTitleMusic;
                break;
            case MusicType.Starmap:
                index = Random.Range(0, starmapMusics.Length);
                nextMusicToPlay = starmapMusics[index];
                break;
            case MusicType.BattlezoneCalm:
                index = Random.Range(0, battlezoneCalmMusics.Length);
                nextMusicToPlay = battlezoneCalmMusics[index];
                break;
            case MusicType.BattlezonHostile:
                index = Random.Range(0, battlezoneHostileMusics.Length);
                nextMusicToPlay = battlezoneHostileMusics[index];
                break;
            case MusicType.BattlezoneAction:
                index = Random.Range(0, battlezoneActionMusics.Length);
                nextMusicToPlay = battlezoneActionMusics[index];
                break;
        }
        return nextMusicToPlay;
    }
}
