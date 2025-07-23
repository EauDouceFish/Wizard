Shader "MyShaders/Skybox"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ColorTop("Color Top", Color) = (0,0.6235643,0.9058824,1)
        _ColorBottom("Color Bottom", Color) = (0.62416,0.8079457,0.83,1)
        _Offset("Offset", Float) = 0
        _Distance("Distance", Float) = 267
        _Falloff("Falloff", Range(0.001, 100)) = 0.8
        [Toggle(_UV_BASED_TOGGLE_ON)] _UV_Based_Toggle("UV Based Toggle", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Background" "Queue"="Background" "PreviewType"="Skybox" }
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature _UV_BASED_TOGGLE_ON

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct a2v
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _ColorTop;
            float4 _ColorBottom;
            float _Offset;
            float _Distance;
            float _Falloff;

            v2f vert (a2v v)
            {
                v2f o;
                o.pos = TransformObjectToHClip(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.worldPos = TransformObjectToWorld(v.vertex).xyz;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 finalColor;
                
                #ifdef _UV_BASED_TOGGLE_ON
                    // UV based gradient (similar to original behavior)
                    finalColor = lerp(_ColorBottom, _ColorTop, i.uv.y);
                #else
                    // World position based gradient (from ASE shader)
                    float t = clamp((_Offset + i.worldPos.y) / _Distance, 0.0, 1.0);
                    t = saturate(pow(t, _Falloff));
                    finalColor = lerp(_ColorBottom, _ColorTop, t);
                #endif
                
                return finalColor;
            }
            ENDHLSL
        }
    }
}