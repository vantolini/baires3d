float4x4 WorldViewProjection;
texture textureDiffuse;              // Mapa de textura diffuse

//----------------------------------------------------------------------------------

struct VS_INPUT
{
   float4 position:	POSITION;
};

struct VS_OUTPUT
{
   float4 position:		POSITION;
   float3 texcoord0:	TEXCOORD0;
};

VS_OUTPUT VShader( VS_INPUT vin )
{
	VS_OUTPUT vout;
   
	float4 Posicion_Vertice = vin.position;
   
	// Calculamos la posición final del punto
	// Si esto lo hacemos al principio, el shader puede detectar que no va a dibujarse y salir del shader
	// De esta forma ganaremos velocidad
	vout.position = mul(Posicion_Vertice, WorldViewProjection);
	vout.position.z = vout.position.w;
	
    // Pasamos las coordenadas de textura intactas
	vout.texcoord0 = vin.position;

	return vout;
}


samplerCUBE map_diffuse = 
sampler_state
{
	Texture = <textureDiffuse>;
	MipFilter = LINEAR;
	MinFilter = LINEAR;
	MagFilter = LINEAR;

	AddressU = Clamp;
	AddressV = Clamp;
};

struct PS_INPUT
{
	float3 texcoord0: TEXCOORD0;
};

float4 PShader( PS_INPUT pin ) : COLOR0
{
	float4 oColor = 1.0;
   
	// Recogemos los valores de la textura
	oColor = texCUBE(map_diffuse, pin.texcoord0);

	return oColor;
}

technique RenderScene
{
	pass p0
	{
		CullMode = None;
		ZWriteEnable = false;
		VertexShader = compile vs_1_1 VShader();
		PixelShader = compile ps_1_1 PShader();
		ZWriteEnable = true;
		CullMode = CCW;
	}
}

