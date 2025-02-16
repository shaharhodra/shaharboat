// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

Shader "Mobile/Examples/DeepSea_Opaqe" {
    Properties {
        _GlowColor ("Glow Color", Color) = (1,0.2391481,0.1102941,1)
        _Diffuse ("Diffuse", 2D) = "white" {}
        _CubeMap ("Cube Map", Cube) = "_Skybox" {}
        _Normal ("Normal", 2D) = "white" {}
        _TimeSpeed ("TimeSpeed", Vector) = (0,0,0,0)
        _BulgeScale ("Bulge Scale", Range(0, 36)) = 0
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            ZWrite Off
            
            Fog {Mode Off}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile LIGHTMAP_ON LIGHTMAP_OFF
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            #pragma glsl
            uniform float4 _TimeEditor;
            uniform float4 _GlowColor;
            uniform float4 _TimeSpeed;
            uniform samplerCUBE _CubeMap;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform float _BulgeScale;
            
       			#ifdef LIGHTMAP_ON
      			// sampler2D unity_Lightmap;
      			fixed4 unity_LightmapST;
      			#endif
      			
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 binormalDir : TEXCOORD4;
                float2 lightmap : TEXCOORD5;
//                float3 shLight : TEXCOORD5;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
//                o.shLight = ShadeSH9(float4(mul(_Object2World, float4(v.normal,0)).xyz * unity_Scale.w,1)) * 0.5;
                o.normalDir = mul(float4(v.normal,0), unity_WorldToObject).xyz;
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                float2 TexUv0 = o.uv0;
                float4 TimeVector = _Time + _TimeEditor;
                float2 Uv_Time00 = float2((TexUv0.r+(TimeVector.g*_TimeSpeed.r)),(TexUv0.g+(TimeVector.g*_TimeSpeed.g)));
                float4 Tex00 = tex2Dlod(_Diffuse,float4(TRANSFORM_TEX(Uv_Time00, _Diffuse),0.0,0));
                float2 TexUv_TimeVector = float2((TexUv0.r+(_TimeSpeed.b*TimeVector.g)),(TexUv0.g+(_TimeSpeed.a*TimeVector.g)));
                float4 Tex01 = tex2Dlod(_Diffuse,float4(TRANSFORM_TEX(TexUv_TimeVector, _Diffuse),0.0,0));
                float Tex00_Tex01_a = (Tex00.a*Tex01.a);
                v.vertex.xyz += (Tex00_Tex01_a*v.normal*_BulgeScale); // vertex Hight
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
                
                #ifdef LIGHTMAP_ON
                o.lightmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                #endif
                
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
/////// Normals:
                float2 TexUv0 = i.uv0;
                float2 lightUv0 = i.lightmap; // lightmap uv
                float4 TimeVector = _Time + _TimeEditor;
                float2 Uv_Time00 = float2((TexUv0.r+(TimeVector.g*_TimeSpeed.r)),(TexUv0.g+(TimeVector.g*_TimeSpeed.g)));
                float AddUv = 0.3;
                float2 Uv_Time01 = float2((AddUv+(_TimeSpeed.b*TimeVector.g)+TexUv0.r),(AddUv+(_TimeSpeed.a*TimeVector.g)+TexUv0.g));
                float3 normalLocal = (tex2D(_Normal,TRANSFORM_TEX(Uv_Time00, _Normal)).rgb*tex2D(_Normal,TRANSFORM_TEX(Uv_Time01, _Normal)).rgb);
                float3 normalDirection =  normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
////// Lighting:
                float2 TexUv_TimeVector = float2((TexUv0.r+(_TimeSpeed.b*TimeVector.g)),(TexUv0.g+(_TimeSpeed.a*TimeVector.g)));
                float4 Tex00 = tex2D(_Diffuse,TRANSFORM_TEX(Uv_Time00, _Diffuse));
                float4 Tex01 = tex2D(_Diffuse,TRANSFORM_TEX(TexUv_TimeVector, _Diffuse));
                float Tex00_Tex01_a = (Tex00.a*Tex01.a);
                float3 finalColor = lerp((texCUBE(_CubeMap,viewReflectDirection).rgb*_GlowColor.rgb*_GlowColor.a),(tex2D(_Diffuse,TRANSFORM_TEX(Uv_Time00, _Diffuse)).rgb*tex2D(_Diffuse,TRANSFORM_TEX(TexUv_TimeVector, _Diffuse)).rgb),Tex00_Tex01_a);
////// Light Map Texture Add:
                #ifdef LIGHTMAP_ON
                finalColor.rgb *= DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, lightUv0.xy));
                #endif
////// Light probe Add:

/// Final Color:
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
