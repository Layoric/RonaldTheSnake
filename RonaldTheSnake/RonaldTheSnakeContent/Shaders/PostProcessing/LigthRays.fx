#include "PPVertexShader.fxh"

#define NUM_SAMPLES 64

cbuffer constants : register(b0)
{
	float2 lightScreenPosition;
	float4x4 matVP;
	float2 halfPixel;
	float Density;
	float Decay;
	float Weight;
	float Exposure;
};

Texture2D SceneTexture : register(t0);
SamplerState SceneSampler : register(s0);


float4 lightRayPS(
				float4 pos : SV_POSITION, 
				float4 posScene : SCENE_POSITION,
				float2 texCoord : TEXCOORD0 
				) : SV_TARGET
{
	// Find light pixel position
	
	float2 TexCoord = texCoord - halfPixel;

	float2 DeltaTexCoord = (TexCoord - lightScreenPosition);
	DeltaTexCoord *= (1.0f / NUM_SAMPLES * Density);

	DeltaTexCoord = DeltaTexCoord ;

	float3 col = SceneTexture.Sample(SceneSampler,TexCoord);
	float IlluminationDecay = 1.0;
	float3 Sample;
	
	for( int i = 0; i < NUM_SAMPLES; ++i )
	{
		TexCoord -= DeltaTexCoord;
		Sample = SceneTexture.Sample(SceneSampler, TexCoord);
		Sample *= IlluminationDecay * Weight;
		col += Sample;
		IlluminationDecay *= Decay;			
	}

	return float4(col * Exposure,1);
	
}

technique LightRayFX
{
	pass p0
	{
		VertexShader = compile vs_3_0 VertexShaderFunction();
		PixelShader = compile ps_3_0 lightRayPS();
	}
}
