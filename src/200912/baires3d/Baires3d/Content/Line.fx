// Line.fx
// Copyright 2006 Michael Anderson
// Version 1.0 -- January 7, 2007

uniform extern float4x4 worldViewProj : WORLDVIEWPROJECTION;
float time;
float length;
float radius;
float4 lineColor;


struct VS_OUTPUT
{
    float4 position : POSITION;
    float3 polar : TEXCOORD0;
    float2 col : TEXCOORD1;
};


VS_OUTPUT MyVS(
    float4 pos  : POSITION, 
    float3 norm : NORMAL, 
    float2 tex : TEXCOORD0 )
{
    VS_OUTPUT Out = (VS_OUTPUT)0;
    
    // Scale X by radius, and translate X by length, in worldspace
    // based on what part of the line we're on
	pos.x *= (tex.x * radius);
	pos.x += (tex.y * length);
		
	// Always scale Y by radius regardless on what part of the line we're on
	pos.y *= radius;

	Out.position = mul(pos, worldViewProj);
	
	Out.polar = norm;

	// Send "modelspace" (adjusted for line length and radius) coords to shader
    Out.col.x = pos.x;
    Out.col.y = pos.y;

    return Out;
}


// Helper function used by several pixel shaders to blur the line edges
float BlurEdge( float rho )
{
	float blurThreshold = 0.90; // This really should vary based on line's screenspace size
	if( rho < blurThreshold )
	{
		return 1.0f;
	}
	else
	{
		float normrho = (rho - blurThreshold) * 1 / (1 - blurThreshold);
		return 1 - normrho;
	}
}


float4 MyPSStandard( float3 polar : TEXCOORD0, float2 modelSpace: TEXCOORD1 ) : COLOR0
{
	float4 finalColor;
	finalColor.rgb = lineColor.rgb;
	finalColor.a = lineColor.a * BlurEdge( polar.x );
	return finalColor;
}


float4 MyPSNoBlur( float3 polar : TEXCOORD0, float2 modelSpace: TEXCOORD1 ) : COLOR0
{
	float4 finalColor = lineColor;
	return finalColor;
}


float4 MyPSAnimatedLinear( float3 polar : TEXCOORD0, float2 modelSpace: TEXCOORD1 ) : COLOR0
{
	float4 finalColor;
	float modulation = sin( ( modelSpace.x * 0.1 + time * 0.05 ) * 80 * 3.14159) * 0.5 + 0.5;
	finalColor.rgb = lineColor.rgb * modulation;
	finalColor.a = lineColor.a * BlurEdge( polar.x );
	return finalColor;
}


float4 MyPSAnimatedRadial( float3 polar : TEXCOORD0, float2 modelSpace: TEXCOORD1 ) : COLOR0
{
	float4 finalColor;
	float modulation = sin( ( -polar.x * 0.1 + time * 0.05 ) * 20 * 3.14159) * 0.5 + 0.5;
	finalColor.rgb = lineColor.rgb * modulation;
	finalColor.a = lineColor.a * BlurEdge( polar.x );
	return finalColor;
}


float4 MyPSModern( float3 polar : TEXCOORD0, float2 modelSpace: TEXCOORD1 ) : COLOR0
{
	float4 finalColor;
	finalColor.rgb = lineColor.rgb;

	float rho = polar.x;

	float a;
	float blurThreshold = 0.25;
	
	if( rho < blurThreshold )
	{
		a = 1.0f;
	}
	else
	{
		float normrho = (rho - blurThreshold) * 1 / (1 - blurThreshold);
		a = normrho;
	}
	
	finalColor.a = lineColor.a * a;

	return finalColor;
}


float4 MyPSTubular( float3 polar : TEXCOORD0, float2 modelSpace: TEXCOORD1 ) : COLOR0
{
	float4 finalColor = lineColor;
	finalColor.a *= polar.x;
	return finalColor;
}


float4 MyPSGlow( float3 polar : TEXCOORD0, float2 modelSpace: TEXCOORD1 ) : COLOR0
{
	float4 finalColor = lineColor;
	finalColor.a *= 1 - polar.x;
	return finalColor;
}


technique Standard
{
    pass P0
    {
		CullMode = CW;
		AlphaBlendEnable = true;
		SrcBlend = SrcAlpha;
		DestBlend = InvSrcAlpha;
		BlendOp = Add;
        vertexShader = compile vs_1_1 MyVS();
        pixelShader = compile ps_2_0 MyPSStandard();
    }
}


technique NoBlur
{
    pass P0
    {
		CullMode = CW;
		AlphaBlendEnable = true;
		SrcBlend = SrcAlpha;
		DestBlend = InvSrcAlpha;
		BlendOp = Add;
        vertexShader = compile vs_1_1 MyVS();
        pixelShader = compile ps_2_0 MyPSNoBlur();
    }
}


technique AnimatedLinear
{
    pass P0
    {
		CullMode = CW;
		AlphaBlendEnable = true;
		SrcBlend = SrcAlpha;
		DestBlend = InvSrcAlpha;
		BlendOp = Add;
        vertexShader = compile vs_1_1 MyVS();
        pixelShader = compile ps_2_0 MyPSAnimatedLinear();
    }
}


technique AnimatedRadial
{
    pass P0
    {
		CullMode = CW;
		AlphaBlendEnable = true;
		SrcBlend = SrcAlpha;
		DestBlend = InvSrcAlpha;
		BlendOp = Add;
        vertexShader = compile vs_1_1 MyVS();
        pixelShader = compile ps_2_0 MyPSAnimatedRadial();
    }
}


technique Modern
{
    pass P0
    {
		CullMode = CW;
		AlphaBlendEnable = true;
		SrcBlend = SrcAlpha;
		DestBlend = InvSrcAlpha;
		BlendOp = Add;
        vertexShader = compile vs_1_1 MyVS();
        pixelShader = compile ps_2_0 MyPSModern();
    }
}


technique Tubular
{
    pass P0
    {
		CullMode = CW;
		AlphaBlendEnable = true;
		SrcBlend = SrcAlpha;
		DestBlend = InvSrcAlpha;
		BlendOp = Add;
        vertexShader = compile vs_1_1 MyVS();
        pixelShader = compile ps_2_0 MyPSTubular();
    }
}


technique Glow
{
    pass P0
    {
		CullMode = CW;
		AlphaBlendEnable = true;
		SrcBlend = SrcAlpha;
		DestBlend = InvSrcAlpha;
		BlendOp = Add;
        vertexShader = compile vs_1_1 MyVS();
        pixelShader = compile ps_2_0 MyPSGlow();
    }
}