using Godot;
using System;
using System.Collections.Generic;

public partial class SoundManager : Singleton<SoundManager>
{
    // [Export] AudioStream bgm;
    [Export] PackedScene audioPlayer;
    [Export] AudioStreamPlayer bgmPlayer;

    public override void _Ready()
    {
        if (bgmPlayer == null)
        {
            bgmPlayer = audioPlayer.Instantiate<AudioStreamPlayer>();
            AddChild(bgmPlayer);
        }

        PlayBGM();
    }

    public void PlayBGM(AudioStream bgm, float volumeDb = 0)
    {
        if (bgm == null)
        {
            GD.PrintErr("AudioStream is null! Cannot play BGM.");
            return;
        }

        if (bgmPlayer.Stream != bgm)
        {
            bgmPlayer.Stream = bgm;
            bgmPlayer.VolumeDb = volumeDb;
            bgmPlayer.Play();
        }
    }


    public void PlayBGM()
    {
        bgmPlayer.Play();
    }

    public void StopBGM()
    {
        bgmPlayer.Stop();
    }

    public void PlaySFX(AudioStream sfx, float volumeDb = 0)
    {
        var sfxPlayer = audioPlayer.Instantiate<AudioStreamPlayer>();
        AddChild(sfxPlayer);

        sfxPlayer.Stream = sfx;
        sfxPlayer.VolumeDb = volumeDb;
        sfxPlayer.Play();

        sfxPlayer.Finished += () => sfxPlayer.QueueFree();
    }
}
