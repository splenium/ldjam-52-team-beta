using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI _teshMeshPro;
    public float _remainingTime;

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
                _teshMeshPro.text = TimeSpan.FromSeconds((double)_remainingTime).ToString(@"mm\:ss");
            }
            else
            {
                TimerIsRunning = false;
                TimerFinishEvent?.Invoke();
            }
        }
    }
}

