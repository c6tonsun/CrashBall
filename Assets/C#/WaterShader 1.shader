Shader "Custom/TreeShader" {
	Properties {
		_MainTex("Texture", 2D) = "white"{}
		_Color ("Color", Color) = (1,1,1,1)
		_BlendColor("Blend Color", Color) = (1,1,1,1)
		_Softness("Softness", Range(0.01, 3.0)) = 1.0
		_FadeLimit("Fade Limit", Range(0.0, 1.0)) = 0.3
		_Speed("Speed", Float) = 1
		_Amplitude("Amplitude", Float) = 1
		_Offset("Offset", Float) = 0
	}


	SubShader {
		Tags {"Queue" = "Transparent"  "RenderType"="Transparent" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard vertex:vert alpha:fade nolightmap
		#pragma target 3.0

		struct Input {
			float4 screenPos;
			float eyeDepth;
			float2 uv_MainTex;
		};

		sampler2D _MainTex;
		sampler2D_float _CameraDepthTexture;

		fixed4 _Color;
		fixed4 _BlendColor;
		float _Softness;
		float _FadeLimit;
		float _Speed;
		float _Amplitude;
		float _Offset;


		void vert(inout appdata_full v, out Input o){
			UNITY_INITIALIZE_OUTPUT(Input, o);
			COMPUTE_EYEDEPTH(o.eyeDepth);


			float3 v0 = mul(unity_ObjectToWorld, v.vertex).xyz;
			float phase0 = 0.1 * sin((_Time.y*_Speed) + (v0.z * _Offset) + (v0.x * _Offset));
			float phase1 = 0.1 * cos((_Time.y*_Speed) + (v0.z * _Offset) - (v0.x* _Offset));


			v.vertex.y += (phase0 + phase1) * _Amplitude;
			v.vertex.x -= (phase0) * _Amplitude;
			v.vertex.z += (phase1) * _Amplitude;

		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			o.Albedo = tex2D(_MainTex, IN.uv_MainTex)*_Color;
			o.Alpha = 1;
			o.Metallic = 0;
			o.Smoothness = 0;

			float rawZ = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(IN.screenPos));
			float sceneZ = LinearEyeDepth(rawZ);
			float partZ = IN.eyeDepth;

			float fade = 1.0;

			if(rawZ > 0.0){
				fade = saturate(_Softness * (sceneZ - partZ));
			}

			if(fade < _FadeLimit){ 
			o.Albedo = tex2D(_MainTex, IN.uv_MainTex) * fade + _BlendColor * (1-fade);
			//o.Albedo = _Color.rgb * fade + _BlendColor * (1-fade);
			}


		}
		ENDCG
	}
	FallBack "Diffuse"
}