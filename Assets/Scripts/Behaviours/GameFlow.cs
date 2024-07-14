using System;
using System.Collections;
using Unity.Entities;
using Unity.VisualScripting;
using UnityEngine;

public class GameFlow : MonoBehaviour
{
    [SerializeField] GameObject gameOverFrame;
    [SerializeField] GameObject winFrame;
    [SerializeField] EcsInitialization init;

    EntityManager em;
    EntityQuery gameStateQuery;
    Entity gameStateEntity;

    bool gameOverShown;
    bool winShown;

    IEnumerator Start()
    {
        while(!World.DefaultGameObjectInjectionWorld.IsCreated) yield return null;
        em = World.DefaultGameObjectInjectionWorld.EntityManager;
        gameStateEntity = em.CreateSingleton<GameStateData>();
    }

    void Update()
    {
        if (!em.Exists(gameStateEntity)) return;
        if (init.IsGenerating) return;
        GameStateData gameState = em.GetComponentData<GameStateData>(gameStateEntity);

        if (gameState.GameOver && !gameOverShown) ShowGameOver();
        if (!gameState.GameOver && gameOverShown) HideGameOver();
        if (gameState.GameWon && !winShown) ShowWinScreen();
        if (!gameState.GameWon && winShown) HideWinScreen();

        if(winShown || gameOverShown)
        {
            if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
            if (Input.GetKeyDown(KeyCode.Space))
            {
                em.SetComponentData<GameStateData>(gameStateEntity, new() { GameOver = false, GameWon = false });
                HideGameOver();
                HideWinScreen();
                init.Generate();
            }
        }
    }

    void ShowGameOver()
    {
        gameOverFrame.SetActive(true);
        gameOverShown = true;
    }

    void HideGameOver()
    {
        gameOverFrame.SetActive(false);
        gameOverShown = false;
    }

    void ShowWinScreen()
    {
        winFrame.SetActive(true);
        winShown = true;
    }

    void HideWinScreen()
    {
        winFrame.SetActive(false);
        winShown = false;
    }
}

public struct GameStateData : IComponentData
{
    public bool GameOver;
    public bool GameWon;
    public int RebelCount;
}
