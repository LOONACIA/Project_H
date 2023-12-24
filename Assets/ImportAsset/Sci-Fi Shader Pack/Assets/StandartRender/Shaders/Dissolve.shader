// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SciFi/Dissolve"
{
	Properties
	{
		_DissolveNoise("DissolveNoise", 2D) = "white" {}
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_Albedo("Albedo", 2D) = "white" {}
		_Normal("Normal", 2D) = "white" {}
		_AO("AO", 2D) = "white" {}
		_Emission("Emission", 2D) = "black" {}
		[HDR]_EmissionColor("EmissionColor", Color) = (0,0,0,0)
		_DissolvePower("DissolvePower", Float) = 0.01
		_OpacityPower("OpacityPower", Float) = 0.01
		[HDR]_DissolveColor("DissolveColor", Color) = (0.180188,4.594794,0,0)
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		_Metallic("Metallic", Range( 0 , 1)) = 0
		_NoisePower("NoisePower", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "AlphaTest+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform float _NoisePower;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform sampler2D _Emission;
		uniform float4 _Emission_ST;
		uniform float4 _EmissionColor;
		uniform float4 _DissolveColor;
		uniform sampler2D _DissolveNoise;
		uniform float4 _DissolveNoise_ST;
		uniform float _ShaderDissolve;
		uniform float _DissolvePower;
		uniform float _Metallic;
		uniform float _Smoothness;
		uniform sampler2D _AO;
		uniform float4 _AO_ST;
		uniform float _OpacityPower;
		uniform float _Cutoff = 0.5;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			o.Normal = UnpackScaleNormal( tex2D( _Normal, uv_Normal ), _NoisePower );
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float4 tex2DNode142 = tex2D( _Albedo, uv_Albedo );
			o.Albedo = tex2DNode142.rgb;
			float2 uv_Emission = i.uv_texcoord * _Emission_ST.xy + _Emission_ST.zw;
			float2 uv_DissolveNoise = i.uv_texcoord * _DissolveNoise_ST.xy + _DissolveNoise_ST.zw;
			float4 tex2DNode104 = tex2D( _DissolveNoise, uv_DissolveNoise );
			float temp_output_109_0 = (1.0 + (_ShaderDissolve - 0.0) * (-0.1 - 1.0) / (1.0 - 0.0));
			float4 temp_output_106_0 = ( _DissolveColor * ( 1.0 - step( tex2DNode104.r , ( temp_output_109_0 + _DissolvePower ) ) ) );
			float4 lerpResult152 = lerp( ( tex2D( _Emission, uv_Emission ) * _EmissionColor ) , temp_output_106_0 , temp_output_106_0);
			o.Emission = lerpResult152.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
			float2 uv_AO = i.uv_texcoord * _AO_ST.xy + _AO_ST.zw;
			o.Occlusion = tex2D( _AO, uv_AO ).r;
			o.Alpha = 1;
			float clampResult136 = clamp( step( tex2DNode104.r , ( temp_output_109_0 + _OpacityPower ) ) , 0.0 , 1.0 );
			clip( ( tex2DNode142.a * clampResult136 ) - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "2"
}
/*ASEBEGIN
Version=18912
-1920;0;1920;1019;2248.405;539.8333;2.339243;True;False
Node;AmplifyShaderEditor.RangedFloatNode;138;-997.228,1721.651;Float;False;Global;_ShaderDissolve;_ShaderDissolve;8;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;109;-679.1983,1727.858;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;1;False;4;FLOAT;-0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;111;-752.9096,1578.244;Float;False;Property;_DissolvePower;DissolvePower;7;0;Create;True;0;0;0;False;0;False;0.01;0.01;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;127;-297.1302,1726.301;Float;False;Property;_OpacityPower;OpacityPower;8;0;Create;True;0;0;0;False;0;False;0.01;0.017;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;110;-448.6479,1559.975;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;104;-618.8198,1330.742;Inherit;True;Property;_DissolveNoise;DissolveNoise;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;124;-50.62356,1553.076;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;105;-160.5787,1254.354;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;108;-10.71651,958.8894;Float;False;Property;_DissolveColor;DissolveColor;9;1;[HDR];Create;True;0;0;0;False;0;False;0.180188,4.594794,0,0;0.863746,1.923077,2.447281,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;154;-4.198193,666.8651;Inherit;False;Property;_EmissionColor;EmissionColor;6;1;[HDR];Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;115;49.06909,1149.05;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;147;-33.56557,463.9685;Inherit;True;Property;_Emission;Emission;5;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StepOpNode;125;249.5306,1364.512;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;136;529.7186,1254.805;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;142;239.0888,40.53311;Inherit;True;Property;_Albedo;Albedo;2;0;Create;True;0;0;0;False;0;False;-1;None;561620b61e9d0ab43a44bd7ccc917433;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;106;355.3208,1019.742;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;150;21.58752,314.1851;Inherit;False;Property;_NoisePower;NoisePower;12;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;153;448.6017,622.065;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;143;238.3805,275.4903;Inherit;True;Property;_Normal;Normal;3;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;152;765.4016,634.8651;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;155;862.6966,1121.646;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;144;235.8472,762.8466;Inherit;True;Property;_AO;AO;4;0;Create;True;0;0;0;False;0;False;-1;None;d502ab90ede97974f8266a141b0c2c96;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;149;667.1396,877.8798;Inherit;False;Property;_Metallic;Metallic;11;0;Create;True;0;0;0;False;0;False;0;0.758;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;148;657.1156,969.5403;Float;False;Property;_Smoothness;Smoothness;10;0;Create;True;0;0;0;False;0;False;0;0.656;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1106.168,669.2069;Float;False;True;-1;2;2;0;0;Standard;SciFi/Dissolve;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;Opaque;;AlphaTest;All;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;109;0;138;0
WireConnection;110;0;109;0
WireConnection;110;1;111;0
WireConnection;124;0;109;0
WireConnection;124;1;127;0
WireConnection;105;0;104;1
WireConnection;105;1;110;0
WireConnection;115;0;105;0
WireConnection;125;0;104;1
WireConnection;125;1;124;0
WireConnection;136;0;125;0
WireConnection;106;0;108;0
WireConnection;106;1;115;0
WireConnection;153;0;147;0
WireConnection;153;1;154;0
WireConnection;143;5;150;0
WireConnection;152;0;153;0
WireConnection;152;1;106;0
WireConnection;152;2;106;0
WireConnection;155;0;142;4
WireConnection;155;1;136;0
WireConnection;0;0;142;0
WireConnection;0;1;143;0
WireConnection;0;2;152;0
WireConnection;0;3;149;0
WireConnection;0;4;148;0
WireConnection;0;5;144;0
WireConnection;0;10;155;0
ASEEND*/
//CHKSM=8DD7EFD72A2C0C6B8D39C222AB3D308A8C849E06