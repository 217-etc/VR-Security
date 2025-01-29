using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Networking;

public class GameManager : Singleton<GameManager>
{
    public Dictionary<string, DialogueStructure> dialogueData;
    public enum GameState
    {
        Loading,
        Ready,
        Playing,
    }
    public GameState gameState = GameState.Loading;

    async void Start()
    {
        //Dialogue ������ �ҷ�����
        PaserCSV parser = new PaserCSV();
        dialogueData = await parser.Parse<DialogueStructure>();

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
