using System;
using System.Collections;
using Enums;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _splashScreen;
    [SerializeField]
    private GameObject _selectScreen;
    [SerializeField]
    private GameObject _startPrompt;
    [SerializeField] 
    private GameObject _victoryScreen;
    [SerializeField] 
    private GameObject _startScreen;
    [SerializeField] 
    private Text _startText;
    [SerializeField] 
    private GameObject _introScreen;
    [SerializeField] 
    private Text _introNameText;
    [SerializeField] 
    private Text _introFlavourText;

    private readonly string[] _startWords = new[]
        { "FIGHT", "TO", "THE", "DEATH", "YOU", "CAN", "ONLY", "TURN", "RIGHT"};

    private readonly string[] _flavour = new[]
    {
        "union rep",
        "antifascist",
        "cocaine enthusiast",
        "mother of two",
        "likes opera",
        "history professor",
        "convicted felon",
        "divorced",
        "party animal",
        "pro-choice activist",
        "bank robber",
        "demolitions expert",
        "amateur brain surgeon",
        "volunteer firefighter",
        "cryptoanarchist",
        "foot fetishist",
        "idiot savant",
        "train driver",
        "engineer",
        "architect",
        "terminally ill",
        "wedding planner",
        "ex military",
        "designated marksman",
        "black belt",
        "hoarder",
        "kind as fuck",
        "immortal",
        "secretly a horse",
        "multilingual",
        "speaks Swahili",
        "Shakespearean actor",
        "soprano",
        "recovering alcoholic",
        "published author",
        "disgraced politician",
        "patient zero",
        "photophopbic",
        "sadomasochist",
        "chaotic neutral",
        "jealous lover"
    };

    private void OnEnable()
    {
        GameManager.StateChanged += HandleStateChanged;
        PlayerManager.EnoughPlayers += HandleEnoughPlayers;
        CameraController.StartIntro += HandleStartIntro;
        CameraController.EndIntros += HandleEndIntros;
    }

    private void OnDisable()
    {
        GameManager.StateChanged -= HandleStateChanged;
        PlayerManager.EnoughPlayers -= HandleEnoughPlayers;
        CameraController.StartIntro -= HandleStartIntro;
        CameraController.EndIntros -= HandleEndIntros;
    }
    
    private void HandleEndIntros()
    {
        _selectScreen.SetActive(true);
    }

    private void HandleStartIntro(KeyCode key)
    {
        if (_selectScreen.activeSelf)
        {
            _selectScreen.SetActive(false);
        }

        StartCoroutine(IntroSlideShow(key));
    }

    private void HandleEnoughPlayers()
    {
        if (!_startPrompt.activeSelf)
        {
            _startPrompt.SetActive(true);
        }
    }

    private void HandleStateChanged(GameState newState)
    {
        switch (newState)
        {
            case GameState.Entry:
                break;
            case GameState.PlayerSelect:
                _splashScreen.SetActive(false);
                _selectScreen.SetActive(true);
                break;
            case GameState.Started:
                StopAllCoroutines();
                _introScreen.SetActive(false);
                _selectScreen.SetActive(false);
                StartCoroutine(SlideShowWords());
                break;
            case GameState.Ended:
                _victoryScreen.SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException("newState", newState, null);
        }
    }

    readonly WaitForSeconds _wait25 = new WaitForSeconds(0.25f);
    readonly WaitForSeconds _wait125 = new WaitForSeconds(0.125f);
    readonly WaitForSeconds _wait5 = new WaitForSeconds(0.5f);

    private IEnumerator SlideShowWords()
    {
        _startScreen.SetActive(true);
        for (int i = 0; i < _startWords.Length; i++)
        {
            _startText.text = _startWords[i];
            yield return _wait25;
        }
        
        _startScreen.SetActive(false);
    }

    private const string Quotes = "\"{0}\"";

    private IEnumerator IntroSlideShow(KeyCode key)
    {
        _selectScreen.SetActive(false);
        yield return _wait125;
        _introScreen.SetActive(true);
        _introNameText.text = string.Format(Quotes, key.ToString());
        _introFlavourText.text = String.Empty;
        yield return _wait25;
        string first = GetFlavour();
        _introFlavourText.text = first;
        yield return _wait5;
        string second;
        do
        {
            second = GetFlavour();
        } while (second == first);
        _introFlavourText.text = second;
        yield return _wait5;
        string third;
        do
        {
            third = GetFlavour();
        } while (third == first || third == second);
        _introFlavourText.text = third;
        yield return _wait5;
        _introScreen.SetActive(false);

    }

    private string GetFlavour()
    {
        return _flavour[Random.Range(0, _flavour.Length)];
    }
}
