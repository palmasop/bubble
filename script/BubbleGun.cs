using Godot;
using System;

public partial class BubbleGun : Node2D
{
	[Export]
	public PackedScene BulletScene { get; set; }

	[Export] Node2D shootPoint;

	[Export]
	public BubblePreview BubblePreview { get; set; }

	[Export]
	public BubbleSettings Settings { get; set; }

	private float _currentCharge = 0.0f;
	private bool _isCharging = false;

	public override void _Ready()
	{
		if (Settings == null)
		{
			GD.PrintErr("BubbleSettings resource is not assigned!");
			return;
		}

		if (BubblePreview != null)
		{
			BubblePreview.Visible = false;  // Hide preview initially
		}
	}

	public override void _Process(double delta)
	{
		if (Settings == null) return;

		LookAt(GetGlobalMousePosition());

		if (Input.IsActionPressed("shoot"))
		{
			if (!_isCharging)
			{
				_isCharging = true;
				_currentCharge = Settings.MinBulletScale;
				if (BubblePreview != null)
				{
					BubblePreview.Show();
				}
			}

			_currentCharge = Mathf.Min(_currentCharge + Settings.ChargeRate * (float)delta, Settings.MaxBulletScale);

			if (BubblePreview != null)
			{
				BubblePreview.UpdatePreview(_currentCharge);
			}
		}
		else if (Input.IsActionJustReleased("shoot") && _isCharging)
		{
			if (BubblePreview != null)
			{
				BubblePreview.Hide();
			}
			Shoot();
			_isCharging = false;
		}
	}

	private void Shoot()
	{
		if (BulletScene == null || Settings == null) return;

		Node2D bullet = BulletScene.Instantiate<Node2D>();
		GetTree().Root.AddChild(bullet);
		bullet.GlobalPosition = shootPoint.GlobalPosition;

		if (bullet is Bubble bubbleScript)
		{
			bubbleScript.Init(Settings.BulletLifetime, Settings.ShootSpeed, _currentCharge);
		}
	}
}
