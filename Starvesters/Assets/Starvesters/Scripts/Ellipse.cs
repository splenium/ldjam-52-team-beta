using System;
using UnityEditor;
using UnityEngine;

namespace Assets.Starvesters.Scripts
{
    [Serializable]
    public class Ellipse
    {
        private float Distance { get; set; }

        public Ellipse ( float distance)
        {
            this.Distance = distance;
        }

        public Vector2 Evaluate(float t)
        {
            float angle = Mathf.Deg2Rad * 360f * t;
            float x = Mathf.Sin(angle) * Distance;
            float y = Mathf.Cos(angle) * Distance;

            return new Vector2(x, y);
        }
    }
}