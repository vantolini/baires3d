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
//	File:		pp_MotionBlur.fx
//
//	Desc:		Post-processing motion-blur using either a depth buffer,
//				or 
//
//======================================================================

#include "pp_Common.fxh"

float g_fBlurAmount = 1.0f;
float4x4 g_matInvView;
float4x4 g_matLastViewProj;

float4 MotionBlur(float2 vTexCoord, float2 vPixelVelocity, int iNumSamples)
{
	// Clamp to a max velocity.  The max we can go without artifacts os
	// is 1.4f * iNumSamples...but we can fudge things a little.
	float2 maxVelocity = (2.0f * iNumSamples) / g_vSourceDimensions;
	vPixelVelocity = clamp(vPixelVelocity, -maxVelocity, maxVelocity);
	
	float2 vFinalSamplePos = vTexCoord + vPixelVelocity;

    // For each sample, sum up each sample's color in "vSum" and then divide
    // to average the color after all the samples are added.
    float4 vSum = 0;    
    for(int i = 0; i < iNumSamples; i++)
    {   
        // Sample texture in a new spot based on vPixelVelocity vector 
        // and average it with the other samples    
        float2 vSampleCoord = vTexCoord + (vPixelVelocity * (i  / (float)iNumSamples));
        
        // Lookup the color at this new spot
        float4 vSample = tex2D(PointSampler0, vSampleCoord);
        
        // Add it with the other samples
        vSum += vSample;
    }
    
    // Return the average color of all the samples
    return vSum / (float)iNumSamples;
}

float4 DepthMotionBlurPS (	in float2 in_vTexCoord			: TEXCOORD0,
							in float3 in_vFrustumPlaneVS	: TEXCOORD1,
							uniform int iNumSamples		)	: COLOR0 
{
	// Reconstruct view-space position from the depth buffer
	float fPixelDepthVS = tex2D(PointSampler1, in_vTexCoord).x;
	float3 vPixelPositionVS = fPixelDepthVS * in_vFrustumPlaneVS;

	// Convert to world-space, then determine the the  clip-space
	// position of the pixel in the previous frame.
	float4 vPixelPositionWS = mul(float4(vPixelPositionVS, 1.0f), g_matInvView);
	float4 vLastPixelPosCS = mul(vPixelPositionWS, g_matLastViewProj);

	// Find the corresponding texture coordinate
	float2 vLastTexCoord = vLastPixelPosCS.xy / vLastPixelPosCS.w;
    vLastTexCoord = (vLastTexCoord / 2.0f) + 0.5f;
    vLastTexCoord.y = 1.0f - vLastTexCoord.y;
    vLastTexCoord += 0.5f / g_vDestinationDimensions;

	float2 vPixelVelocity = in_vTexCoord - vLastTexCoord;
	
	return MotionBlur(in_vTexCoord, vPixelVelocity, iNumSamples);
}

float4 VelocityMotionBlurPS (	in float2 in_vTexCoord			: TEXCOORD0,
								in float3 in_vFrustumPlaneVS	: TEXCOORD1,
								uniform int iNumSamples		)	: COLOR0 
{
	// Sample velocity from our velocity buffer
	float4 vCurFramePixelVelocity = tex2D(PointSampler1, in_vTexCoord);
	
	return MotionBlur(in_vTexCoord, vCurFramePixelVelocity, iNumSamples);
}

