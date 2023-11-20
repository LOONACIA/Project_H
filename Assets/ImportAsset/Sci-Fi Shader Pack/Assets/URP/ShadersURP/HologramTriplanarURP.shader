// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SciFI_URP/HologramTriplanarURP"
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

	}

	SubShader
	{
		LOD 0

		
		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent" }
		
		Cull Back
		HLSLINCLUDE
		#pragma target 3.0
		ENDHLSL

		
		Pass
		{
			Name "Forward"
			Tags { "LightMode"="UniversalForward" }
			
			Blend SrcAlpha OneMinusSrcAlpha , One OneMinusSrcAlpha
			ZWrite Off
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA
			

			HLSLPROGRAM
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define _EMISSION
			#define ASE_SRP_VERSION 999999

			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
			#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
			#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
			#pragma multi_compile _ _SHADOWS_SOFT
			#pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE
			
			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma multi_compile _ LIGHTMAP_ON

			#pragma vertex vert
			#pragma fragment frag


			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			
			#define ASE_NEEDS_FRAG_WORLD_VIEW_DIR
			#define ASE_NEEDS_FRAG_WORLD_NORMAL
			#pragma shader_feature _INVERT_ON


			sampler2D _Texture;
			sampler2D _Noise;
			CBUFFER_START( UnityPerMaterial )
			float4 _EmissionColor;
			float _NoiseTiling;
			float4 _Noise_ST;
			float _NoisePower;
			float _Scale;
			float _Power;
			CBUFFER_END


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_tangent : TANGENT;
				float4 texcoord1 : TEXCOORD1;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float4 lightmapUVOrVertexSH : TEXCOORD0;
				half4 fogFactorAndVertexLight : TEXCOORD1;
				float4 shadowCoord : TEXCOORD2;
				float4 tSpace0 : TEXCOORD3;
				float4 tSpace1 : TEXCOORD4;
				float4 tSpace2 : TEXCOORD5;
				float4 ase_texcoord7 : TEXCOORD7;
				float4 ase_texcoord8 : TEXCOORD8;
				float3 ase_normal : NORMAL;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			
			VertexOutput vert ( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				o.ase_texcoord7 = v.vertex;
				o.ase_texcoord8.xy = v.ase_texcoord.xy;
				o.ase_normal = v.ase_normal;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord8.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif
				v.ase_normal = v.ase_normal;

				float3 lwWNormal = TransformObjectToWorldNormal(v.ase_normal);
				float3 lwWorldPos = TransformObjectToWorld(v.vertex.xyz);
				float3 lwWTangent = TransformObjectToWorldDir(v.ase_tangent.xyz);
				float3 lwWBinormal = normalize(cross(lwWNormal, lwWTangent) * v.ase_tangent.w);
				o.tSpace0 = float4(lwWTangent.x, lwWBinormal.x, lwWNormal.x, lwWorldPos.x);
				o.tSpace1 = float4(lwWTangent.y, lwWBinormal.y, lwWNormal.y, lwWorldPos.y);
				o.tSpace2 = float4(lwWTangent.z, lwWBinormal.z, lwWNormal.z, lwWorldPos.z);

				VertexPositionInputs vertexInput = GetVertexPositionInputs(v.vertex.xyz);
				
				OUTPUT_LIGHTMAP_UV( v.texcoord1, unity_LightmapST, o.lightmapUVOrVertexSH.xy );
				OUTPUT_SH(lwWNormal, o.lightmapUVOrVertexSH.xyz );

				half3 vertexLight = VertexLighting(vertexInput.positionWS, lwWNormal);
				#ifdef ASE_FOG
					half fogFactor = ComputeFogFactor( vertexInput.positionCS.z );
				#else
					half fogFactor = 0;
				#endif
				o.fogFactorAndVertexLight = half4(fogFactor, vertexLight);
				o.clipPos = vertexInput.positionCS;

				#ifdef _MAIN_LIGHT_SHADOWS
					o.shadowCoord = GetShadowCoord(vertexInput);
				#endif
				return o;
			}

			half4 frag ( VertexOutput IN  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

				float3 WorldSpaceNormal = normalize(float3(IN.tSpace0.z,IN.tSpace1.z,IN.tSpace2.z));
				float3 WorldSpaceTangent = float3(IN.tSpace0.x,IN.tSpace1.x,IN.tSpace2.x);
				float3 WorldSpaceBiTangent = float3(IN.tSpace0.y,IN.tSpace1.y,IN.tSpace2.y);
				float3 WorldSpacePosition = float3(IN.tSpace0.w,IN.tSpace1.w,IN.tSpace2.w);
				float3 WorldSpaceViewDirection = _WorldSpaceCameraPos.xyz  - WorldSpacePosition;
	
				#if SHADER_HINT_NICE_QUALITY
					WorldSpaceViewDirection = SafeNormalize( WorldSpaceViewDirection );
				#endif

				float3 break8 = ( IN.ase_texcoord7.xyz * _NoiseTiling );
				float2 appendResult11 = (float2(break8.y , break8.z));
				float2 panner13 = ( 1.0 * _Time.y * float2( 0.03,0.05 ) + appendResult11);
				float2 uv_Noise = IN.ase_texcoord8.xy * _Noise_ST.xy + _Noise_ST.zw;
				float4 temp_output_15_0 = ( tex2D( _Noise, uv_Noise ) * _NoisePower );
				float3 break23 = abs( IN.ase_normal );
				float4 lerpResult31 = lerp( float4( 0,0,0,0 ) , tex2D( _Texture, ( float4( panner13, 0.0 , 0.0 ) + temp_output_15_0 ).rg ) , break23.x);
				float2 appendResult14 = (float2(break8.x , break8.z));
				float2 panner20 = ( 1.0 * _Time.y * float2( 0.05,0.07 ) + appendResult14);
				float4 lerpResult34 = lerp( lerpResult31 , tex2D( _Texture, ( float4( panner20, 0.0 , 0.0 ) + temp_output_15_0 ).rg ) , break23.y);
				float2 appendResult19 = (float2(break8.x , break8.y));
				float2 panner26 = ( 1.0 * _Time.y * float2( 0.05,0.06 ) + appendResult19);
				float4 lerpResult36 = lerp( lerpResult34 , tex2D( _Texture, ( float4( panner26, 0.0 , 0.0 ) + temp_output_15_0 ).rg ) , break23.z);
				float fresnelNdotV22 = dot( WorldSpaceNormal, WorldSpaceViewDirection );
				float fresnelNode22 = ( 0.0 + _Scale * pow( 1.0 - fresnelNdotV22, _Power ) );
				float clampResult32 = clamp( ( 1.0 - fresnelNode22 ) , 0.0 , 1.0 );
				#ifdef _INVERT_ON
				float staticSwitch35 = clampResult32;
				#else
				float staticSwitch35 = fresnelNode22;
				#endif
				float4 clampResult39 = clamp( ( lerpResult36 * staticSwitch35 ) , float4( 0,0,0,0 ) , float4( 1,0,0,0 ) );
				
				float3 Albedo = float3(0.5, 0.5, 0.5);
				float3 Normal = float3(0, 0, 1);
				float3 Emission = _EmissionColor.rgb;
				float3 Specular = 0.5;
				float Metallic = 0;
				float Smoothness = 0.5;
				float Occlusion = 1;
				float Alpha = clampResult39.r;
				float AlphaClipThreshold = 0.5;
				float3 BakedGI = 0;

				InputData inputData;
				inputData.positionWS = WorldSpacePosition;

				#ifdef _NORMALMAP
					inputData.normalWS = normalize(TransformTangentToWorld(Normal, half3x3(WorldSpaceTangent, WorldSpaceBiTangent, WorldSpaceNormal)));
				#else
					#if !SHADER_HINT_NICE_QUALITY
						inputData.normalWS = WorldSpaceNormal;
					#else
						inputData.normalWS = normalize(WorldSpaceNormal);
					#endif
				#endif

				inputData.viewDirectionWS = WorldSpaceViewDirection;
				inputData.shadowCoord = IN.shadowCoord;

				#ifdef ASE_FOG
					inputData.fogCoord = IN.fogFactorAndVertexLight.x;
				#endif

				inputData.vertexLighting = IN.fogFactorAndVertexLight.yzw;
				inputData.bakedGI = SAMPLE_GI( IN.lightmapUVOrVertexSH.xy, IN.lightmapUVOrVertexSH.xyz, inputData.normalWS );
				#ifdef _ASE_BAKEDGI
					inputData.bakedGI = BakedGI;
				#endif
				half4 color = UniversalFragmentPBR(
					inputData, 
					Albedo, 
					Metallic, 
					Specular, 
					Smoothness, 
					Occlusion, 
					Emission, 
					Alpha);

				#ifdef ASE_FOG
					#ifdef TERRAIN_SPLAT_ADDPASS
						color.rgb = MixFogColor(color.rgb, half3( 0, 0, 0 ), IN.fogFactorAndVertexLight.x );
					#else
						color.rgb = MixFog(color.rgb, IN.fogFactorAndVertexLight.x);
					#endif
				#endif
				
				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif
				
				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif

				return color;
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "ShadowCaster"
			Tags { "LightMode"="ShadowCaster" }

			ZWrite On
			ZTest LEqual

			HLSLPROGRAM
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define _EMISSION
			#define ASE_SRP_VERSION 999999

			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

			#pragma vertex ShadowPassVertex
			#pragma fragment ShadowPassFragment


			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			#pragma shader_feature _INVERT_ON


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			sampler2D _Texture;
			sampler2D _Noise;
			CBUFFER_START( UnityPerMaterial )
			float4 _EmissionColor;
			float _NoiseTiling;
			float4 _Noise_ST;
			float _NoisePower;
			float _Scale;
			float _Power;
			CBUFFER_END


			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float4 ase_texcoord7 : TEXCOORD7;
				float4 ase_texcoord8 : TEXCOORD8;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord9 : TEXCOORD9;
				float4 ase_texcoord10 : TEXCOORD10;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			
			float3 _LightDirection;

			VertexOutput ShadowPassVertex( VertexInput v )
			{
				VertexOutput o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				float3 ase_worldPos = mul(GetObjectToWorldMatrix(), v.vertex).xyz;
				o.ase_texcoord9.xyz = ase_worldPos;
				float3 ase_worldNormal = TransformObjectToWorldNormal(v.ase_normal);
				o.ase_texcoord10.xyz = ase_worldNormal;
				
				o.ase_texcoord7 = v.vertex;
				o.ase_texcoord8.xy = v.ase_texcoord.xy;
				o.ase_normal = v.ase_normal;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord8.zw = 0;
				o.ase_texcoord9.w = 0;
				o.ase_texcoord10.w = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld(v.vertex.xyz);
				float3 normalWS = TransformObjectToWorldDir(v.ase_normal);

				float4 clipPos = TransformWorldToHClip( ApplyShadowBias( positionWS, normalWS, _LightDirection ) );

				#if UNITY_REVERSED_Z
					clipPos.z = min(clipPos.z, clipPos.w * UNITY_NEAR_CLIP_VALUE);
				#else
					clipPos.z = max(clipPos.z, clipPos.w * UNITY_NEAR_CLIP_VALUE);
				#endif
				o.clipPos = clipPos;

				return o;
			}

			half4 ShadowPassFragment(VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID( IN );

				float3 break8 = ( IN.ase_texcoord7.xyz * _NoiseTiling );
				float2 appendResult11 = (float2(break8.y , break8.z));
				float2 panner13 = ( 1.0 * _Time.y * float2( 0.03,0.05 ) + appendResult11);
				float2 uv_Noise = IN.ase_texcoord8.xy * _Noise_ST.xy + _Noise_ST.zw;
				float4 temp_output_15_0 = ( tex2D( _Noise, uv_Noise ) * _NoisePower );
				float3 break23 = abs( IN.ase_normal );
				float4 lerpResult31 = lerp( float4( 0,0,0,0 ) , tex2D( _Texture, ( float4( panner13, 0.0 , 0.0 ) + temp_output_15_0 ).rg ) , break23.x);
				float2 appendResult14 = (float2(break8.x , break8.z));
				float2 panner20 = ( 1.0 * _Time.y * float2( 0.05,0.07 ) + appendResult14);
				float4 lerpResult34 = lerp( lerpResult31 , tex2D( _Texture, ( float4( panner20, 0.0 , 0.0 ) + temp_output_15_0 ).rg ) , break23.y);
				float2 appendResult19 = (float2(break8.x , break8.y));
				float2 panner26 = ( 1.0 * _Time.y * float2( 0.05,0.06 ) + appendResult19);
				float4 lerpResult36 = lerp( lerpResult34 , tex2D( _Texture, ( float4( panner26, 0.0 , 0.0 ) + temp_output_15_0 ).rg ) , break23.z);
				float3 ase_worldPos = IN.ase_texcoord9.xyz;
				float3 ase_worldViewDir = ( _WorldSpaceCameraPos.xyz - ase_worldPos );
				ase_worldViewDir = normalize(ase_worldViewDir);
				float3 ase_worldNormal = IN.ase_texcoord10.xyz;
				float fresnelNdotV22 = dot( ase_worldNormal, ase_worldViewDir );
				float fresnelNode22 = ( 0.0 + _Scale * pow( 1.0 - fresnelNdotV22, _Power ) );
				float clampResult32 = clamp( ( 1.0 - fresnelNode22 ) , 0.0 , 1.0 );
				#ifdef _INVERT_ON
				float staticSwitch35 = clampResult32;
				#else
				float staticSwitch35 = fresnelNode22;
				#endif
				float4 clampResult39 = clamp( ( lerpResult36 * staticSwitch35 ) , float4( 0,0,0,0 ) , float4( 1,0,0,0 ) );
				
				float Alpha = clampResult39.r;
				float AlphaClipThreshold = 0.5;

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif
				return 0;
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthOnly"
			Tags { "LightMode"="DepthOnly" }

			ZWrite On
			ColorMask 0

			HLSLPROGRAM
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define _EMISSION
			#define ASE_SRP_VERSION 999999

			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

			#pragma vertex vert
			#pragma fragment frag


			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			#pragma shader_feature _INVERT_ON


			sampler2D _Texture;
			sampler2D _Noise;
			CBUFFER_START( UnityPerMaterial )
			float4 _EmissionColor;
			float _NoiseTiling;
			float4 _Noise_ST;
			float _NoisePower;
			float _Scale;
			float _Power;
			CBUFFER_END


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			
			VertexOutput vert( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float3 ase_worldPos = mul(GetObjectToWorldMatrix(), v.vertex).xyz;
				o.ase_texcoord2.xyz = ase_worldPos;
				float3 ase_worldNormal = TransformObjectToWorldNormal(v.ase_normal);
				o.ase_texcoord3.xyz = ase_worldNormal;
				
				o.ase_texcoord = v.vertex;
				o.ase_texcoord1.xy = v.ase_texcoord.xy;
				o.ase_normal = v.ase_normal;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord1.zw = 0;
				o.ase_texcoord2.w = 0;
				o.ase_texcoord3.w = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				o.clipPos = TransformObjectToHClip(v.vertex.xyz);
				return o;
			}

			half4 frag(VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				float3 break8 = ( IN.ase_texcoord.xyz * _NoiseTiling );
				float2 appendResult11 = (float2(break8.y , break8.z));
				float2 panner13 = ( 1.0 * _Time.y * float2( 0.03,0.05 ) + appendResult11);
				float2 uv_Noise = IN.ase_texcoord1.xy * _Noise_ST.xy + _Noise_ST.zw;
				float4 temp_output_15_0 = ( tex2D( _Noise, uv_Noise ) * _NoisePower );
				float3 break23 = abs( IN.ase_normal );
				float4 lerpResult31 = lerp( float4( 0,0,0,0 ) , tex2D( _Texture, ( float4( panner13, 0.0 , 0.0 ) + temp_output_15_0 ).rg ) , break23.x);
				float2 appendResult14 = (float2(break8.x , break8.z));
				float2 panner20 = ( 1.0 * _Time.y * float2( 0.05,0.07 ) + appendResult14);
				float4 lerpResult34 = lerp( lerpResult31 , tex2D( _Texture, ( float4( panner20, 0.0 , 0.0 ) + temp_output_15_0 ).rg ) , break23.y);
				float2 appendResult19 = (float2(break8.x , break8.y));
				float2 panner26 = ( 1.0 * _Time.y * float2( 0.05,0.06 ) + appendResult19);
				float4 lerpResult36 = lerp( lerpResult34 , tex2D( _Texture, ( float4( panner26, 0.0 , 0.0 ) + temp_output_15_0 ).rg ) , break23.z);
				float3 ase_worldPos = IN.ase_texcoord2.xyz;
				float3 ase_worldViewDir = ( _WorldSpaceCameraPos.xyz - ase_worldPos );
				ase_worldViewDir = normalize(ase_worldViewDir);
				float3 ase_worldNormal = IN.ase_texcoord3.xyz;
				float fresnelNdotV22 = dot( ase_worldNormal, ase_worldViewDir );
				float fresnelNode22 = ( 0.0 + _Scale * pow( 1.0 - fresnelNdotV22, _Power ) );
				float clampResult32 = clamp( ( 1.0 - fresnelNode22 ) , 0.0 , 1.0 );
				#ifdef _INVERT_ON
				float staticSwitch35 = clampResult32;
				#else
				float staticSwitch35 = fresnelNode22;
				#endif
				float4 clampResult39 = clamp( ( lerpResult36 * staticSwitch35 ) , float4( 0,0,0,0 ) , float4( 1,0,0,0 ) );
				
				float Alpha = clampResult39.r;
				float AlphaClipThreshold = 0.5;

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif
				return 0;
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "Meta"
			Tags { "LightMode"="Meta" }

			Cull Off

			HLSLPROGRAM
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define _EMISSION
			#define ASE_SRP_VERSION 999999

			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

			#pragma vertex vert
			#pragma fragment frag


			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			#pragma shader_feature _INVERT_ON


			sampler2D _Texture;
			sampler2D _Noise;
			CBUFFER_START( UnityPerMaterial )
			float4 _EmissionColor;
			float _NoiseTiling;
			float4 _Noise_ST;
			float _NoisePower;
			float _Scale;
			float _Power;
			CBUFFER_END


			#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			
			VertexOutput vert( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float3 ase_worldPos = mul(GetObjectToWorldMatrix(), v.vertex).xyz;
				o.ase_texcoord2.xyz = ase_worldPos;
				float3 ase_worldNormal = TransformObjectToWorldNormal(v.ase_normal);
				o.ase_texcoord3.xyz = ase_worldNormal;
				
				o.ase_texcoord = v.vertex;
				o.ase_texcoord1.xy = v.ase_texcoord.xy;
				o.ase_normal = v.ase_normal;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord1.zw = 0;
				o.ase_texcoord2.w = 0;
				o.ase_texcoord3.w = 0;
				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				o.clipPos = MetaVertexPosition( v.vertex, v.texcoord1.xy, v.texcoord1.xy, unity_LightmapST, unity_DynamicLightmapST );
				return o;
			}

			half4 frag(VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				float3 break8 = ( IN.ase_texcoord.xyz * _NoiseTiling );
				float2 appendResult11 = (float2(break8.y , break8.z));
				float2 panner13 = ( 1.0 * _Time.y * float2( 0.03,0.05 ) + appendResult11);
				float2 uv_Noise = IN.ase_texcoord1.xy * _Noise_ST.xy + _Noise_ST.zw;
				float4 temp_output_15_0 = ( tex2D( _Noise, uv_Noise ) * _NoisePower );
				float3 break23 = abs( IN.ase_normal );
				float4 lerpResult31 = lerp( float4( 0,0,0,0 ) , tex2D( _Texture, ( float4( panner13, 0.0 , 0.0 ) + temp_output_15_0 ).rg ) , break23.x);
				float2 appendResult14 = (float2(break8.x , break8.z));
				float2 panner20 = ( 1.0 * _Time.y * float2( 0.05,0.07 ) + appendResult14);
				float4 lerpResult34 = lerp( lerpResult31 , tex2D( _Texture, ( float4( panner20, 0.0 , 0.0 ) + temp_output_15_0 ).rg ) , break23.y);
				float2 appendResult19 = (float2(break8.x , break8.y));
				float2 panner26 = ( 1.0 * _Time.y * float2( 0.05,0.06 ) + appendResult19);
				float4 lerpResult36 = lerp( lerpResult34 , tex2D( _Texture, ( float4( panner26, 0.0 , 0.0 ) + temp_output_15_0 ).rg ) , break23.z);
				float3 ase_worldPos = IN.ase_texcoord2.xyz;
				float3 ase_worldViewDir = ( _WorldSpaceCameraPos.xyz - ase_worldPos );
				ase_worldViewDir = normalize(ase_worldViewDir);
				float3 ase_worldNormal = IN.ase_texcoord3.xyz;
				float fresnelNdotV22 = dot( ase_worldNormal, ase_worldViewDir );
				float fresnelNode22 = ( 0.0 + _Scale * pow( 1.0 - fresnelNdotV22, _Power ) );
				float clampResult32 = clamp( ( 1.0 - fresnelNode22 ) , 0.0 , 1.0 );
				#ifdef _INVERT_ON
				float staticSwitch35 = clampResult32;
				#else
				float staticSwitch35 = fresnelNode22;
				#endif
				float4 clampResult39 = clamp( ( lerpResult36 * staticSwitch35 ) , float4( 0,0,0,0 ) , float4( 1,0,0,0 ) );
				
				
				float3 Albedo = float3(0.5, 0.5, 0.5);
				float3 Emission = _EmissionColor.rgb;
				float Alpha = clampResult39.r;
				float AlphaClipThreshold = 0.5;

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				MetaInput metaInput = (MetaInput)0;
				metaInput.Albedo = Albedo;
				metaInput.Emission = Emission;
				
				return MetaFragment(metaInput);
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "Universal2D"
			Tags { "LightMode"="Universal2D" }

			Blend SrcAlpha OneMinusSrcAlpha , One OneMinusSrcAlpha
			ZWrite Off
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA

			HLSLPROGRAM
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define _EMISSION
			#define ASE_SRP_VERSION 999999

			#pragma enable_d3d11_debug_symbols
			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

			#pragma vertex vert
			#pragma fragment frag


			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			
			#pragma shader_feature _INVERT_ON


			sampler2D _Texture;
			sampler2D _Noise;
			CBUFFER_START( UnityPerMaterial )
			float4 _EmissionColor;
			float _NoiseTiling;
			float4 _Noise_ST;
			float _NoisePower;
			float _Scale;
			float _Power;
			CBUFFER_END


			#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
			};

			
			VertexOutput vert( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;

				float3 ase_worldPos = mul(GetObjectToWorldMatrix(), v.vertex).xyz;
				o.ase_texcoord2.xyz = ase_worldPos;
				float3 ase_worldNormal = TransformObjectToWorldNormal(v.ase_normal);
				o.ase_texcoord3.xyz = ase_worldNormal;
				
				o.ase_texcoord = v.vertex;
				o.ase_texcoord1.xy = v.ase_texcoord.xy;
				o.ase_normal = v.ase_normal;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord1.zw = 0;
				o.ase_texcoord2.w = 0;
				o.ase_texcoord3.w = 0;
				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				VertexPositionInputs vertexInput = GetVertexPositionInputs( v.vertex.xyz );
				o.clipPos = vertexInput.positionCS;
				return o;
			}

			half4 frag(VertexOutput IN  ) : SV_TARGET
			{
				float3 break8 = ( IN.ase_texcoord.xyz * _NoiseTiling );
				float2 appendResult11 = (float2(break8.y , break8.z));
				float2 panner13 = ( 1.0 * _Time.y * float2( 0.03,0.05 ) + appendResult11);
				float2 uv_Noise = IN.ase_texcoord1.xy * _Noise_ST.xy + _Noise_ST.zw;
				float4 temp_output_15_0 = ( tex2D( _Noise, uv_Noise ) * _NoisePower );
				float3 break23 = abs( IN.ase_normal );
				float4 lerpResult31 = lerp( float4( 0,0,0,0 ) , tex2D( _Texture, ( float4( panner13, 0.0 , 0.0 ) + temp_output_15_0 ).rg ) , break23.x);
				float2 appendResult14 = (float2(break8.x , break8.z));
				float2 panner20 = ( 1.0 * _Time.y * float2( 0.05,0.07 ) + appendResult14);
				float4 lerpResult34 = lerp( lerpResult31 , tex2D( _Texture, ( float4( panner20, 0.0 , 0.0 ) + temp_output_15_0 ).rg ) , break23.y);
				float2 appendResult19 = (float2(break8.x , break8.y));
				float2 panner26 = ( 1.0 * _Time.y * float2( 0.05,0.06 ) + appendResult19);
				float4 lerpResult36 = lerp( lerpResult34 , tex2D( _Texture, ( float4( panner26, 0.0 , 0.0 ) + temp_output_15_0 ).rg ) , break23.z);
				float3 ase_worldPos = IN.ase_texcoord2.xyz;
				float3 ase_worldViewDir = ( _WorldSpaceCameraPos.xyz - ase_worldPos );
				ase_worldViewDir = normalize(ase_worldViewDir);
				float3 ase_worldNormal = IN.ase_texcoord3.xyz;
				float fresnelNdotV22 = dot( ase_worldNormal, ase_worldViewDir );
				float fresnelNode22 = ( 0.0 + _Scale * pow( 1.0 - fresnelNdotV22, _Power ) );
				float clampResult32 = clamp( ( 1.0 - fresnelNode22 ) , 0.0 , 1.0 );
				#ifdef _INVERT_ON
				float staticSwitch35 = clampResult32;
				#else
				float staticSwitch35 = fresnelNode22;
				#endif
				float4 clampResult39 = clamp( ( lerpResult36 * staticSwitch35 ) , float4( 0,0,0,0 ) , float4( 1,0,0,0 ) );
				
				
				float3 Albedo = float3(0.5, 0.5, 0.5);
				float Alpha = clampResult39.r;
				float AlphaClipThreshold = 0.5;

				half4 color = half4( Albedo, Alpha );

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				return color;
			}
			ENDHLSL
		}
		
	}
	CustomEditor "UnityEditor.ShaderGraph.PBRMasterGUI"
	Fallback "Hidden/InternalErrorShader"
	
}
/*ASEBEGIN
Version=17700
88;44;1816;974;1445.438;372.1021;1;True;False
Node;AmplifyShaderEditor.RangedFloatNode;5;-3340.962,-418.6822;Inherit;False;Property;_NoiseTiling;NoiseTiling;7;0;Create;True;0;0;False;0;1;7;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;6;-3372.132,-577.5372;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-3109.075,-523.5217;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.BreakToComponentsNode;8;-2911.123,-523.3418;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.RangedFloatNode;10;-2562.431,-827.4629;Float;False;Property;_NoisePower;NoisePower;5;0;Create;True;0;0;False;0;0;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;9;-2681.77,-1032.625;Inherit;True;Property;_Noise;Noise;1;0;Create;True;0;0;False;0;-1;None;6110bf08410df5d41984fa2fdf8db4e0;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;11;-2572.86,-486.8245;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;14;-2578.395,-714.0182;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.NormalVertexDataNode;12;-2187.318,-48.02522;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;-2321.253,-843.9607;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.PannerNode;13;-2365.365,-488.1129;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.03,0.05;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;19;-2565.457,-265.7976;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;18;-2051.625,-492.1388;Inherit;False;2;2;0;FLOAT2;0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.AbsOpNode;21;-1930.242,-48.19685;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-1807.921,170.7698;Float;False;Property;_Scale;Scale;3;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;17;-1806.921,249.7694;Float;False;Property;_Power;Power;4;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;20;-2361.365,-714.113;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.05,0.07;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;26;-2357.678,-265.9137;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.05,0.06;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;24;-1830.672,-519.4325;Inherit;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Instance;29;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BreakToComponentsNode;23;-1747.578,-48.18317;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.FresnelNode;22;-1602.608,163.3103;Inherit;False;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;25;-2056.198,-713.3943;Inherit;False;2;2;0;FLOAT2;0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;29;-1837.683,-742.426;Inherit;True;Property;_Texture;Texture;0;0;Create;True;0;0;False;0;-1;None;918aca7f461f42042bcec5f45c31b36d;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;30;-2048.899,-266.9873;Inherit;False;2;2;0;FLOAT2;0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;28;-1225.477,-657.8456;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;31;-1451.839,-538.8312;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;27;-1333.711,262.2848;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;32;-1143.087,263.0886;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;33;-1833.226,-295.7735;Inherit;True;Property;_TextureSample1;Texture Sample 1;0;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Instance;29;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;34;-1183.202,-756.3133;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;36;-1002.534,-314.4348;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;35;-956.0117,159.9848;Float;False;Property;_Invert;Invert;6;0;Create;True;0;0;False;0;0;0;0;True;;Toggle;2;Key0;Key1;Create;False;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;-579.3727,144.4069;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;39;-344.4631,146.0343;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;38;-411.6606,-46.4123;Float;False;Property;_EmissionColor;EmissionColor;2;1;[HDR];Create;True;0;0;False;0;0,0,0,0;0.953191,1,0.5613208,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;2;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;DepthOnly;0;2;DepthOnly;0;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;2;0;False;False;False;False;True;False;False;False;False;0;False;-1;False;True;1;False;-1;False;False;True;1;LightMode=DepthOnly;False;0;Hidden/InternalErrorShader;0;0;Standard;0;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;ShadowCaster;0;1;ShadowCaster;0;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;2;0;False;False;False;False;False;False;True;1;False;-1;True;3;False;-1;False;True;1;LightMode=ShadowCaster;False;0;Hidden/InternalErrorShader;0;0;Standard;0;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;3;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;Meta;0;3;Meta;0;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;2;0;False;False;False;True;2;False;-1;False;False;False;False;False;True;1;LightMode=Meta;False;0;Hidden/InternalErrorShader;0;0;Standard;0;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;0,0;Float;False;True;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;2;SciFI_URP/HologramTriplanarURP;94348b07e5e8bab40bd6c8a1e3df54cd;True;Forward;0;0;Forward;12;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;True;2;0;True;1;5;False;-1;10;False;-1;1;1;False;-1;10;False;-1;False;False;False;True;True;True;True;True;0;False;-1;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;2;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;LightMode=UniversalForward;False;0;Hidden/InternalErrorShader;0;0;Standard;12;Workflow;1;Surface;1;  Blend;0;Two Sided;1;Cast Shadows;1;Receive Shadows;1;GPU Instancing;1;LOD CrossFade;1;Built-in Fog;1;Meta Pass;1;Override Baked GI;0;Vertex Position,InvertActionOnDeselection;1;0;5;True;True;True;True;True;False;;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;4;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;Universal2D;0;4;Universal2D;0;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;2;0;True;1;5;False;-1;10;False;-1;1;1;False;-1;10;False;-1;False;False;False;True;True;True;True;True;0;False;-1;False;True;2;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;LightMode=Universal2D;False;0;Hidden/InternalErrorShader;0;0;Standard;0;0
WireConnection;7;0;6;0
WireConnection;7;1;5;0
WireConnection;8;0;7;0
WireConnection;11;0;8;1
WireConnection;11;1;8;2
WireConnection;14;0;8;0
WireConnection;14;1;8;2
WireConnection;15;0;9;0
WireConnection;15;1;10;0
WireConnection;13;0;11;0
WireConnection;19;0;8;0
WireConnection;19;1;8;1
WireConnection;18;0;13;0
WireConnection;18;1;15;0
WireConnection;21;0;12;0
WireConnection;20;0;14;0
WireConnection;26;0;19;0
WireConnection;24;1;18;0
WireConnection;23;0;21;0
WireConnection;22;2;16;0
WireConnection;22;3;17;0
WireConnection;25;0;20;0
WireConnection;25;1;15;0
WireConnection;29;1;25;0
WireConnection;30;0;26;0
WireConnection;30;1;15;0
WireConnection;28;0;23;1
WireConnection;31;1;24;0
WireConnection;31;2;23;0
WireConnection;27;0;22;0
WireConnection;32;0;27;0
WireConnection;33;1;30;0
WireConnection;34;0;31;0
WireConnection;34;1;29;0
WireConnection;34;2;28;0
WireConnection;36;0;34;0
WireConnection;36;1;33;0
WireConnection;36;2;23;2
WireConnection;35;1;22;0
WireConnection;35;0;32;0
WireConnection;37;0;36;0
WireConnection;37;1;35;0
WireConnection;39;0;37;0
WireConnection;0;2;38;0
WireConnection;0;6;39;0
ASEEND*/
//CHKSM=0BC44C87D21A7FD7D12BE65B07481F9D702B4595