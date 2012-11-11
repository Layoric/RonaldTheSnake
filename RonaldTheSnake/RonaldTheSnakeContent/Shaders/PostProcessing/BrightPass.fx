// Pixel shader extracts the brighter areas of an image.
// This is the first step in applying a bloom postprocess.
cbuffer constants : register(b0)
{
	uniform extern float BloomThreshold;
	float2 halfPixel;
};

//uniform extern texture SceneTex;

Texture2D InputTexture : register(t0);
SamplerState TextureSampler : register(s0);


float4 BrightPassPS(float4 pos: SV_POSITION,
					float4 posScene: SCENE_POSITION, 
					float2 texCoord : TEXCOORD0) : SV_TARGET
{
	texCoord -= halfPixel;
    // Look up the original image color.
    float4 c = InputTexture.Sample(TextureSampler, texCoord);

    // Adjust it to keep only values brighter than the specified threshold.
    return saturate((c - BloomThreshold) / (1 - BloomThreshold));
}


technique BloomExtract
{
    pass P0
    {
        PixelShader = compile ps_3_0 BrightPassPS();
    }
}