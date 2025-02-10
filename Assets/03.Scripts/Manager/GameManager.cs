using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Networking;

public class GameManager : Singleton<GameManager>
{
    public enum GameState
    {
        Loading,
        Ready,
        Playing,
    }
    public GameState gameState = GameState.Loading;

    async void Start()
    {
        //�� �ҷ������� ���� ���� -> Ready
        gameState = GameState.Ready;
        Debug.Log("Game Ready");
    }

    void Update()
    {
        if(gameState == GameState.Ready)
        {
            //ȭ�� Ŭ�� ��, �������� ���� ������ �̵�
        }
        
    }
}
