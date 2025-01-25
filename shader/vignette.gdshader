shader_type canvas_item;

uniform vec2 vignetteCenter = vec2(0.5, 0.5);  // Center of the vignette effect (normalized coordinates)
uniform float vignetteRadius : hint_range(0.0, 1.0) = 0.75;  // Radius where the vignette starts
uniform float vignetteSoftness : hint_range(0.0, 1.0) = 0.25;  // Softness of the vignette edge
uniform vec4 vignetteColor = vec4(0.0, 0.0, 0.0, 1.0);  // Color of the vignette (default is black)

uniform sampler2D screen_texture : hint_screen_texture, filter_linear_mipmap;  // Capture the screen texture

float smootherstep(float edge0, float edge1, float x) {
    x = clamp((x - edge0) / (edge1 - edge0), 0.0, 1.0);
    return x * x * x * (x * (x * 6.0 - 15.0) + 10.0);
}

void fragment() {
    // Get the distance from the center of the vignette
    float dist = 1.0 - distance(UV, vignetteCenter);

    // Calculate the vignette effect using a smootherstep transition
    float vignette = smootherstep(vignetteRadius, vignetteRadius - vignetteSoftness, dist);

    // Sample the screen texture, which contains what's been drawn (like circles)
    vec4 screen_color = texture(screen_texture, SCREEN_UV);

    // Apply the vignette effect by blending the screen texture with the vignette color
    COLOR = mix(screen_color, vignetteColor, vignette);
}
