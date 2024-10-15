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
	public override void _Ready()
	{
		GetViewport().GuiEmbedSubwindows = false;
		for (int i = 0; i < DisplayServer.GetScreenCount(); i++)
		{
			screenSizes.Add(DisplayServer.ScreenGetSize(i));
		}
		GetWindow().MousePassthrough = true;
		//GetWindow().Size = new Vector2I(10000, 10000);
		//GetWindow().Position = new Vector2I(0, 0);
		//GetWindow().MousePassthroughPolygon picking up bulb and sleep area? 
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (TargetPosition == new Vector2I(0, 0) || TargetPosition == Position)
		{
			TargetPosition = new Vector2I(new RandomNumberGenerator().RandiRange(0, DisplayServer.ScreenGetSize().X), new RandomNumberGenerator().RandiRange(0, DisplayServer.ScreenGetSize().Y));
		}
		Vector2I pos = GetWindow().Position;
		Vector2 posV2 = ((Vector2)pos).Lerp(TargetPosition, (float)delta);
		pos = (Vector2I)posV2;
		GetWindow().Position += pos;
		GD.Print(GetWindow().Position);
	}
}
