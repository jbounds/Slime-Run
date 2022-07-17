using Godot;
using Scripts.Enums;

public class OptionsScreen : Control
{
    public override void _Ready()
    {
        GetNode("Back").Connect("pressed", this, nameof(BackPressed));
    }

    public void BackPressed()
    {
        GetTree().ChangeScene("res://Scenes/TitleScreen.tscn");
    }

    public void ButtonDown()
    {
        var button = (TextureButton)GetNode("Easy");
        button.Modulate = new Color(0.6f, 0.6f, 0.6f);
    }
}
