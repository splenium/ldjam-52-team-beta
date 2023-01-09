Shader "Unlit/SunShader"
{
    Properties
    {
        _NoiseTex ("Noise", 2D) = "white" {}
        _SunCol("Sun Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"}
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
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
            };

            sampler2D _NoiseTex;
            float4 _NoiseTex_ST;
            float _time;
            float3 _SunCol;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _NoiseTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }
            float2x2 r2d(float a) { float c = cos(a), s = sin(a); return float2x2(c, -s, s, c); }
            float hash11(float seed)
            {
                return abs(fmod(sin(seed * 123.456789) * 123.456, 1.));
            }
            float3 getCam(float3 rd, float2 uv)
            {
                float fov = 1.;
                float3 r = normalize(cross(rd, float3(0., 1., 0.)));
                float3 u = normalize(cross(rd, r));
                return normalize(rd + fov * (r * uv.x + u * uv.y));
            }

            float2 _min(float2 a, float2 b)
            {
                if (a.x < b.x)
                    return a;
                return b;
            }

            float _seed;
            float rand()
            {
                _seed++;
                return hash11(_seed);
            }
            float2 map(float3 p)
            {
                float2 acc = float2(10000., -1.);

                float2 uv = float2(atan2(p.z, p.x), acos(p.y));

                float rad = 1.;
                for (float i = 0.; i < 8.; ++i)
                {

                    rad -= -tex2D(_NoiseTex, uv * .03 * (1. + i) + _time * .001).x * .05 / (1. + i);
                }
                float shape = length(p) - rad;
                float sz = 15.;
                float gyroid = dot(sin(p * sz + float3(0., 0., _time * .25)), cos(p.yzx * sz)) / sz;
                //shape = min(shape, sin(p.x*10.)+1.2);
                //shape = min(shape, gyroid+length(p)*.25);
                acc = _min(acc, float2(shape, 0.));
                uv = mul(uv, r2d(sin(uv.x * 5. + uv.y * 3.) * 2.));
                //uv* r2d(p.y * 10.);
                //uv.x += sin(uv.y*10.);
                gyroid = max(tex2D(_NoiseTex, uv * .1 + _time * .002).x - .1, length(p) - 1.3);
                //gyroid = max(gyroid, -(length(normalize(p) * 1.) - 1.25));
                acc = _min(acc, float2(gyroid, 1.));

                return acc;
            }

            float3 getNorm(float3 p, float d)
            {
                float2 e = float2(0.001, 0.);
                return normalize(float3(d,d,d) - float3(map(p - e.xyy).x, map(p - e.yxy).x, map(p - e.yyx).x));
            }
            float3 accCol;
            float3 trace(float3 ro, float3 rd, int steps)
            {
                accCol = 0.;
                float3 p = ro;
                for (int i = 0; i < steps && distance(p, ro) < 10.; ++i)
                {
                    float2 res = map(p);
                    if (res.x < 0.001)
                        return float3(res.x, distance(p, ro), res.y);
                    if (res.y == 1.)
                        res.x += .1;
                    p += rd * res.x;
                    float f = 1.;
                    if (res.y == 1.)
                        f = 4.;//*saturate(dot(rd, -normalize(p)));
                    accCol += _SunCol * (1. - saturate(res.x / .5)) * .041 * f;
                }
                return -1.;
            }

            float4 rdr(float2 uv, float2 ouv)
            {
                float3 col = 0.;
                float rad = 3.;
                float an = _time*.5;
                float3 ro = float3(sin(an) * rad, 0., cos(an) * rad);
                float3 ta = float3(0., 0., 0.);
                float3 rd = normalize(ta - ro);

                rd = getCam(rd, uv);
                float3 res = trace(ro, rd, 16);
                float alpha = 0.;
                if (res.y > 0.)
                {
                    alpha = 1.;
                    float3 p = ro + rd * res.y;
                    float3 n = getNorm(p, res.x);
                    col = n * .5 + .5;
                    float2 sunuv = float2(atan2(p.z, p.x), acos(p.y));
                    float matterFloat = (saturate(dot(rd, n) + .9));
                    float pattSunDark = tex2D(_NoiseTex, sunuv * .05 + float2(_time * .0002, sin(_time * .012) * .2)).x +
                        tex2D(_NoiseTex, sunuv * .1 + .2 * tex2D(_NoiseTex, mul(float2(1., 4.) * sunuv * .01,r2d(sunuv.x + _time * .01))).xx).x;

                    col = _SunCol.zxy * matterFloat * .5 * tex2D(_NoiseTex, sunuv * .05).x
                        + pattSunDark *
                        1.7 * _SunCol * (1. - saturate(dot(rd, n) + 1.2));
                    col = lerp(col, col * .0, tex2D(_NoiseTex, sunuv * .05).x * (1. - matterFloat));
                    col -= saturate((tex2D(_NoiseTex, sunuv * .05).x + tex2D(_NoiseTex, sunuv * .04 + _time * .001).x) * .5 - .6);
                    col -= tex2D(_NoiseTex, sunuv * 3.).x * .25;
                    col -= tex2D(_NoiseTex, sunuv * 3.2).x * .25;
                    col += .28;
                }
                col += accCol;
                col = saturate(col);
                if (alpha < 0.1)
                    alpha = length(col);
                return float4(col, alpha);
            }


            fixed4 frag(v2f i) : SV_Target
            {
                _time = _Time.y*.3;
                float4 col = rdr((i.uv-.5)*2.,i.uv)*1.5;
                return col;
            }
            ENDCG
        }
    }
}
