using Godot;
using System;

public partial class BubbleGun : Node2D
{
	[Export]
	public PackedScene BulletScene { get; set; }
	
	[Export]
	public BubblePreview PreviewSprite { get; set; }  // Reference to preview scene
	
	[ExportGroup("Bullet Properties")]
	[Export]
	public float ShootSpeed = 400.0f;
	
	[Export]
	public float BulletLifetime = 3.0f;
	
	[Export]
	public float MinBulletScale = 0.5f;
	
	[Export]
	public float MaxBulletScale = 3.0f;
	
	[Export]
	public float ChargeRate = 2.0f;  // How fast the bubble grows

	private float _currentCharge = 0.0f;
	private bool _isCharging = false;

	public override void _Ready()
	{
		if (PreviewSprite != null)
		{
			PreviewSprite.Visible = false;  // Hide preview initially
		}
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("shoot"))
		{
			if (!_isCharging)
			{
				_isCharging = true;
				_currentCharge = MinBulletScale;
				if (PreviewSprite != null)
				{
					PreviewSprite.Show();
				}
			}
			
			_currentCharge = Mathf.Min(_currentCharge + ChargeRate * (float)delta, MaxBulletScale);
			
			if (PreviewSprite != null)
			{
				PreviewSprite.UpdatePreview(_currentCharge);
			}
		}
		else if (Input.IsActionJustReleased("shoot") && _isCharging)
		{
			if (PreviewSprite != null)
			{
				PreviewSprite.Hide();
			}
			Shoot();
			_isCharging = false;
		}
	}

	private void Shoot()
	{
		if (BulletScene == null) return;

		GD.Print("Shooting with scale: ", _currentCharge);  // Debug print
		Node2D bullet = BulletScene.Instantiate<Node2D>();
		GetTree().Root.AddChild(bullet);
		bullet.GlobalPosition = GlobalPosition;
		
		if (bullet is Bubble bubbleScript)
		{
			bubbleScript.Init(BulletLifetime, ShootSpeed, _currentCharge);
		}
	}
}
