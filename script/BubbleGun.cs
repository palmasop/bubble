using Godot;
using System;

public partial class BubbleGun : Node2D
{
	[Export] public PackedScene BulletScene { get; set; }

	[Export] Node2D shootPoint;

	[Export] public BubblePreview BubblePreview { get; set; }

	[Export] public BubbleSettings Settings { get; set; }

	[ExportCategory("UI Display")]
	[Export] Node2D uiDisplay;
	[Export] Label bulletDisplay;

	[Export] public float ShootInterval = 0.5f; // Interval between each shoot in seconds
	private float _timeSinceLastShoot = 0.0f;

	float _currentCharge = 0.0f;
	bool _isCharging = false;
	BubbleSettings defaultSettings;

	int bulletLeft;

	public override void _Ready()
	{
		if (Settings == null)
		{
			GD.PrintErr("BubbleSettings resource is not assigned!");
			return;
		}

		if (BubblePreview != null)
			BubblePreview.Visible = false;
		defaultSettings = Settings;

		ChangeGun(Settings);
	}

	public override void _Process(double delta)
	{
		if (Settings == null) return;

		Rotation = Mathf.Pi / 2; // Always point to the north

		_timeSinceLastShoot += (float)delta;

		if (Input.IsActionPressed("shoot") && _timeSinceLastShoot >= ShootInterval)
		{
			if (!_isCharging)
			{
				_isCharging = Settings.isChargable;
				_currentCharge = Settings.MinBulletScale;
				if (Settings.isChargable)
				{
					BubblePreview?.Show();
				}
				else
				{
					Shoot();
					_timeSinceLastShoot = 0.0f; // Reset the shoot timer
				}
			}

			if (Settings.isChargable)
			{
				_currentCharge = Mathf.Min(_currentCharge + Settings.ChargeRate * (float)delta, Settings.MaxBulletScale);
				BubblePreview?.UpdatePreview(_currentCharge);
			}
		}
		else if (Input.IsActionJustReleased("shoot") && _isCharging)
		{
			if (Settings.isChargable)
			{
				BubblePreview?.Hide();
				Shoot();
				_timeSinceLastShoot = 0.0f; // Reset the shoot timer
			}
			_isCharging = false;
		}
	}

	void Shoot()
	{
		if (BulletScene == null || Settings == null) return;

		bulletLeft--;
		UpdateBulletDisplay();
		if (bulletLeft < 1)
			ChangeGun(defaultSettings);

		var bullet = BulletScene.Instantiate<Bubble>();
		bullet.OnHit += BubbleOnHit;
		GetTree().Root.AddChild(bullet);
		bullet.GlobalPosition = shootPoint.GlobalPosition;

		if (bullet is Bubble bubbleScript)
			bubbleScript.Init(Settings.BulletLifetime, Settings.ShootSpeed, _currentCharge, Settings.damage, true, Settings.displayGFX);
	}

	void BubbleOnHit(Enemy enemy)
	{
		if (bulletLeft > 0)
			return;
		var setting = BulletManager.Instance.GetBubbleSettingsByEnemey(enemy.type);
		enemy.QueueFree();
		ChangeGun(setting);
	}

	void ChangeGun(BubbleSettings bubbleSettings)
	{
		Settings = bubbleSettings;
		bulletLeft = Settings.bullet;
		UpdateBulletDisplay();

		foreach(var child in uiDisplay.GetChildren())
			child.QueueFree();
		var display = bubbleSettings.displayGFX?.Instantiate<Node2D>();
		if(display == null)
			return;
		uiDisplay.AddChild(display);
		display.GlobalPosition = uiDisplay.GlobalPosition;
		BubblePreview.ChangePreview(bubbleSettings.displayGFX);
	}

	void UpdateBulletDisplay()
	{
		if (bulletLeft == -1)
		{
			bulletDisplay.Text = "";
			return;
		}

		bulletDisplay.Text = bulletLeft.ToString();
	}
}
