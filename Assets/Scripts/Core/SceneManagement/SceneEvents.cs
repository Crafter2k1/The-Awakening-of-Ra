namespace Core.SceneManagement
{
    public readonly struct SceneLoadingStarted { public readonly string Target; public SceneLoadingStarted(string t)=>Target=t; }
    public readonly struct SceneLoaded        { public readonly string Name;   public SceneLoaded(string n)=>Name=n; }
    public readonly struct SceneReady         { public readonly string Name;   public SceneReady(string n)=>Name=n; }
}