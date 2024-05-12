// ReSharper disable CheckNamespace

using Godot;
using Godot.Collections;

public partial class AudioManager : Node
{
    public const int MusicChannelCount = 2;
    public const int SoundEffectChannelCount = 10;

    public static AudioManager Instance => Core.GetSingleton<AudioManager>();

    #region Fields

    private Array<AudioStreamPlayer> _musicChannels;
    private Array<AudioStreamPlayer> _soundEffectChannels;
    private Array<AudioStreamPlayer> _linkedMusicChannels;

    #endregion

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;

        ConfigureMusicChannels();
        ConfigureLinkedMusicChannels();
        ConfigureSoundEffectChannels();
    }

    #region Internal Functions / Methods

    private void ConfigureMusicChannels()
    {
        var musicChannelNode = new Node()
        {
            Name = "Music Channels"
        };

        _musicChannels = new Array<AudioStreamPlayer>();
        for (var i = 0; i < MusicChannelCount; i++)
        {
            var player = new AudioStreamPlayer
            {
                Name = $"Channel {i + 1}",
                Bus = AudioBusses.Music.ToString(),
                ProcessMode = ProcessModeEnum.Always
            };
            player.Connect(AudioStreamPlayer.SignalName.Finished, Callable.From(() => OnMusicChannelFinished(player)));
            musicChannelNode.AddChild(player);
            _musicChannels.Add(player);
        }

        AddChild(musicChannelNode);
    }

    private void ConfigureLinkedMusicChannels()
    {
        var linkedMusicChannelNode = new Node()
        {
            Name = "Linked Music Channels"
        };

        _linkedMusicChannels = new Array<AudioStreamPlayer>();
        for (var i = 0; i < MusicChannelCount; i++)
        {
            var player = new AudioStreamPlayer
            {
                Name = $"Channel {i + 1}",
                Bus = AudioBusses.Music.ToString(),
                ProcessMode = ProcessModeEnum.Always
            };
            player.Connect(AudioStreamPlayer.SignalName.Finished, Callable.From(() => OnLinkedMusicChannelFinished(player)));
            linkedMusicChannelNode.AddChild(player);
            _linkedMusicChannels.Add(player);
        }

        AddChild(linkedMusicChannelNode);
    }

    private void ConfigureSoundEffectChannels()
    {
        var soundEffectsNode = new Node()
        {
            Name = "Sound Effects Channels"
        };

        _soundEffectChannels = new Array<AudioStreamPlayer>();
        for (var i = 0; i < SoundEffectChannelCount; i++)
        {
            var player = new AudioStreamPlayer
            {
                Name = $"Channel {i + 1}",
                Bus = AudioBusses.SFX.ToString(),
                ProcessMode = ProcessModeEnum.Always
            };
            player.Connect(AudioStreamPlayer.SignalName.Finished, Callable.From(() => OnSoundEffectChannelFinished(player)));
            soundEffectsNode.AddChild(player);
            _soundEffectChannels.Add(player);
        }

        AddChild(soundEffectsNode);
    }

    #endregion

    #region Audio Functions / Methods

    public void SetVolume(AudioBusses bus, float value)
    {
        AudioServer.SetBusVolumeDb((int)bus, Mathf.LinearToDb(value));
        AudioServer.SetBusMute((int)bus, value < 0.05f);
    }

    public void PlaySound(string path)
    {

    }

    #endregion

    #region Signals / Events

    private void OnMusicChannelFinished(AudioStreamPlayer player)
    {

    }

    private void OnLinkedMusicChannelFinished(AudioStreamPlayer player)
    {
        
    }

    private void OnSoundEffectChannelFinished(AudioStreamPlayer player)
    {

    }

    #endregion
}