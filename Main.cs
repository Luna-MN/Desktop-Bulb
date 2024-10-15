using Godot;
using System;
using System.Collections.Generic;
public partial class Main : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public List<Vector2> screenSizes = new List<Vector2>();
	[Export]
	public PackedScene windowScene;
	public Vector2 TargetPosition = new Vector2I(0, 0), middle;
	public List<int> ScreenBounds = new List<int>();
	public float oldPos;
	public override void _Ready()
	{
		GetViewport().GuiEmbedSubwindows = false;
		for (int i = 0; i < DisplayServer.GetScreenCount(); i++)
		{
			screenSizes.Add(DisplayServer.ScreenGetSize(i));
		}
		ScreenBounds.Add(DisplayServer.ScreenGetSize().X / 2);
		ScreenBounds.Add(DisplayServer.ScreenGetSize().Y / 2);
		middle = GetWindow().Position;

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
		RandomMove((float)delta);
	}
	public void RandomMove(float delta)
	{
		if (TargetPosition == new Vector2I(0, 0) || TargetPosition.DistanceTo(GetWindow().Position) < 50 || TargetPosition.DistanceTo(GetWindow().Position) == oldPos)
		{
			TargetPosition = new Vector2(new RandomNumberGenerator().RandiRange((int)middle.X - ScreenBounds[0], (int)middle.X + ScreenBounds[0]), new RandomNumberGenerator().RandiRange((int)middle.Y - ScreenBounds[1], (int)middle.Y + ScreenBounds[1]));
		}

		Vector2I pos = GetWindow().Position;
		Vector2 posV2 = LinearInterpolate(TargetPosition, delta, pos);
		oldPos = TargetPosition.DistanceTo(GetWindow().Position);
		GetWindow().Position = new Vector2I((int)MathF.Ceiling(posV2.X), (int)MathF.Ceiling(posV2.Y));
		GD.Print(((Vector2)TargetPosition).DistanceTo(GetWindow().Position), " + ", oldPos);
	}

}
