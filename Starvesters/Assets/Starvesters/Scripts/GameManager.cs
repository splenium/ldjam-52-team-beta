using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    public delegate void TimerFinish();
    public event TimerFinish TimerFinishEvent;

    public float _remainingTime = 1;

    List<object> lightHardvest = new List<object> { };

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
    // - Création d'evènement pour : ajouter du temps
    // - Enlever de la luminosité au fur et a mesure du temps
    // - Implémenter l'UI de récupération de la lumière
    // - Pouvoir récupérer une lumière
    // - Additionner les lumières et le setup sur le soleil

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
            }
            else
            {
                TimerIsRunning = false;
                TimerFinishEvent?.Invoke();
            }
        }
    }
}

