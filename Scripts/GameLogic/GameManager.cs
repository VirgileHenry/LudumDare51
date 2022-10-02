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
    public GameObject gameUiPanel;

    public static GameManager instance;

    public Bar fuelBar;
    public Bar sismicBar;
    public Bar spaceshipBar;
    public GameObject spaceshipPanel;

    private void Awake() {
        if(instance == null) {
            instance = this;
        }
    }

    private void Start() {
        currentState = GameState.Menu;
        
    }

    public void StartGame() {
        fuelBar.Set(0);
        sismicBar.Set(0);
        spaceshipBar.Set(0);
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
            sismicBar.Set(timer / 10.0f);
            if(timer > 10) {
                timer = 0;
                player.GetComponent<PlayerController>().ShakeCamera();
                // handle planet tile collapse
                Planet.instance.UpdateTilesStates();
            }

            yield return new WaitForEndOfFrame();
        }
        gameUiPanel.SetActive(false);
        spaceshipPanel.SetActive(false);
        // handle defeat
        if(currentState == GameState.Defeat) {
            // todo : cutscene !
            Destroy(player);
            menuCamera.enabled = true;
            defeatScreen.SetActive(true);
            gameUiPanel.SetActive(false);
        }

        // handle victory
        if(currentState == GameState.Victory) {
            // todo : cutscene !
            // move camera to spaceship nicely
            Transform camTf = player.GetComponentInChildren<Camera>().transform;
            camTf.SetParent(EndAnimation.instance.cameraPos, true);
            Destroy(player);
            float camtimer = 0;
            Vector3 camPos = camTf.localPosition;
            Quaternion camRot = camTf.localRotation;
            while(camtimer < 1) {
                camTf.localPosition = (1 - camtimer) * camPos;
                camTf.localRotation = Quaternion.Lerp(camRot, Quaternion.identity, camtimer);
                camtimer += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            EndAnimation.instance.Launch();
            yield return new WaitForSeconds(5f);
            menuCamera.enabled = true;
            victoryScreen.SetActive(true);
            gameUiPanel.SetActive(false);
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
