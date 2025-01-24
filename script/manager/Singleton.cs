using Godot;

public abstract partial class Singleton<T> : Node where T : Node
{
    static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
                GD.PrintErr($"{typeof(T)} instance is not yet initialized!");
            return _instance;
        }
    }

    public override void _EnterTree()
    {
        if (_instance == null)
        {
            _instance = this as T;
        }
        else
        {
            GD.PrintErr($"{typeof(T)} instance already exists, removing duplicate");
            QueueFree();
        }
    }
}