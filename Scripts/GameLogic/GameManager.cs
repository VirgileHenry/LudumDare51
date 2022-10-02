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

    private void Start() {
        currentState = GameState.Menu;
        // StartGame();
    }

    public void StartGame() {
        StartCoroutine(MainGameLoop());
    }

    private IEnumerator MainGameLoop() {
        // intro animation
        currentState = GameState.Menu;
        // todo

        // main game loop
        currentState = GameState.Playing;
        float timer = 0; // start at negative if need time at the beginning !

        while(currentState == GameState.Playing) {
            // wait 10 sec ! (this is the game jam theme, I mean...)
            timer += Time.deltaTime;

            // handle planet tile collapse
            Planet.instance.UpdateTilesStates();

            yield return new WaitForEndOfFrame();
        }
        // handle defeat
        // handle victory
    }

    public void PlayerDefeat() {
        currentState = GameState.Defeat;
    }

    public void PlayerVictory() {
        currentState = GameState.Victory;
    }
}
