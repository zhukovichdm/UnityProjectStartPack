using UnityEngine;
using System.Collections.Generic;
using Scripts.Data;
using Scripts.Behaviours;
using Scripts.Component.Actions;
using Scripts.Component;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class Testing : MonoBehaviour
{
    public static bool TestingMode { get; set; }

    [SerializeField] private DataCameraParameters dataCameraParameters;
    [SerializeField] private ControlAnimator testingPanel;
    [SerializeField] private TMP_Text progressText;
    [SerializeField] private GameObject finalPanel;
    [SerializeField] private TMP_Text finalText;

    [SerializeField] private List<GameObject> buttons;
    [SerializeField] private List<ControlObjectInformation> itemList;

    // Список уникальных названий.
    private List<string> uniqueNames = new List<string>();
    // Ключ - верный ответ. Список - все варианты ответов в случайном порядке. Bool? - ответили ли на вопрос. String - какой ответ был выбран.
    private Dictionary<string, (List<string>, bool?, string)> testingList = new Dictionary<string, (List<string>, bool?, string)>();

    private string currentTrueAnswer;
    private int currentItem;
    private int trueCounter;
    private int falseCounter;

    private void Awake()
    {
        GameActions.SelectedObjectForTesting.Subscribe(UpdatePanel);
        //UserInput.InputEscapeAction.Subscribe(() => testingPanel.SetSign(false));
        ButtonAddListener();
        GenerateTest();
    }

    private void ButtonAddListener()
    {
        foreach (var button in buttons)
            button.GetComponent<Button>().onClick.AddListener(() => AnswerTheQuestion(button));

        void AnswerTheQuestion(GameObject button)
        {
            string text = button.transform.GetChild(1).GetComponent<TMP_Text>().text;

            var temp = testingList[currentTrueAnswer];
            if (temp.Item2 == null)
            {
                bool result = currentTrueAnswer == text;
                temp.Item2 = result;
                temp.Item3 = text;
                testingList[currentTrueAnswer] = temp;
                ColorizeButton(button.GetComponent<Button>(), temp.Item2);

                if (result)
                    trueCounter++;
                else
                    falseCounter++;

                StartCoroutine(GoToNext(0.5f));
            }
        }
    }

    private void GenerateTest()
    {
        uniqueNames.Clear();
        testingList.Clear();

        // Получение уникальных названий названий.
        foreach (var item in itemList)
        {
            var name = item.dataObjectInformation.name;
            if (!uniqueNames.Contains(name))
                uniqueNames.Add(name);
        }
        // Генерация теста.
        foreach (var trueName in uniqueNames)
        {
            List<string> answers = new List<string>();
            var trueAnswerId = Random.Range(0, buttons.Count - 1);

            while (answers.Count < buttons.Count)
            {
                if (answers.Count == trueAnswerId)
                {
                    answers.Add(trueName);
                    continue;
                }

                var falseName = uniqueNames[Random.Range(0, uniqueNames.Count)];
                if (!answers.Contains(falseName) && falseName != trueName)
                    answers.Add(falseName);
            }
            testingList.Add(trueName, (answers, null, ""));
        }
    }

    private void UpdatePanel(ControlObjectInformation controlObjectInformation)
    {
        if (!TestingMode) return;
        if (controlObjectInformation)
        {
            currentTrueAnswer = controlObjectInformation.dataObjectInformation.name;
            if (uniqueNames.Contains(currentTrueAnswer))
            {
                for (int i = 0; i < buttons.Count; i++)
                {
                    buttons[i].transform.GetChild(1).GetComponent<TMP_Text>().text = testingList[currentTrueAnswer].Item1[i];

                    // Если уже отвечали на вопрос то красим в зеленый/красный, иначе в белый.
                    if (testingList[currentTrueAnswer].Item1[i] == testingList[currentTrueAnswer].Item3)
                        ColorizeButton(buttons[i].GetComponent<Button>(), testingList[currentTrueAnswer].Item3 == currentTrueAnswer);
                    else
                        ColorizeButton(buttons[i].GetComponent<Button>(), null);
                }

                testingPanel.SetSignAll(true);

                Debug.Log("Выбран: " + currentTrueAnswer);
                return;
            }
        }
        currentTrueAnswer = "";
        testingPanel.SetSignAll(false);
    }

    private void ColorizeButton(Button button, bool? value)
    {
        var colors = button.colors;
        switch (value)
        {
            case null:
                colors.normalColor = Color.white;
                colors.highlightedColor = Color.white;
                break;
            case true:
                colors.normalColor = Color.green;
                colors.highlightedColor = Color.green;
                break;
            case false:
                colors.normalColor = Color.red;
                colors.highlightedColor = Color.red;
                break;
        }
        button.colors = colors;
    }

    private IEnumerator GoToNext(float time)
    {
        progressText.text =
                     $"Всего: {testingList.Count}\n" +
                     $"Ответов: {currentItem}\n";

        if (itemList.Count == currentItem)
        {
            finalText.text =
               $"Всего: {testingList.Count}\n" +
               $"Верно: {trueCounter}\n" +
               $"Не верно: {falseCounter}";
            finalPanel.SetActive(true);
        }

        yield return new WaitForSeconds(time);
        if (itemList.Count > currentItem)
        {
            GameActions.LookAt.Publish((itemList[currentItem].gameObject, CameraModes.Pivot, dataCameraParameters.dataScrollParameter, dataCameraParameters.dataLimitationParameter));
            GameActions.SelectedObjectForTesting.Publish(itemList[currentItem]);
            currentItem++;
        }
    }

    public void StartTest(bool value)
    {
        testingPanel.SetSignAll(value);
        if (TestingMode = value)
        {
            currentTrueAnswer = "";
            currentItem = 0;
            trueCounter = 0;
            falseCounter = 0;
            finalPanel.SetActive(false);
            GenerateTest();
            StartCoroutine(GoToNext(0));
        }
    }
}
