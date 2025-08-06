//======================================================================
//
//	MotionBlurSample
//
//		by MJP 
//      mpettineo@gmail.com
//      http://mynameismjp.wordpress.com/
//		12/1/09
//
//======================================================================
//
//	File:		Model.fx
//
//	Desc:		Renders the scene model, writing color, depth, and 
//				velocity to three render targets
//
//======================================================================

// Transform matrices
float4x4 g_matWorld;
float4x4 g_matView;
float4x4 g_matProj;
float4x4 g_matPrevWorldViewProj;

// Camera position
float3 g_vCameraPositionWS;
float g_fCameraFarClip;

// Lighting parameters
float3 g_vSunlightDirectionWS;
float3 g_vSunlightColor;
float g_fSunlightBrightness;

// Material parameters
float3 g_vDiffuseAlbedo;
float3 g_vSpecularAlbedo; 
float g_fSpecularPower;
float g_fReflectivity;

float g_fReflectionBrightness;

texture ReflectionMap;
samplerCUBE ReflectionSampler = sampler_state
{
    Texture = <ReflectionMap>;
    MinFilter = anisotropic;
    MagFilter = linear;
    MipFilter = linear;
    AddressU = Clamp;
    AddressV = Clamp;
    AddressW = Clamp;
    MaxAnisotropy = 16;
};

// Calculated the contribution for a single light
float3 CalcLighting (	float3 vDiffuseAlbedo, 
						float3 vSpecularAlbedo, 
						float fSpecularPower, 
						float3 vLightColor, 
						float3 vNormal, 
						float3 vLightDir, 
						float3 vViewDir	)
{
	float3 R = normalize(reflect(-vLightDir, vNormal));
    
    // Calculate the raw lighting terms
    float fDiffuseReflectance = saturate(dot(vNormal, vLightDir));
    float fSpecularReflectance = saturate(dot(R, vViewDir));

	// Modulate the lighting terms based on the material colors, and the attenuation factor
    float3 vSpecular = vSpecularAlbedo * vLightColor * pow(fSpecularReflectance, fSpecularPower);
    float3 vDiffuse = vDiffuseAlbedo * vLightColor * fDiffuseReflectance;	

	// Lighting contribution is the sum of diffuse and specular terms
	return vDiffuse + vSpecular;
}

// Vertex shader
void ModelVS (	in float4 in_vPositionOS	: POSITION,
				in float3 in_vNormalOS		: NORMAL,
				out float4 out_vPositionCS	: POSITION,
				out float3 out_vNormalWS	: TEXCOORD0,
				out float3 out_vPositionWS	: TEXCOORD1,
				out float3 out_vPositionVS	: TEXCOORD2,
				out float4 out_vCurrPositionCS	: TEXCOORD3,
				out float4 out_vPrevPositionCS : TEXCOORD4	)
{
	// Calculate the clip-space vertex position
	float4x4 matWorldViewProj = mul(mul(g_matWorld, g_matView), g_matProj);
	out_vPositionCS = mul(in_vPositionOS, matWorldViewProj);

	// Calculate world-space position and normal
	out_vPositionWS = mul(in_vPositionOS, g_matWorld);
	out_vNormalWS = mul(in_vNormalOS, g_matWorld);

	// Calculate view-space position
	out_vPositionVS = mul(float4(out_vPositionWS, 1.0f), g_matView);
	
	// Pass along the current vertex position in clip-space,
	// as well as the previous vertex position in  clip-space
	out_vCurrPositionCS = out_vPositionCS;
	out_vPrevPositionCS = mul(in_vPositionOS, g_matPrevWorldViewProj);
}				

// Pixel shader
void ModelPS (	in float3 in_vNormalWS		: TEXCOORD0,
				in float3 in_vPositionWS	: TEXCOORD1,
				in float3 in_vPositionVS	: TEXCOORD2,
				in float4 in_vCurrPositionCS : TEXCOORD3,
				in float4 in_vPrevPositionCS : TEXCOORD4,
				out float4 out_vColor : COLOR0,
				out float4 out_vDepth : COLOR1,
				out float4 out_vVelocity : COLOR2	)
{
	// Calculate the reflected view direction
	float3 vNormalWS = normalize(in_vNormalWS);
	float3 vViewDirWS = normalize(g_vCameraPositionWS - in_vPositionWS);
	float3 vReflectedDirWS = reflect(-vViewDirWS, vNormalWS);
	vReflectedDirWS.z *= -1;
	
	// Get the sunlight term
	float3 vColor = CalcLighting(	g_vDiffuseAlbedo,
									g_vSpecularAlbedo,
									g_fSpecularPower,
									g_vSunlightColor * g_fSunlightBrightness,
									vNormalWS,
									normalize(-g_vSunlightDirectionWS),
									vViewDirWS	);
									
	// Add in the reflection
	float3 vReflection = texCUBE(ReflectionSampler, vReflectedDirWS);
	vColor += vReflection * g_fReflectivity * g_fReflectionBrightness;																			
	out_vColor = float4(vColor, 1.0f);							

	// Output normalized view-space depth
	out_vDepth = float4(-in_vPositionVS.z / g_fCameraFarClip, 1.0f, 1.0f, 1.0f);
	
	// Calculate the instantaneous pixel velocity. Since clip-space coordinates are of the range [-1, 1] 
	// with Y increasing from the bottom to the top of screen, we'll rescale x and y and flip y so that
	// the velocity corresponds to texture coordinates (which are of the range [0,1], and y increases from top to bottom)
	float2 vVelocity = (in_vCurrPositionCS.xy / in_vCurrPositionCS.w) - (in_vPrevPositionCS.xy / in_vPrevPositionCS.w);
	vVelocity *= 0.5f;
	vVelocity.y *= -1;
	out_vVelocity = float4(vVelocity, 1.0f, 1.0f);
}


technique Render
{
    pass p0
    {
		ZEnable = true;
		ZWriteEnable = true;
		AlphaBlendEnable = false;
		CullMode = CCW;
		AlphaTestEnable = false;
		StencilEnable = true;
		StencilFunc = ALWAYS;
		StencilRef = 1;
		StencilPass = REPLACE;
		StencilZFail = KEEP;
    
        VertexShader = compile vs_2_0 ModelVS();
        PixelShader = compile ps_2_0 ModelPS();
    }
}
			