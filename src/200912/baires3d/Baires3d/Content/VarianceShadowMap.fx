//-----------------------------------------
//	ShadowMap
//-----------------------------------------

//------------------
//--- Parameters ---
shared float4x4 World;
shared float4x4 View;
shared float4x4 Projection;
shared float4x4 LightView;
shared float4x4 LightProjection;
shared float4 xLightPos;
shared float xLightPower;
shared float farClip;

Texture ShadowMap;
sampler ShadowMapSampler = sampler_state
{
    Texture = <ShadowMap>;

    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};

bool TextureEnabled;
texture Texture;

sampler ModelTextureSampler = sampler_state
{
    Texture = <Texture>;

    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};

//------------------
//--- Structures ---
struct VertexShaderInput
{
	float4 Position		: POSITION;
	float2 TexCoords	: TEXCOORD0;
	float3 Normal		: NORMAL;
};

struct VertexShaderOutput
{
	float4 position			: POSITION;
	float4 ShadowMapPos		: TEXCOORD0;
	float4 RealDistance     : TEXCOORD1;
	float2 TexCoords	    : TEXCOORD2;
	float3 Normal           : TEXCOORD3;
	float3 Position3D       : TEXCOORD4;
};

struct PixelShaderInput
{
	float4 Color : COLOR0;
	float4 position : POSITION;
	float4 ShadowMapPos : TEXCOORD0;
	float4 RealDistance     : TEXCOORD1;
	float2 TexCoords	    : TEXCOORD2;
	float3 Normal           : TEXCOORD3;
	float3 Position3D       : TEXCOORD4;
};

//--------------------
//--- VertexShader ---
VertexShaderOutput VertexShader(VertexShaderInput input)
{
	VertexShaderOutput output;
	output.position = mul(mul(mul(input.Position, World), View), Projection);
	output.ShadowMapPos = mul(mul(mul(input.Position, World), LightView), LightProjection);
	
	output.RealDistance = output.ShadowMapPos.z / farClip;
    output.Normal = normalize(mul(input.Normal, (float3x3)World));
    output.Position3D = mul(input.Position, World);
 
    output.TexCoords = input.TexCoords;
	
	return  output;
}

//-------------------
//--- PixelShader ---
float DotProduct(float4 LightPos, float3 Pos3D, float3 Normal)
{
    float3 LightDir = normalize(LightPos - Pos3D);
    return dot(LightDir, Normal);
}

float4 PixelShader(PixelShaderInput input) : COLOR
{
    float2 ProjectedTexCoords;
    
    ProjectedTexCoords[0] = input.ShadowMapPos.x / input.ShadowMapPos.w / 2.0f + 0.5f;
    ProjectedTexCoords[1] = -input.ShadowMapPos.y / input.ShadowMapPos.w / 2.0f + 0.5f;
	
	float4 ColorComponent = tex2D(ModelTextureSampler, input.TexCoords);
	float DiffuseLightingFactor = DotProduct(xLightPos, input.Position3D, input.Normal);
    
    float4 result = {0,0,0,0};
    if ((saturate(ProjectedTexCoords.x) == ProjectedTexCoords.x) && (saturate(ProjectedTexCoords.y) == ProjectedTexCoords.y))
	{
		float len = input.RealDistance.x;
		
		float4 moments = tex2D(ShadowMapSampler, ProjectedTexCoords);
	
  		float E_x2 = moments.y;
		float Ex_2 = moments.x * moments.x;
		float variance = min(max(E_x2 - Ex_2, 0.0) + 0.0005f, 1.0);
		float m_d = (moments.x - len);
		float p = variance / (variance + m_d * m_d);
	    	
		float shadow = p;//max(step(len, moments.x), p);
		
		result = ColorComponent * DiffuseLightingFactor * xLightPower * shadow;
	}
	else
		result = ColorComponent * DiffuseLightingFactor * xLightPower; //Lo que está fuera de la visión de la luz
    
    return result;
}


//------------------
//--- Techniques ---
technique SimpleEffect
{
    pass P0
    {
          VertexShader = compile vs_2_0 VertexShader();
          PixelShader = compile ps_2_0 PixelShader();
    }
}