float4x4 World;
float4x4 View;
float4x4 Projection;

float3 LightDirection = {1.0f,1.0f,0.0f};


struct VertexShaderInput
{
    float4 Position : POSITION0;
    float3 Normal	: NORMAL0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float3 Normal	: TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
	output.Normal = normalize(mul(input.Normal,(float3x3)World));
    
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float NdotL = dot (input.Normal, normalize(LightDirection));
    return float4(NdotL, NdotL, NdotL, 1);
}

technique Shaded
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}



// MOTION BLUR TECHNIQUE

float4x4 modelView;
float4x4 prevModelView;
float4x4 modelViewProj;
float4x4 prevModelViewProj;
float    blurScale;

texture SceneTexture;

sampler2D sceneTex = sampler_state
{
	Texture = <SceneTexture>;
    ADDRESSU = CLAMP;
	ADDRESSV = CLAMP;
	MAGFILTER = LINEAR;
	MINFILTER = LINEAR;
};

struct MBVertexShaderInput
{
    float4 coord   : POSITION0;
    float3 normal  : NORMAL;
};

struct MBVertexShaderOutput
{
    float4 pos		: POSITION0;
    float3 velocity	: TEXCOORD0;
	float4 pos3D	: TEXCOORD1;
};


MBVertexShaderOutput MBVertexShader(MBVertexShaderInput In)
{
	MBVertexShaderOutput Out;

	float4 P = mul(In.coord, modelView);
	float4 Pprev = mul(In.coord, prevModelView);

	float3 N = mul(In.normal, (float3x3) modelView);

	float3 motionVector = P.xyz - Pprev.xyz;

	P = mul(In.coord, modelViewProj);
	Pprev = mul( In.coord, prevModelViewProj);

	Pprev = lerp(P, Pprev, blurScale);


	Out.pos = P;
	
	// *** Geometry Stretching ***
	//float flag = dot(motionVector, N) > 0;
	//float4 Pstretch = flag ? P : Pprev;
	//Out.pos = Pstretch;

	P.xyz = P.xyz / P.w;
	Pprev.xyz = Pprev.xyz / Pprev.w;
	
	float3 dP = (P.xyz - Pprev.xyz);

	Out.velocity = dP;
	Out.pos3D = mul(In.coord, modelViewProj);

	return Out;
}

float4 MBPixelShader(MBVertexShaderOutput In) : COLOR0
{
	const float samples = 16;

	float2 wpos;
	wpos.x = In.pos3D.x / In.pos3D.w / 2.0f + 0.5f;
	wpos.y = -In.pos3D.y / In.pos3D.w / 2.0f + 0.5f;
	
	float2 velocity;
	velocity.x = - In.velocity.x * blurScale;  
	velocity.y = In.velocity.y * blurScale;        
		
	const float w = 1.0 / samples;  
	float4 color = 0;
	for(float i=0; i<samples; i+=1) 
	{
		float t = i / (samples-1);
		color = color + tex2D(sceneTex, wpos + velocity*t) * w;
	}

	return color;
}

technique MotionBlur
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 MBVertexShader();
        PixelShader = compile ps_2_0 MBPixelShader();
    }
}