#define sat(a) saturate(a)

float2 raymarchSphere(float3 sphereCenter, float sphereRadius, float3 rayOrigin, float3 rayDir)
{
	float3 p = rayOrigin;
	float dNear = 0.;
	if (distance(sphereCenter, rayOrigin) > sphereRadius)
	{

		for (int i = 0; i < 128; ++i)
		{
			float d = length(p - sphereCenter) - sphereRadius;
			if (d < 0.01)
			{
				dNear = distance(rayOrigin, p);
				break;
			}
			p += rayDir * d;
		}
	}
	float dFar = 0.;

	//if (dNear != -1.)
	{
		//if (distance(p1, rayOrigin) > 0.01)
		p += -normalize(p - sphereCenter) * 0.05; // avoid hitting the sphere
		for (int i = 0; i < 32; ++i)
		{
			float d = -(length(p - sphereCenter) - sphereRadius); // we inverse the shape to calculate the other intersection
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
	if (false)//uv.x < 0.5)
	{

		// inputCol += float3(.2,0.,0.);
		float2 hit = raymarchSphere(planetPos, planetSize, worldSpaceCameraPos, -viewDir);
		float3 hitPos = worldSpaceCameraPos + hit.x * viewDir;
		//inputCol += float3(1., 0., 0.);
		float distToAtmos = hit.x;
		float distThrough = min(hit.y - hit.x, depth - distToAtmos);
		float coefScatter = (distThrough) / (planetSize * 2.);
		float3 pcolshadow = planetPos + normalize(planetPos) * .1;//*planetSize; // origin of the center of the gradient
		float3 rgb = viewDir;
		coefScatter = pow(coefScatter, lerp(2., 3., saturate(depth / 1000.))) * 2.5 * (1. - sat(distToAtmos / 10.));
		float oklerp = 1. - sat(dot(normalize(hitPos - planetPos), normalize(planetPos)));
		planetColA *= sat((length(hitPos + normalize(planetPos) * .5) - planetSize * .5) * 10.);
		float oklerp2 = sat(dot(viewDir, normalize(hitPos - planetPos)));
		rgb = lerp(planetColA, planetColB, oklerp);
		rgb *= oklerp2;
		rgb *= 1. + pow(sat(dot(normalize(planetPos), viewDir) - .95), 2.5) * 1500. * float3(224, 182, 34) / 255.;
		col = lerp(inputCol, rgb, sat(coefScatter));
		col = hit.x*float3(1.,0.,0.);
	}
	else
	{

		// inputCol += float3(.2,0.,0.);
		float2 hit = RaySphereIntersection(planetPos, planetSize, worldSpaceCameraPos, viewDir);

		float dstToAtmos = hit.x;
		float dstThroughAtmos = min(hit.y, depth - dstToAtmos);
		float factor = (dstThroughAtmos / (planetSize * 2.0));
		factor = pow(sat(factor), 2.0);
		col = lerp(inputCol, (viewDir*.5+.5),factor);
	}
		
//		col = inputCol;
}