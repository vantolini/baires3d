half4x4 worldViewProj;
half4 vecLightDir;
half4 vTexelSize;
half fNormalScale;
half maxHeight = 128;

half fBiasU;
half fBiasV;
half fScale;

half displacementWidth = 256;
half displacementHeight = 256;
       
// this texture will point to the heightmap
texture displacementMap;
//this sampler will be used to read (sample) the heightmap
sampler displacementSampler = sampler_state
{
        Texture = <displacementMap>;
        MipFilter = Point;
        MinFilter = Point;
        MagFilter = Point;
        AddressU = Clamp;
        AddressV = Clamp;
};

texture NormalMap;
sampler NormalMapSampler = sampler_state
{
   Texture = <NormalMap>;
   MinFilter = Linear;
   MagFilter = Linear;
   MipFilter = None;   
   AddressU  = Clamp;
   AddressV  = Clamp;
};

texture sandMap;
sampler sandSampler = sampler_state
{
    Texture   = <sandMap>;
    MipFilter = Linear;
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU  = Wrap;
    AddressV  = Wrap;
};

texture grassMap;
sampler grassSampler = sampler_state
{
    Texture   = <grassMap>;
    MipFilter = Linear;
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU  = Wrap;
    AddressV  = Wrap;
};

texture rockMap;
sampler rockSampler = sampler_state
{
    Texture   = <rockMap>;
    MipFilter = Linear;
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU  = Wrap;
    AddressV  = Wrap;
};

texture snowMap;
sampler snowSampler = sampler_state
{
    Texture   = <snowMap>;
    MipFilter = Linear;
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU  = Wrap;
    AddressV  = Wrap;
};

struct VS_OUTPUT
{
    half4 position	: POSITION0;
    half4 uv		: TEXCOORD0;
    half4 weights	: TEXCOORD1;
    half2 normal    : TEXCOORD2;
};

VS_OUTPUT Transform(half4 position : POSITION0)
{
    VS_OUTPUT Out;

	half4 uv = half4(position.x, position.z, 0, 0);
	
	uv.xy = half2(uv.x * fScale + fBiasU, uv.y * fScale + fBiasV);
	
	position.x = uv.x * displacementWidth;
	position.z = uv.y * displacementHeight;
	
	// this instruction reads from the heightmap, the value at the corresponding texture coordinate
    // Note: we selected level 0 for the mipmap parameter of tex2Dlod, since we want to read data exactly 
    // as it appears in the heightmap
    half height = tex2Dlod ( displacementSampler, half4(uv.xy , 0 , 0 ) ).x;
    
    // with the newly read height, we compute the new value of the Y coordinate
    // we multiply the height, which is in the (0,1) range by a value representing the Maximum Height 
    // of the Terrain
    position.y *= height * maxHeight;
	//Compute the final projected position by multiplying with the world, view and projection matrices
	Out.position = mul(position, worldViewProj);
	Out.uv = uv;

	half4 TexWeights = 0;
    TexWeights.x = saturate( 1.0f - abs(height - 0) / 0.2f);
    TexWeights.y = saturate( 1.0f - abs(height - 0.3) / 0.25f);
    TexWeights.z = saturate( 1.0f - abs(height - 0.6) / 0.25f);
    TexWeights.w = saturate( 1.0f - abs(height - 0.9) / 0.25f);
    half totalWeight = TexWeights.x + TexWeights.y + TexWeights.z + TexWeights.w;
	Out.weights = TexWeights/totalWeight;
	
	Out.normal = uv.xy;
	
    return Out;
}

half4 PixelShader(VS_OUTPUT In) : COLOR
{
	half4 sand = tex2D(sandSampler,In.uv*8);
	half4 grass = tex2D(grassSampler,In.uv*8);
	half4 rock = tex2D(rockSampler,In.uv*8);
	half4 snow = tex2D(snowSampler,In.uv*8);
	half4 weights = sand * In.weights.x + 
			grass * In.weights.y + 
			rock * In.weights.z + 
			snow * In.weights.w;	
	
    float4 normal = normalize(2.0f * (tex2D(NormalMapSampler, In.normal) - 0.5f));
    
    float4 light = normalize(-vecLightDir);
    
    // dot product between light and normal
    float ldn = dot(light, normal);
    
    ldn = max(0, ldn);
    
    // add some ambient light
    return weights * (0.5f + ldn);
}


technique GridDraw
{
    pass P0
    {
        VertexShader = compile vs_3_0 Transform();
        PixelShader = compile ps_3_0 PixelShader();
    }
}
