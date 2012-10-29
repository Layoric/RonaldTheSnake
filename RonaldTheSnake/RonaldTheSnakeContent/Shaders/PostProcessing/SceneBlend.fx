#include "PPVertexShader.fxh"

float2 halfPixel;

sampler2D Scene: register(s0){
	AddressU = Mirror;
	AddressV = Mirror;
};

texture OrgScene: register(t0);

sampler2D orgScene = sampler_state
{
	Texture = <OrgScene>;
	AddressU = CLAMP;
	AddressV = CLAMP;
};


float4 BlendPS(float4 pos : POSITION, float2 texCoord : TEXCOORD0 ) : COLOR0
{
	texCoord -= halfPixel;
	float4 col = tex2D(orgScene,texCoord) * tex2D(Scene,texCoord);

	return col;
}

float4 AditivePS(float4 pos : POSITION, float2 texCoord : TEXCOORD0 ) : COLOR0
{
	texCoord -= halfPixel;
	float4 col = tex2D(orgScene,texCoord) + tex2D(Scene,texCoord);

	return col;
}

technique Blend
{
	pass p0
	{
		VertexShader = compile vs_3_0 VertexShaderFunction();
		PixelShader = compile ps_3_0 BlendPS();
	}
}

technique Aditive
{
	pass p0
	{
		VertexShader = compile vs_3_0 VertexShaderFunction();
		PixelShader = compile ps_3_0 AditivePS();
	}
}