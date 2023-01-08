Shader "Unlit/Thruster"
{
    Properties
    {
        _NoiseTex ("Texture", 2D) = "white" {}
        _Acceleration("Accel", Float) = 0.0
        _OffsetCol("Offset col", Float) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZTest Always
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
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 localVertex : TEXCOORD2;
            };

            sampler2D _NoiseTex;
            float _Acceleration;
            float _OffsetCol;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex*(1.0f+.1*(.5+.5*sin(_Time.y*40.))*saturate(v.vertex.y)));
                o.localVertex = v.vertex;
                o.uv = v.uv;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            float2x2 r2d(float a) { float c = cos(a), s = sin(a); return float2x2(c, -s, s, c); }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture

                // apply fog
                //UNITY_APPLY_FOG(i.fogCoord, col);
                fixed3 noise = tex2D(_NoiseTex, .2*i.uv * float2(.5,.1) + float2(0.,_Time.y)).xyz;
                float coefOpa = 1.0f-saturate(abs(i.localVertex.y));
                float3 rgb = noise*2.0f*saturate(sin(i.localVertex.y*10.0f+sin(i.uv.x*20.0f)*3.0f+_Time.y)*0.3+0.7f);
                fixed3 noise2 = tex2D(_NoiseTex, i.uv * float2(.5, .1)*.25f + float2(0., _Time.y*.5f)).xyz;
                rgb += float3(0.1f, 0.3f, 0.8f) * noise2;
                rgb = lerp(rgb * float3(0.5, 0.1, 0.2), rgb * float3(0.2, 0.1, 0.7), saturate(i.localVertex.y + 0.5f))*3.0f;
                rgb.xy = mul(rgb.xy, r2d(_OffsetCol));
                rgb = abs(rgb);
                return float4(rgb*1.5,coefOpa* saturate(_Acceleration));
            }
            ENDCG
        }
    }
}
