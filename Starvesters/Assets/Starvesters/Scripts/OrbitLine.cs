using System.Collections;
using UnityEngine;

namespace Assets.Starvesters.Scripts
{
    [ExecuteInEditMode]
    public class OrbitLine : MonoBehaviour
    {
        [Range(3, 100)]
        public int _segments;
        public LineRenderer _lineRenderer;

        private void CalculatePlanetEllipse()
        {
            Vector3[] points = new Vector3[_segments + 1];
            var ellipse = GetOrbitEllipse();

            for (int i = 0; i < _segments; i++)
            {
                Vector2 position2D = ellipse.Evaluate((float)i / (float)_segments);
                points[i] = new Vector3(position2D.x, 0f, position2D.y);
            }

            points[_segments] = points[0];

            _lineRenderer.positionCount = _segments + 1;
            _lineRenderer.SetPositions(points);
        }

        public Ellipse GetOrbitEllipse()
        {
            // Le game object de l'elipse doit être au niveau du soleil
            var distance = Vector3.Distance(transform.position, _lineRenderer.gameObject.transform.position);

            return new Ellipse(distance);
        }

        public void Awake()
        {
            CalculatePlanetEllipse();
        }

        private void OnValidate()
        {
            CalculatePlanetEllipse();
        }
    }
}