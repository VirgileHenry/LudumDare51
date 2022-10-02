using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    enum GameState {
        Menu,
        CutScene,
        Playing,
        Victory,
        Defeat,
    }

    GameState currentState;
    public GameObject playerPrefab;
    GameObject player;
    public Camera menuCamera;

    public GameObject victoryScreen;
    public GameObject defeatScreen;

    public static GameManager instance;

    private void Awake() {
        if(instance == null) {
            instance = this;
        }
    }

    private void Start() {
        currentState = GameState.Menu;
    }

    public void StartGame() {
        StartCoroutine(MainGameLoop());
    }

    private IEnumerator MainGameLoop() {
        // intro animation
        currentState = GameState.CutScene;
        // todo

        player = Instantiate(playerPrefab, Planet.instance.mainParent);
        menuCamera.enabled = false;
        // main game loop
        currentState = GameState.Playing;
        float timer = 0; // start at negative if need time at the beginning !

        while(currentState == GameState.Playing) {
            // wait 10 sec ! (this is the game jam theme, I mean...)
            timer += Time.deltaTime;
            if(timer > 10) {
                timer = 0;
                // handle planet tile collapse
                Planet.instance.UpdateTilesStates();
            }

            yield return new WaitForEndOfFrame();
        }
        // handle defeat
        if(currentState == GameState.Defeat) {
            // todo : cutscene !
            Destroy(player);
            menuCamera.enabled = true;
            defeatScreen.SetActive(true);
        }

        // handle victory
        if(currentState == GameState.Victory) {
            // todo : cutscene !
            Destroy(player);
            menuCamera.enabled = true;
            victoryScreen.SetActive(true);
        }
    }

    public void PlayerDefeat() {
        currentState = GameState.Defeat;
    }

    public void PlayerVictory() {
        currentState = GameState.Victory;
    }

    public void ResetGame() {
        if(player != null) {
            Destroy(player);
        }
        menuCamera.enabled = true;
        currentState = GameState.Menu;
        Planet.instance.ResetPlanet();
    }
}
