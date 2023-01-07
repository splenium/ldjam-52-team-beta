// Taken and tweaked from https://www.shadertoy.com/view/Ms33WB


Shader "AtmosphereScatterPostFX"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}

        _PlanetSize("Planet size", Float) = 1.0
        _PlanetPosition("Planet position", Vector) = (0,0,0,0)
    }

    SubShader
    {
        Cull Off

        Pass
        {
            CGPROGRAM
                #pragma vertex VertexProgram
                #pragma fragment FragmentProgram

                #define MOD3 half3(.1031,.11369,.13787)
                    #include "UnityCG.cginc"

                sampler2D _MainTex;
                sampler2D _CameraDepthTexture;
                texture2D _CameraNormalsTexture;
                float4 _MainTex_TexelSize;
                float3 __WorldSpaceCameraPos;
                float4 _PlanetPosition;
                float _PlanetSize;

                struct VertexData {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct MyInterpolators {
                    float4 pos : SV_POSITION;
                    float2 uv : TEXCOORD0;
                    float3 viewDirection : TEXCOORD1;
                    float3 viewVector : TEXCOORD2;
                };

                MyInterpolators VertexProgram(VertexData v) {
                    MyInterpolators i;
                    i.viewDirection = WorldSpaceViewDir(v.vertex);
                    i.pos = UnityObjectToClipPos(v.vertex);

#if UNITY_UV_STARTS_AT_TOP
                    i.uv = float2(v.uv.x, 1 - v.uv.y);
#else
                    i.uv = v.uv;
#endif
                    // Camera space matches OpenGL convention where cam forward is -z. In unity forward is positive z.
// (https://docs.unity3d.com/ScriptReference/Camera-cameraToWorldMatrix.html)
                    float3 viewVector = mul(unity_CameraInvProjection, float4(v.uv.xy * 2 - 1, 0, 1));
                    i.viewVector = mul(unity_CameraToWorld, float4(viewVector, 0));
                    return i;
                }
                
                SamplerState sampler_CameraDepthTexture
                {

                };
                SamplerState sampler_CameraNormalsTexture
                {

                };
                

                float hash12(float2 p)
                {
                    half3 p3 = frac(half3(p.x, p.y, p.x) * MOD3);
                    p3 += dot(p3, half3(p3.y, p3.z, p3.x) + 19.19);
                    return frac((p3.x + p3.y) * p3.z);
                }

                float2 hash22(float2 p)
                {
                    float3 p3 = frac(float3(p.x, p.y, p.x) * MOD3);
                    p3 += dot(p3, float3(p3.y, p3.z, p3.x) + 19.19);
                    return frac(float2((p3.x + p3.y) * p3.z, (p3.x+p3.z)*p3.y));
                }

                float2 RaySphereIntersection(float3 sphereCenter, float sphereRadius, float3 rayOrigin, float3 rayDir)
                {
                    float3 offset = rayOrigin - sphereCenter;
                    float a = 1.; // set to dot (raydir, raydir) if raydir is not normalized
                    float b = dot(offset, rayDir);
                    float c = dot(offset, offset) - sphereRadius * sphereRadius;
                    float d = b * b - 4. * a * c;

                    if (d > 0.)
                    {
                        float s = sqrt(d);
                        float dstToSphereNear = max(0., (-b - s) / (2. * a));
                        float dstToSphereFar = (-b + s) / (2. * a);

                        // We ignore intersections behind the camera
                        if (dstToSphereFar >= 0.)
                        {
                            return float2(dstToSphereNear, dstToSphereFar- dstToSphereNear);
                        }
                    }
                    return 10000.; // We did not hit
                }


                // based on http://kylehalladay.com/blog/tutorial/math/2013/12/24/Ray-Sphere-Intersection.html
                float2 RaySphereIntersection2(float3 sphereCenter, float sphereRadius, float3 rayOrigin, float3 rayDir)
                {
                    float L = length(sphereCenter - rayOrigin);
                    float tc = dot(L, rayDir);
                    if (tc < 0.0) return -1.;

                    float d = sqrt((tc * tc) - (L * L));
                    if (d > sphereRadius) return -1.;

                    //solve for t1c
                    float t1c = sqrt((sphereRadius * sphereRadius) - (d * d));

                    //solve for intersection points
                    float t1 = tc - t1c;
                    float t2 = tc + t1c;
                    return float2(t1, t2);
                }
                float2 raymarchSphere(float3 sphereCenter, float sphereRadius, float3 rayOrigin, float3 rayDir)
                {
                    float3 p = rayOrigin;
                    float dNear = -1.;
                    float3 p1 = rayOrigin;
                    for (int i = 0; i < 128; ++i)
                    {
                        float d = length(p) - sphereRadius;
                        if (d < 0.01)
                        {
                            dNear = distance(rayOrigin, p);
                            p1 = p;
                            break;
                        }
                        p += rayDir * d;
                    }
                    return dNear;
                    float dFar = 10000.;
                    
                    //if (dNear != -1.)
                    {
                        //if (distance(p1, rayOrigin) > 0.01)
                            p += -normalize(p)*0.05; // avoid hitting the sphere
                        for (int i = 0; i < 256; ++i)
                        {
                            float d = -(length(p) - sphereRadius); // we inverse the shape to calculate the other intersection
                            if (d < 0.01)
                            {
                                dFar = distance(rayOrigin, p);
                                break;
                            }
                            p += rayDir * d;
                        }
                    }
                    return float2(dNear, dFar);
                }

                float4 FragmentProgram(MyInterpolators i) : SV_Target
                {
                    float2 uv = i.uv;


                    //float3 p = getPosition(uv);

                    float depth = _CameraNormalsTexture.Sample(sampler_CameraNormalsTexture, uv).r;
                    depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, float2(0.,1.)+uv*float2(1.,-1.));

                    depth = LinearEyeDepth(depth) * length(i.viewVector);
                    ////if (depth < 0.01)
                    //float3 cameraRightW = mul((float3x3)unity_CameraToWorld, float3(1, 0, 0));
                    //float3 cameraUpW = mul((float3x3)unity_CameraToWorld, float3(0, 1, 0));
                    //float3 cameraFwdW = mul((float3x3)unity_CameraToWorld, float3(0, 0, 1));

                    //// Screen size
                    //float scrW = _ScreenParams.x;
                    //float scrH = _ScreenParams.y;
                    //float h = 2.0f / unity_CameraProjection._m11;
                    //float w = h * scrW / scrH;
                    //float3 ray = cameraFwdW + (i.uv.x - 0.5f) * w * cameraRightW
                    //    + (i.uv.y - 0.5f) * h * cameraUpW;

                    float3 normViewDir = normalize(i.viewVector);
                    normViewDir.z *= -1.;
                    float3 worldPos = _WorldSpaceCameraPos + normViewDir * depth;

                    float3 n = normalize(cross(ddy(worldPos), ddx(worldPos)));
                    //normViewDir = normalize(i.viewDirection);
                    float3 col = tex2D(_MainTex, uv).xyz;
                    //float2 hit = RaySphereIntersection(_PlanetPosition, _PlanetSize, _WorldSpaceCameraPos, normViewDir);
                    float2 hit = raymarchSphere(_PlanetPosition, _PlanetSize, _WorldSpaceCameraPos, normViewDir);
                    if (hit.x > 0.)
                    {
                        col += float3(1., 0., 0.) * 1.;
                    }
                    if (i.uv.x < 0.5)
                    col = hit.x;
                    return float4(col, 1.);
                }
            ENDCG
        }
    }
}
