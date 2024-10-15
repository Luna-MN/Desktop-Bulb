using Godot;
using System;
using System.Collections.Generic;
public partial class Main : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public List<Vector2> screenSizes = new List<Vector2>();
	[Export]
	public PackedScene windowScene;
	public Vector2I TargetPosition = new Vector2I(0, 0);
	public List<int> ScreenBounds = new List<int>();
	public override void _Ready()
	{
		GetViewport().GuiEmbedSubwindows = false;
		for (int i = 0; i < DisplayServer.GetScreenCount(); i++)
		{
			screenSizes.Add(DisplayServer.ScreenGetSize(i));
		}
		ScreenBounds.Add(DisplayServer.ScreenGetSize().X / 2);
		ScreenBounds.Add(DisplayServer.ScreenGetSize().Y / 2);
		//GetWindow().Size = new Vector2I(10000, 10000);
		//GetWindow().Position = new Vector2I(0, 0);
		//GetWindow().MousePassthroughPolygon picking up bulb and sleep area? 
	}
	public Vector2 LinearInterpolate(Vector2 b, float t, Vector2 p)
	{
		p = p.Lerp(b, t);
		return p;
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (TargetPosition == new Vector2I(0, 0) || ((Vector2)TargetPosition).DistanceTo(GetWindow().Position) < 50)
		{
			TargetPosition = new Vector2I(new RandomNumberGenerator().RandiRange(GetWindow().Position.X - ScreenBounds[0], GetWindow().Position.X + ScreenBounds[0]), new RandomNumberGenerator().RandiRange(GetWindow().Position.Y - ScreenBounds[1], GetWindow().Position.Y + ScreenBounds[1]));
			GD.Print(TargetPosition);
		}

		Vector2I pos = GetWindow().Position;
		Vector2 posV2 = LinearInterpolate(TargetPosition, (float)delta, pos);
		GetWindow().Position = new Vector2I((int)MathF.Ceiling(posV2.X), (int)MathF.Ceiling(posV2.Y));
		GD.Print(((Vector2)TargetPosition).DistanceTo(GetWindow().Position), delta);
	}

}
