using Godot;
using System;
using System.Collections.Generic;
public partial class Main : Node2D
{
	// Called when the node enters the scene tree for the first time.
	[Export]
	public PackedScene windowScene;
	[Export]
	public AnimatedSprite2D animatedSprite;
	public Vector2 TargetPosition = new Vector2(0, 0), middle, screenSize;
	public Vector2I ScreenBoundsmin, ScreenBoundsmax;
	public float oldPos, Speed = 0.05f;
	public bool isMouse, timerStart, warp = false, first = true, isSleeping = false, oldSleeping = false, inhibitMove = false;
	public Timer timer;
	public RandomNumberGenerator RandomMoveGen = new RandomNumberGenerator(), RandomChoice = new RandomNumberGenerator();
	public int randomChoice, maxChoice = 3, b = 0;
	public Callable callable;
	public override void _Ready()
	{
		GetViewport().GuiEmbedSubwindows = false;
		ScreenBoundsmin = DisplayServer.ScreenGetPosition();
		ScreenBoundsmax = new Vector2I(DisplayServer.ScreenGetPosition().X + DisplayServer.ScreenGetSize().X - 125, DisplayServer.ScreenGetPosition().Y + DisplayServer.ScreenGetSize().Y - 125);
		middle = GetWindow().Position;
		GD.Print(DisplayServer.ScreenGetPosition());
		randomChoice = RandomChoice.RandiRange(0, maxChoice);
		GD.Print(randomChoice);
		timer = new Timer { WaitTime = 60, Autostart = true };
		AddChild(timer);
		callable = new Callable(this, "timerTimeout");
		timer.Connect("timeout", callable);
		//GetWindow().MousePassthroughPolygon picking up bulb and sleep area? 
		GD.Print(ScreenBoundsmax, "+", ScreenBoundsmin);
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
		if (!isSleeping)
		{
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
				Grabies((float)delta);
			}
			else if (randomChoice == 3)
			{
				sit();
			}
		}
		if (isMouse && (warp || first))
		{
			Uppies();
		}
		goSleep((float)delta);
	}
	public void RandomMove(float delta)
	{
		if (TargetPosition == new Vector2I(0, 0) || TargetPosition.DistanceTo(GetWindow().Position) < 50 || TargetPosition.DistanceTo(GetWindow().Position) == oldPos)
		{
			TargetPosition = new Vector2(RandomMoveGen.RandiRange(ScreenBoundsmin.X, ScreenBoundsmax.X), RandomMoveGen.RandiRange(ScreenBoundsmin.Y, ScreenBoundsmax.Y));
			GD.Print(TargetPosition);
			warp = true; // picking a position when grab mouse and then instantly picking somthing else to do
			b++;
			if (TargetPosition.X < GetWindow().Position.X)
			{
				animatedSprite.FlipH = true;
			}
			else
			{
				animatedSprite.FlipH = false;
			}
			animatedSprite.Play("Walk");
			GD.Print(b);
		}

		Vector2I pos = GetWindow().Position;
		Vector2 posV2 = LinearInterpolate(TargetPosition, delta * Speed, pos);
		oldPos = TargetPosition.DistanceTo(GetWindow().Position);
		Vector2I newPosition = new Vector2I(
			posV2.X > GetWindow().Position.X ? (int)MathF.Ceiling(posV2.X) : (int)MathF.Floor(posV2.X),
			posV2.Y > GetWindow().Position.Y ? (int)MathF.Ceiling(posV2.Y) : (int)MathF.Floor(posV2.Y)
		);
		GetWindow().Position = newPosition;
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
	public void goSleep(float delta)
	{
		if (isSleeping && !oldSleeping)
		{
			animatedSprite.Play("Walk");
		}
		if (isSleeping)
		{
			if (GetWindow().Position != new Vector2I(ScreenBoundsmax.X - 100, ScreenBoundsmax.Y - 100) && !inhibitMove)
			{
				TargetPosition = LinearInterpolate(new Vector2I(ScreenBoundsmax.X - 100, ScreenBoundsmax.Y - 100), delta * Speed, GetWindow().Position);
				if (TargetPosition.X < GetWindow().Position.X)
				{
					animatedSprite.FlipH = true;
				}
				else
				{
					animatedSprite.FlipH = false;
				}
				Vector2I newPosition = new Vector2I(
					TargetPosition.X > GetWindow().Position.X ? (int)MathF.Ceiling(TargetPosition.X) : (int)MathF.Floor(TargetPosition.X),
					TargetPosition.Y > GetWindow().Position.Y ? (int)MathF.Ceiling(TargetPosition.Y) : (int)MathF.Floor(TargetPosition.Y)
				);
				GetWindow().Position = newPosition;
			}
			else
			{
				sleep();
				inhibitMove = true;
			}

		}
		else if (isSleeping != oldSleeping)
		{
			inhibitMove = false;
			randomChoice = 0;
		}
		oldSleeping = isSleeping;
	}
	public void sleep()
	{
		animatedSprite.Play("Sleep");
	}
	public void sit()
	{
		animatedSprite.Play("Sit");
	}
	private void MouseEnterExit()
	{
		isMouse = !isMouse;
	}
	private void timerTimeout()
	{
		first = false;
		randomChoice = RandomChoice.RandiRange(0, maxChoice);
		GD.PushWarning(randomChoice);
	}
	private void Grabies(float delta)
	{
		if (((Vector2)DisplayServer.MouseGetPosition()).DistanceTo(GetWindow().Position) >= 50 && warp == false)
		{
			TargetPosition = LinearInterpolate(DisplayServer.MouseGetPosition() + new Vector2I(-30, -30), delta * Speed, GetWindow().Position);
			if (TargetPosition.X < GetWindow().Position.X)
			{
				animatedSprite.FlipH = true;
			}
			else
			{
				animatedSprite.FlipH = false;
			}
			animatedSprite.Play("Walk");
			Vector2I newPosition = new Vector2I(
				TargetPosition.X > GetWindow().Position.X ? (int)MathF.Ceiling(TargetPosition.X) : (int)MathF.Floor(TargetPosition.X),
				TargetPosition.Y > GetWindow().Position.Y ? (int)MathF.Ceiling(TargetPosition.Y) : (int)MathF.Floor(TargetPosition.Y)
			);
			GetWindow().Position = newPosition;
			b = 0;
		}
		else
		{
			RandomMove(delta);
			Input.WarpMouse(new Vector2I(30, 30));
		}
		if (warp == true && b >= 2)
		{
			randomChoice = RandomChoice.RandiRange(0, maxChoice);
		}

	}
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
		{
			if (isMouse && Input.IsMouseButtonPressed(MouseButton.Right))
			{
				isSleeping = !isSleeping;
			}
		}
	}
}

