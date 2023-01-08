using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // public TextMeshProUGUI _teshMeshPro;
    public float _winningTime;
    public float _remainingTime;
    public float _addingTime;

    public Color _actualSunColor;
    public HandleSun _handleSun;
        
    public Canvas _canvas;
    public Material _uiMaterial;

    private List<GameObject> LightHarvest { get; set; }
    public static GameManager Instance { get; private set; }
    
    public delegate void TimerFinish();
    public event TimerFinish TimerFinishEvent;

    private bool TimerIsRunning { get; set; } = true;
    
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void Start()
    {
        LightHarvest = new List<GameObject>();

        _handleSun.SunColor = _actualSunColor;
        _uiMaterial.SetColor("_ColorSun", _actualSunColor);
    }
    // TODO :
    // - Cr�ation d'ev�nement pour : ajouter du temps
    // - Enlever de la luminosit� au fur et a mesure du temps
    // - Impl�menter l'UI de r�cup�ration de la lumi�re
    // - Pouvoir r�cup�rer une lumi�re
    // - Additionner les lumi�res et le setup sur le soleil

    void FixedUpdate()
    {
        Timer();
    }

    private void Timer()
    {
        if (TimerIsRunning)
        {
            if (_remainingTime > 0)
            {
                _remainingTime -= Time.deltaTime;

                _uiMaterial.SetFloat("_Progress", _remainingTime / 120.0f );
                // _teshMeshPro.text = TimeSpan.FromSeconds((double)_remainingTime).ToString(@"mm\:ss");
            }
            else
            {
                TimerIsRunning = false;
                TimerFinishEvent?.Invoke();

                // Restart canvas appear
                _canvas.gameObject.SetActive(true);
                Time.timeScale = 0f;
            }
        }
    }

    public void LightIsHarvest(GameObject light)
    {
        // On ajoute notre lumi�re � notre personnage
        LightHarvest.Add(light);
        Debug.Log($"Player have {LightHarvest.Count()} ");
        // On change la couleur de notre soleil
        _actualSunColor = light.GetComponent<TreeCollectible>().Color;
        _handleSun.SunColor = _actualSunColor;
        _uiMaterial.SetColor("_ColorSun", _actualSunColor);

        // On icr�ment le temps restant
        _remainingTime += _addingTime;
    }

}

