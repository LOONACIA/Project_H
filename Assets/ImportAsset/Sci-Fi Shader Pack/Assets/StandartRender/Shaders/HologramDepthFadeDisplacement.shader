// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SciFi/HologramDepthFadeDisplacement"
{
	Properties
	{
		_Hologram("Hologram", 2D) = "white" {}
		_ObjectHigh("ObjectHigh", Float) = 0
		_ObjectLow("ObjectLow", Float) = 0
		_Scale("Scale", Float) = 1
		_Power("Power", Float) = 1
		[HDR]_FirstColor("FirstColor", Color) = (0,0,0,0)
		[HDR]_SecondColor("SecondColor", Color) = (0,0,0,0)
		_DepthPower("DepthPower", Float) = 0
		[Toggle(_INVERT_ON)] _Invert("Invert", Float) = 0
		_UV("UV", Float) = 1
		_VertexMovement("VertexMovement", Float) = 0.02
		_Const4("Const4", Float) = 2
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
		#pragma target 3.0
		#pragma shader_feature _INVERT_ON
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
			float4 screenPos;
		};

		uniform sampler2D _Hologram;
		uniform float _UV;
		uniform float _Scale;
		uniform float _Power;
		uniform float _VertexMovement;
		uniform float _ShaderHologramDisplacement;
		uniform float _ObjectLow;
		uniform float _ObjectHigh;
		uniform float4 _FirstColor;
		uniform float4 _SecondColor;
		uniform float _Const4;
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
			float3 ase_vertexNormal = v.normal.xyz;
			float temp_output_56_0 = ( 0.0 + (_ObjectLow + (_ShaderHologramDisplacement - 0.0) * (_ObjectHigh - _ObjectLow) / (1.0 - 0.0)) );
			float3 ase_vertex3Pos = v.vertex.xyz;
			float vpY51 = ase_vertex3Pos.y;
			float temp_output_60_0 = ( temp_output_56_0 - vpY51 );
			float clampResult63 = clamp( temp_output_60_0 , 0.0 , temp_output_60_0 );
			float4 appendResult74 = (float4(0.0 , ( ( ase_vertexNormal.y * 0.02 ) + clampResult63 ) , 0.0 , 0.0));
			float smoothstepResult69 = smoothstep( ( temp_output_56_0 - 0.2 ) , ( temp_output_56_0 + 0.2 ) , vpY51);
			float4 vertex84 = ( appendResult74 * smoothstepResult69 );
			float smoothstepResult73 = smoothstep( ( temp_output_56_0 - 0.5 ) , ( temp_output_56_0 + 0.5 ) , vpY51);
			float mask81 = smoothstepResult73;
			float4 lerpResult80 = lerp( ( temp_output_18_0 * _VertexMovement ) , vertex84 , mask81);
			v.vertex.xyz += lerpResult80.xyz;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float temp_output_56_0 = ( 0.0 + (_ObjectLow + (_ShaderHologramDisplacement - 0.0) * (_ObjectHigh - _ObjectLow) / (1.0 - 0.0)) );
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float vpY51 = ase_vertex3Pos.y;
			float smoothstepResult73 = smoothstep( ( temp_output_56_0 - 0.5 ) , ( temp_output_56_0 + 0.5 ) , vpY51);
			float mask81 = smoothstepResult73;
			float clampResult92 = clamp( pow( ( mask81 * _Const4 ) , 2.5 ) , 0.0 , 1.0 );
			float4 lerpResult77 = lerp( _FirstColor , _SecondColor , clampResult92);
			o.Emission = lerpResult77.rgb;
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
			#pragma target 3.0
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
7;29;1906;1004;1820.68;1294.768;1.783428;True;False
Node;AmplifyShaderEditor.PosVertexDataNode;36;-1950.587,1026.272;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;38;-1731.069,1886.888;Float;False;Property;_ObjectLow;ObjectLow;2;0;Create;True;0;0;False;0;0;-0.8;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;39;-1853.917,1801.123;Float;False;Global;_ShaderHologramDisplacement;_ShaderHologramDisplacement;1;0;Create;True;0;0;False;0;0;0.6317102;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;37;-1717.367,1963.082;Float;False;Property;_ObjectHigh;ObjectHigh;1;0;Create;True;0;0;False;0;0;1.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;51;-1664.87,1066.01;Float;False;vpY;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;86;-1443.14,1700.349;Inherit;False;Constant;_Const6;Const6;12;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;42;-1462.485,1803.855;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;53;-1419.494,1389.448;Inherit;False;51;vpY;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;56;-1203.039,1780.002;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;62;-1072.002,1145.072;Float;False;Constant;_Const3;Const3;7;0;Create;True;0;0;False;0;0.5;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;55;-887.0455,1606.433;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;67;-1011.136,1028.613;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;72;-783.7194,1204.275;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;71;-780.9543,1069.769;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;9;-1763.214,146.1808;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;59;-1230.325,1897.713;Float;False;Constant;_Const1;Const1;5;0;Create;True;0;0;False;0;0.2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;27;-1704.074,500.165;Float;False;Property;_Scale;Scale;3;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;26;-1703.074,579.1648;Float;False;Property;_Power;Power;4;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;60;-793.4824,1631.33;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;57;-817.2834,1529.451;Float;False;Constant;_Const2;Const2;5;0;Create;True;0;0;False;0;0.02;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NormalVertexDataNode;58;-838.5274,1352.076;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FresnelNode;28;-1498.761,492.7057;Inherit;False;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;65;-584.5743,1450.323;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;73;-464.8781,1090.394;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;64;-955.1935,1770.035;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-1525.344,370.5782;Float;False;Constant;_Panner;Panner;5;0;Create;True;0;0;False;0;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;63;-581.8472,1631.808;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;66;-786.4113,1771.651;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;12;-1517.352,272.2877;Float;False;Property;_UV;UV;9;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;10;-1508.23,169.5674;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;61;-936.4496,1880.406;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;69;-711.1703,1833.656;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;29;-1229.864,591.6802;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;81;-203.243,1083.321;Float;False;mask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;-1292.262,167.6683;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;68;-392.0069,1525.814;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;24;-1290.464,287.4843;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;13;-1096.095,167.7198;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0.1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;83;-762.856,-294.8188;Inherit;False;81;mask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;91;-756.0549,-196.6873;Inherit;False;Property;_Const4;Const4;11;0;Create;True;0;0;False;0;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;75;-188.733,1797.288;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;30;-1039.24,592.4838;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;74;-209.2449,1504.16;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;31;-1072.379,782.1913;Float;False;Property;_DepthPower;DepthPower;7;0;Create;True;0;0;False;0;0;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;76;38.92288,1503.487;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;90;-546.0549,-243.6874;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;89;-557.1548,-130.7146;Inherit;False;Constant;_Const5;Const5;12;0;Create;True;0;0;False;0;2.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;32;-853.4387,762.7697;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;7;-858.938,139.6053;Inherit;True;Property;_Hologram;Hologram;0;0;Create;True;0;0;False;0;-1;None;1dbcddf7c0c736f4d84ec53ff8243eb7;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;25;-852.1646,489.3801;Float;False;Property;_Invert;Invert;8;0;Create;True;0;0;False;0;0;0;0;True;;Toggle;2;Key0;Key1;Create;False;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;88;-355.9948,-245.5431;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;-481.6273,144.3135;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;84;273.0179,1499.543;Float;False;vertex;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ClampOpNode;33;-558.4519,761.8586;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;21;-261.5485,265.2099;Float;False;Property;_VertexMovement;VertexMovement;10;0;Create;True;0;0;False;0;0.02;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;78;-290.1926,-436.4225;Float;False;Property;_SecondColor;SecondColor;6;1;[HDR];Create;True;0;0;False;0;0,0,0,0;2,2,2,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;92;-128.3611,-243.8913;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;4;-231.9084,-620.4058;Float;False;Property;_FirstColor;FirstColor;5;1;[HDR];Create;True;0;0;False;0;0,0,0,0;0.6901961,0.6745098,2.996078,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;-15.46295,145.5854;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;34;-271.7393,422.1565;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;82;-48.70969,673.7181;Inherit;False;81;mask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;85;-90.03085,562.231;Inherit;False;84;vertex;1;0;OBJECT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.LerpOp;77;78.10708,-288.5376;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;35;-94.73682,420.2528;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;41;-1666.1,1155.843;Float;False;vpZ;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;80;221.4242,485.0276;Inherit;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;40;-1666.077,975.3897;Float;False;vpX;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;6;509.3441,48.20477;Float;False;True;-1;2;2;0;0;Standard;SciFi/HologramDepthFadeDisplacement;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;51;0;36;2
WireConnection;42;0;39;0
WireConnection;42;3;38;0
WireConnection;42;4;37;0
WireConnection;56;0;86;0
WireConnection;56;1;42;0
WireConnection;55;0;53;0
WireConnection;67;0;53;0
WireConnection;72;0;56;0
WireConnection;72;1;62;0
WireConnection;71;0;56;0
WireConnection;71;1;62;0
WireConnection;60;0;56;0
WireConnection;60;1;55;0
WireConnection;28;2;27;0
WireConnection;28;3;26;0
WireConnection;65;0;58;2
WireConnection;65;1;57;0
WireConnection;73;0;67;0
WireConnection;73;1;71;0
WireConnection;73;2;72;0
WireConnection;64;0;56;0
WireConnection;64;1;59;0
WireConnection;63;0;60;0
WireConnection;63;2;60;0
WireConnection;66;0;53;0
WireConnection;10;0;9;1
WireConnection;10;1;9;2
WireConnection;61;0;56;0
WireConnection;61;1;59;0
WireConnection;69;0;66;0
WireConnection;69;1;64;0
WireConnection;69;2;61;0
WireConnection;29;0;28;0
WireConnection;81;0;73;0
WireConnection;11;0;10;0
WireConnection;11;1;12;0
WireConnection;68;0;65;0
WireConnection;68;1;63;0
WireConnection;24;1;16;0
WireConnection;13;0;11;0
WireConnection;13;2;24;0
WireConnection;75;0;69;0
WireConnection;30;0;29;0
WireConnection;74;1;68;0
WireConnection;76;0;74;0
WireConnection;76;1;75;0
WireConnection;90;0;83;0
WireConnection;90;1;91;0
WireConnection;32;0;31;0
WireConnection;7;1;13;0
WireConnection;25;1;28;0
WireConnection;25;0;30;0
WireConnection;88;0;90;0
WireConnection;88;1;89;0
WireConnection;18;0;7;0
WireConnection;18;1;25;0
WireConnection;84;0;76;0
WireConnection;33;0;32;0
WireConnection;92;0;88;0
WireConnection;20;0;18;0
WireConnection;20;1;21;0
WireConnection;34;0;18;0
WireConnection;34;1;33;0
WireConnection;77;0;4;0
WireConnection;77;1;78;0
WireConnection;77;2;92;0
WireConnection;35;0;34;0
WireConnection;41;0;36;3
WireConnection;80;0;20;0
WireConnection;80;1;85;0
WireConnection;80;2;82;0
WireConnection;40;0;36;1
WireConnection;6;2;77;0
WireConnection;6;9;35;0
WireConnection;6;11;80;0
ASEEND*/
//CHKSM=211EC16CC872FB904D1F52C40BAD8064B93ED496