float4 DualVelocityMotionBlurPS (	in float2 in_vTexCoord			: TEXCOORD0,
								in float3 in_vFrustumPlaneVS	: TEXCOORD1,
								uniform int iNumSamples		)	: COLOR0 
{
	// Sample velocity from our velocity buffers
	float4 vCurFramePixelVelocity = tex2D(PointSampler1, in_vTexCoord);
    float4 vLastFramePixelVelocity = tex2D(PointSampler2, in_vTexCoord);

	// We'll compare the magnitude of the velocity from the current frame and from
	// the previous frame, and then use whichever is larger
	float2 vPixelVelocity = 0;
	float fCurVelocitySqMag = vCurFramePixelVelocity.x * vCurFramePixelVelocity.x +
	                       vCurFramePixelVelocity.y * vCurFramePixelVelocity.y;
    float fLastVelocitySqMag = vLastFramePixelVelocity.x * vLastFramePixelVelocity.x +
                              vLastFramePixelVelocity.y * vLastFramePixelVelocity.y;
                                   
    if (fLastVelocitySqMag > fCurVelocitySqMag )
        vPixelVelocity = vLastFramePixelVelocity;
    else
		vPixelVelocity = vCurFramePixelVelocity;
	
	return MotionBlur(in_vTexCoord, vPixelVelocity, iNumSamples);
}

technique DepthBuffer4Samples
{
    pass p0
    {
        VertexShader = compile vs_2_0 PostProcessVS();
        PixelShader = compile ps_2_0 DepthMotionBlurPS(4);
        
        ZEnable = false;
        ZWriteEnable = false;
        AlphaBlendEnable = false;
        AlphaTestEnable = false;
    }
}

technique DepthBuffer8Samples
{
    pass p0
    {
        VertexShader = compile vs_2_0 PostProcessVS();
        PixelShader = compile ps_2_0 DepthMotionBlurPS(8);
        
        ZEnable = false;
        ZWriteEnable = false;
        AlphaBlendEnable = false;
        AlphaTestEnable = false;
    }
}

technique DepthBuffer12Samples
{
    pass p0
    {
        VertexShader = compile vs_2_0 PostProcessVS();
        PixelShader = compile ps_2_0 DepthMotionBlurPS(12);
        
        ZEnable = false;
        ZWriteEnable = false;
        AlphaBlendEnable = false;
        AlphaTestEnable = false;
    }
}

technique VelocityBuffer4Samples
{
    pass p0
    {
        VertexShader = compile vs_2_0 PostProcessVS();
        PixelShader = compile ps_2_0 VelocityMotionBlurPS(4);
        
        ZEnable = false;
        ZWriteEnable = false;
        AlphaBlendEnable = false;
        AlphaTestEnable = false;
    }
}

technique VelocityBuffer8Samples
{
    pass p0
    {
        VertexShader = compile vs_2_0 PostProcessVS();
        PixelShader = compile ps_2_0 VelocityMotionBlurPS(8);
        
        ZEnable = false;
        ZWriteEnable = false;
        AlphaBlendEnable = false;
        AlphaTestEnable = false;
    }
}

technique VelocityBuffer12Samples
{
    pass p0
    {
        VertexShader = compile vs_2_0 PostProcessVS();
        PixelShader = compile ps_2_0 VelocityMotionBlurPS(12);
        
        ZEnable = false;
        ZWriteEnable = false;
        AlphaBlendEnable = false;
        AlphaTestEnable = false;
    }
}

technique DualVelocityBuffer4Samples
{
    pass p0
    {
        VertexShader = compile vs_2_0 PostProcessVS();
        PixelShader = compile ps_2_0 DualVelocityMotionBlurPS(4);
        
        ZEnable = false;
        ZWriteEnable = false;
        AlphaBlendEnable = false;
        AlphaTestEnable = false;
    }
}

technique DualVelocityBuffer8Samples
{
    pass p0
    {
        VertexShader = compile vs_2_0 PostProcessVS();
        PixelShader = compile ps_2_0 DualVelocityMotionBlurPS(8);
        
        ZEnable = false;
        ZWriteEnable = false;
        AlphaBlendEnable = false;
        AlphaTestEnable = false;
    }
}

technique DualVelocityBuffer12Samples
{
    pass p0
    {
        VertexShader = compile vs_2_0 PostProcessVS();
        PixelShader = compile ps_2_0 DualVelocityMotionBlurPS(12);
        
        ZEnable = false;
        ZWriteEnable = false;
        AlphaBlendEnable = false;
        AlphaTestEnable = false;
    }
}