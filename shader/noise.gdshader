shader_type canvas_item;

uniform float time : hint_range(0.0, 1.0);  // Time value to control animation
uniform float glitch_strength : hint_range(0.0, 1.0) = 0.5;  // Intensity of glitch effect
uniform float glitch_fre : hint_range(0.0, 1.0) = 0.5;  // Intensity of glitch effect

uniform vec4 color = vec4(0, 0, 0, 1);
uniform float block_size = 8.0;
uniform float noise_strength = 0.5;
uniform float noise_speed = 0.5;
uniform sampler2D screen_texture : hint_screen_texture, filter_linear_mipmap;

float random(vec2 uv) {
    return fract(sin(dot(uv.xy, vec2(12.9898, 78.233))) * 43758.5453);
}

void fragment() {
    vec2 uv = UV;
    vec2 screen_resolution = SCREEN_PIXEL_SIZE.xy;

	// glitch
    float glitch_block_size = 0.01;
    vec2 glitch_block_uv = floor(uv / glitch_block_size) * glitch_block_size;

    float displacement = (cos(glitch_block_uv.y * 5631.0 + time * 50.0) +
						 sin(glitch_block_uv.y * 651.0 + time * 500.0) +
						 cos(glitch_block_uv.y * 450.0 + time * 5000.0) +
						 sin(glitch_block_uv.y * 541.0 + time * 50000.0)) / 4.0;
    if(abs(displacement) > (1.0 - glitch_fre))
		uv.x += displacement * glitch_strength * 0.05;

	//noise
    float aspect_ratio = screen_resolution.x / screen_resolution.y;
    vec2 adjusted_uv = vec2(uv.x, uv.y * aspect_ratio);
    vec2 block_uv = floor(adjusted_uv * block_size) / block_size;
    float noiseValue = random(block_uv * floor(TIME * noise_speed));
    vec4 texture_color = texture(screen_texture, uv);

    vec4 noise_texture = vec4(vec3(noiseValue), 1.0);
    vec4 noise_color = mix(color, noise_texture, noise_strength);
    vec4 final_color = mix(texture_color, noise_color, .6);

    COLOR = final_color;
}
