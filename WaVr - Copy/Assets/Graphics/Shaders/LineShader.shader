Shader "Custom/LineShader"
{
	Properties
	{
		_NoiseTex("Noise texture", 2D) = "white" {}
		_DisplGuide("Displacement guide", 2D) = "white" {}
		_DisplAmount("Displacement amount", float) = 0
		_ColorFront("_ColorFront", color) = (1,1,1,1)
		_ColorBack("_ColorBack", color) = (1,1,1,1)
		_ScrollSpeed("_ScrollSpeed", float) = 0
		_BottomFoamThreshold("Bottom foam threshold", Range(0,1)) = 0.1
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				// make fog work
				#pragma multi_compile_fog

				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float4 vertex : SV_POSITION;
					float2 uv : TEXCOORD0;
					float2 noiseUV : TEXCOORD1;
					float2 displUV : TEXCOORD2;
					UNITY_FOG_COORDS(3)
				};

				sampler2D _NoiseTex;
				float4 _NoiseTex_ST;
				sampler2D _DisplGuide;
				float4 _DisplGuide_ST;
				fixed4 _ColorFront;
				fixed4 _ColorBack;
				float _ScrollSpeed;
				half _DisplAmount;
				half _BottomFoamThreshold;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.noiseUV = TRANSFORM_TEX(v.uv, _NoiseTex);
					o.displUV = TRANSFORM_TEX(v.uv, _DisplGuide);
					o.uv = v.uv;
					UNITY_TRANSFER_FOG(o,o.vertex);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					//Displacement
					half2 displ = tex2D(_DisplGuide, i.displUV + _Time.y / 5).xy;
					displ = ((displ * 2) - 1) * _DisplAmount;

					//Noise
					half noise = tex2D(_NoiseTex, float2(i.noiseUV.x - _Time.y / _ScrollSpeed, i.noiseUV.y) + displ).x;

					fixed4 col = lerp(lerp(_ColorFront, _ColorFront, i.uv.y), lerp(_ColorBack, _ColorBack, i.uv.y), noise);
					col = lerp(fixed4(1,1,1,1), col, step(_BottomFoamThreshold, i.uv.y + displ.y));
					UNITY_APPLY_FOG(i.fogCoord, col);
					return col;
				}
				ENDCG
			}
		}
			Fallback "VertexLit"
	}
