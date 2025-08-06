
// Standard Matricies
float4x4 World;
float4x4 View;
float4x4 Projection;
static float4x4 ViewProjection = mul(View, Projection);

// Bezier Control Points (World Space)
float3 P0;	// Control Point 0 (Start)
float3 P1;	// Control Point 1 (End)

float3 CameraPosition;

// Phong Lighting Values
float ConstantAtten = 0;
float LinearAtten = 0.1f;
float QuadraticAtten = 0;

texture DiffuseTexture;
sampler2D DiffuseSampler = sampler_state
{
	Texture = <DiffuseTexture>;
	AddressU = WRAP;
	AddressV = WRAP;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
	MipFilter = LINEAR;
};

struct VS_Input
{
    float4 Position : POSITION0;
    float2 TexCoord	: TEXCOORD0;
    float3 Normal	: NORMAL;
};

struct VS_Output
{
    float4 Position : POSITION0;
    float2 TexCoord	: TEXCOORD0;
    float3 Normal	: TEXCOORD1;
    float4 PosWorld	: TEXCOORD2;
};

// Performs simple transformations, similar to those
// found in standard point or spot lighting shaders.
VS_Output VS_Common(VS_Input input)
{
    VS_Output output;
    
    // Transform and store world position
    output.PosWorld = mul(input.Position, World);
    
    // Transform and store post-perspective position
    output.Position = mul(output.PosWorld, ViewProjection);
    
    // Transform normal to world space
    output.Normal = mul(input.Normal, (float3x3)World);
    
    // Copy texture coordinate
    output.TexCoord = input.TexCoord;
    
    return output;
}

// Basic Linear Bezier Function
float3 Bezier(float t)
{
	return (1 - t) * P0 + t * P1;
}

// Performs phong lighting calculations per pixel.
// 1. Calculate the nearest position on the path light from the current pixel.
// 2. Use this position as a point light in the Phong lighting model.
float4 PS_LinearPathLight(VS_Output input) : COLOR0
{


	// Find the nearest point on the path light from the
	// given pixel position in terms of t.
	float3 top = (P1 * P0) - (P1 * input.PosWorld) - pow(P0, 2) + (P0 * input.PosWorld);
	float3 bottom = 2 * P1 * P0 - pow(P1, 2) - pow(P0, 2);
	float t = ((top.z + top.y + top.x) / (bottom.z + bottom.y + bottom.x));
	
	// Calculate world position from this t value
	// Clamp 0 <= t <= 1, keeps the lighting position within the bounds of the path.
	float3 lightPos = Bezier(clamp(t, 0, 1));
	
	
	
	
	// Perform Phong lighting calculations using the light position.
	float distance = distance(lightPos, input.PosWorld);
	
	float3 L = normalize(lightPos - input.PosWorld);
	float3 V = normalize(CameraPosition - input.PosWorld);
	float3 N = normalize(input.Normal);
	float3 R = normalize(reflect(-L, N));
	
	float4 diffuse = tex2D(DiffuseSampler, input.TexCoord) * saturate(dot(L, N));
	float specular = pow(saturate(dot(R, V)), 55);

	float attenuation = 1 / (ConstantAtten + (LinearAtten * distance) + (QuadraticAtten * pow(distance, 2)));
	
	return float4((attenuation * (diffuse + specular)).rgb, 1);
}

technique LinearPathLight
{
    pass Pass0
    {
        VertexShader = compile vs_3_0 VS_Common();
        PixelShader = compile ps_3_0 PS_LinearPathLight();
    }
}
