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
	public float oldPos, Speed = 0.1f;
	public bool isMouse, timerStart, warp = false;
	public Timer timer;
	public int randomChoice, maxChoice = 2;
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
		DetectScreenChange();
		if (randomChoice == 0)
		{
			RandomMove((float)delta);
		}
		else if (randomChoice == 1)
		{
			sleep();
		}
		else if (randomChoice == 2)
		{
			warp = false;
			Grabies((float)delta);
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
			warp = true;
		}

		Vector2I pos = GetWindow().Position;
		Vector2 posV2 = LinearInterpolate(TargetPosition, delta * Speed, pos);
		oldPos = TargetPosition.DistanceTo(GetWindow().Position);
		GetWindow().Position = new Vector2I((int)MathF.Ceiling(posV2.X), (int)MathF.Ceiling(posV2.Y));
	}
	public bool DetectScreenChange()
	{
		if (screenSize != new Vector2(DisplayServer.ScreenGetSize().X, DisplayServer.ScreenGetSize().Y))
		{
			ScreenBoundsmin = DisplayServer.ScreenGetPosition();
			ScreenBoundsmax = new Vector2I(DisplayServer.ScreenGetPosition().X + DisplayServer.ScreenGetSize().X - 125, DisplayServer.ScreenGetPosition().Y + DisplayServer.ScreenGetSize().Y - 125);
			return true;
		}
		return false;
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
		GD.PushWarning(randomChoice);
	}
	private void Grabies(float delta)
	{
		if (((Vector2)DisplayServer.MouseGetPosition()).DistanceTo(GetWindow().Position) >= 30)
		{
			TargetPosition = LinearInterpolate(DisplayServer.MouseGetPosition() + new Vector2I(-30, -30), delta * Speed, GetWindow().Position);
			if (DisplayServer.MouseGetPosition() < TargetPosition)
			{
				TargetPosition = TargetPosition - new Vector2(1, 1);
			}
			GD.Print(TargetPosition, GetWindow().Position);
			GetWindow().Position = new Vector2I((int)MathF.Ceiling(TargetPosition.X), (int)MathF.Ceiling(TargetPosition.Y));
		}
		else
		{
			RandomMove(delta);
			Input.WarpMouse(new Vector2I(0, 0));
		}
		if (warp == true)
		{
			randomChoice = new RandomNumberGenerator().RandiRange(0, maxChoice);
		}
		// input.warpmouse();
	}
}
