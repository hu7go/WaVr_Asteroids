Shader "Custom/Glass"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	_Color("Color", Color) = (1,1,1, 1)
		_EdgeColor("Edge Color", Color) = (1,1,1,1)
		_EdgeThickness("Silhouette Dropoff Rate", float) = 1.0
	}

		Subshader
	{
		Tags
	{
		"Queue" = "Transparent"
	}
		Pass
	{
		Cull Off
		ZWrite On
		Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM

#pragma vertex vert
#pragma fragment frag

		sampler2D _MainTex;
	uniform float4 _Color;
	uniform float4 _EdgeColor;
	uniform float _EdgeThickness;

	struct vertexInput
	{
		float4 vertex : POSITION;
		float3 normal : NORMAL;
		float3 texCoord : TEXCOORD0;
	};

	struct vertexOutput
	{
		float4 pos : SV_POSITION;
		float3 normal : NORMAL;
		float3 texCoord : TEXCOORD0;
		float3 viewDir : TEXCOORD1;
	};

	vertexOutput vert(vertexInput input)
	{
		vertexOutput output;

		output.pos = UnityObjectToClipPos(input.vertex);
		float4 normal4 = float4(input.normal, 0.0);
		output.normal = normalize(mul(normal4, unity_WorldToObject).xyz);
		output.viewDir = normalize(_WorldSpaceCameraPos - mul(unity_ObjectToWorld, input.vertex).xyz);

		output.texCoord = input.texCoord;

		return output;
	}

	float4 frag(vertexOutput input) : COLOR
	{
		float4 texColor = tex2D(_MainTex, input.texCoord.xy);

		float edgeFactor = abs(dot(input.viewDir, input.normal));

		float oneMinusEdge = 1.0 - edgeFactor;
		float3 rgb = (_Color.rgb * edgeFactor) + (_EdgeColor * oneMinusEdge);
		rgb = min(float3(1, 1, 1), rgb);

		float opacity = min(1.0, _Color.a / edgeFactor);
		opacity = opacity * texColor.a;

		float4 output = float4(rgb, opacity);
		return output;
	}
		ENDCG
	}
	}
}