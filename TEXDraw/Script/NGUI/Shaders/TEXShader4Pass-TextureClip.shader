Shader "Hidden/TEXDraw/Default/Full (TextureClip)"
{
	Properties
	{
		[Header(Using 33 Textures and 5 Batches per Component)]
		[Space]
		[MiniThumbTexture] _Font0("Font 0", 2D) = "white" {}
		[MiniThumbTexture] _Font1("Font 1", 2D) = "white" {}
		[MiniThumbTexture] _Font2("Font 2", 2D) = "white" {}
		[MiniThumbTexture] _Font3("Font 3", 2D) = "white" {}
		[MiniThumbTexture] _Font4("Font 4", 2D) = "white" {}
		[MiniThumbTexture] _Font5("Font 5", 2D) = "white" {}
		[Space]
		[MiniThumbTexture] _Font6("Font 6", 2D) = "white" {}
		[MiniThumbTexture] _Font7("Font 7", 2D) = "white" {}
		[MiniThumbTexture] _Font8("Font 8", 2D) = "white" {}
		[MiniThumbTexture] _Font9("Font 9", 2D) = "white" {}
		[MiniThumbTexture] _FontA("Font A", 2D) = "white" {}
		[MiniThumbTexture] _FontB("Font B", 2D) = "white" {}
		[Space]
		[MiniThumbTexture] _FontC("Font C", 2D) = "white" {}
		[MiniThumbTexture] _FontD("Font D", 2D) = "white" {}
		[MiniThumbTexture] _FontE("Font E", 2D) = "white" {}
		[MiniThumbTexture] _FontF("Font F", 2D) = "white" {}
		[MiniThumbTexture] _Font10("Font 10", 2D) = "white" {}
		[MiniThumbTexture] _Font11("Font 11", 2D) = "white" {}
		[Space]
		[MiniThumbTexture] _Font12("Font 12", 2D) = "white" {}
		[MiniThumbTexture] _Font13("Font 13", 2D) = "white" {}
		[MiniThumbTexture] _Font14("Font 14", 2D) = "white" {}
		[MiniThumbTexture] _Font15("Font 15", 2D) = "white" {}
		[MiniThumbTexture] _Font16("Font 16", 2D) = "white" {}
		[MiniThumbTexture] _Font17("Font 17", 2D) = "white" {}
		[Space]
		[MiniThumbTexture] _Font18("Font 18", 2D) = "white" {}
		[MiniThumbTexture] _Font19("Font 19", 2D) = "white" {}
		[MiniThumbTexture] _Font1A("Font 1A", 2D) = "white" {}
		[MiniThumbTexture] _Font1B("Font 1B", 2D) = "white" {}
		[MiniThumbTexture] _Font1C("Font 1C", 2D) = "white" {}
		[MiniThumbTexture] _Font1D("Font 1D", 2D) = "white" {}
		[MiniThumbTexture] _Font1E("Font 1E", 2D) = "white" {}
	}
	SubShader
	{
		Tags 
		{
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
			"PreviewType"="Plane"
			"TexMaterialType"="Standard"
		}
		Lighting Off 
		Cull Off 
		ZTest [unity_GUIZTestMode]
		ZWrite Off 
		Blend SrcAlpha OneMinusSrcAlpha

		CGINCLUDE
		#include "UnityCG.cginc"
		#include "TEXDrawIncludes.cginc"
			
		sampler2D _ClipTex;
		float4 _ClipRange0 = float4(0.0, 0.0, 1.0, 1.0);

		struct v2f
		{
			half2 uv : TEXCOORD0;
			float2 uv1 : TEXCOORD1;
			float3 uv2 : TEXCOORD2;
			float4 vertex : SV_POSITION;
			half4 color : COLOR;
			float2 clipUV : TEXCOORD3;
		};

		v2f vert_tex (appdata v)
		{
			v2f o;
			o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
			o.uv = v.uv;
			o.uv1 = v.uv1;
			half2 tanXY = v.tangent.xy;
			o.uv2 = half3(tanXY, determineIndex(tanXY));
			o.color = v.color;
			o.clipUV = (v.vertex.xy * _ClipRange0.zw + _ClipRange0.xy) * 0.5 + float2(0.5, 0.5);
			
			return o;
		}
		
		half mask(half2 clipUV)
		{
			half alpha = tex2D(_ClipTex, clipUV).a;
			return alpha;
		}
				
		ENDCG
		Pass
		{
			Name "SecondPass"
			CGPROGRAM
			#pragma vertex vert_tex
			#pragma fragment frag
			#define TEX_5_2
			#include "TEXDrawIncludes.cginc"

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = getTexPoint(i.uv, i.uv2.z);
				return mix(i.color, col) * mask(i.clipUV);
			}
			ENDCG
		}
		Pass
		{
			Name "ThirdPass"
			CGPROGRAM
			#pragma vertex vert_tex
			#pragma fragment frag
			#define TEX_5_3
			#include "TEXDrawIncludes.cginc"

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = getTexPoint(i.uv, i.uv2.z);
				return mix(i.color, col) * mask(i.clipUV);
			}
			ENDCG
		}
		Pass
		{
			Name "FourthPass"
			CGPROGRAM
			#pragma vertex vert_tex
			#pragma fragment frag
			#define TEX_5_4
			#include "TEXDrawIncludes.cginc"

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = getTexPoint(i.uv, i.uv2.z);
				return mix(i.color, col) * mask(i.clipUV);
			}
			ENDCG
		}
		Pass
		{
			Name "FifthPass"
			CGPROGRAM
			#pragma vertex vert_tex
			#pragma fragment frag
			#define TEX_5_5
			#include "TEXDrawIncludes.cginc"

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = getTexPoint(i.uv, i.uv2.z);
				return mix(i.color, col) * mask(i.clipUV);
			}
			ENDCG
		}
		Pass
		{
			Name "PrimaryPass"
			CGPROGRAM
			#pragma vertex vert_tex
			#pragma fragment frag
			#define TEX_5_1
			#include "TEXDrawIncludes.cginc"

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = getTexPoint(i.uv, i.uv2.z);
				return mix(i.color, col) * mask(i.clipUV);
			}
			ENDCG
		}
	}
}
