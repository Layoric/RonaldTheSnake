#include "PPVertexShader.fxh"

cbuffer constants : register(b0)
{
	float2 lightScreenPosition;

	float4x4 matVP;

	float2 halfPixel;

	float SunSize;
};


Texture2D flare : register(t0);
Texture2D scene : register(t1);

SamplerState Scene : register(s0)
{
	AddressU = Clamp;
	AddressV = Clamp;  
};

SamplerState Flare : register(s1)
{
    Texture = (flare);
    AddressU = CLAMP;
    AddressV = CLAMP;
};

float4 LightSourceMaskPS(float4 pos : SV_POSITION, 
						float4 posScene : SCENE_POSITION,
						 float2 texCoord : TEXCOORD ) : SV_TARGET
{
	texCoord -= halfPixel;

	// Get the scene
	float4 col = 0;
	
	// Find the suns position in the world and map it to the screen space.
	float2 coord;
		
	float size = SunSize / 1;
					
	float2 center = lightScreenPosition;

	coord = .5 - (texCoord - center) / size * .5;
	col += (pow(flare.Sample(Flare,coord),2) * 1) * 2;						
	
	
	return col * scene.Sample(Scene,texCoord);	
}

technique LightSourceMask
{
	pass p0
	{
		VertexShader = compile vs_3_0 VertexShaderFunction();
		PixelShader = compile ps_3_0 LightSourceMaskPS();
	}
}