using Godot;
using System;

public abstract partial class UI_Menu : Control
{
	[Export] string closeAction = "ui_cancel";
	[Export] string shortcutAction = "";
	[Export] Button closeButton;
	[Export] bool backgroundClosesMenu = true;
	[Export] bool pauseGameWhenOpen = true;

	bool isOpen = false;
	Control target;

	protected bool GetTargetVisible() => target == null ? Visible : target.Visible;
	protected void SetTargetVisible(bool value)
	{
		if (target == null)
			Visible = value;
		else
			target.Visible = value;
	}

	public override void _Ready()
	{
		ToggleMenu(false);
		// GuiInput += OnBackgroundClicked;
		if (closeButton != null)
			closeButton.Pressed += () => ToggleMenu(false);
		// SceneManager.Instance.OnChangeScene += (_, _) => ToggleMenu(false);
	}

	public virtual void ToggleMenu(bool? visible = null, Control target = null)
	{
		bool newState = visible ?? !GetTargetVisible();
		this.target = target;
		SetTargetVisible(newState);
		isOpen = newState;
		SetProcess(newState);
		ProcessMode = newState ? ProcessModeEnum.Always : ProcessModeEnum.Inherit;

		if (pauseGameWhenOpen)
			GetTree().Paused = newState;
	}

	protected virtual void OnBackgroundClicked(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseButton)
		{
			if (mouseButton.ButtonIndex == MouseButton.Left && mouseButton.Pressed && backgroundClosesMenu)
			{
				// ToggleMenu(false);
				// GetViewport().SetInputAsHandled();
			}
		}
	}

	// public override void _UnhandledInput(InputEvent @event)
	public override void _Input(InputEvent @event)
	{
		if (!isOpen)
			GD.Print("Not visible");

		if (isOpen && !string.IsNullOrEmpty(closeAction) && @event.IsActionPressed(closeAction))
		{
			ToggleMenu(false);
			GetViewport().SetInputAsHandled();
			return;
		}

		if (!string.IsNullOrEmpty(shortcutAction) && @event.IsActionPressed(shortcutAction))
		{
			if (isOpen)
				ToggleMenu(false);
			else if (!GetViewport().GuiGetFocusOwner()?.HasFocus() ?? true)
				ToggleMenu(true);

			GetViewport().SetInputAsHandled();
		}
	}
}