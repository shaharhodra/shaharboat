#warning Upgrade NOTE: unity_Scale shader variable was removed; replaced 'unity_Scale.w' with '1.0'
// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge Beta 0.34 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.34;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:0,limd:0,uamb:True,mssp:True,lmpd:False,lprd:True,enco:False,frtr:False,vitr:False,dbil:False,rmgx:True,rpth:0,hqsc:False,hqlp:False,blpr:5,bsrc:3,bdst:7,culm:0,dpts:2,wrdp:False,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:31696,y:32397|normal-10096-OUT,custl-10147-OUT,alpha-10177-OUT,voffset-10060-OUT;n:type:ShaderForge.SFN_NormalVector,id:139,x:33340,y:32702,pt:True;n:type:ShaderForge.SFN_Tex2d,id:151,x:33623,y:32449,ntxv:0,isnm:False|UVIN-9628-OUT,TEX-10130-TEX;n:type:ShaderForge.SFN_Color,id:168,x:32987,y:33062,ptlb:Glow Color,ptin:_GlowColor,glob:False,c1:1,c2:0.2391481,c3:0.1102941,c4:1;n:type:ShaderForge.SFN_Time,id:9244,x:35225,y:32378;n:type:ShaderForge.SFN_TexCoord,id:9288,x:35235,y:31784,uv:0;n:type:ShaderForge.SFN_Append,id:9628,x:34437,y:31827|A-9629-OUT,B-9681-OUT;n:type:ShaderForge.SFN_Add,id:9629,x:34661,y:31827|A-9288-U,B-9885-OUT;n:type:ShaderForge.SFN_Add,id:9681,x:34661,y:31960|A-9288-V,B-9963-OUT;n:type:ShaderForge.SFN_Multiply,id:9885,x:34909,y:31916|A-9244-T,B-9946-X;n:type:ShaderForge.SFN_Vector4Property,id:9946,x:35235,y:31993,ptlb:TimeSpeed,ptin:_TimeSpeed,glob:False,v1:0,v2:0,v3:0,v4:0;n:type:ShaderForge.SFN_Multiply,id:9963,x:34909,y:32064|A-9244-T,B-9946-Y;n:type:ShaderForge.SFN_Multiply,id:10023,x:34909,y:32318|A-9946-Z,B-9244-T;n:type:ShaderForge.SFN_Multiply,id:10024,x:34909,y:32467|A-9946-W,B-9244-T;n:type:ShaderForge.SFN_Add,id:10025,x:34665,y:32260|A-9288-U,B-10023-OUT;n:type:ShaderForge.SFN_Add,id:10026,x:34665,y:32400|A-9288-V,B-10024-OUT;n:type:ShaderForge.SFN_Append,id:10027,x:34430,y:32260|A-10025-OUT,B-10026-OUT;n:type:ShaderForge.SFN_Tex2d,id:10029,x:33623,y:32615,ntxv:0,isnm:False|UVIN-10027-OUT,TEX-10130-TEX;n:type:ShaderForge.SFN_Multiply,id:10031,x:33340,y:32528|A-151-A,B-10029-A;n:type:ShaderForge.SFN_Multiply,id:10060,x:33110,y:32712|A-10031-OUT,B-139-OUT,C-10173-OUT;n:type:ShaderForge.SFN_Tex2d,id:10084,x:33350,y:32181,ntxv:0,isnm:False|UVIN-10172-OUT,TEX-10093-TEX;n:type:ShaderForge.SFN_Cubemap,id:10086,x:32794,y:32878,ptlb:Cube Map,ptin:_CubeMap;n:type:ShaderForge.SFN_Tex2dAsset,id:10093,x:33673,y:32047,ptlb:Normal,ptin:_Normal,glob:False;n:type:ShaderForge.SFN_Tex2d,id:10095,x:33350,y:31973,ntxv:0,isnm:False|UVIN-9628-OUT,TEX-10093-TEX;n:type:ShaderForge.SFN_Multiply,id:10096,x:33137,y:31973|A-10095-RGB,B-10084-RGB;n:type:ShaderForge.SFN_Tex2d,id:10115,x:32830,y:33451,ntxv:0,isnm:False|UVIN-9628-OUT,TEX-10130-TEX;n:type:ShaderForge.SFN_Multiply,id:10129,x:32630,y:33451|A-10115-RGB,B-10131-RGB;n:type:ShaderForge.SFN_Tex2dAsset,id:10130,x:34163,y:32982,ptlb:Diffuse,ptin:_Diffuse,glob:False;n:type:ShaderForge.SFN_Tex2d,id:10131,x:32830,y:33586,ntxv:0,isnm:False|UVIN-10027-OUT,TEX-10130-TEX;n:type:ShaderForge.SFN_Lerp,id:10147,x:32105,y:32942|A-10161-OUT,B-10129-OUT,T-10031-OUT;n:type:ShaderForge.SFN_Multiply,id:10161,x:32562,y:33144|A-10086-RGB,B-168-RGB,C-168-A;n:type:ShaderForge.SFN_Add,id:10167,x:34588,y:32816|A-10168-OUT,B-10170-OUT,C-9288-V;n:type:ShaderForge.SFN_Vector1,id:10168,x:34774,y:32731,v1:0.3;n:type:ShaderForge.SFN_Multiply,id:10169,x:34909,y:32631|A-9946-Z,B-9244-T;n:type:ShaderForge.SFN_Multiply,id:10170,x:34909,y:32781|A-9946-W,B-9244-T;n:type:ShaderForge.SFN_Add,id:10171,x:34588,y:32649|A-10168-OUT,B-10169-OUT,C-9288-U;n:type:ShaderForge.SFN_Append,id:10172,x:34352,y:32700|A-10171-OUT,B-10167-OUT;n:type:ShaderForge.SFN_Slider,id:10173,x:33340,y:32876,ptlb:Bulge Scale,ptin:_BulgeScale,min:0,cur:0,max:36;n:type:ShaderForge.SFN_Slider,id:10174,x:32562,y:32608,ptlb:Opasity,ptin:_Opasity,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Add,id:10176,x:33028,y:32378|A-151-A,B-10029-A;n:type:ShaderForge.SFN_Multiply,id:10177,x:32372,y:32354|A-10176-OUT,B-10174-OUT;proporder:168-10130-10086-10093-9946-10173-10174;pass:END;sub:END;*/

