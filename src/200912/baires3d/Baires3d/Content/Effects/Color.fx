
float4x4 World;
float4x4 WorldViewProjection;

float4x4 LightView;

float Attenuation = 10000.0f;
static const float BIAS = 0.001f;

texture BaseTexture;

float4 g_materialAmbientColor = 1;
float4 g_materialDiffuseColor = 1;
float4 g_materialSpecularColor = 1;

float4 g_LightDiffuse = 1;
float4 g_LightAmbient = 1;

float3 LightPosition;
float3 CameraPos;
float linearAttenuation = 0.0005f;
float quadraticAttenuation = 0.0000125f;


sampler2D baseSampler = sampler_state
{
	Texture = <BaseTexture>;
    ADDRESSU = WRAP;
	ADDRESSV = WRAP;
	MAGFILTER = LINEAR;
	MINFILTER = LINEAR;
	MIPFILTER = LINEAR;
};

texture frontTexture;

sampler2D frontSampler = sampler_state
{
	Texture = <frontTexture>;
    ADDRESSU = WRAP;
	ADDRESSV = WRAP;
	MAGFILTER = POINT;
	MINFILTER = POINT;
	MIPFILTER = POINT;
};

texture backTexture;

sampler2D backSampler = sampler_state
{
	Texture = <backTexture>;
    ADDRESSU = WRAP;
	ADDRESSV = WRAP;
	MAGFILTER = POINT;
	MINFILTER = POINT;
	MIPFILTER = POINT;
};

struct VertexShaderInput
{
    float3 Position : POSITION0;
    float3 Normal	: NORMAL0;
    float4 TexCoord : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position		: POSITION0;
    float3 Normal		: TEXCOORD0;
    float4 TexCoord		: TEXCOORD1;
    float3 Position3D	: TEXCOORD2;
    float3 LightDir		: TEXCOORD3;
    float3 ViewDir		: TEXCOORD4;
    float  dist			: TEXCOORD5;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    output.Position = mul(float4(input.Position, 1.0f), WorldViewProjection);
	output.Position3D = mul(float4(input.Position, 1.0f), World).xyz;
	
	output.TexCoord = input.TexCoord;
	
	output.Normal = mul(input.Normal, World);
	
	float3 lightDir =  LightPosition - mul(float4(input.Position, 1.0f), World);
	output.LightDir = normalize(lightDir);
	output.ViewDir =  CameraPos - mul(float4(input.Position, 1.0f), World);
	output.dist = length(lightDir);

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    
    float4 texColor = tex2D(baseSampler, input.TexCoord);
    
    float3 Normal = normalize(input.Normal);
	float4 Diffuse = saturate(dot(normalize(input.Normal), normalize(input.LightDir)));
	Diffuse = Diffuse * g_LightDiffuse * g_materialDiffuseColor;
	float4 finalColor = 1;
	float att = input.dist * linearAttenuation;
	att += input.dist * input.dist * quadraticAttenuation;
	att = 1 / att;
	finalColor = ((Diffuse * att)
		 +  g_materialAmbientColor) * texColor;
		 
    
    float3 pos = mul(float4(input.Position3D, 1.0f), LightView);
    
    float L = length(pos);
	float3 P0 = pos / L;
	
	float alpha = 0.5f + pos.z / Attenuation;
	
	P0.z = P0.z + 1;
	P0.x = P0.x / P0.z;
	P0.y = P0.y / P0.z;
	P0.z = L / Attenuation;
	
	P0.x =  0.5f * P0.x + 0.5f;
	P0.y = -0.5f * P0.y + 0.5f;
	
	float3 P1 = pos / L;
	
	P1.z = 1 - P1.z;
	P1.x = P1.x / P1.z;
	P1.y = P1.y / P1.z;
	P1.z = L / Attenuation;
	
	P1.x =  0.5f * P1.x + 0.5f;
	P1.y = -0.5f * P1.y + 0.5f;
	
	float depthFromMap;
	float depthFromPoint;
	
	if(alpha >= 0.5f)
	{
		depthFromMap = tex2D(frontSampler, P0.xy).x;
		depthFromPoint = P0.z;
	}
	else
	{
		depthFromMap = tex2D(backSampler, P1.xy).x;
		depthFromPoint = P1.z;
	}
	
	if((depthFromMap + BIAS) < depthFromPoint)
		finalColor.xyz *= 0.3f;

	return finalColor;
}

technique Technique1
{
    pass Pass1
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_1_1 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
