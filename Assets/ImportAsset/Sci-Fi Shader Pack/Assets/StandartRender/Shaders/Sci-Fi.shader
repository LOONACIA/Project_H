// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SciFi/Sci-Fi"
{
	Properties
	{
		_Albedo("Albedo", 2D) = "white" {}
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_AlbedoColor("AlbedoColor", Color) = (0,0,0,0)
		_Metallic("Metallic", Range( 0 , 1)) = 0
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		_Normal("Normal", 2D) = "white" {}
		_AO("AO", 2D) = "white" {}
		_Emission("Emission", 2D) = "black" {}
		[HDR]_EmissionColor("EmissionColor", Color) = (0,0,0,0)
		[HDR]_VertexColor("VertexColor", Color) = (0,0,0,0)
		_ObjectHigh("ObjectHigh", Float) = 0
		_ObjectLow("ObjectLow", Float) = 0
		_Noises("Noises", 2D) = "white" {}
		_DissolveTiling("DissolveTiling", Float) = 1
		[HDR]_DissolveColor("DissolveColor", Color) = (0.180188,4.594794,0,0)
		_DissolvePower("DissolvePower", Range( 0 , 1)) = 0
		_DissolveColorPower("DissolveColorPower", Float) = 0.02
		_DissolveSquareScale("DissolveSquareScale", Range( 0.01 , 1)) = 0
		_DissolveSquarePower("DissolveSquarePower", Range( 0 , 10)) = 1
		[HDR]_FirstTextureColor("FirstTextureColor", Color) = (0,0.07736176,0.8773585,0)
		_FirstTextureHight("FirstTextureHight", Float) = 0
		_FirstTextureScale("FirstTextureScale", Range( 0.1 , 10)) = 0
		_FirstTextureTiling("FirstTextureTiling", Float) = 0
		[HDR]_SecondTextureColor("SecondTextureColor", Color) = (0,0.07736176,0.8773585,0)
		_SecondTextureHight("SecondTextureHight", Float) = 0
		_SecondTextureScale("SecondTextureScale", Range( 0.1 , 10)) = 0
		_SecondTextureTiling("SecondTextureTiling", Float) = 0
		_DarkAreaScale("DarkAreaScale", Float) = 0
		_DarkAreaPower("DarkAreaPower", Float) = 1
		_OpacityScale("OpacityScale", Range( 0.01 , 0.9)) = 0
		_OpacityPower("OpacityPower", Range( 0 , 10)) = 1
		_NormalPower("NormalPower", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "AlphaTest+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
		};

		uniform float _ShaderSciFi;
		uniform float _ObjectLow;
		uniform float _ObjectHigh;
		uniform float _NormalPower;
		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform float4 _AlbedoColor;
		uniform float _DarkAreaScale;
		uniform float _DarkAreaPower;
		uniform float4 _DissolveColor;
		uniform sampler2D _Noises;
		uniform float _DissolveTiling;
		uniform float _DissolvePower;
		uniform float _DissolveColorPower;
		uniform float4 _VertexColor;
		uniform float _DissolveSquareScale;
		uniform float _DissolveSquarePower;
		uniform float4 _FirstTextureColor;
		uniform float _FirstTextureTiling;
		uniform float _FirstTextureHight;
		uniform float _FirstTextureScale;
		uniform float4 _SecondTextureColor;
		uniform float _SecondTextureTiling;
		uniform float _SecondTextureHight;
		uniform float _SecondTextureScale;
		uniform sampler2D _Emission;
		uniform float4 _Emission_ST;
		uniform float4 _EmissionColor;
		uniform float _Metallic;
		uniform float _Smoothness;
		uniform sampler2D _AO;
		uniform float4 _AO_ST;
		uniform float _OpacityScale;
		uniform float _OpacityPower;
		uniform float _Cutoff = 0.5;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertexNormal = v.normal.xyz;
			float temp_output_54_0 = (_ObjectLow + (_ShaderSciFi - 0.0) * (_ObjectHigh - _ObjectLow) / (1.0 - 0.0));
			float temp_output_59_0 = ( -0.3 + temp_output_54_0 );
			float3 ase_vertex3Pos = v.vertex.xyz;
			float vpY32 = ase_vertex3Pos.y;
			float temp_output_37_0 = ( temp_output_59_0 - vpY32 );
			float clampResult62 = clamp( temp_output_37_0 , 0.0 , temp_output_37_0 );
			float4 appendResult47 = (float4(0.0 , ( ( ase_vertexNormal.y * 0.02 ) + clampResult62 ) , 0.0 , 0.0));
			float smoothstepResult65 = smoothstep( ( temp_output_59_0 - 0.2 ) , ( temp_output_59_0 + 0.2 ) , vpY32);
			v.vertex.xyz += ( appendResult47 * smoothstepResult65 ).xyz;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			o.Normal = UnpackScaleNormal( tex2D( _Normal, uv_Normal ), _NormalPower );
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float4 tex2DNode126 = tex2D( _Albedo, uv_Albedo );
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float vpY32 = ase_vertex3Pos.y;
			float temp_output_54_0 = (_ObjectLow + (_ShaderSciFi - 0.0) * (_ObjectHigh - _ObjectLow) / (1.0 - 0.0));
			float temp_output_71_0 = ( -1.0 * temp_output_54_0 );
			float temp_output_76_0 = ( vpY32 + temp_output_71_0 + 0.4 );
			float clampResult139 = clamp( pow( ( temp_output_76_0 * -1.0 * _DarkAreaScale ) , _DarkAreaPower ) , 0.0 , 1.0 );
			float4 lerpResult136 = lerp( float4( 0,0,0,0 ) , ( tex2DNode126 * _AlbedoColor ) , clampResult139);
			o.Albedo = lerpResult136.rgb;
			float2 temp_cast_1 = (_DissolveTiling).xx;
			float2 uv_TexCoord87 = i.uv_texcoord * temp_cast_1;
			float2 panner89 = ( 1.0 * _Time.y * float2( 0,-0.5 ) + uv_TexCoord87);
			float temp_output_82_0 = ( 1.0 - tex2D( _Noises, panner89 ).r );
			float temp_output_2_0 = (1.0 + (_DissolvePower - 0.0) * (-0.1 - 1.0) / (1.0 - 0.0));
			float temp_output_59_0 = ( -0.3 + temp_output_54_0 );
			float smoothstepResult46 = smoothstep( ( temp_output_59_0 - 0.5 ) , ( temp_output_59_0 + 0.5 ) , vpY32);
			float4 lerpResult128 = lerp( float4( 0,0,0,0 ) , _VertexColor , smoothstepResult46);
			float clampResult13 = clamp( step( temp_output_82_0 , ( temp_output_2_0 + 0.1 ) ) , 0.0 , 1.0 );
			float clampResult85 = clamp( pow( ( temp_output_76_0 * -1.0 * _DissolveSquareScale ) , _DissolveSquarePower ) , 0.0 , 1.0 );
			float lerpResult101 = lerp( ( 1.0 - clampResult13 ) , 1.0 , clampResult85);
			float4 lerpResult91 = lerp( ( _DissolveColor * ( 1.0 - step( temp_output_82_0 , ( temp_output_2_0 + _DissolveColorPower ) ) ) ) , lerpResult128 , lerpResult101);
			float2 temp_cast_2 = (_FirstTextureTiling).xx;
			float2 uv_TexCoord182 = i.uv_texcoord * temp_cast_2;
			float2 panner184 = ( 1.0 * _Time.y * float2( 0,-0.5 ) + uv_TexCoord182);
			float4 tex2DNode75 = tex2D( _Noises, panner184 );
			float clampResult86 = clamp( abs( ( ( vpY32 + temp_output_71_0 + _FirstTextureHight ) * -1.0 * _FirstTextureScale ) ) , 0.0 , 1.0 );
			float lerpResult27 = lerp( tex2DNode75.g , 0.0 , clampResult86);
			float4 lerpResult131 = lerp( lerpResult91 , _FirstTextureColor , lerpResult27);
			float2 temp_cast_3 = (_SecondTextureTiling).xx;
			float2 uv_TexCoord179 = i.uv_texcoord * temp_cast_3;
			float2 panner183 = ( 1.0 * _Time.y * float2( 0,-0.5 ) + uv_TexCoord179);
			float4 tex2DNode167 = tex2D( _Noises, panner183 );
			float clampResult166 = clamp( abs( ( ( vpY32 + temp_output_71_0 + _SecondTextureHight ) * -1.0 * _SecondTextureScale ) ) , 0.0 , 1.0 );
			float lerpResult168 = lerp( tex2DNode167.b , 0.0 , clampResult166);
			float4 lerpResult170 = lerp( lerpResult131 , _SecondTextureColor , lerpResult168);
			float2 uv_Emission = i.uv_texcoord * _Emission_ST.xy + _Emission_ST.zw;
			float4 tex2DNode218 = tex2D( _Emission, uv_Emission );
			float4 clampResult222 = clamp( ( tex2DNode218 / lerpResult170 ) , float4( 0,0,0,0 ) , float4( 1,0,0,0 ) );
			float grayscale221 = Luminance(clampResult222.rgb);
			float4 lerpResult220 = lerp( lerpResult170 , ( tex2DNode218 * _EmissionColor ) , grayscale221);
			o.Emission = lerpResult220.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
			float2 uv_AO = i.uv_texcoord * _AO_ST.xy + _AO_ST.zw;
			o.Occlusion = tex2D( _AO, uv_AO ).r;
			o.Alpha = 1;
			float lerpResult79 = lerp( clampResult13 , 1.0 , clampResult85);
			float lerpResult199 = lerp( tex2DNode75.g , 1.0 , clampResult86);
			float lerpResult200 = lerp( tex2DNode167.b , 1.0 , clampResult166);
			float clampResult207 = clamp( pow( ( temp_output_76_0 * -1.0 * _OpacityScale ) , _OpacityPower ) , 0.0 , 1.0 );
			float lerpResult201 = lerp( ( lerpResult79 * lerpResult199 * lerpResult200 ) , 1.0 , clampResult207);
			clip( ( tex2DNode126.a * lerpResult201 ) - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "2"
}
/*ASEBEGIN
Version=17700
189;102;1036;806;1228.223;2300.451;2.379721;True;False
Node;AmplifyShaderEditor.RangedFloatNode;57;-3179.321,106.7301;Float;False;Global;_ShaderSciFi;_ShaderSciFi;9;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;53;-3064.572,189.4937;Float;False;Property;_ObjectLow;ObjectLow;11;0;Create;True;0;0;False;0;0;-0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;52;-3065.768,271.6888;Float;False;Property;_ObjectHigh;ObjectHigh;10;0;Create;True;0;0;False;0;0;4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;90;-3114.039,-2342.804;Float;False;Property;_DissolveTiling;DissolveTiling;13;0;Create;True;0;0;False;0;1;18.25;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;29;-2909.303,823.0121;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;54;-2830.536,108.1034;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;72;-2684.178,-974.984;Float;False;Constant;_Const4;Const4;12;0;Create;True;0;0;False;0;-1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;87;-2854.703,-2360.003;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;1;-2792.722,-2101.98;Float;False;Property;_DissolvePower;DissolvePower;15;0;Create;True;0;0;False;0;0;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;71;-2492.365,-970.7411;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;32;-2623.588,862.7499;Float;False;vpY;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;89;-2571.735,-2358.736;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,-0.5;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TFHCRemapNode;2;-2446.693,-2096.773;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;1;False;4;FLOAT;-0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;135;-2259.349,-1390.222;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;124;-2500.86,497.955;Float;False;Constant;_Const8;Const8;27;0;Create;True;0;0;False;0;-0.3;-0.3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;77;-2152.69,-1541.649;Inherit;False;32;vpY;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;7;-2267.807,-1982.189;Float;False;Constant;_OpacityPowerConst;OpacityPowerConst;10;0;Create;True;0;0;False;0;0.1;0.12;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;5;-2352.462,-2387.743;Inherit;True;Property;_Noises;Noises;12;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;78;-2157.578,-1400.337;Float;False;Constant;_Const6;Const6;16;0;Create;True;0;0;False;0;0.4;0.4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;3;-2285.197,-2187.716;Float;False;Property;_DissolveColorPower;DissolveColorPower;16;0;Create;True;0;0;False;0;0.02;-0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;59;-2240.56,504.5043;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;169;-2124.522,-521.1563;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;161;-1919.356,-505.4121;Inherit;False;32;vpY;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;38;-1923.596,256.404;Float;False;Constant;_Const3;Const3;5;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;160;-1962.382,-343.8934;Float;False;Property;_SecondTextureHight;SecondTextureHight;24;0;Create;True;0;0;False;0;0;1.79;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;73;-1980.877,-874.433;Float;False;Property;_FirstTextureHight;FirstTextureHight;20;0;Create;True;0;0;False;0;0;1.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;123;-2007.609,-1342.877;Float;False;Property;_DissolveSquareScale;DissolveSquareScale;17;0;Create;True;0;0;False;0;0;1;0.01;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;76;-1920.564,-1511.635;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;9;-1963.244,-2048.934;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;74;-1962.605,-1012.7;Inherit;False;32;vpY;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;82;-2021.693,-2361.025;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;12;-1750.319,-2070.921;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;43;-1563.435,188.0176;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;121;-1692.489,-1512.4;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;4;-1973.861,-2179.033;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;181;-1768.607,-1198.311;Float;False;Property;_FirstTextureTiling;FirstTextureTiling;22;0;Create;True;0;0;False;0;0;14.13;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;180;-1905.148,-694.7529;Float;False;Property;_SecondTextureTiling;SecondTextureTiling;26;0;Create;True;0;0;False;0;0;24.33;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;163;-1660.955,-454.2181;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;18;-1670.451,-981.7579;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;151;-1699.766,-830.1476;Float;False;Property;_FirstTextureScale;FirstTextureScale;21;0;Create;True;0;0;False;0;0;1.69;0.1;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;44;-1561.198,322.523;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;107;-1702.104,-1343.205;Float;False;Property;_DissolveSquarePower;DissolveSquarePower;18;0;Create;True;0;0;False;0;1;0.84;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;162;-1690.271,-302.6078;Float;False;Property;_SecondTextureScale;SecondTextureScale;25;0;Create;True;0;0;False;0;0;2.55;0.1;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;142;-1599.708,87.17714;Inherit;False;32;vpY;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;6;-1669.651,-2207.523;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;179;-1607.818,-711.7132;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;122;-1457.957,-1512.218;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;46;-1218.142,283.7689;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;147;-1401.591,-981.7869;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;164;-1431.72,-454.247;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;13;-1533.909,-2069.516;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;182;-1508.908,-1215.927;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;129;-1235.265,93.8752;Float;False;Property;_VertexColor;VertexColor;9;1;[HDR];Create;True;0;0;False;0;0,0,0,0;6.351943,6.059227,0.9952353,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.AbsOpNode;165;-1165.751,-455.2575;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;10;-1448.613,-2205.864;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;85;-1236.578,-1512.556;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;26;-1147.334,-981.7818;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;99;-1278.452,-2068.698;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;8;-1575.142,-2422.35;Float;False;Property;_DissolveColor;DissolveColor;14;1;[HDR];Create;True;0;0;False;0;0.180188,4.594794,0,0;0.9057248,1.289886,5.486105,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;184;-1238.644,-1215.87;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,-0.5;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;183;-1327.559,-709.9534;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,-0.5;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;128;-902.9351,233.9587;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;167;-1100.969,-736.4238;Inherit;True;Property;_SecondTexture;SecondTexture;12;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Instance;5;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;101;-946.7266,-1970.933;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;-1168.177,-2226.772;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;210;-459.1187,-282.6382;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;86;-911.9623,-980.7171;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;166;-918.9913,-455.5774;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;75;-1000.334,-1240.665;Inherit;True;Property;_FirstTexture;FirstTexture;12;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Instance;5;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;168;-633.8624,-659.8631;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;27;-593.0814,-1187.504;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;91;-344.5497,-1669.205;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;132;-363.2149,-1396.56;Float;False;Property;_FirstTextureColor;FirstTextureColor;19;1;[HDR];Create;True;0;0;False;0;0,0.07736176,0.8773585,0;0.9057248,1.289886,5.486105,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;140;-1855.454,-1770.352;Float;False;Property;_DarkAreaScale;DarkAreaScale;27;0;Create;True;0;0;False;0;0;0.34;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;174;-2.21844,-999.6563;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;208;-1655.069,-91.7218;Float;False;Property;_OpacityScale;OpacityScale;29;0;Create;True;0;0;False;0;0;0.14;0.01;0.9;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;131;-69.11788,-1336.691;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;171;-181.4835,-1197.442;Float;False;Property;_SecondTextureColor;SecondTextureColor;23;1;[HDR];Create;True;0;0;False;0;0,0.07736176,0.8773585,0;0.9057248,1.289886,5.486105,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;33;-1837.311,773.9226;Inherit;False;32;vpY;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;35;-1597.163,647.6983;Float;False;Constant;_Const2;Const2;5;0;Create;True;0;0;False;0;0.02;0.02;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;137;-1567.363,-1811.357;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;205;-1287.982,-239.0935;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;204;-1337.839,-78.9095;Float;False;Property;_OpacityPower;OpacityPower;30;0;Create;True;0;0;False;0;1;1;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.NormalVertexDataNode;36;-1618.408,492.3244;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;218;-28.16073,-1597.008;Inherit;True;Property;_Emission;Emission;7;0;Create;True;0;0;False;0;-1;None;None;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;170;138.0131,-1215.853;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;141;-1597.59,-1661.058;Float;False;Property;_DarkAreaPower;DarkAreaPower;28;0;Create;True;0;0;False;0;1;3.24;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;37;-1575.962,749.5783;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;60;-2012.805,1015.961;Float;False;Constant;_Const1;Const1;5;0;Create;True;0;0;False;0;0.2;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;63;-1737.674,888.2828;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;138;-1370.794,-1811.176;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;200;-636.1179,-504.6269;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;79;-952.0895,-1695.641;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;199;-587.0931,-1028.132;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;206;-1053.45,-238.9115;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;64;-1558.403,929.7582;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;62;-1364.327,750.0562;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;227;306.3863,-1362.585;Inherit;False;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;126;-882.5004,-2435.056;Inherit;True;Property;_Albedo;Albedo;0;0;Create;True;0;0;False;0;-1;None;6366c4dd23347414ab38af3b7195d973;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;125;-841.5622,-2245.002;Float;False;Property;_AlbedoColor;AlbedoColor;2;0;Create;True;0;0;False;0;0,0,0,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;61;-1718.928,998.6542;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;-1361.053,583.57;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;41;-1174.487,644.0614;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;198;-152.7149,-803.7125;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;207;-832.0707,-239.2495;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;222;463.577,-1318.638;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;127;-501.2606,-2199.842;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;224;137.7501,-1782.019;Inherit;False;Property;_EmissionColor;EmissionColor;8;1;[HDR];Create;True;0;0;False;0;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;214;-306.9906,-1999.957;Inherit;False;Property;_NormalPower;NormalPower;31;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;139;-1162.599,-1811.832;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;65;-1493.65,951.9038;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;175;-91.24333,-2005.258;Inherit;True;Property;_Normal;Normal;5;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;201;241.4608,-704.0665;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;47;-991.725,622.4083;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.LerpOp;136;-293.4542,-2148.688;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCGrayscale;221;620.1107,-1231.787;Inherit;False;0;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;66;-971.213,915.5359;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;223;453.8795,-1480.208;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;211;218.3422,-1094.12;Inherit;False;Property;_Metallic;Metallic;3;0;Create;True;0;0;False;0;0;0.668;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;176;209.9737,-932.2772;Inherit;True;Property;_AO;AO;6;0;Create;True;0;0;False;0;-1;None;8991419b74cad054798614e5bdf5d34f;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;228;711.3327,-808.7163;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;220;829.8907,-1153.826;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;31;-2624.818,952.5828;Float;False;vpZ;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;216;551.1693,-1732.495;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;30;-2624.795,772.1302;Float;False;vpX;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;215;519.5323,-1867.988;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;48;-743.5569,621.7353;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;212;223.3008,-1010.384;Inherit;False;Property;_Smoothness;Smoothness;4;0;Create;True;0;0;False;0;0;0.691;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1102.589,-1124.183;Float;False;True;-1;2;2;0;0;Standard;SciFi/Sci-Fi;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;Opaque;;AlphaTest;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.CommentaryNode;213;2569.158,-760.936;Inherit;False;100;100;Comment;0;;1,1,1,1;0;0
WireConnection;54;0;57;0
WireConnection;54;3;53;0
WireConnection;54;4;52;0
WireConnection;87;0;90;0
WireConnection;71;0;72;0
WireConnection;71;1;54;0
WireConnection;32;0;29;2
WireConnection;89;0;87;0
WireConnection;2;0;1;0
WireConnection;135;0;71;0
WireConnection;5;1;89;0
WireConnection;59;0;124;0
WireConnection;59;1;54;0
WireConnection;169;0;71;0
WireConnection;76;0;77;0
WireConnection;76;1;135;0
WireConnection;76;2;78;0
WireConnection;9;0;2;0
WireConnection;9;1;7;0
WireConnection;82;0;5;1
WireConnection;12;0;82;0
WireConnection;12;1;9;0
WireConnection;43;0;59;0
WireConnection;43;1;38;0
WireConnection;121;0;76;0
WireConnection;121;2;123;0
WireConnection;4;0;2;0
WireConnection;4;1;3;0
WireConnection;163;0;161;0
WireConnection;163;1;169;0
WireConnection;163;2;160;0
WireConnection;18;0;74;0
WireConnection;18;1;71;0
WireConnection;18;2;73;0
WireConnection;44;0;59;0
WireConnection;44;1;38;0
WireConnection;6;0;82;0
WireConnection;6;1;4;0
WireConnection;179;0;180;0
WireConnection;122;0;121;0
WireConnection;122;1;107;0
WireConnection;46;0;142;0
WireConnection;46;1;43;0
WireConnection;46;2;44;0
WireConnection;147;0;18;0
WireConnection;147;2;151;0
WireConnection;164;0;163;0
WireConnection;164;2;162;0
WireConnection;13;0;12;0
WireConnection;182;0;181;0
WireConnection;165;0;164;0
WireConnection;10;0;6;0
WireConnection;85;0;122;0
WireConnection;26;0;147;0
WireConnection;99;0;13;0
WireConnection;184;0;182;0
WireConnection;183;0;179;0
WireConnection;128;1;129;0
WireConnection;128;2;46;0
WireConnection;167;1;183;0
WireConnection;101;0;99;0
WireConnection;101;2;85;0
WireConnection;11;0;8;0
WireConnection;11;1;10;0
WireConnection;210;0;128;0
WireConnection;86;0;26;0
WireConnection;166;0;165;0
WireConnection;75;1;184;0
WireConnection;168;0;167;3
WireConnection;168;2;166;0
WireConnection;27;0;75;2
WireConnection;27;2;86;0
WireConnection;91;0;11;0
WireConnection;91;1;210;0
WireConnection;91;2;101;0
WireConnection;174;0;168;0
WireConnection;131;0;91;0
WireConnection;131;1;132;0
WireConnection;131;2;27;0
WireConnection;137;0;76;0
WireConnection;137;2;140;0
WireConnection;205;0;76;0
WireConnection;205;2;208;0
WireConnection;170;0;131;0
WireConnection;170;1;171;0
WireConnection;170;2;174;0
WireConnection;37;0;59;0
WireConnection;37;1;33;0
WireConnection;63;0;59;0
WireConnection;63;1;60;0
WireConnection;138;0;137;0
WireConnection;138;1;141;0
WireConnection;200;0;167;3
WireConnection;200;2;166;0
WireConnection;79;0;13;0
WireConnection;79;2;85;0
WireConnection;199;0;75;2
WireConnection;199;2;86;0
WireConnection;206;0;205;0
WireConnection;206;1;204;0
WireConnection;64;0;33;0
WireConnection;62;0;37;0
WireConnection;62;2;37;0
WireConnection;227;0;218;0
WireConnection;227;1;170;0
WireConnection;61;0;59;0
WireConnection;61;1;60;0
WireConnection;39;0;36;2
WireConnection;39;1;35;0
WireConnection;41;0;39;0
WireConnection;41;1;62;0
WireConnection;198;0;79;0
WireConnection;198;1;199;0
WireConnection;198;2;200;0
WireConnection;207;0;206;0
WireConnection;222;0;227;0
WireConnection;127;0;126;0
WireConnection;127;1;125;0
WireConnection;139;0;138;0
WireConnection;65;0;64;0
WireConnection;65;1;63;0
WireConnection;65;2;61;0
WireConnection;175;5;214;0
WireConnection;201;0;198;0
WireConnection;201;2;207;0
WireConnection;47;1;41;0
WireConnection;136;1;127;0
WireConnection;136;2;139;0
WireConnection;221;0;222;0
WireConnection;66;0;65;0
WireConnection;223;0;218;0
WireConnection;223;1;224;0
WireConnection;228;0;126;4
WireConnection;228;1;201;0
WireConnection;220;0;170;0
WireConnection;220;1;223;0
WireConnection;220;2;221;0
WireConnection;31;0;29;3
WireConnection;216;0;175;0
WireConnection;30;0;29;1
WireConnection;215;0;136;0
WireConnection;48;0;47;0
WireConnection;48;1;66;0
WireConnection;0;0;215;0
WireConnection;0;1;216;0
WireConnection;0;2;220;0
WireConnection;0;3;211;0
WireConnection;0;4;212;0
WireConnection;0;5;176;0
WireConnection;0;10;228;0
WireConnection;0;11;48;0
ASEEND*/
//CHKSM=A28E0B77A94D19E2E2A27FF8F8BC5570EC171729