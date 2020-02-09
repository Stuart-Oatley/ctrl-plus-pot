//Adapted from the edge length based tesselation and phong tesselation shaders at https://docs.unity3d.com/2019.1/Documentation/Manual/SL-SurfaceShaderTessellation.html

Shader "Custom/TesselationShader" {
	Properties{
			_EdgeLength("Edge length", Range(0.1,50)) = 15
			_MainTex("Base (RGB)", 2D) = "white" {}
			_NormalMap("Normalmap", 2D) = "bump" {}
			_Color("Color", color) = (1,1,1,0)
			_Phong("Phong strength", Range(0, 1)) = 0.5
	}
		SubShader{
			Tags { "RenderType" = "Opaque" }
			LOD 300

			CGPROGRAM
			#pragma surface surf BlinnPhong addshadow fullforwardshadows Lambert vertex:dispNone tessellate:tessEdge tessphong:_Phong nolightmap
			#pragma target 4.6
			#include "Tessellation.cginc"

			struct appdata {
				float4 vertex : POSITION;
				float4 tangent : TANGENT;
				float3 normal : NORMAL;
				float2 texcoord : TEXCOORD0;
			};

			float _Phong;
			float _EdgeLength;

			float4 tessEdge(appdata v0, appdata v1, appdata v2)
			{
				return UnityEdgeLengthBasedTess(v0.vertex, v1.vertex, v2.vertex, _EdgeLength);
			}

			void dispNone(inout appdata v) { }

			struct Input {
				float2 uv_MainTex;
			};

			sampler2D _MainTex;
			sampler2D _NormalMap;
			fixed4 _Color;

			void surf(Input IN, inout SurfaceOutput o) {
				half4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
				o.Albedo = c.rgb;
				o.Specular = 0.2;
				o.Gloss = 1.0;
				o.Normal = UnpackNormal(tex2D(_NormalMap, IN.uv_MainTex));
			}
			ENDCG
			}
				FallBack "Diffuse"
		   FallBack "Diffuse"
}