Shader "Shader Forge/Examples/DeepSea_AlphaBlending" {
    Properties {
        _GlowColor ("Glow Color", Color) = (1,0.2391481,0.1102941,1)
        _Diffuse ("Diffuse", 2D) = "white" {}
        _CubeMap ("Cube Map", Cube) = "_Skybox" {}
        _Normal ("Normal", 2D) = "white" {}
        _TimeSpeed ("TimeSpeed", Vector) = (0,0,0,0)
        _BulgeScale ("Bulge Scale", Range(0, 36)) = 0
        _Opasity ("Opasity", Range(0, 1)) = 0
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
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
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            Fog {Mode Off}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
//            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            #pragma glsl
            uniform float4 _TimeEditor;
            uniform float4 _GlowColor;
            uniform float4 _TimeSpeed;
            uniform samplerCUBE _CubeMap;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform float _BulgeScale;
            uniform float _Opasity;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 binormalDir : TEXCOORD4;
                float3 shLight : TEXCOORD5;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.shLight = ShadeSH9(float4(mul(unity_ObjectToWorld, float4(v.normal,0)).xyz * 1.0,1)) * 0.5;
                o.normalDir = mul(float4(v.normal,0), unity_WorldToObject).xyz;
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                float2 node_9288 = o.uv0;
                float4 node_9244 = _Time + _TimeEditor;
                float2 node_9628 = float2((node_9288.r+(node_9244.g*_TimeSpeed.r)),(node_9288.g+(node_9244.g*_TimeSpeed.g)));
                float4 node_151 = tex2Dlod(_Diffuse,float4(TRANSFORM_TEX(node_9628, _Diffuse),0.0,0));
                float2 node_10027 = float2((node_9288.r+(_TimeSpeed.b*node_9244.g)),(node_9288.g+(_TimeSpeed.a*node_9244.g)));
                float4 node_10029 = tex2Dlod(_Diffuse,float4(TRANSFORM_TEX(node_10027, _Diffuse),0.0,0));
                float node_10031 = (node_151.a*node_10029.a);
                v.vertex.xyz += (node_10031*v.normal*_BulgeScale);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
/////// Normals:
                float2 node_9288 = i.uv0;
                float4 node_9244 = _Time + _TimeEditor;
                float2 node_9628 = float2((node_9288.r+(node_9244.g*_TimeSpeed.r)),(node_9288.g+(node_9244.g*_TimeSpeed.g)));
                float node_10168 = 0.3;
                float2 node_10172 = float2((node_10168+(_TimeSpeed.b*node_9244.g)+node_9288.r),(node_10168+(_TimeSpeed.a*node_9244.g)+node_9288.g));
                float3 normalLocal = (tex2D(_Normal,TRANSFORM_TEX(node_9628, _Normal)).rgb*tex2D(_Normal,TRANSFORM_TEX(node_10172, _Normal)).rgb);
                float3 normalDirection =  normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
////// Lighting:
                float2 node_10027 = float2((node_9288.r+(_TimeSpeed.b*node_9244.g)),(node_9288.g+(_TimeSpeed.a*node_9244.g)));
                float4 node_151 = tex2D(_Diffuse,TRANSFORM_TEX(node_9628, _Diffuse));
                float4 node_10029 = tex2D(_Diffuse,TRANSFORM_TEX(node_10027, _Diffuse));
                float node_10031 = (node_151.a*node_10029.a);
                float3 finalColor = lerp((texCUBE(_CubeMap,viewReflectDirection).rgb*_GlowColor.rgb*_GlowColor.a),(tex2D(_Diffuse,TRANSFORM_TEX(node_9628, _Diffuse)).rgb*tex2D(_Diffuse,TRANSFORM_TEX(node_10027, _Diffuse)).rgb),node_10031);
/// Final Color:
                return fixed4(finalColor,((node_151.a+node_10029.a)*_Opasity));
            }
            ENDCG
        }
        Pass {
            Name "ShadowCollector"
            Tags {
                "LightMode"="ShadowCollector"
            }
            
            Fog {Mode Off}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCOLLECTOR
            #define SHADOW_COLLECTOR_PASS
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcollector
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            #pragma glsl
            uniform float4 _TimeEditor;
            uniform float4 _TimeSpeed;
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform float _BulgeScale;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_COLLECTOR;
                float2 uv0 : TEXCOORD5;
                float3 normalDir : TEXCOORD6;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.normalDir = mul(float4(v.normal,0), unity_WorldToObject).xyz;
                float2 node_9288 = o.uv0;
                float4 node_9244 = _Time + _TimeEditor;
                float2 node_9628 = float2((node_9288.r+(node_9244.g*_TimeSpeed.r)),(node_9288.g+(node_9244.g*_TimeSpeed.g)));
                float4 node_151 = tex2Dlod(_Diffuse,float4(TRANSFORM_TEX(node_9628, _Diffuse),0.0,0));
                float2 node_10027 = float2((node_9288.r+(_TimeSpeed.b*node_9244.g)),(node_9288.g+(_TimeSpeed.a*node_9244.g)));
                float4 node_10029 = tex2Dlod(_Diffuse,float4(TRANSFORM_TEX(node_10027, _Diffuse),0.0,0));
                float node_10031 = (node_151.a*node_10029.a);
                v.vertex.xyz += (node_10031*v.normal*_BulgeScale);
                o.pos = UnityObjectToClipPos(v.vertex);
                TRANSFER_SHADOW_COLLECTOR(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                SHADOW_COLLECTOR_FRAGMENT(i)
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Cull Off
            Offset 1, 1
            
            Fog {Mode Off}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            #pragma glsl
            uniform float4 _TimeEditor;
            uniform float4 _TimeSpeed;
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform float _BulgeScale;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.normalDir = mul(float4(v.normal,0), unity_WorldToObject).xyz;
                float2 node_9288 = o.uv0;
                float4 node_9244 = _Time + _TimeEditor;
                float2 node_9628 = float2((node_9288.r+(node_9244.g*_TimeSpeed.r)),(node_9288.g+(node_9244.g*_TimeSpeed.g)));
                float4 node_151 = tex2Dlod(_Diffuse,float4(TRANSFORM_TEX(node_9628, _Diffuse),0.0,0));
                float2 node_10027 = float2((node_9288.r+(_TimeSpeed.b*node_9244.g)),(node_9288.g+(_TimeSpeed.a*node_9244.g)));
                float4 node_10029 = tex2Dlod(_Diffuse,float4(TRANSFORM_TEX(node_10027, _Diffuse),0.0,0));
                float node_10031 = (node_151.a*node_10029.a);
                v.vertex.xyz += (node_10031*v.normal*_BulgeScale);
                o.pos = UnityObjectToClipPos(v.vertex);
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
