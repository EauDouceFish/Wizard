Shader "MyShaders/ToonShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1, 1, 1, 1)
        _EdgeNoiseTexDiff ("Diffuse Edge Noise Texture", 2D) = "white" {}
        _EdgeNoiseTexSpec ("Specular Edge Noise Texture", 2D) = "white" {}
        _EdgeNoiseScale ("Edge Noise Scale", Range(0, 1)) = 1

        _DarknessRange ("Darkness Range", Range(-1, 1)) = 0

        _SpecularMinRange ("Edge SmoothStep Dark Range", Range(0, 1)) = 0.8
        _SpecularMaxRange ("Edge SmoothStep Light Range", Range(0, 1)) = 1

        _AmbientColorModifier("AmbientColor Modifier", Color) = (1, 1, 1, 1)
        _AmbientColorIntensity("AmbientColor Intensity", Float) = 1.0

        _DiffuseColor ("Diffuse Color", Color) = (1, 1, 1, 1)
        _DiffuseIntensity("Diffuse Intensity", Float) = 1.0


        _SpecularColor ("Specular Color", Color) = (1, 1, 1, 1)
        _SpecularIntensity ("Specular Intensity", Range(0, 50)) = 1
        _ShadowColor ("Shadow Color", Color) = (1, 1, 1, 1)
        //_Ramp ("Ramp Texture", 2D) = "white" {}

        [Toggle(_CastShadows)] _CastShadows ("CastShadows", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            #pragma multi_compile_shadowcaster // 支持阴影相关的多编译选项

            // 其他阴影相关宏编译
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS 
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE  
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS 
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS 
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma shader_feature _ALPHATEST_ON

            #pragma shader_feature _CastShadows


            struct a2v
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                //float4 shadowCoord : TEXCOORD3;
            };

            // 贴图采样
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_ST;

            TEXTURE2D(_EdgeNoiseTexDiff);
            SAMPLER(sampler_EdgeNoiseTexDiff);
            float4 _EdgeNoiseTexDiff_ST;         
            
            TEXTURE2D(_EdgeNoiseTexSpec);
            SAMPLER(sampler_EdgeNoiseTexSpec);
            float4 _EdgeNoiseTexSpec_ST;


            //TEXTURE2D(_Ramp);
            //SAMPLER(sampler_Ramp);

            // 颜色参数
            float4 _Color;

            float4 _AmbientColorModifier;
            float _AmbientColorIntensity;
            
            float4 _DiffuseColor;
            float _DiffuseIntensity;
            
            float4 _SpecularColor;
            float _SpecularIntensity;

            float _DarknessRange;
            float _SpecularMinRange;
            float _SpecularMaxRange;

            float _EdgeNoiseScale;
            float4 _ShadowColor; 

            v2f vert (a2v v)
            {
                v2f o;
                o.pos = TransformObjectToHClip(v.vertex.xyz);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.worldNormal = TransformObjectToWorldNormal(v.normal);
                //float3 worldPos = TransformObjectToWorld(v.vertex.xyz);
                o.worldPos = TransformObjectToWorld(v.vertex.xyz);
                //o.worldPos = worldPos;
                //o.shadowCoord = TransformWorldToShadowCoord(o.worldPos);

                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 fragColor;
                // 定义与归一化相关向量
                half3 worldLightDir = normalize((_MainLightPosition.xyz));
                half3 worldNormal = normalize(i.worldNormal);
                half3 worldViewDir = normalize(GetWorldSpaceViewDir(i.worldPos));
                half3 worldHalfDir = normalize(worldLightDir + worldViewDir);
                Light mainLight = GetMainLight(TransformWorldToShadowCoord(i.worldPos));
                mainLight.color = _ShadowColor;

                half cut = dot(worldNormal, worldLightDir);
                
                // Albedo 物体表面基础颜色
                // 环境光内置_GlossyEnvironmentColor捕获天空盒/Color
                // 漫反射 使用smoothStep形成色块叠加Noise
                half edgeNoiseDiff = SAMPLE_TEXTURE2D(_EdgeNoiseTexDiff, sampler_EdgeNoiseTexDiff, i.uv + _EdgeNoiseTexDiff_ST.xy * _EdgeNoiseScale).r;

                half diffuseCut = step(_DarknessRange + edgeNoiseDiff, cut);

                half diffRange = diffuseCut;

                half3 darkAmbient = _GlossyEnvironmentColor.rgb * _AmbientColorModifier.rgb * _AmbientColorIntensity * (1 - diffRange);

                half3 diffuse = _MainLightColor.rgb * diffRange* _DiffuseColor.rgb * _DiffuseIntensity;
                
                half edgeNoiseSpec = SAMPLE_TEXTURE2D(_EdgeNoiseTexSpec, sampler_EdgeNoiseTexSpec, i.uv * _EdgeNoiseTexSpec_ST.xy * _EdgeNoiseScale).r;
                // 防止高光区域溢出到暗部
                half specularRange = smoothstep(_SpecularMinRange + edgeNoiseSpec, _SpecularMaxRange + edgeNoiseSpec, cut);

                // Phong模型计算
                half3 specular = _MainLightColor.rgb * (_SpecularColor.rgb - _DiffuseColor.rgb)* specularRange * _SpecularIntensity;
                fragColor = half4((darkAmbient + diffuse + specular), 1.0); 

                #ifdef _CastShadows
                    fragColor = lerp(fragColor, _ShadowColor, 1.0 - mainLight.shadowAttenuation) * _Color;
                #endif

                return fragColor;
            }
            ENDHLSL
        }
        // 投影Pass
        Pass 
        {
            Name "My ShadowCaster"
            Tags { "LightMode"="ShadowCaster" }
            Offset 1, 1 // 设置bias
            Cull Back
    
            HLSLPROGRAM
            #pragma vertex vert 
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl" 
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl" 
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

            #pragma multi_compile_shadowcaster // 支持阴影相关的多编译选项

            // 其他阴影相关宏编译
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS 
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE  
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS 
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS 
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma shader_feature _ALPHATEST_ON

            struct a2v {
                float4 vertex : POSITION;
            };
            struct v2f { // 顶点输出结构
                float4 pos : SV_POSITION; 
                float3 worldPos : TEXCOORD0; // 世界空间位置
                float4 shadowCoord : TEXCOORD1;
            };

            // 自定义阴影颜色
            float4 _ShadowColor;

            
            v2f vert (a2v v) 
            {
                v2f o = (v2f)0;
                o.pos = TransformObjectToHClip(v.vertex.xyz);
                float3 worldPos = TransformObjectToWorld(v.vertex.xyz);
                o.worldPos = worldPos;
                o.shadowCoord = TransformWorldToShadowCoord(worldPos);

                return o;
            }

            float4 frag(v2f i) : SV_Target
            {

                // 获取阴影坐标
        
                // 获取主光源信息
                Light mainLight = GetMainLight(i.shadowCoord);
        
                // 计算阴影强度
                float shadowAttenuation = mainLight.shadowAttenuation;

                // 根据阴影衰减计算彩色阴影
                float4 shadowColor = _ShadowColor * shadowAttenuation;

                return shadowColor;
            }
           
            ENDHLSL
        }
        //UsePass "Universal Render Pipeline/Lit/ShadowCaster"

        
    }
}
