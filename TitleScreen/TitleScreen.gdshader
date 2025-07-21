shader_type canvas_item;

// Uniforms to control the effects
uniform float scanline_thickness : hint_range(0.0, 10.0) = 2.0; // Controls the density/spacing of scanlines
uniform float scanline_speed : hint_range(0.0, 1.0) = 0.1;     // Speed at which scanlines scroll
uniform float scanline_intensity : hint_range(0.0, 1.0) = 0.5; // How dark the scanlines are

uniform float chromatic_aberration_strength : hint_range(0.0, 0.05) = 0.005; // Strength of the chromatic aberration (increased range)
uniform vec2 chromatic_aberration_direction = vec2(1.0, 0.0); // Direction of chromatic aberration (e.g., horizontal)

// Fix for SCREEN_TEXTURE deprecation: Declare it as a uniform with hint_screen_texture
uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;

void fragment() {
    vec2 uv = SCREEN_UV; // Get screen UV for full-screen effects

    // --- Chromatic Aberration ---
    // Sample the texture with slight offsets for R, G, B channels
    // The offset direction can be controlled by chromatic_aberration_direction
    // and its strength by chromatic_aberration_strength.
    vec4 color_r = texture(SCREEN_TEXTURE, uv + chromatic_aberration_direction * chromatic_aberration_strength);
    vec4 color_g = texture(SCREEN_TEXTURE, uv); // Green channel is usually centered
    vec4 color_b = texture(SCREEN_TEXTURE, uv - chromatic_aberration_direction * chromatic_aberration_strength);

    // Combine the shifted color channels
    vec4 final_color = vec4(color_r.r, color_g.g, color_b.b, 1.0); // Keep alpha as 1.0 or from one of the samples

    // --- Scrolling Scanlines (Revised for smoother, non-strobing effect) ---
    // Calculate the Y coordinate for the scanline effect.
    // We use FRAGCOORD.y to get pixel-perfect scanlines that don't scale with resolution.
    // The 'scanline_thickness' uniform now influences the overall spacing of the scanlines.
    // TIME is used to make them scroll.
    float scanline_period = scanline_thickness * 5.0 + 10.0; // Adjusts density based on thickness
    float scanline_uv_y = FRAGCOORD.y / scanline_period + TIME * scanline_speed;

    // Use fract to get a repeating 0-1 value for each scanline cycle
    float scanline_value = fract(scanline_uv_y);

    // Create a smooth, dark scanline effect using a sine wave.
    // This creates a continuous dark band that fades out, avoiding sharp transitions.
    // The darkest part will be in the middle of the 0-1 cycle.
    float scanline_shape = sin(scanline_value * PI); // Goes from 0 (at 0) to 1 (at 0.5) to 0 (at 1)
    scanline_shape = pow(scanline_shape, 0.5); // Softens the shape, making the dark band wider and smoother

    // Invert the shape to get the dark line: 1 (bright) - shape (0-1) = 1 (bright) to 0 (dark)
    float scanline_darkness_factor = 1.0 - scanline_shape;

    // Apply intensity: mix between no darkness (1.0) and the calculated darkness factor.
    // 'scanline_intensity' of 0 means no darkness, 1 means full darkness.
    float final_scanline_effect = mix(1.0, scanline_darkness_factor, scanline_intensity);

    // Multiply the final_color (with CA) by the scanline effect
    final_color.rgb *= final_scanline_effect;

    COLOR = final_color;
}
