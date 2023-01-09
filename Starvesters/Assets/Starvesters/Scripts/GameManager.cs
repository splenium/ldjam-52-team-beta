using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Color = UnityEngine.Color;
using Debug = UnityEngine.Debug;

public class GameManager : MonoBehaviour
{
    public enum GameStateEnum { Introduction, Played, Ended }
    public List<GameObject> _playedGameObjects;
    public List<GameObject> _introductionGameObjects;

    public GameStateEnum _gameState;
    public GameObject _playerElements;
    public GameObject _camera;

    public float _winningTime;
    public float _remainingTime;

    public Color _actualSunColor;
    public HandleSun _handleSun;
        
    public Canvas _menu;
    public TextMeshProUGUI _teshMeshProInformationMessage;
    private const string _winningMessage = "Well played , you've won";
    private const string _losingMessage = "Nice try, but you lose";

    public Color[] AllColors;
    public bool UseForceIndex;
    public int ForcedIndex;

    [Tooltip("Plus la valeur est grande plus toute les couleurs seront égale pour le GameManager (calcul de distance entre la couleur du collectible et celle de la target)")]
    public float ColorSensitive = 0.2f;

    public Color TargetColor { get; private set; }

    public Material _uiMaterial;

    private List<GameObject> LightHarvest { get; set; }
    public static GameManager Instance { get; private set; }
    
    public delegate void TimerFinish();
    public event TimerFinish TimerFinishEvent;

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

    void SetSunColor(Color newColor)
    {
        _actualSunColor = newColor;
        _handleSun.SunColor = newColor;
        _uiMaterial.SetColor("_ColorSun", newColor);
    }

    public void Start()
    {
        if(AllColors == null || AllColors.Length == 0)
        {
            Debug.LogError("GameManager.AllColors cannot be empty or null !");
        }

        LightHarvest = new List<GameObject>();
        Time.timeScale = 1f;

        InitState();

        TargetColor = AllColors[UseForceIndex ? ForcedIndex : Random.Range(0, AllColors.Length - 1)];
        SetSunColor(TargetColor);
    }

    void InitState()
    {
        switch (_gameState)
        {
            case GameStateEnum.Introduction:
                PlayedActived(false);
                foreach (var playedGameObject in _introductionGameObjects)
                {
                    playedGameObject.SetActive(true);
                }

                _showHideAtmospheres.Visible = false;

                break;
            case GameStateEnum.Played:
                PlayedActived(true);
                foreach (var playedGameObject in _introductionGameObjects)
                {
                    playedGameObject.SetActive(false);
                }

                _showHideAtmospheres.Visible = true;
                break;
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

                    InitState();
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
        TreeCollectible collectible = light.GetComponent<TreeCollectible>();
        if(collectible)
        {
            // Compare la distance entre les couleurs
            Vector3 collectibleColorVectorized = new Vector3(collectible.Color.r, collectible.Color.g, collectible.Color.b);
            Vector3 targetColorVectorized = new Vector3(TargetColor.r, TargetColor.g, TargetColor.b);
            bool sameColor = Vector3.Distance(collectibleColorVectorized, targetColorVectorized) < 0.2f;

            //Debug.Log($"{sameColor}, distance was {Vector3.Distance(collectibleColorVectorized, targetColorVectorized)} target is {TargetColor.ToHexString()} and collectible was {collectible.Color.ToHexString()}");
            if (sameColor)
            {
                Debug.Log("Harvest Light !");
                //LightHarvest.Add(light);
                //Debug.Log($"Player have {LightHarvest.Count()}");

                // On icrément le temps restant
                _remainingTime += collectible._timeGain;
            }
        }
    }

    public void PlayedActived(bool active)
    {
        foreach (var playedGameObject in _playedGameObjects)
        {
            playedGameObject.SetActive(active);
        }
    }
}

