using Godot;
using System;

public partial class EnemyGFXAnimation : Node2D
{
    [Export] float _minRotationSpeed = 25.0f; // Speed of rotation
    [Export] float _maxRotationSpeed = 50.0f; // Speed of rotation
    [Export] int _minMaxRotationAngle = 5; // Minimum value for max rotation angle
    [Export] int _maxMaxRotationAngle = 8; // Maximum value for max rotation angle
    [Export] float _currentAngle = 0.0f; // Current rotation angle
    [Export] bool _rotatingLeft = true; // Direction of rotation

    float _rotationSpeed = 2.0f; // Speed of rotation
    float _maxRotationAngle;

    public override void _Ready()
    {
        _rotationSpeed = (float)GD.RandRange(_minRotationSpeed, _maxRotationSpeed);
        _maxRotationAngle = (float)GD.RandRange(_minMaxRotationAngle, _maxMaxRotationAngle); // Set random max rotation angle
    }

    public override void _Process(double delta)
    {
        // Update the rotation angle based on the direction
        if (_rotatingLeft)
        {
            _currentAngle -= _rotationSpeed * (float)delta;
            if (_currentAngle <= -_maxRotationAngle)
            {
                _rotatingLeft = false; // Change direction
            }
        }
        else
        {
            _currentAngle += _rotationSpeed * (float)delta;
            if (_currentAngle >= _maxRotationAngle)
            {
                _rotatingLeft = true; // Change direction
            }
        }

        Rotation = _currentAngle * Mathf.Pi / 180; // Convert to radians manually
    }
}
