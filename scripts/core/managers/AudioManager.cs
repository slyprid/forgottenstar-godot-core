using Godot;

public partial class AudioManager : Node
{
    public static AudioManager Instance => Core.GetSingleton<AudioManager>();

    public void SetVolume(AudioBusses bus, float value)
    {
        AudioServer.SetBusVolumeDb((int)bus, Mathf.LinearToDb(value));
        AudioServer.SetBusMute((int)bus, value < 0.05f);
    }
}