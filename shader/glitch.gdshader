shader_type canvas_item;

// Use the screen texture as input
uniform sampler2D screen_texture : hint_screen_texture, filter_linear_mipmap;
uniform float time : hint_range(0.0, 1.0);  // Time value to control animation
uniform float glitch_strength : hint_range(0.0, 1.0) = 0.5;  // Intensity of glitch effect
uniform float glitch_fre : hint_range(0.0, 1.0) = 0.5;  // Intensity of glitch effect

// Random function for TV static noise
float random(vec2 uv) {
    return fract(sin(dot(uv.xy, vec2(12.9898, 78.233))) * 43758.5453);
}

void fragment() {
    vec2 uv = UV;

    float block_size = 0.01;
    vec2 block_uv = floor(uv / block_size) * block_size;

    float displacement = (cos(block_uv.y * 5631.0 + time * 50.0) +
						 sin(block_uv.y * 651.0 + time * 500.0) +
						 cos(block_uv.y * 450.0 + time * 5000.0) +
						 sin(block_uv.y * 541.0 + time * 50000.0)) / 4.0; 
    if(abs(displacement) > (1.0 - glitch_fre))
		uv.x += displacement * glitch_strength * 0.05;

    COLOR = texture(screen_texture, uv);
}
