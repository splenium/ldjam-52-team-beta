#define sat(a) clamp(a, 0., 1.)

// Thanks IQ :)
float sdRoundedBox(float2 p, float2 b, float4 r)
{
    r.xy = (p.x > 0.0) ? r.xy : r.zw;
    r.x = (p.y > 0.0) ? r.x : r.y;
    float2 q = abs(p) - b + r.x;
    return min(max(q.x, q.y), 0.0) + length(max(q, 0.0)) - r.x;
}

float3 rdr(float2 uv, float3 color, float progressUser)
{
    float3 col = 0.;
    float sharp = 400.;//iResolution.x;
    float2 sz = float2(.02, .3);
    float outerShell = abs(sdRoundedBox(uv, sz, .005)) - .0002;
    float inner = sdRoundedBox(uv, sz * float2(.7, .97), (.005));
    outerShell = max(outerShell, -sdRoundedBox(uv - .007, sz, (.005)));
    float ticks = max(sdRoundedBox(uv - .007, sz, (.005)), sin(uv.y * 100.) + .99);
    col = lerp(col, 1., 1. - sat(outerShell * sharp));
    float completeness = lerp(-sz.y, sz.y, 1.0f-sat(progressUser));
    float limHardness = 30.;
    float3 progessCol = lerp(color*.8+ color.zxy*.2, color, sin(uv.y * 45. - _Time.y) * .5 + .5);
    col = lerp(col, progessCol, (1. - sat(inner * sharp)) * sat((-uv.y - completeness) * limHardness));

    col += 2. * progessCol * (1. - sat(inner * 30.)) * sat((-uv.y - completeness) * limHardness);
    col +=   .2 * (1. - sat(ticks * sharp));
    return col;
}

void UIHarvest_hlsl_float(float2 uv, float3 color, float progress, out float3 col)
{
    col = pow(rdr(uv*2., color, progress),2.);
}