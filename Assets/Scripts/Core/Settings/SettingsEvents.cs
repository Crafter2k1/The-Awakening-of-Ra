namespace Core.Settings
{
    public readonly struct SettingsLoaded
    {
        public readonly bool MusicOn;
        public readonly bool SfxOn;
        public SettingsLoaded(bool musicOn, bool sfxOn) { MusicOn = musicOn; SfxOn = sfxOn; }
    }
    
    public readonly struct SettingsChanged
    {
        public readonly bool MusicOn;
        public readonly bool SfxOn;
        public SettingsChanged(bool musicOn, bool sfxOn) { MusicOn = musicOn; SfxOn = sfxOn; }
    }
    
    public readonly struct MusicToggleRequested { public readonly bool IsOn; public MusicToggleRequested(bool isOn) { IsOn = isOn; } }
    public readonly struct SfxToggleRequested   { public readonly bool IsOn; public SfxToggleRequested(bool isOn)   { IsOn = isOn; } }
    
    public readonly struct SettingsSyncRequested { }
}