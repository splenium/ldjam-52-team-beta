#define sat(a) saturate(a)

float2 RaySphereIntersection(float3 sphereCenter, float sphereRadius, float3 rayOrigin, float3 rayDir)
{
	float3 offset = sphereCenter - rayOrigin;
	float a = 1.0;// dot(rayDir, rayDir); // set to dot (raydir, raydir) if raydir is not normalized
	float b = 2.0 * dot(offset, rayDir);
	float c = dot(offset, offset) - sphereRadius * sphereRadius;
	float d = b * b - 4. * a * c;

	if (d > 0.)
	{
		float s = sqrt(max(d,0.0));
		float dstToSphereNear = max(0., (-b - s) / (2. * a));
		float dstToSphereFar = (-b + s) / (2. * a);

		// We ignore intersections behind the camera
		if (dstToSphereFar >= 0.)
		{
			return float2(dstToSphereNear, dstToSphereFar - dstToSphereNear);
		}
	}
	return float2(100000., 0.); // We did not hit
}
void HandlePlanetAtmosphere_hlsl_float(float2 uv, float3 inputCol, float depth, float planetSize, float3 planetPos, float3 viewDir, float3 worldSpaceCameraPos, float3 planetColA, float3 planetColB, out float3 col)
{
		float2 hit = RaySphereIntersection(planetPos, planetSize, worldSpaceCameraPos, viewDir);
		float3 hitPos = worldSpaceCameraPos + hit.x * viewDir;
		float dstToAtmos = hit.x;
		float dstThroughAtmos = min(hit.y, depth - dstToAtmos);
		float factor = (dstThroughAtmos / (planetSize * 2.0));
		factor = pow(sat(factor), 2.0);
		float oklerp = 1. - sat(dot(normalize(hitPos - planetPos), normalize(planetPos)));

		planetColA *= sat((length(hitPos + normalize(planetPos) * .5) - planetSize * .5) * 10.);
		float oklerp2 = sat(dot(viewDir, normalize(hitPos - planetPos)));
		float3 rgb = lerp(planetColA, planetColB, oklerp);
		col = lerp(inputCol, rgb,factor);

}