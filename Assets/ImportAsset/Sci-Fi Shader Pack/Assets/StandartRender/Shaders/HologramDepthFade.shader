// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SciFi/HologramDepthFade"
{
	Properties
	{
		_Scale("Scale", Float) = 1
		_Power("Power", Float) = 1
		[HDR]_EmissionColor("EmissionColor", Color) = (0,0,0,0)
		_DepthPower("DepthPower", Float) = 0
		_Hologram("Hologram", 2D) = "white" {}
		[Toggle(_INVERT_ON)] _Invert("Invert", Float) = 0
		_UV("UV", Float) = 1
		_VertexMovement("VertexMovement", Float) = 0.02
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 4.0
		#pragma shader_feature _INVERT_ON
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
			float4 screenPos;
		};

		uniform float _VertexMovement;
		uniform sampler2D _Hologram;
		uniform float _UV;
		uniform float _Scale;
		uniform float _Power;
		uniform float4 _EmissionColor;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float _DepthPower;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float2 appendResult24 = (float2(0.0 , 0.1));
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float2 appendResult10 = (float2(ase_worldPos.x , ase_worldPos.y));
			float2 panner13 = ( 1.0 * _Time.y * appendResult24 + ( appendResult10 * _UV ));
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = UnityObjectToWorldNormal( v.normal );
			float fresnelNdotV28 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode28 = ( 0.0 + _Scale * pow( 1.0 - fresnelNdotV28, _Power ) );
			float clampResult30 = clamp( ( 1.0 - fresnelNode28 ) , 0.0 , 1.0 );
			#ifdef _INVERT_ON
				float staticSwitch25 = clampResult30;
			#else
				float staticSwitch25 = fresnelNode28;
			#endif
			float4 temp_output_18_0 = ( tex2Dlod( _Hologram, float4( panner13, 0, 0.0) ) * staticSwitch25 );
			v.vertex.xyz += ( _VertexMovement * temp_output_18_0 ).rgb;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Emission = _EmissionColor.rgb;
			float2 appendResult24 = (float2(0.0 , 0.1));
			float3 ase_worldPos = i.worldPos;
			float2 appendResult10 = (float2(ase_worldPos.x , ase_worldPos.y));
			float2 panner13 = ( 1.0 * _Time.y * appendResult24 + ( appendResult10 * _UV ));
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float fresnelNdotV28 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode28 = ( 0.0 + _Scale * pow( 1.0 - fresnelNdotV28, _Power ) );
			float clampResult30 = clamp( ( 1.0 - fresnelNode28 ) , 0.0 , 1.0 );
			#ifdef _INVERT_ON
				float staticSwitch25 = clampResult30;
			#else
				float staticSwitch25 = fresnelNode28;
			#endif
			float4 temp_output_18_0 = ( tex2D( _Hologram, panner13 ) * staticSwitch25 );
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth32 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float distanceDepth32 = abs( ( screenDepth32 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _DepthPower ) );
			float clampResult33 = clamp( distanceDepth32 , 0.0 , 1.0 );
			float4 clampResult35 = clamp( ( temp_output_18_0 * clampResult33 ) , float4( 0,0,0,0 ) , float4( 1,0,0,0 ) );
			o.Alpha = clampResult35.r;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard alpha:fade keepalpha fullforwardshadows vertex:vertexDataFunc 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float3 worldPos : TEXCOORD1;
				float4 screenPos : TEXCOORD2;
				float3 worldNormal : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				vertexDataFunc( v, customInputData );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.worldNormal = worldNormal;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.screenPos = ComputeScreenPos( o.pos );
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = IN.worldNormal;
				surfIN.screenPos = IN.screenPos;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "2"
}
/*ASEBEGIN
Version=17700
7;29;1906;1004;1538.032;312.9685;1.336511;True;False
Node;AmplifyShaderEditor.RangedFloatNode;26;-1622.074,585.1648;Float;False;Property;_Power;Power;1;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;27;-1623.074,506.165;Float;False;Property;_Scale;Scale;0;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;9;-1763.214,146.1808;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;10;-1508.23,169.5674;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;12;-1517.352,272.2877;Float;False;Property;_UV;UV;6;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-1525.344,365.5782;Float;False;Constant;_Panner;Panner;5;0;Create;True;0;0;False;0;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;28;-1417.761,498.7057;Inherit;False;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;29;-1148.864,597.6802;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;-1292.262,167.6683;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;24;-1290.464,287.4843;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;31;-979.288,800.1174;Float;False;Property;_DepthPower;DepthPower;3;0;Create;True;0;0;False;0;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;30;-958.24,598.4838;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;13;-1096.095,167.7198;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0.1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DepthFade;32;-759.3484,782.6957;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;25;-771.1646,495.3801;Float;False;Property;_Invert;Invert;5;0;Create;True;0;0;False;0;0;0;0;True;;Toggle;2;Key0;Key1;Create;False;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;7;-858.938,139.6053;Inherit;True;Property;_Hologram;Hologram;4;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;33;-464.3613,781.7846;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;-434.7216,360.4145;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;21;-291.4192,274.0114;Float;False;Property;_VertexMovement;VertexMovement;7;0;Create;True;0;0;False;0;0.02;0.02;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;34;-210.1876,551.1613;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;4;-206.841,-147.2808;Float;False;Property;_EmissionColor;EmissionColor;2;1;[HDR];Create;True;0;0;False;0;0,0,0,0;0.6886792,0.6604874,0.1721698,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;17.6657,335.2642;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;35;3.614795,552.4577;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;6;263.4762,80.46301;Float;False;True;-1;4;2;0;0;Standard;SciFi/HologramDepthFade;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;10;0;9;1
WireConnection;10;1;9;2
WireConnection;28;2;27;0
WireConnection;28;3;26;0
WireConnection;29;0;28;0
WireConnection;11;0;10;0
WireConnection;11;1;12;0
WireConnection;24;1;16;0
WireConnection;30;0;29;0
WireConnection;13;0;11;0
WireConnection;13;2;24;0
WireConnection;32;0;31;0
WireConnection;25;1;28;0
WireConnection;25;0;30;0
WireConnection;7;1;13;0
WireConnection;33;0;32;0
WireConnection;18;0;7;0
WireConnection;18;1;25;0
WireConnection;34;0;18;0
WireConnection;34;1;33;0
WireConnection;20;0;21;0
WireConnection;20;1;18;0
WireConnection;35;0;34;0
WireConnection;6;2;4;0
WireConnection;6;9;35;0
WireConnection;6;11;20;0
ASEEND*/
//CHKSM=878B18BC846E44D17FFFE8ACBA980A39A6BAF583