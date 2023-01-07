﻿using UnityEngine;

namespace Assets.Starvesters.Scripts
{
    public class PlanetOrbit : MonoBehaviour
    {
        public float _orbitSpeed = 1;
        private float _orbitProgres = 0;
       
        public void Update()
        {
            var ellipse = GetComponent<OrbitLine>().GetOrbitEllipse();
            if (_orbitSpeed != 0)
            {
                float orbitSpeed = 1f / _orbitSpeed;

                _orbitProgres += Time.deltaTime * orbitSpeed;
                _orbitProgres %= 1f;

                Vector2 orbitpos = ellipse.Evaluate(_orbitProgres);
                gameObject.transform.position = new Vector3(orbitpos.x, 0, orbitpos.y);

            }
            
        }
    }
}