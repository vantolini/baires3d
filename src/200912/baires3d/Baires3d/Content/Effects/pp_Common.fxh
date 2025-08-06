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
//	File:		pp_Common.fx
//
//	Desc:		Common samplers, global variable, and vertex shader 
//				shared by post-processing shaders.
//
//======================================================================

float2	g_vSourceDimensions;
float2	g_vDestinationDimensions;

float3	g_vFrustumCornersVS [4];

texture2D SourceTexture0;
sampler2D PointSampler0 = sampler_state
{
    Texture = <SourceTexture0>;
    MinFilter = point;
    MagFilter = point;
    MipFilter = point;
    MaxAnisotropy = 1;
    AddressU = CLAMP;
    AddressV = CLAMP;
};

sampler2D LinearSampler0 = sampler_state
{
    Texture = <SourceTexture0>;
    MinFilter = linear;
    MagFilter = linear;
    MipFilter = point;
    MaxAnisotropy = 1;
    AddressU = CLAMP;
    AddressV = CLAMP;
};

texture2D SourceTexture1;
sampler2D PointSampler1 = sampler_state
{
    Texture = <SourceTexture1>;
    MinFilter = point;
    MagFilter = point;
    MipFilter = point;
    MaxAnisotropy = 1;
    AddressU = CLAMP;
    AddressV = CLAMP;
};

sampler2D LinearSampler1 = sampler_state
{
    Texture = <SourceTexture1>;
    MinFilter = linear;
    MagFilter = linear;
    MipFilter = point;
    MaxAnisotropy = 1;
    AddressU = CLAMP;
    AddressV = CLAMP;
};

texture2D SourceTexture2;
sampler2D PointSampler2 = sampler_state
{
    Texture = <SourceTexture2>;
    MinFilter = point;
    MagFilter = point;
    MipFilter = point;
    MaxAnisotropy = 1;
    AddressU = CLAMP;
    AddressV = CLAMP;
};

sampler2D LinearSampler2 = sampler_state
{
    Texture = <SourceTexture2>;
    MinFilter = linear;
    MagFilter = linear;
    MipFilter = point;
    MaxAnisotropy = 1;
    AddressU = CLAMP;
    AddressV = CLAMP;
};

// Reconstruct position from a linear depth buffer
float3 PositionFromDepth(sampler2D DepthSampler, float2 vTexCoord, float3 vFrustumRay)
{	
	float fPixelDepth = tex2D(DepthSampler, vTexCoord).x;
	return fPixelDepth * vFrustumRay;
}

void PostProcessVS (	in float3 in_vPositionOS				: POSITION,
						in float3 in_vTexCoordAndCornerIndex	: TEXCOORD0,					
						out float4 out_vPositionCS				: POSITION,
						out float2 out_vTexCoord				: TEXCOORD0,
						out float3 out_vFrustumCornerVS			: TEXCOORD1	)
{
	// Offset the position by half a pixel to correctly align texels to pixels
	out_vPositionCS.x = in_vPositionOS.x - (1.0f / g_vDestinationDimensions.x);
	out_vPositionCS.y = in_vPositionOS.y + (1.0f / g_vDestinationDimensions.y);
	out_vPositionCS.z = in_vPositionOS.z;
	out_vPositionCS.w = 1.0f;
	
	// Pass along the texture coordinate and the position of the frustum corner
	out_vTexCoord = in_vTexCoordAndCornerIndex.xy;
	out_vFrustumCornerVS = g_vFrustumCornersVS[in_vTexCoordAndCornerIndex.z];
}	


	