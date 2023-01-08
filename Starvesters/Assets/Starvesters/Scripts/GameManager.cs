using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Color = UnityEngine.Color;

public class GameManager : MonoBehaviour
{
    public enum GameStateEnum { Introduction, Played, Ended }
    public List<GameObject> _playedGameObjects;
    public List<GameObject> _introductionGameObjects;
    public List<GameObject> _endGameObects;

    public GameStateEnum _gameState;
    public GameObject _playerElements;
    public GameObject _camera;

    public float _winningTime;
    public float _remainingTime;
    public float _addingTime;

    public Color _actualSunColor;
    public HandleSun _handleSun;
        
    public Canvas _menu;
    public TextMeshProUGUI _teshMeshProInformationMessage;
    private const string _winningMessage = "Well played , you've won";
    private const string _losingMessage = "Nice try, but you lose";

    public Material _uiMaterial;

    private List<GameObject> LightHarvest { get; set; }
    public static GameManager Instance { get; private set; }
    
    public delegate void TimerFinish();
    public event TimerFinish TimerFinishEvent;

    private bool StartingGame { get; set; }
    private bool TimerIsRunning { get; set; } = true;

    public ShowHideAtmospheres _showHideAtmospheres;
    
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
        Time.timeScale = 1f;
        _gameState = GameStateEnum.Introduction;
        _handleSun.SunColor = _actualSunColor;
        _uiMaterial.SetColor("_ColorSun", _actualSunColor);
        StartingGame = true;

        PlayedActived(false);

        foreach (var playedGameObject in _introductionGameObjects)
        {
            playedGameObject.SetActive(true);
        }
    }

    void FixedUpdate()
    {
        switch (_gameState)
        {
            case GameStateEnum.Introduction:
                if (Input.GetKey(KeyCode.Space))
                {
                    _gameState = GameStateEnum.Played;

                    PlayedActived(true);
                    foreach (var playedGameObject in _introductionGameObjects)
                    {
                        playedGameObject.SetActive(false);
                    }

                    _showHideAtmospheres.Visible = true;
                }
                break;
            case GameStateEnum.Played:
                Timer();
                break;
        }
    }

    private void Timer()
    {
        if (TimerIsRunning)
        {
            if (_remainingTime >= _winningTime)
            {
                _teshMeshProInformationMessage.text = _winningMessage;
                _menu.gameObject.SetActive(true);

                _camera.SetActive(true);
                _playerElements.SetActive(false);

                _showHideAtmospheres.Visible = false;

                Time.timeScale = 0f;
            }
            else if (_remainingTime > 0)
            {
                _remainingTime -= Time.deltaTime;

                _uiMaterial.SetFloat("_Progress", _remainingTime / _winningTime);
            }
            else
            {
                TimerIsRunning = false;
                TimerFinishEvent?.Invoke();

                // Restart canvas appear
                _teshMeshProInformationMessage.text = _losingMessage;
                _menu.gameObject.SetActive(true);

                _camera.SetActive(true);
                _playerElements.SetActive(false);
                _showHideAtmospheres.Visible = false;
                Time.timeScale = 0f;
            }
        }
    }

    public void LightIsHarvest(GameObject light)
    {
        // On ajoute notre lumière à notre personnage
        LightHarvest.Add(light);
        Debug.Log($"Player have {LightHarvest.Count()} ");

        // On change la couleur de notre soleil
        _actualSunColor = light.GetComponent<TreeCollectible>().Color;
        _handleSun.SunColor = _actualSunColor;
        _uiMaterial.SetColor("_ColorSun", _actualSunColor);

        // On icrément le temps restant
        _remainingTime += _addingTime;
    }

    public void PlayedActived(bool active)
    {
        foreach (var playedGameObject in _playedGameObjects)
        {
            playedGameObject.SetActive(active);
        }
    }
}

