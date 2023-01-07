
                float2 raymarchSphere(float3 sphereCenter, float sphereRadius, float3 rayOrigin, float3 rayDir)
                {
                    float3 p = rayOrigin;
                    float dNear = 0.;

					if (distance(sphereCenter, rayOrigin) > sphereRadius)
					{
						
                    for (int i = 0; i < 128; ++i)
                    {
                        float d = length(p) - sphereRadius;
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
                    return float2(10000., 0.); // We did not hit
                }
				#define sat(a) saturate(a)
void HandlePlanetAtmosphere_hlsl_float(float2 uv, float3 inputCol, float depth, float planetSize, float3 planetPos, float3 viewDir, float3 worldSpaceCameraPos, out float3 col)
{
	if (true)//uv.x < 0.5)
	{
		
		// inputCol += float3(.2,0.,0.);
		float2 hit = raymarchSphere(planetPos, planetSize, worldSpaceCameraPos, -viewDir);

        //inputCol += float3(1., 0., 0.);
		float distToAtmos = hit.x;
		float distThrough = min(hit.y-hit.x, depth-distToAtmos);
		float coefScatter= (distThrough)/(planetSize*2.);
		float3 pcolshadow = planetPos+normalize(planetPos)*.1;//*planetSize; // origin of the center of the gradient
		float3 rgb = viewDir;
		coefScatter = pow(coefScatter,lerp(.5, 3., saturate(depth/100.)));
		rgb = viewDir*.5+.5;
		col = lerp(inputCol, rgb, sat(coefScatter));

	}
	else
		col = inputCol;
}