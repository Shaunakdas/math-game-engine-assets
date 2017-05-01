Shader "Hidden/TEXDraw/Default/Full 2"
{
	Properties
	{
		[Space]
		[MiniThumbTexture] _Font0("Font 0", 2D) = "white" {}
		[MiniThumbTexture] _Font1("Font 1", 2D) = "white" {}
		[MiniThumbTexture] _Font2("Font 2", 2D) = "white" {}
		[MiniThumbTexture] _Font3("Font 3", 2D) = "white" {}
		[MiniThumbTexture] _Font4("Font 4", 2D) = "white" {}
		[MiniThumbTexture] _Font5("Font 5", 2D) = "white" {}
		[MiniThumbTexture] _Font6("Font 6", 2D) = "white" {}
		[MiniThumbTexture] _Font7("Font 7", 2D) = "white" {}
		[Space]
		[MiniThumbTexture] _Font8("Font 8", 2D) = "white" {}
		[MiniThumbTexture] _Font9("Font 9", 2D) = "white" {}
		[MiniThumbTexture] _FontA("Font A", 2D) = "white" {}
		[MiniThumbTexture] _FontB("Font B", 2D) = "white" {}
		[MiniThumbTexture] _FontC("Font C", 2D) = "white" {}
		[MiniThumbTexture] _FontD("Font D", 2D) = "white" {}
		[MiniThumbTexture] _FontE("Font E", 2D) = "white" {}
		[MiniThumbTexture] _FontF("Font F", 2D) = "white" {}
		[Space]
		[MiniThumbTexture] _Font10("Font 10", 2D) = "white" {}
		[MiniThumbTexture] _Font11("Font 11", 2D) = "white" {}
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

		struct v2f
		{
			half2 uv : TEXCOORD0;
			float2 uv1 : TEXCOORD1;
			float3 uv2 : TEXCOORD2;
			float4 vertex : SV_POSITION;
			half4 color : COLOR;
			float4 worldPos : TEXCOORD3;
		};

		float4 _ClipRange0 = float4(0.0, 0.0, 1.0, 1.0);
		float4 _ClipArgs0 = float4(1000.0, 1000.0, 0.0, 1.0);
		float4 _ClipRange1 = float4(0.0, 0.0, 1.0, 1.0);
		float4 _ClipArgs1 = float4(1000.0, 1000.0, 0.0, 1.0);
		
		
		float2 Rotate (float2 v, float2 rot)
		{
			float2 ret;
			ret.x = v.x * rot.y - v.y * rot.x;
			ret.y = v.x * rot.x + v.y * rot.y;
			return ret;
		}
		
		#include "TEXDrawIncludes.cginc"
		
		fixed4 mix (fixed4 vert_tex, fixed4 tex, fixed alpha)
		{
			return fixed4(max(vert_tex, tex).rgb, vert_tex.a * tex.a * alpha);
		}



		v2f vert_tex (appdata v)
		{
			v2f o;
			o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
			o.uv = v.uv;
			o.uv1 = v.uv1;
			half2 tanXY = v.tangent.xy;
			o.uv2 = half3(tanXY, determineIndex(tanXY));
			o.color = v.color;
			
			o.worldPos.xy = v.vertex.xy * _ClipRange0.zw + _ClipRange0.xy;
			o.worldPos.zw = Rotate(v.vertex.xy, _ClipArgs1.zw) * _ClipRange1.zw + _ClipRange1.xy;
			return o;
		}
		
		float mask (v2f IN)
		{
			// First clip region
			float2 factor = (float2(1.0, 1.0) - abs(IN.worldPos.xy)) * _ClipArgs0.xy;
			float f = min(factor.x, factor.y);

			// Second clip region
			factor = (float2(1.0, 1.0) - abs(IN.worldPos.zw)) * _ClipArgs1.xy;
			f = min(f, min(factor.x, factor.y));
			
			return clamp(f, 0.0, 1.0);
		}
		
		
		
		ENDCG
		Pass
		{
			Name "SecondPass"
			CGPROGRAM
			#pragma vertex vert_tex
			#pragma fragment frag
			#define TEX_4_2
			#include "TEXDrawIncludes.cginc"
			#include "UnityCG.cginc"
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = getTexPoint(i.uv, i.uv2.z);
				return mix(i.color, col, mask(i));
			}

			ENDCG
		}
		Pass
		{
			Name "ThirdPass"
			CGPROGRAM
			#pragma vertex vert_tex
			#pragma fragment frag
			#define TEX_4_3
			#include "TEXDrawIncludes.cginc"
			#include "UnityCG.cginc"
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = getTexPoint(i.uv, i.uv2.z);
				return mix(i.color, col, mask(i));
			}
			ENDCG
		}
		Pass
		{
			Name "FourthPass"
			CGPROGRAM
			#pragma vertex vert_tex
			#pragma fragment frag
			#define TEX_4_4
			#include "TEXDrawIncludes.cginc"
			#include "UnityCG.cginc"
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = getTexPoint(i.uv, i.uv2.z);
				return mix(i.color, col, mask(i));
			}

			ENDCG
		}
		Pass
		{
			Name "PrimaryPass"
			CGPROGRAM
			#pragma vertex vert_tex
			#pragma fragment frag
			#define TEX_4_1
			#include "TEXDrawIncludes.cginc"
			#include "UnityCG.cginc"
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = getTexPoint(i.uv, i.uv2.z);
				return mix(i.color, col, mask(i));
			}

			ENDCG
		}
	}
}
