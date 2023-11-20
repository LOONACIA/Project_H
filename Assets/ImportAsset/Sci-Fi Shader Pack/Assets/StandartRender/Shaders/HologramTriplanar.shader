// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SciFi/HologramTriplanar"
{
	Properties
	{
		_Texture("Texture", 2D) = "white" {}
		_Noise("Noise", 2D) = "white" {}
		[HDR]_EmissionColor("EmissionColor", Color) = (0,0,0,0)
		_Scale("Scale", Float) = 1
		_Power("Power", Float) = 1
		_NoisePower("NoisePower", Float) = 0
		[Toggle(_INVERT_ON)] _Invert("Invert", Float) = 0
		_NoiseTiling("NoiseTiling", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma shader_feature _INVERT_ON
		struct Input
		{
			float3 worldPos;
			float2 uv_texcoord;
			float3 worldNormal;
		};

		uniform float4 _EmissionColor;
		uniform sampler2D _Texture;
		uniform float _NoiseTiling;
		uniform sampler2D _Noise;
		uniform float4 _Noise_ST;
		uniform float _NoisePower;
		uniform float _Scale;
		uniform float _Power;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Emission = _EmissionColor.rgb;
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float3 break94 = ( ase_vertex3Pos * _NoiseTiling );
			float2 appendResult38 = (float2(break94.y , break94.z));
			float2 panner83 = ( 1.0 * _Time.y * float2( 0.03,0.05 ) + appendResult38);
			float2 uv_Noise = i.uv_texcoord * _Noise_ST.xy + _Noise_ST.zw;
			float4 temp_output_87_0 = ( tex2D( _Noise, uv_Noise ) * _NoisePower );
			float3 ase_worldNormal = i.worldNormal;
			float3 ase_vertexNormal = mul( unity_WorldToObject, float4( ase_worldNormal, 0 ) );
			float3 break77 = abs( ase_vertexNormal );
			float4 lerpResult78 = lerp( float4( 0,0,0,0 ) , tex2D( _Texture, ( float4( panner83, 0.0 , 0.0 ) + temp_output_87_0 ).rg ) , break77.x);
			float2 appendResult35 = (float2(break94.x , break94.z));
			float2 panner84 = ( 1.0 * _Time.y * float2( 0.05,0.07 ) + appendResult35);
			float4 lerpResult79 = lerp( lerpResult78 , tex2D( _Texture, ( float4( panner84, 0.0 , 0.0 ) + temp_output_87_0 ).rg ) , break77.y);
			float2 appendResult39 = (float2(break94.x , break94.y));
			float2 panner82 = ( 1.0 * _Time.y * float2( 0.05,0.06 ) + appendResult39);
			float4 lerpResult80 = lerp( lerpResult79 , tex2D( _Texture, ( float4( panner82, 0.0 , 0.0 ) + temp_output_87_0 ).rg ) , break77.z);
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float fresnelNdotV28 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode28 = ( 0.0 + _Scale * pow( 1.0 - fresnelNdotV28, _Power ) );
			float clampResult30 = clamp( ( 1.0 - fresnelNode28 ) , 0.0 , 1.0 );
			#ifdef _INVERT_ON
				float staticSwitch25 = clampResult30;
			#else
				float staticSwitch25 = fresnelNode28;
			#endif
			float4 clampResult95 = clamp( ( lerpResult80 * staticSwitch25 ) , float4( 0,0,0,0 ) , float4( 1,0,0,0 ) );
			o.Alpha = clampResult95.r;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard alpha:fade keepalpha fullforwardshadows 

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
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
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
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.worldNormal = worldNormal;
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
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
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = IN.worldNormal;
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
7;29;1906;1004;1505.885;367.9652;1;True;False
Node;AmplifyShaderEditor.RangedFloatNode;93;-3267.732,-432.7842;Inherit;False;Property;_NoiseTiling;NoiseTiling;7;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;33;-3298.902,-591.6392;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;92;-3011.33,-523.615;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.BreakToComponentsNode;94;-2813.378,-523.4351;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SamplerNode;85;-2584.025,-1032.718;Inherit;True;Property;_Noise;Noise;1;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;88;-2464.686,-827.5562;Float;False;Property;_NoisePower;NoisePower;5;0;Create;True;0;0;False;0;0;0.26;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;38;-2475.115,-486.9178;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.NormalVertexDataNode;41;-2089.573,-48.11854;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;83;-2267.62,-488.2062;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.03,0.05;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;35;-2480.65,-714.1115;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;87;-2223.508,-844.0541;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;27;-1710.176,170.6764;Float;False;Property;_Scale;Scale;3;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;26;-1709.176,249.6761;Float;False;Property;_Power;Power;4;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;89;-1953.88,-492.2321;Inherit;False;2;2;0;FLOAT2;0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;39;-2467.712,-265.8909;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;84;-2263.62,-714.2063;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.05,0.07;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.AbsOpNode;57;-1832.497,-48.29017;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FresnelNode;28;-1504.863,163.217;Inherit;False;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;77;-1649.833,-48.2765;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SamplerNode;37;-1736.927,-518.5258;Inherit;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Instance;32;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;90;-1958.453,-713.4876;Inherit;False;2;2;0;FLOAT2;0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.PannerNode;82;-2259.933,-266.007;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.05,0.06;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.OneMinusNode;29;-1235.966,262.1915;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;91;-1127.732,-657.9389;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;32;-1739.938,-742.5193;Inherit;True;Property;_Texture;Texture;0;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;86;-1951.154,-267.0806;Inherit;False;2;2;0;FLOAT2;0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;78;-1356.094,-537.9245;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;30;-1045.342,262.9952;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;40;-1735.481,-295.8668;Inherit;True;Property;_TextureSample1;Texture Sample 1;0;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Instance;32;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;79;-1085.457,-756.4067;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;25;-858.2663,159.8914;Float;False;Property;_Invert;Invert;6;0;Create;True;0;0;False;0;0;0;0;True;;Toggle;2;Key0;Key1;Create;False;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;80;-904.788,-314.5281;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;-481.6273,144.3135;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;4;-298.9152,-20.50562;Float;False;Property;_EmissionColor;EmissionColor;2;1;[HDR];Create;True;0;0;False;0;0,0,0,0;0.05957331,0.003921588,0.9058824,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;95;-246.7176,145.9409;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;6;-6.7675,-62.12812;Float;False;True;-1;2;2;0;0;Standard;SciFi/HologramTriplanar;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;92;0;33;0
WireConnection;92;1;93;0
WireConnection;94;0;92;0
WireConnection;38;0;94;1
WireConnection;38;1;94;2
WireConnection;83;0;38;0
WireConnection;35;0;94;0
WireConnection;35;1;94;2
WireConnection;87;0;85;0
WireConnection;87;1;88;0
WireConnection;89;0;83;0
WireConnection;89;1;87;0
WireConnection;39;0;94;0
WireConnection;39;1;94;1
WireConnection;84;0;35;0
WireConnection;57;0;41;0
WireConnection;28;2;27;0
WireConnection;28;3;26;0
WireConnection;77;0;57;0
WireConnection;37;1;89;0
WireConnection;90;0;84;0
WireConnection;90;1;87;0
WireConnection;82;0;39;0
WireConnection;29;0;28;0
WireConnection;91;0;77;1
WireConnection;32;1;90;0
WireConnection;86;0;82;0
WireConnection;86;1;87;0
WireConnection;78;1;37;0
WireConnection;78;2;77;0
WireConnection;30;0;29;0
WireConnection;40;1;86;0
WireConnection;79;0;78;0
WireConnection;79;1;32;0
WireConnection;79;2;91;0
WireConnection;25;1;28;0
WireConnection;25;0;30;0
WireConnection;80;0;79;0
WireConnection;80;1;40;0
WireConnection;80;2;77;2
WireConnection;18;0;80;0
WireConnection;18;1;25;0
WireConnection;95;0;18;0
WireConnection;6;2;4;0
WireConnection;6;9;95;0
ASEEND*/
//CHKSM=E70B6EEFB992DE8C91BAD1FD8C70DAB24D1157E5