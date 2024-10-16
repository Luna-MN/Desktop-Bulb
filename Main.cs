using Godot;
using System;
using System.Collections.Generic;
public partial class Main : Node2D
{
	// Called when the node enters the scene tree for the first time.
	[Export]
	public PackedScene windowScene;
	public Vector2 TargetPosition = new Vector2(0, 0), middle, screenSize;
	public Vector2I ScreenBoundsmin, ScreenBoundsmax;
	public float oldPos;
	public bool isMouse, timerStart;
	public Timer timer;
	public int randomChoice, maxChoice = 1;
	public Callable callable;
	public override void _Ready()
	{
		GetViewport().GuiEmbedSubwindows = false;
		ScreenBoundsmin = DisplayServer.ScreenGetPosition();
		ScreenBoundsmax = new Vector2I(DisplayServer.ScreenGetPosition().X + DisplayServer.ScreenGetSize().X - 125, DisplayServer.ScreenGetPosition().Y + DisplayServer.ScreenGetSize().Y - 125);
		middle = GetWindow().Position;
		GD.Print(DisplayServer.ScreenGetPosition());
		randomChoice = new RandomNumberGenerator().RandiRange(0, maxChoice);
		GD.Print(randomChoice);
		timer = new Timer { WaitTime = 60, Autostart = true };
		AddChild(timer);
		callable = new Callable(this, "timerTimeout");
		timer.Connect("timeout", callable);
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

		if (randomChoice == 0)
		{
			RandomMove((float)delta);
		}
		else if (randomChoice == 1)
		{
			sleep();
		}
		if (isMouse)
		{
			Uppies();
		}
		GD.Print(timer.TimeLeft);
	}
	public void RandomMove(float delta)
	{
		if (TargetPosition == new Vector2I(0, 0) || TargetPosition.DistanceTo(GetWindow().Position) < 50 || TargetPosition.DistanceTo(GetWindow().Position) == oldPos)
		{
			TargetPosition = new Vector2(new RandomNumberGenerator().RandiRange(ScreenBoundsmin.X, ScreenBoundsmax.X), new RandomNumberGenerator().RandiRange(ScreenBoundsmin.Y, ScreenBoundsmax.Y));
		}

		Vector2I pos = GetWindow().Position;
		Vector2 posV2 = LinearInterpolate(TargetPosition, delta, pos);
		oldPos = TargetPosition.DistanceTo(GetWindow().Position);
		GetWindow().Position = new Vector2I((int)MathF.Ceiling(posV2.X), (int)MathF.Ceiling(posV2.Y));
	}
	public void DetectScreenChange()
	{
		sleep();
		if (screenSize != new Vector2(DisplayServer.ScreenGetSize().X, DisplayServer.ScreenGetSize().Y))
		{
			ScreenBoundsmin = DisplayServer.ScreenGetPosition();
			ScreenBoundsmax = new Vector2I(DisplayServer.ScreenGetPosition().X + DisplayServer.ScreenGetSize().X - 125, DisplayServer.ScreenGetPosition().Y + DisplayServer.ScreenGetSize().Y - 125);
		}
	}
	public void Uppies()
	{
		if (isMouse && Input.IsMouseButtonPressed(MouseButton.Left))
		{
			GetWindow().Position = (Vector2I)GetViewport().GetMousePosition() + (GetWindow().Position + new Vector2I(-63, -63));
			GD.Print(GetViewport().GetMousePosition());
		}
	}
	public void sleep()
	{

	}
	private void MouseEnterExit()
	{
		isMouse = !isMouse;
	}
	private void timerTimeout()
	{
		randomChoice = new RandomNumberGenerator().RandiRange(0, maxChoice);
	}
}
