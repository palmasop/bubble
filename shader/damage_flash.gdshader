shader_type canvas_item;

uniform vec4 flash_color : source_color = vec4(0.5, 0.0, 1.0, 1.0);  // Purple color
uniform float flash_modifier : hint_range(0.0, 1.0) = 0.0;

void fragment() {
    vec4 original_color = texture(TEXTURE, UV);
    
    // If there's no texture, use white as the base color
    if (original_color.a == 0.0) {
        original_color = vec4(1.0, 1.0, 1.0, 1.0);  // Default white for ColorRect
    }
    
    vec4 flash = flash_color * flash_modifier;
    COLOR = vec4(
        min(1.0, original_color.r + flash.r),
        min(1.0, original_color.g + flash.g),
        min(1.0, original_color.b + flash.b),
        original_color.a
    );
} 