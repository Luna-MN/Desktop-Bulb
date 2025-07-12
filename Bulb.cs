using Godot;
using System;
using System.Collections.Generic;
using DesktopBulb;

public partial class Bulb : Node2D
{
    [Export]
    public AnimatedSprite2D animatedSprite;
    private int RandomChoice, MaxChoice = 3;
    private ScreenHandler screenHandler;
    private List<Vector2> path = new List<Vector2>();
    private Action currentAction, PrevAction;
    private bool isTransitioning = false;
    private bool Forever = false;
    private bool changing = false;
    public override void _Ready()
    {
        screenHandler = new ScreenHandler();
        AddChild(screenHandler);
        
        ChangeSelection();

        // animatedSprite.AnimationFinished += () => AnimationEnd();
        animatedSprite.AnimationLooped += () => AnimationEnd();
        
        var timer = new Timer { WaitTime = 60, Autostart = true };
        AddChild(timer);
        timer.Timeout += () => timerTimeout();
    }

    public override void _Process(double delta)
    {
        Move((float)delta);
        
        if (!isTransitioning && currentAction != null && !changing)
        {
            currentAction();
        }
        MoveBulb();
    }
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent)
        {
            if (mouseEvent.Pressed)
            {
                if (mouseEvent.ButtonIndex == MouseButton.Middle)
                {
                    Forever = !Forever;
                    if (Forever)
                    {
                        currentAction = Sit;
                        path.Clear();
                    }
                    else
                    {
                        ChangeSelection();
                    }
                }

                if (mouseEvent.ButtonIndex == MouseButton.Right)
                {
                    Forever = !Forever;
                    if (Forever)
                    {
                        currentAction = sleep;
                        path.Clear();
                    }
                    else
                    {
                        ChangeSelection();
                    }
                }
            }
        }

    }
    public void MoveBulb()
    {
        if (Input.IsMouseButtonPressed(MouseButton.Left))
        {
            GetWindow().Position = (Vector2I)GetViewport().GetMousePosition() + (GetWindow().Position + new Vector2I(-63, -63));
        }
    }
    private async void ChangeSelection()
    {
        if (isTransitioning) { isTransitioning = false; }
        changing = true;
        
        RandomChoice = new RandomNumberGenerator().RandiRange(0, MaxChoice);
        // handle changing animation
        if (currentAction != null)
        {
            if (currentAction == sleep)
            {
                if (RandomChoice == 2 || RandomChoice == 3)
                {
                    animatedSprite.Play("SitFromSleep");
                }
                else if (RandomChoice == 1)
                {
                    while (true)
                    {
                        RandomChoice = new RandomNumberGenerator().RandiRange(0, MaxChoice);
                        if (RandomChoice != 1)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    animatedSprite.Play("WakeUp");
                }
                
                await ToSignal(animatedSprite, "animation_looped");
            }

            if (currentAction == Sit)
            {
                if (RandomChoice == 1)
                {
                    animatedSprite.Play("SleepFromSit");
                }
                else if (RandomChoice == 2)
                {
                    while (true)
                    {
                        RandomChoice = new RandomNumberGenerator().RandiRange(0, MaxChoice);
                        if (RandomChoice != 2)
                        {
                            break;
                        }
                    }
                }

                if (RandomChoice == 3)
                {
                    animatedSprite.Play("Sleepy1");
                }
                else
                {
                    animatedSprite.Play("Stand");
                }

                await ToSignal(animatedSprite, "animation_looped");
            }

            PrevAction = currentAction;
        }

        if (currentAction == sleepy)
        {
            if (RandomChoice == 0)
            {
                animatedSprite.Play("Stand");
            }
            else if (RandomChoice == 1)
            {
                animatedSprite.Play("SleepFromSit");
            }
            else if (RandomChoice == 2)
            {
                animatedSprite.Play("Sit");
            }
            else if (RandomChoice == 3)
            {
                while (true)
                {
                    RandomChoice = new RandomNumberGenerator().RandiRange(0, MaxChoice);
                    if (RandomChoice != 3)
                    {
                        break;
                    }
                }
            }
        }
        
        if (RandomChoice == 0)
        {
            currentAction = RandMove;
        }
        else if (RandomChoice == 1)
        {
            currentAction = sleep;
        }
        else if (RandomChoice == 2)
        {
            currentAction = Sit;
        }
        else if (RandomChoice == 3)
        {
            currentAction = sleepy;
        }
        changing = false;
    }
    private async void RandMove()
    {
        if(isTransitioning) { return; }

        isTransitioning = true;
        if (path.Count <= 0)
        {
            var rng = new RandomNumberGenerator();
            
            var idle = rng.RandiRange(0, 10);
            
            if (idle >= 5 && animatedSprite.Animation == "Walk2")
            {
                
                animatedSprite.Play("Idle");
                var count = 0;
                while (true)
                {
                    var flip = rng.RandiRange(0, 10);
                    if (flip >= 5)
                    {
                        GD.Print("flipping");
                        animatedSprite.FlipH = !animatedSprite.FlipH;
                    }
                    count++;
                    if (count >= 10)
                    {
                        break;
                    }
                    await ToSignal(GetTree().CreateTimer(0.5f), "timeout");
                }

            }
            var randomPos = new Vector2(rng.RandfRange(screenHandler.ScreenBoundsMin.X, screenHandler.ScreenBoundsMax.X), rng.RandfRange(screenHandler.ScreenBoundsMin.Y, screenHandler.ScreenBoundsMax.Y));
            path.Add(randomPos);
            animatedSprite.Play("Walk2");
        }
        isTransitioning = false;
    }
    private void sleep()
    {
        NormalAnimationLoop("Sleep", "GoToSleep", "SleepFromSit", Sit, sleepy);
    }
    private void sleepy()
    {
        if (animatedSprite.Animation == "Sleepy2" || animatedSprite.Animation == "Sleepy1")
        {
            return;
        }
        NormalAnimationLoop("Sleepy1", "Sit", "SitFromSleep", sleep, Sit);
    }
    private void Sit()
    {
        NormalAnimationLoop("Sit", "SitDown", "SitFromSleep", sleep, sleepy);
    }
    private async void NormalAnimationLoop(string FinalAnimation, string Animation1, string Animation2 = null, Action prevAction = null, Action prevAction2 = null)
    {
        if (((string)animatedSprite.Animation) == FinalAnimation) { return; }

        isTransitioning = true;
        if (PrevAction != null && Animation2 != null)
        {
            if (PrevAction == prevAction || PrevAction == prevAction2)
            {
                animatedSprite.Play(Animation2);
            }
            else
            {
                animatedSprite.Play(Animation1);
            }
        }
        else
        {
            animatedSprite.Play(Animation1);
        }
        await ToSignal(animatedSprite, "animation_looped");
        animatedSprite.Play(FinalAnimation);
        isTransitioning = false;
    }
    private void Move(float delta)
    {
        if(path.Count <= 0) { return; }
        
        var target = path[0];
        
        var pos = ((Vector2)GetWindow().Position).Lerp(target, delta * 0.05f);
        
        Vector2I newPosition = new Vector2I(
            pos.X > GetWindow().Position.X ? (int)MathF.Ceiling(pos.X) : (int)MathF.Floor(pos.X),
            pos.Y > GetWindow().Position.Y ? (int)MathF.Ceiling(pos.Y) : (int)MathF.Floor(pos.Y)
        );

        GetWindow().Position = newPosition;

        if (target.X < GetWindow().Position.X)
        {
            animatedSprite.FlipH = false;
        }
        else
        {
            animatedSprite.FlipH = true;
        }

        if (pos.DistanceTo(target) < 3f)
        {
            path.RemoveAt(0);
        }
    }
    private void AnimationEnd()
    {
        if (animatedSprite.Animation == "Sleepy1")
        {
            animatedSprite.Play("Sleepy2");
        }
        else if (animatedSprite.Animation == "Sleepy2")
        {
            animatedSprite.Play("Sleepy1");
        }
    }
    private void timerTimeout()
    {
        if (Forever)
        {
            return;
        }
        if (currentAction == sleepy)
        {
            currentAction = sleep;
        }
        else
        {
            ChangeSelection();
        }
    }
}
