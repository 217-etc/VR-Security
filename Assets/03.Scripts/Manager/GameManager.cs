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
        //Dialogue 데이터 불러오기
        PaserCSV parser = new PaserCSV();
        dialogueData = await parser.Parse<DialogueStructure>();

        //다 불러와지면 게임 상태 -> Ready
        gameState = GameState.Ready;
        Debug.Log("Game Ready");
    }

    void Update()
    {
        if(gameState == GameState.Ready)
        {
            //화면 클릭 시, 스테이지 선택 씬으로 이동
        }
        
    }
}
