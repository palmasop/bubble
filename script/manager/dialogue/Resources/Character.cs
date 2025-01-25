using Godot;

public enum CharacterT
{
    NoOne,
    Player,
    SunShine,
    Yandere,
    Angle,
}

[Tool]
[GlobalClass]
public partial class Character : Resource
{
    [Export] public CharacterT type = CharacterT.Player;
    [Export] public string name = "";
    [Export] public Texture2D image;

    public Character() { }

    public Character(string name, Texture2D image)
    {
        this.name = name;
        this.image = image;
    }

    public Character(string name) { this.name = name; }
}