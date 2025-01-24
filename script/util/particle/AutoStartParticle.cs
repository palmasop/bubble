using Godot;

public partial class AutoStartParticle : CpuParticles2D
{
    public override void _Ready() {
        Emitting = true;
    }
}