using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class QuizContent
{
    public string questionText;
    public bool isOX;
    public List<Sprite> answerImgs = new List<Sprite>();
    public int answerNum;
    public List<Sprite> correctFeedback = new List<Sprite>();
    public List<Sprite> wrongFeedback = new List<Sprite>();
}
public class QuizManager : MonoBehaviour
{
    [SerializeField] private GameObject _QuizOX;
    [SerializeField] private GameObject _Quiz4Answer;
    [SerializeField] private GameObject _CorrectQuiz;
    [SerializeField] private GameObject _WrongQuiz;

    [SerializeField] private int curQuizIndex = 0;
    private QuizContent _curQuizContent;
    [SerializeField] private int totalQuizCnt;
    [SerializeField] private int totalCorrectCnt = 0;
    [SerializeField] List<QuizContent> _quiz = new List<QuizContent>();

    [SerializeField] private TextMeshProUGUI _questionTextOX;
    [SerializeField] private TextMeshProUGUI _questionText4Answer;
    [SerializeField] private Image _Answer1Img;
    [SerializeField] private Image _Answer2Img;
    [SerializeField] private Image _Answer3Img;
    [SerializeField] private Image _Answer4Img;


    void Start()
    {
        totalQuizCnt = _quiz.Count;
        ShowQuiz();
    }

    public void ShowQuiz()
    {
        QuizContent quizContent = _quiz[curQuizIndex];
        _CorrectQuiz.SetActive(false);

        if (quizContent != null)
        {
            _curQuizContent = quizContent;
            if (quizContent.isOX)
            {
                SetOXQuiz(quizContent);
                _QuizOX.SetActive(true);
            }
            else
            {
                Set4AnserQuiz(quizContent);
                _Quiz4Answer.SetActive(true);
            }
        }
    }

    public void SetOXQuiz(QuizContent quizContent)
    {
        // Set Question Text
        _questionTextOX.text = quizContent.questionText;

        // Set Correct Feedback Image
        _CorrectQuiz.GetComponent<Image>().sprite = quizContent.correctFeedback[0];

        // Set Wrong Feedback Image
        //_WrongQuiz.GetComponent<Image>().sprite = quizContent.wrongFeedback[0];
    }

    public void Set4AnserQuiz(QuizContent quizContent)
    {
        // Set Question Text
        _questionText4Answer.text = quizContent.questionText;

        // Set Answer Images
        _Answer1Img.sprite = quizContent.answerImgs[0];
        _Answer2Img.sprite = quizContent.answerImgs[1];
        _Answer3Img.sprite = quizContent.answerImgs[2];
        _Answer4Img.sprite = quizContent.answerImgs[3];

        // Set Correct Feedback Image
        _CorrectQuiz.GetComponent<Image>().sprite = quizContent.correctFeedback[0];

        // Set Wrong Feedback Images Later. Because, This Quiz have 4 answers.
    }

    public void SelectAnswerO()
    {
        if(_curQuizContent.answerNum == 0)
        {
            ShowCorrectFeedback();
        }
        else
        {
            ShowWrongFeedback();
        }
    }

    public void SelectAnswerX()
    {
        if (_curQuizContent.answerNum == 1)
        {
            ShowCorrectFeedback();
        }
        else
        {
            ShowWrongFeedback();
        }
    }

    public void SelectAnswerIndex(int answerIndex)
    {
        if(_curQuizContent.answerNum == answerIndex)
        {
            totalCorrectCnt++;
            _Quiz4Answer.SetActive(false);
            _CorrectQuiz.SetActive(true);
        }
        else
        {
            _WrongQuiz.GetComponent<Image>().sprite = _curQuizContent.wrongFeedback[answerIndex];
            _Quiz4Answer.SetActive(false);
            _WrongQuiz.SetActive(true);
        }
    }

    public void ShowCorrectFeedback()
    {
        totalCorrectCnt++;
        _QuizOX.SetActive(false);
        _CorrectQuiz.SetActive(true);
    }

    public void ShowWrongFeedback()
    {
        _QuizOX.SetActive(false);
        _WrongQuiz.SetActive(true);
    }

    public void MoveToNextQuiz()
    {
        curQuizIndex++;
        if (curQuizIndex >= totalQuizCnt)
        {
            OnQuizEnd();
        }
        else
        {
            ShowQuiz();
        }
    }

    public void OnQuizEnd()
    {
        Debug.LogWarning($"퀴즈 끝. 스코어 {totalCorrectCnt}/{totalQuizCnt}");
    }
}
