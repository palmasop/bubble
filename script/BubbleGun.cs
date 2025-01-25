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

	bool disableTilNextShoot;
	bool isLastChargeShot;
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

		if (Input.IsActionJustPressed("shoot"))
			disableTilNextShoot = false;

		if (disableTilNextShoot)
			return;

		if (Settings.isChargable)
			ChargeShot(delta);
		else
			ChargeShot(delta);
	}

	void ChargeShot(double delta)
	{
		if (Input.IsActionPressed("shoot") && _timeSinceLastShoot >= ShootInterval)
		{
			if (!_isCharging)
			{
				_isCharging = true;
				_currentCharge = Settings.MinBulletScale;
				BubblePreview?.Show();
			}

			_currentCharge = Mathf.Min(_currentCharge + Settings.ChargeRate * (float)delta, Settings.MaxBulletScale);
			BubblePreview?.UpdatePreview(_currentCharge);
		}
		else if (Input.IsActionJustReleased("shoot") && _isCharging)
		{
			BubblePreview?.Hide();
			Shoot();
			_timeSinceLastShoot = 0.0f;
			_isCharging = false;
			disableTilNextShoot = true;
		}
	}

	void NormalShot(double delta)
	{
		if (Input.IsActionPressed("shoot") && _timeSinceLastShoot >= ShootInterval)
		{
			_currentCharge = Settings.MinBulletScale;
			Shoot();
			_timeSinceLastShoot = 0.0f;
		}
	}

	void Shoot()
	{
		if (BulletScene == null || Settings == null) return;

		var bullet = BulletScene.Instantiate<Bubble>();
		bullet.OnCapture += (node) => BubbleOnCapture(node, bullet);
		bullet.EnableCapture(Settings.isCapturable);

		GetTree().Root.AddChild(bullet);
		bullet.GlobalPosition = shootPoint.GlobalPosition;

		if (bullet is Bubble bubbleScript)
			bubbleScript.Init(Settings.BulletLifetime, Settings.ShootSpeed, _currentCharge, Settings.damage, Settings.displayGFX);

		bulletLeft--;
		UpdateBulletDisplay();
		if (bulletLeft == 0)
			ChangeGun(defaultSettings);
	}


	void BubbleOnCapture(Node2D node, Bubble bullet)
	{
		if (bulletLeft > 0)
			return;

		if (node is Enemy enemy)
		{
			var changeToSetting = BulletManager.Instance.GetBubbleSettingsByEnemey(enemy.type);
			ChangeGun(changeToSetting);
		}
		node.QueueFree();
		bullet.Explode();
	}

	void ChangeGun(BubbleSettings bubbleSettings)
	{
		Settings = bubbleSettings;
		bulletLeft = Settings.bullet;
		UpdateBulletDisplay();

		foreach (var child in uiDisplay.GetChildren())
			child.QueueFree();
		var display = bubbleSettings.displayGFX?.Instantiate<Node2D>();
		if (display == null)
			return;
		uiDisplay.AddChild(display);
		display.GlobalPosition = uiDisplay.GlobalPosition;
		BubblePreview.ChangePreview(bubbleSettings.displayGFX);
	}

	void UpdateBulletDisplay()
	{
		if (bulletLeft <= -1)
		{
			bulletDisplay.Text = "Ꝏ";
			return;
		}

		bulletDisplay.Text = bulletLeft.ToString();
	}
}
