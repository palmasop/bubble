using System;
using System.Threading.Tasks;
using Godot;

public static class CameraShake
{
    static readonly Random _random = new();
    static Vector2 _originalPosition;

    public static async void StartShake(Node self, float intensity = 10, float duration = 0.15f) {
        var camera = self.GetViewport().GetCamera2D();
        if (camera == null) {
            GD.PrintErr("No Camera2D found in the viewport.");
            return;
        }

        _originalPosition = camera.GlobalPosition;

        var timer = duration;

        while (timer > 0) {
            var curIntensity = Mathf.Lerp(intensity, intensity / 2, (duration - timer) / duration);
            var shakeOffset = new Vector2((float)(_random.NextDouble() * 2 - 1) * curIntensity, (float)(_random.NextDouble() * 2 - 1) * curIntensity);

            camera.GlobalPosition = _originalPosition + shakeOffset;
            await Task.Delay(16);
            timer -= 0.016f;
        }

        camera.GlobalPosition = _originalPosition;
    }
}