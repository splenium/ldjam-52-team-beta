using UnityEngine;

namespace Assets.Starvesters.Scripts
{
    public class PlanetOrbit : MonoBehaviour
    {
        private bool _timerIsFinishing { get; set; }
        public float _orbitSpeed = 1;
        [Range(0,1)]
        public float _orbitProgres = 0;

        [Tooltip("Randomize la position d'orbite de départ, ignore le paramètre _orbitProgres de base")]
        public bool _randomizeStartOrbitPosition = false;


        public void Start()
        {
            GameManager.Instance.TimerFinishEvent += TimerFinish;
            _timerIsFinishing = false;

            if(_randomizeStartOrbitPosition)
            {
                _orbitProgres = Random.Range(0f, 0.999f);
                Debug.Log($"{gameObject.name} start at orbit position : {_orbitProgres}");
            }
        }

        private void TimerFinish()
        {
            _timerIsFinishing = true;
        }

        public void Update()
        {
            var ellipse = GetComponent<OrbitLine>().GetOrbitEllipse();
            if (_orbitSpeed != 0 && !_timerIsFinishing)
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