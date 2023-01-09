#define sat(a) saturate(a)

float2 RaySphereIntersection(float3 sphereCenter, float sphereRadius, float3 rayOrigin, float3 rayDir)
{
	float3 offset = rayOrigin-sphereCenter;
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
float _densityFalloff;
float3 _planetPos;
float _planetRadius;
float _atmosRadius;
// TODO densityFalloff setting
float densityAtPoint(float3 densitySamplePoint)
{

	float heightAboveSurface = length(densitySamplePoint - _planetPos) - _planetRadius;
	float height01 = heightAboveSurface / (_atmosRadius - _planetRadius);
	float localDensity = exp(-height01 * _densityFalloff)*(1.-height01);
	return localDensity;
}

float opticalDepth(float3 rayOrigin, float3 rayDir, float rayLength)
{
	int densityPointCnt = 10;
	float3 densitySamplePoint = rayOrigin;
	float stepSize = rayLength / (densityPointCnt - 1);
	float opticalDepth = 0.;

	for (int i = 0; i < densityPointCnt; ++i)
	{
		float localDensity = densityAtPoint(densitySamplePoint);
		opticalDepth += localDensity * stepSize;
		densitySamplePoint += rayDir * stepSize;
	}
	return opticalDepth;

}

float3 _scatterCoefs;
float3 calculateLight(float3 rayOrigin, float3 rayDir, float rayLength)
{
	int scatteringPointCnt = 10;
	float3 inScatterPoint = rayOrigin;
	float stepSize = rayLength / (scatteringPointCnt - 1);
	float3 inScatterLight = 0.;

	for (int i = 0; i < scatteringPointCnt;++i)
	{
		float3 dirToSun = -normalize(inScatterPoint); // assume sun is 0 0 0 // unsure
		float sunRayLength = RaySphereIntersection(_planetPos, _atmosRadius, inScatterPoint, dirToSun).y;
		float sunRayOpticalDepth = opticalDepth(inScatterPoint, dirToSun, sunRayLength);
		float viewRayOpticalDepth = opticalDepth(inScatterPoint, -rayDir, stepSize*i);
		float3 transmittance = exp(-(sunRayOpticalDepth+viewRayOpticalDepth)*_scatterCoefs);
		float localDensity = densityAtPoint(inScatterPoint);
		inScatterLight += localDensity * transmittance * stepSize;
		inScatterPoint += rayDir * stepSize;
	}
	return inScatterLight;//saturate(dot(normalize(rayOrigin), -normalize(_planetPos)));//inScatterLight;
}

void HandlePlanetAtmosphere_hlsl_float(float2 uv, float3 inputCol, float depth, float planetSize, float3 planetPos, float3 viewDir, float3 worldSpaceCameraPos, float3 planetColA, float3 planetColB, out float3 col)
{
	_scatterCoefs=planetColA;
			_densityFalloff = 10.0f;
			_planetPos = planetPos;
			_planetRadius = planetSize;
			_atmosRadius = _planetRadius+_planetRadius*2.1f;
		viewDir = -viewDir;
		float2 hit = RaySphereIntersection(planetPos, _atmosRadius, worldSpaceCameraPos, viewDir);
				float dstToAtmos = hit.x;
		float dstThroughAtmos = min(hit.y, depth - dstToAtmos);

		float3 rgb = 0.;//lerp(planetColA, planetColB, oklerp);

		if (dstThroughAtmos > 0.)
		{


			float epsilon = 0.001;
			float3 pointInAtmos = worldSpaceCameraPos + viewDir * (dstToAtmos+epsilon);
			rgb = calculateLight(pointInAtmos, viewDir, dstThroughAtmos-epsilon*2)*planetColA;
			col = inputCol * (1.-rgb)+rgb;

			return;
		}
		
		col = inputCol;//lerp(inputCol, rgb,factor);

}
