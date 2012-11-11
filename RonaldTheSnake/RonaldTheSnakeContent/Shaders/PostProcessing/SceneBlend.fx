#include "PPVertexShader.fxh"
cbuffer constants : register(b0)
{
	float2 halfPixel;
};


Texture2D sceneTexture : register(t0);
SamplerState Scene: register(s0){
	AddressU = Mirror;
	AddressV = Mirror;
};

Texture2D OrgScene: register(t1);

SamplerState orgScene : register(s1)
{
	Texture = <OrgScene>;
	AddressU = CLAMP;
	AddressV = CLAMP;
};


float4 BlendPS(float2 texCoord : TEXCOORD0 ) : COLOR0
{
	texCoord -= halfPixel;
	float4 col = OrgScene.Sample(orgScene,texCoord) * sceneTexture.Sample(Scene,texCoord);

	return col;
}

float4 AditivePS(float2 texCoord : TEXCOORD0 ) : COLOR0
{
	texCoord -= halfPixel;
	float4 col = OrgScene.Sample(orgScene,texCoord) + sceneTexture.Sample(Scene,texCoord);

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