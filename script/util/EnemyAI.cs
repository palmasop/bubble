using Godot;
using System;
using System.Collections.Generic;

[Tool]
[GlobalClass]
public partial class EnemyAI : Node2D
{
	[Export] public bool disabledMovement = false;
	[Export] public bool useCircularMovement = false;

	[ExportCategory("Movement")]
	[Export] public float speed = 100.0f;
	[Export] public float pathfindingInterval = 0.1f;
	[Export] public float minDistanceToPlayer = 20.0f;
	[Export] public float awayDistance = 20.0f;
	NavigationAgent2D navigationAgent;
	Player targetPlayer;
	bool isNavigationReady;
	CharacterBody2D body;
	Line2D pathLine;

	Timer pathfindingTimer;


	public Vector2 Velocity;

	float pathChangeTimer = 0f;
	const float PATH_CHANGE_INTERVAL = 1.0f;

	bool isChangingPath = false;

	public override void _Ready()
	{
		if (Engine.IsEditorHint()) return;

		navigationAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");

		navigationAgent.NavigationFinished += OnNavigationFinished;
		navigationAgent.VelocityComputed += OnVelocityComputed;
		navigationAgent.PathChanged += OnPathChanged;

		CallDeferred(nameof(SetupNavigation));

		pathfindingTimer = new Timer();
		pathfindingTimer.WaitTime = pathfindingInterval;
		pathfindingTimer.Timeout += () => UpdatePathfindingTarget();
		AddChild(pathfindingTimer);
		body = GetParent<CharacterBody2D>();
	}

	void SetupNavigation()
	{
		isNavigationReady = true;
		pathfindingTimer.Start();
		UpdatePathfindingTarget();
	}

	void OnPathChanged()
	{
		if (isChangingPath) return;

		try
		{
			isChangingPath = true;

			if (pathChangeTimer > 0f)
				return;

			if (navigationAgent.IsTargetReachable())
				return;

			var currentPos = GlobalPosition;
			var targetPos = navigationAgent.TargetPosition;
			var direction = (targetPos - currentPos).Normalized();

			for (float angle = 45; angle <= 360; angle += 45)
			{
				var rotated = direction.Rotated(Mathf.DegToRad(angle));
				var testPoint = currentPos + rotated * 100;

				navigationAgent.TargetPosition = testPoint;
				if (navigationAgent.IsTargetReachable())
				{
					return;
				}
			}
			pathChangeTimer = PATH_CHANGE_INTERVAL;
		}
		finally
		{
			isChangingPath = false;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Engine.IsEditorHint() || !isNavigationReady) return;

		if (useCircularMovement)
			MoveToTarget_Circular();
		else
			MoveToTarget();
	}

	void MoveToTarget()
	{
		if (disabledMovement) return;
		if (!UpdatePathfindingTarget()) return;
		if (!navigationAgent.IsTargetReachable()) return;

		Vector2 directionToPlayer = (targetPlayer.GlobalPosition - GlobalPosition).Normalized();
		float distanceToPlayer = GlobalPosition.DistanceTo(targetPlayer.GlobalPosition);

		if (distanceToPlayer < awayDistance)
			MoveToTarget_Circular();
		else if (distanceToPlayer > minDistanceToPlayer)
			navigationAgent.TargetPosition = targetPlayer.GlobalPosition;
		else
			MoveToTarget_Circular();

		Vector2 nextPathPosition = navigationAgent.GetNextPathPosition();
		Vector2 direction = (nextPathPosition - GlobalPosition).Normalized();
		Vector2 targetVelocity = direction * speed;

		navigationAgent.SetVelocity(targetVelocity);
		body.MoveAndSlide();
	}


	float circularOffset = 0f;
	float circularTimer = 0f;
	const float CIRCULAR_CHANGE_TIME = 1f;  // Time before changing circular direction

	void MoveToTarget_Circular()
	{
		if (disabledMovement) return;
		if (!UpdatePathfindingTarget()) return;
		if (!navigationAgent.IsTargetReachable()) return;

		Vector2 directionToPlayer = (targetPlayer.GlobalPosition - GlobalPosition).Normalized();
		float distanceToPlayer = GlobalPosition.DistanceTo(targetPlayer.GlobalPosition);

		circularTimer += (float)GetPhysicsProcessDeltaTime();
		if (circularTimer >= CIRCULAR_CHANGE_TIME)
		{
			circularTimer = 0f;
			circularOffset = (float)GD.RandRange(-Mathf.Pi / 2, Mathf.Pi / 2);
		}

		Vector2 circularDirection = directionToPlayer.Rotated(Mathf.Pi / 2 + circularOffset);
		navigationAgent.TargetPosition = targetPlayer.GlobalPosition + (circularDirection * minDistanceToPlayer);

		Vector2 nextPathPosition = navigationAgent.GetNextPathPosition();
		Vector2 direction = (nextPathPosition - GlobalPosition).Normalized();
		Vector2 targetVelocity = direction * speed;

		navigationAgent.SetVelocity(targetVelocity);
		body.MoveAndSlide();
	}

	bool UpdatePathfindingTarget()
	{
		if (Engine.IsEditorHint() || !isNavigationReady) return false;

		targetPlayer = Player.GetPlayer();
		return targetPlayer != null;
	}

	void OnNavigationFinished()
	{
		navigationAgent.SetVelocity(Vector2.Zero);
		body.Velocity = Vector2.Zero;
	}

	void OnVelocityComputed(Vector2 safeVelocity)
	{
		body.Velocity = safeVelocity;
		body.MoveAndSlide();
	}

	public override string[] _GetConfigurationWarnings()
	{
		var warnings = new List<string>();
		if (IsInsideTree() && GetParent() is not CharacterBody2D)
			warnings.Add("EnemyAI node should be a child of CharacterBody2D");
		if (GetNodeOrNull<NavigationAgent2D>("NavigationAgent2D") == null)
			warnings.Add("NavigationAgent2D node is missing");
		return warnings.ToArray();
	}

	public override void _Process(double delta)
	{
		if (pathChangeTimer > 0f)
			pathChangeTimer -= (float)delta;
	}
}