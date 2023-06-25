#if OPENGL
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define VS_SHADERMODEL vs_4_0_level_9_1
    #define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float4x4 WorldMatrix;
float4x4 ViewMatrix;
float4x4 ProjectionMatrix;

float4 AmbienceColor = float4(0.2f, 0.2f, 0.2f, 1.0f);

// For Diffuse Lightning
float4x4 WorldInverseTransposeMatrix;
float3 DiffuseLightDirection = float3(-1.0f, 0.0f, 0.0f);
float4 DiffuseColor = float4(1.0f, 1.0f, 1.0f, 1.0f);

struct VertexShaderInput
{
    float4 Position : POSITION0;
    // For Diffuse Lightning
    float4 NormalVector : NORMAL0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    // For Diffuse Lightning
    float4 VertexColor : COLOR0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, WorldMatrix);
    float4 viewPosition = mul(worldPosition, ViewMatrix);
    output.Position = mul(viewPosition, ProjectionMatrix);
    
    // For Diffuse Lightning
    float4 normal = normalize(mul(input.NormalVector, WorldInverseTransposeMatrix));
    float lightIntensity = dot(normal, DiffuseLightDirection);
    output.VertexColor = saturate(DiffuseColor * lightIntensity);    

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    return saturate(input.VertexColor + AmbienceColor);
}

technique Diffuse
{
    pass Pass1
    {
        VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
        PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
    }
}