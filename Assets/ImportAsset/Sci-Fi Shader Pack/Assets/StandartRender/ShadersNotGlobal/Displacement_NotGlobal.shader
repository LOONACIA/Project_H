// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SciFi/NotGlobal/Displacement 1"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_Albedo("Albedo", 2D) = "white" {}
		_AlbedoColor("AlbedoColor", Color) = (0,0,0,0)
		_Metallic("Metallic", Range( 0 , 1)) = 0
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		_Normal("Normal", 2D) = "white" {}
		_AO("AO", 2D) = "white" {}
		_Emission1("Emission", 2D) = "black" {}
		_ShaderDisplacement("_ShaderDisplacement", Range( 0 , 1)) = 0
		[HDR]_DispEmissionColor("DispEmissionColor", Color) = (0,0,0,0)
		[HDR]_EmissionColor1("EmissionColor", Color) = (0,0,0,0)
		_ObjectHigh("ObjectHigh", Float) = 0
		_ObjectLow("ObjectLow", Float) = 0
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

		uniform float _ShaderDisplacement;
		uniform float _ObjectLow;
		uniform float _ObjectHigh;
		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform float _NormalPower;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform float4 _AlbedoColor;
		uniform float4 _DispEmissionColor;
		uniform sampler2D _Emission1;
		uniform float4 _Emission1_ST;
		uniform float4 _EmissionColor1;
		uniform float _Metallic;
		uniform float _Smoothness;
		uniform sampler2D _AO;
		uniform float4 _AO_ST;
		uniform float _Cutoff = 0.5;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertexNormal = v.normal.xyz;
			float temp_output_18_0 = ( 0.0 + (_ObjectLow + (_ShaderDisplacement - 0.0) * (_ObjectHigh - _ObjectLow) / (1.0 - 0.0)) );
			float3 ase_vertex3Pos = v.vertex.xyz;
			float vpY58 = ase_vertex3Pos.y;
			float temp_output_34_0 = ( temp_output_18_0 - vpY58 );
			float clampResult52 = clamp( temp_output_34_0 , 0.0 , temp_output_34_0 );
			float4 appendResult40 = (float4(0.0 , ( ( ase_vertexNormal.y * 0.02 ) + clampResult52 ) , 0.0 , 0.0));
			float smoothstepResult35 = smoothstep( ( temp_output_18_0 - 0.2 ) , ( temp_output_18_0 + 0.2 ) , vpY58);
			v.vertex.xyz += ( appendResult40 * smoothstepResult35 ).xyz;
			v.vertex.w = 1;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			o.Normal = UnpackScaleNormal( tex2D( _Normal, uv_Normal ), _NormalPower );
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float4 tex2DNode73 = tex2D( _Albedo, uv_Albedo );
			float temp_output_18_0 = ( 0.0 + (_ObjectLow + (_ShaderDisplacement - 0.0) * (_ObjectHigh - _ObjectLow) / (1.0 - 0.0)) );
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float vpY58 = ase_vertex3Pos.y;
			float smoothstepResult24 = smoothstep( ( temp_output_18_0 - 0.5 ) , ( temp_output_18_0 + 0.5 ) , vpY58);
			float4 lerpResult74 = lerp( ( tex2DNode73 * _AlbedoColor ) , float4( 0,0,0,0 ) , smoothstepResult24);
			o.Albedo = lerpResult74.rgb;
			float4 lerpResult68 = lerp( float4( 0,0,0,0 ) , _DispEmissionColor , smoothstepResult24);
			float2 uv_Emission1 = i.uv_texcoord * _Emission1_ST.xy + _Emission1_ST.zw;
			float4 tex2DNode100 = tex2D( _Emission1, uv_Emission1 );
			float4 clampResult104 = clamp( ( tex2DNode100 / lerpResult68 ) , float4( 0,0,0,0 ) , float4( 1,0,0,0 ) );
			float grayscale105 = Luminance(clampResult104.rgb);
			float4 lerpResult108 = lerp( lerpResult68 , ( tex2DNode100 * _EmissionColor1 ) , grayscale105);
			o.Emission = lerpResult108.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
			float2 uv_AO = i.uv_texcoord * _AO_ST.xy + _AO_ST.zw;
			o.Occlusion = tex2D( _AO, uv_AO ).r;
			o.Alpha = 1;
			clip( tex2DNode73.a - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "2"
}
/*ASEBEGIN
Version=18912
-1920;0;1920;1019;3460.577;304.3175;1;True;False
Node;AmplifyShaderEditor.RangedFloatNode;92;-2639.729,416.7958;Float;False;Property;_ObjectHigh;ObjectHigh;11;0;Create;True;0;0;0;False;0;False;0;1.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;67;-2776.281,254.8373;Float;False;Property;_ShaderDisplacement;_ShaderDisplacement;8;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;56;-2784.11,-541.8356;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;93;-2662.532,327.601;Float;False;Property;_ObjectLow;ObjectLow;12;0;Create;True;0;0;0;False;0;False;0;-0.8;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;91;-2384.848,257.5692;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;58;-2498.395,-502.0976;Float;False;vpY;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;94;-2375.221,153.7478;Inherit;False;Constant;_Const;Const;12;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;18;-2116.228,235.2097;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;62;-2332.683,-155.3445;Inherit;False;58;vpY;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;17;-1985.19,-399.7205;Float;False;Constant;_Const3;Const3;5;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;21;-1696.908,-340.5179;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;79;-1924.324,-516.1799;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;19;-1694.144,-475.0235;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;24;-1378.067,-454.3983;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;99;-1930.023,-13.59978;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;69;-1424.542,-666.7405;Float;False;Property;_DispEmissionColor;DispEmissionColor;9;1;[HDR];Create;True;0;0;0;False;0;False;0,0,0,0;2.198293,1.729805,7.387708,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;100;-1011.079,-712.525;Inherit;True;Property;_Emission1;Emission;7;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;29;-1727.497,-5.516846;Float;False;Constant;_Const2;Const2;5;0;Create;True;0;0;0;False;0;False;0.02;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;68;-1100.938,-495.6503;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.NormalVertexDataNode;23;-1747.254,-178.4279;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;34;-1703.696,96.36316;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;25;-2143.513,352.9199;Float;False;Constant;_Const1;Const1;5;0;Create;True;0;0;0;False;0;False;0.2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;102;-726.0995,-321.4065;Inherit;False;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;73;-1283.234,-1413.708;Inherit;True;Property;_Albedo;Albedo;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;31;-1849.638,335.6133;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;63;-1699.601,226.8587;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;36;-1489.425,-84.64515;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;90;-1191.917,-1195.775;Float;False;Property;_AlbedoColor;AlbedoColor;2;0;Create;True;0;0;0;False;0;False;0,0,0,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;52;-1486.699,96.84103;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;32;-1868.383,225.2419;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;104;-546.524,-287.053;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;109;-976.4257,-969.1727;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;103;-845.1683,-897.5359;Inherit;False;Property;_EmissionColor1;EmissionColor;10;1;[HDR];Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SmoothstepOpNode;35;-1624.359,288.8629;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;89;-869.215,-1214.758;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;38;-1296.858,-9.153788;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;96;-532.0192,-713.5339;Inherit;False;Property;_NormalPower;NormalPower;13;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;40;-1114.096,-30.80689;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;106;-529.0396,-595.7252;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCGrayscale;105;-362.8084,-276.9507;Inherit;False;0;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;66;-1101.922,252.495;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;74;-603.1743,-1139.006;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;77;-56.28937,-117.6194;Float;False;Property;_Smoothness;Smoothness;4;0;Create;True;0;0;0;False;0;False;0;0.803;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;57;-2499.602,-592.7177;Float;False;vpX;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;42;-865.928,-31.47985;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;59;-2499.625,-412.2646;Float;False;vpZ;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;76;-259.4026,-606.8073;Inherit;True;Property;_Normal;Normal;5;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;110;60.84052,-652.5882;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;95;-37.30445,-201.5494;Inherit;False;Property;_Metallic;Metallic;3;0;Create;True;0;0;0;False;0;False;0;0.647;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;78;-72.2245,98.2066;Inherit;True;Property;_AO;AO;6;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;108;-116.2529,-349.2897;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;370.7261,-292.7559;Float;False;True;-1;2;2;0;0;Standard;SciFi/NotGlobal/Displacement 1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;Opaque;;AlphaTest;All;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;91;0;67;0
WireConnection;91;3;93;0
WireConnection;91;4;92;0
WireConnection;58;0;56;2
WireConnection;18;0;94;0
WireConnection;18;1;91;0
WireConnection;21;0;18;0
WireConnection;21;1;17;0
WireConnection;79;0;62;0
WireConnection;19;0;18;0
WireConnection;19;1;17;0
WireConnection;24;0;79;0
WireConnection;24;1;19;0
WireConnection;24;2;21;0
WireConnection;99;0;62;0
WireConnection;68;1;69;0
WireConnection;68;2;24;0
WireConnection;34;0;18;0
WireConnection;34;1;99;0
WireConnection;102;0;100;0
WireConnection;102;1;68;0
WireConnection;31;0;18;0
WireConnection;31;1;25;0
WireConnection;63;0;62;0
WireConnection;36;0;23;2
WireConnection;36;1;29;0
WireConnection;52;0;34;0
WireConnection;52;2;34;0
WireConnection;32;0;18;0
WireConnection;32;1;25;0
WireConnection;104;0;102;0
WireConnection;109;0;24;0
WireConnection;35;0;63;0
WireConnection;35;1;32;0
WireConnection;35;2;31;0
WireConnection;89;0;73;0
WireConnection;89;1;90;0
WireConnection;38;0;36;0
WireConnection;38;1;52;0
WireConnection;40;1;38;0
WireConnection;106;0;100;0
WireConnection;106;1;103;0
WireConnection;105;0;104;0
WireConnection;66;0;35;0
WireConnection;74;0;89;0
WireConnection;74;2;109;0
WireConnection;57;0;56;1
WireConnection;42;0;40;0
WireConnection;42;1;66;0
WireConnection;59;0;56;3
WireConnection;76;5;96;0
WireConnection;110;0;74;0
WireConnection;108;0;68;0
WireConnection;108;1;106;0
WireConnection;108;2;105;0
WireConnection;0;0;110;0
WireConnection;0;1;76;0
WireConnection;0;2;108;0
WireConnection;0;3;95;0
WireConnection;0;4;77;0
WireConnection;0;5;78;0
WireConnection;0;10;73;4
WireConnection;0;11;42;0
ASEEND*/
//CHKSM=0C78334781B1B5DABA08D98941A59BC100FF426A