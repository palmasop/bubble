using Godot;

public interface ICapturable
{
    [Export] bool Capturable { get; }
    [Export] float minCaptureSize { get; }
    void OnCapture();
    void OnReleaseCapture();
}