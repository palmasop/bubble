shader_type canvas_item;

uniform float distortionStrength : hint_range(0.0, 1.0) = 0.1;  // Controls the strength of the distortion
uniform float distortionSpeed : hint_range(0.0, 10.0) = 2.0;    // Speed of the distortion effect
uniform vec2 distortionFrequency = vec2(10.0, 10.0);            // Frequency of the distortion (horizontal and vertical waves)
uniform float distortionSoftness : hint_range(0.0, 10.0) = 2;  // Softness of the distortion

uniform float aberrationStrength : hint_range(0.0, 10.0) = 1.0;

uniform vec2 vignetteCenter = vec2(0.5, 0.5);  // Center of the vignette effect (normalized coordinates)
uniform float vignetteRadius : hint_range(0.0, 1.0) = 0.75;  // Radius where the vignette starts
uniform float vignetteSoftness : hint_range(0.0, 1.0) = 0.25;  // Softness of the vignette edge
uniform vec4 vignetteColor = vec4(0.0, 0.0, 0.0, 1.0);  // Color of the vignette (default is black)

uniform sampler2D screen_texture : hint_screen_texture, filter_linear_mipmap;

void fragment() {
    vec2 uv = SCREEN_UV;

	// Chromatic Aberration
    vec2 red_offset = uv + vec2(0.005 * aberrationStrength, 0.0);
    vec2 green_offset = uv;
    vec2 blue_offset = uv - vec2(0.005 * aberrationStrength, 0.0);

    float r = texture(screen_texture, red_offset).r;
    float g = texture(screen_texture, green_offset).g;
    float b = texture(screen_texture, blue_offset).b;
	vec4 color = vec4(r, g, b, 1.0);

	// Distortion
	vec2 selfUv = UV;
	float areaStrength = pow(distance(selfUv, vec2(0.5f, 0.6f)), distortionSoftness);
	//float areaStrength = 1.0;

    uv.x += distortionStrength * areaStrength * sin(TIME * distortionSpeed + uv.y * distortionFrequency.y);
    uv.y += distortionStrength * areaStrength * cos(TIME * distortionSpeed + uv.x * distortionFrequency.x);

	vec4 distortionCOLOR = texture(screen_texture, uv);
	color = mix(color, distortionCOLOR, .5);

	// Vignette
	float dist = 1.0 - distance(uv, vignetteCenter);
    float vignette = smoothstep(vignetteRadius, vignetteRadius - vignetteSoftness, dist);

	COLOR = mix(color, vignetteColor, vignette);
}
