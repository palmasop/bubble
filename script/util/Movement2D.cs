using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Bridge;

[Tool]
[GlobalClass]
public partial class Movement2D : Node2D
{ 
    [Export] bool disabled = false;
    [Export] bool disabledRun = false;
    [Export] bool disabledKnockback = false;
    
    [ExportCategory("Movement Attributes")]
    [Export] float walkSpeed = 500;
    [Export] float runSpeed = 1000;
    [Export] float knockbackDecay = 2000;

    [ExportCategory("Action Input")]
    [Export] string runInputName = "run";
    [Export] string topInputName = "top";
    [Export] string bottomInputName = "down";
    [Export] string leftInputName = "left";
    [Export] string rightInputName = "right";

    CharacterBody2D body;
    Vector2 knockbackVelocity = Vector2.Zero;
    bool isKnockedBack;

    public override void _Ready() => body = GetParent<CharacterBody2D>();

    public override void _Process(double delta)
    {
        if (Engine.IsEditorHint() || disabled) return;

        var dir = Input.GetVector(leftInputName, rightInputName, topInputName, bottomInputName);

        var isRunPress = Input.IsActionPressed(runInputName);
        var speed = (isRunPress && !disabledRun) ? runSpeed : walkSpeed;

        var knock = KnockbackUpdate(delta);
        body.Velocity = speed * dir.Normalized() + knock;
        body.MoveAndSlide();
    }
    
    public Vector2 KnockbackUpdate(double delta)
    {
        if (!isKnockedBack || disabledKnockback) return Vector2.Zero;
        knockbackVelocity = knockbackVelocity.MoveToward(Vector2.Zero, knockbackDecay * (float)delta);

        if (knockbackVelocity.Length() >= 10) return knockbackVelocity;
        isKnockedBack = false;
        knockbackVelocity = Vector2.Zero;
        return knockbackVelocity;
    }

        public void Knockback(Vector2 force)
    {
        knockbackVelocity = force;
        isKnockedBack = true;
    }

    public override string[] _GetConfigurationWarnings()
    {
        var warnings = new List<string>();
        if (IsInsideTree() && GetParent() is not CharacterBody2D)
            warnings.Add("Movement node should be a child of CharacterBody2D");
        return warnings.ToArray();
    }
}