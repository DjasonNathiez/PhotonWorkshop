Shader "Special/Phong_hlsl"
{
    Properties
    {
        _BaseMap("Base Map", 2D) = "white" {}
        _MyColor("My Color", Color) = (.25, .5, .5, 1)
        
        [MaterialToggle] _IsCellShading("isCellShading", int) = 0
        [MaterialSlider] _Step("Cell Step", Range(1,8)) = 5
    }

        SubShader
        {
            Tags { "RenderType" = "Opaque" /*"Queue" = "Geometry"*/ "RenderPipeline" = "UniversalPipeline" }

            Blend SrcAlpha OneMinusSrcAlpha

            Pass
            {
                HLSLPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl" 
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

        struct Attributes
         {
             float4 positionOS   : POSITION;
             float2 uv       	: TEXCOORD0;
             float3 normalOS   	 : NORMAL;
         };

         struct Varyings
         {
             float4 positionHCS  : SV_POSITION;
             float3 viewDirWS	: TEXCOORD1; //SPECULAR
             float2 uv       	: TEXCOORD0;
             float3 normalOS   	 : NORMAL;
         };

        Texture2D _BaseMap;
        SamplerState sampler_BaseMap;
        float4 _MyColor;
        float _Step;
        int _IsCellShading;
                
        CBUFFER_START(UnityPerMaterial)
            float4 _BaseMap_ST;
        CBUFFER_END

        void Unity_ColorspaceConversion_RGB_HSV_float(float3 In, out float3 Out)
        {
            float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
            float4 P = lerp(float4(In.bg, K.wz), float4(In.gb, K.xy), step(In.b, In.g));
            float4 Q = lerp(float4(P.xyw, In.r), float4(In.r, P.yzx), step(P.x, In.r));
            float D = Q.x - min(Q.w, Q.y);
            float  E = 1e-10; //0,0000000001
            Out = float3(abs(Q.z + (Q.w - Q.y) / (6.0 * D + E)), D / (Q.x + E), Q.x);
        }

        void Unity_ColorspaceConversion_HSV_RGB_float(float3 In, out float3 Out)
        {
            float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
            float3 P = abs(frac(In.xxx + K.xyz) * 6.0 - K.www);
            Out = In.z * lerp(K.xxx, saturate(P - K.xxx), In.y);
        }

        Varyings vert(Attributes IN)
         {
             Varyings OUT;
             //IN.positionOS.y += sin(_Time.y + IN.positionOS.x) * 0.1; //vertex displacement exemple kdo
             OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
             OUT.viewDirWS = normalize(GetWorldSpaceViewDir(mul(unity_ObjectToWorld, IN.positionOS))); //SPECULAR
             OUT.normalOS = IN.normalOS;
             OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
             return OUT;
         }

        half4 frag(Varyings IN) : SV_Target
         {
            half4 tex_color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
            half4 surface_color = tex_color * _MyColor;

             Light light = GetMainLight();
             float4 normalWS = normalize(mul(unity_ObjectToWorld, float4(IN.normalOS.xyz, 0)));

             float3 diffuse_contrib = clamp(dot(light.direction, normalWS.xyz), 0, 1); //light.direction c’est  light_provenance, pas besoin de l’inverser	
             float sharpness = 8;
             float3 specular_contrib = pow(clamp(dot(reflect(light.direction, normalWS), -IN.viewDirWS), 0, 1), sharpness);
             float3 ambient_contrib = float3(0, 0.1, 0.2) *0.3;

             //float3 phong_lighting = ((diffuse_contrib * surface_color) + specular_contrib) * light.color ;
             float3 phong_lighting =    diffuse_contrib * light.color * surface_color 
                                      + specular_contrib * light.color 
                                      + ambient_contrib;
             
             float3 hsv;
             Unity_ColorspaceConversion_RGB_HSV_float(phong_lighting.rgb, hsv);
             
             if(_IsCellShading == 1)
             {
              //value postérisée
                float v = hsv.z; //value hsv
                v *= _Step;
                v = round(v); //on a aussi ceil(); floor();
                 v /= _Step;
                hsv.z = v;
             }
             
             float3 phong_cel_shaded;
             Unity_ColorspaceConversion_HSV_RGB_float(hsv, phong_cel_shaded);

             return half4(phong_cel_shaded.rgb, 1);
         }
        ENDHLSL
             }
        }
}
