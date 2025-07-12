using Godot;

namespace DesktopBulb;

public partial class ScreenHandler : Node
{
    public Vector2I ScreenBoundsMin, ScreenBoundsMax;
    public Vector2 Middle, ScreenSize;
    public override void _Ready()
    {
        GetViewport().GuiEmbedSubwindows = false;
        ScreenBoundsMin = DisplayServer.ScreenGetPosition();
        ScreenBoundsMax = new Vector2I(DisplayServer.ScreenGetPosition().X + DisplayServer.ScreenGetSize().X - 125,
            DisplayServer.ScreenGetPosition().Y + DisplayServer.ScreenGetSize().Y - 125);
        Middle = GetWindow().Position;
    }
    public override void _Process(double delta)
    {
        DetectScreenChange();
    }
    public bool DetectScreenChange()
    {
        if (ScreenSize != new Vector2(DisplayServer.ScreenGetSize().X, DisplayServer.ScreenGetSize().Y))
        {
            ScreenBoundsMin = DisplayServer.ScreenGetPosition();
            ScreenBoundsMax = new Vector2I(DisplayServer.ScreenGetPosition().X + DisplayServer.ScreenGetSize().X - 125, DisplayServer.ScreenGetPosition().Y + DisplayServer.ScreenGetSize().Y - 125);
            return true;
        }
        return false;
    }
}