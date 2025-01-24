using Godot;

public partial class AutoStartParticleGroup : Node2D
{
    public override void _Ready() {
        foreach (Node child in GetChildren()) {
            if (child is CpuParticles2D particles)
                particles.Emitting = true;
        }
    }
}