using Godot;
using Scripts.Enums;

public class ButtonBehaviours : TextureButton
{
    public override void _Ready()
    {
        this.Connect("button_down", this, nameof(ButtonDown));
        this.Connect("button_up", this, nameof(ButtonUp));
    }

    public void ButtonDown()
    {
        this.Modulate = new Color(0.6f, 0.6f, 0.6f);
    }

    public void ButtonUp()
    {
        this.Modulate = new Color(1f, 1f, 1f);
    }
}
