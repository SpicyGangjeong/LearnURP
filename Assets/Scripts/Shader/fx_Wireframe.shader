Shader "Custom/fx_Wireframe"
{
    Properties
    {
        [MainColor] _BaseColor("Wire Color", Color) = (0, 1, 1, 1)
        _Thickness("Thickness", Range(0.0, 10.0)) = 1.0
        _Softness("Softness", Range(0.0, 5.0)) = 1.0
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "IgnoreProjector" = "True"
        }

        Pass
        {
            Name "Wireframe"
            Tags { "LightMode" = "UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma target 4.0
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 barycentric : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float _Thickness;
                float _Softness;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.barycentric = 0;
                return OUT;
            }

            [maxvertexcount(3)]
            void geom(triangle Varyings IN[3], inout TriangleStream<Varyings> pStream)
            {
                Varyings v0 = IN[0];
                Varyings v1 = IN[1];
                Varyings v2 = IN[2];

                // 각 꼭짓점에 바리센트릭 좌표 부여 → 프래그먼트에서 엣지 거리 계산
                v0.barycentric = float3(1, 0, 0);
                v1.barycentric = float3(0, 1, 0);
                v2.barycentric = float3(0, 0, 1);

                pStream.Append(v0);
                pStream.Append(v1);
                pStream.Append(v2);
                pStream.RestartStrip();
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float3 pBary = IN.barycentric;

                // 화면 공간 변화량으로 두께를 픽셀 단위에 가깝게 맞춤
                float3 vDelta = fwidth(pBary);
                float3 vThresh = vDelta * _Thickness;
                float3 vSoft = vDelta * _Softness;

                float3 vEdge = smoothstep(vThresh, vThresh + vSoft, pBary);
                float fWire = 1.0 - min(vEdge.x, min(vEdge.y, vEdge.z));

                half4 color = _BaseColor;
                color.a *= fWire;

                // 완전 투명 픽셀은 버림 (오버드로우 약간 줄임)
                clip(color.a - 0.01);

                return color;
            }
            ENDHLSL
        }
    }

    FallBack Off
}