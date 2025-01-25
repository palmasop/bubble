shader_type canvas_item;

uniform float fade_amount : hint_range(0.0, 1.0) = 1.0; // Controls the fade amount (1 = fully visible, 0 = invisible)
uniform float max_size : hint_range(0.0, 1.0) = 1.0; // Controls the fade amount (1 = fully visible, 0 = invisible)

uniform float vignetteRadius : hint_range(0.0, 2.0) = 0.75;  // Radius where the vignette starts
uniform float vignetteSoftness : hint_range(0.0, 1.0) = 0.25;  // Softness of the vignette edge
uniform vec4 vignetteColor = vec4(0.0, 0.0, 0.0, 1.0);  // Color of the vignette (default is black)

float smootherstep(float edge0, float edge1, float x) {
    x = clamp((x - edge0) / (edge1 - edge0), 0.0, 1.0);
    return x * x * x * (x * (x * 6.0 - 15.0) + 10.0);
}

void fragment() {
	float dist = distance(UV, vec2(0.5, 0.5));
	float vignette = smootherstep(vignetteRadius, vignetteRadius - vignetteSoftness, 1.0 - dist);
	COLOR = mix(COLOR, vignetteColor, vignette);

	COLOR.a *= (1.0 - fade_amount) * pow((1.6 - dist), 3);
	if(dist > max_size)
		COLOR.a = 0.0;
}
