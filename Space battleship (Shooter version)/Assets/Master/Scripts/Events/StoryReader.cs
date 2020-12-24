using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;
using Ink.Runtime;


public class StoryReader : MonoBehaviour
{
    public TextAsset inkJSONAsset;
    public Story story;

    public Text mainText;
    public Button buttonPrefab;

    public GameObject buttonsParents;
    public Button[] buttons;
    public GameObject nextStepButton;

    public static event Action<Story> OnCreateStory;

    private void Awake()
    {
        //StartStory();
    }

    // Creates a new Story object with the compiled story which we can then play!
    public void StartStory()
    {
        nextStepButton.SetActive(false);

        story = new Story(inkJSONAsset.text);
        if (OnCreateStory != null) OnCreateStory(story);
        RefreshView();
    }

    void RefreshView()
    {
        // Remove all the UI on screen
        RemoveButtons();

        // Read all the content until we can't continue any more
        while (story.canContinue)
        {
            // Continue gets the next line of the story
            string text = story.Continue();
            // This removes any white space from the text.
            text = text.Trim();
            // Display the text on screen!
            CreateContentView(text);
        }

        // Display all the choices, if there are any!
        if (story.currentChoices.Count > 0)
        {
            for (int i = 0; i < story.currentChoices.Count; i++)
            {
                Choice choice = story.currentChoices[i];
                Button button = CreateChoiceView(choice.text.Trim());
                // Tell the button what to do when we press it
                button.onClick.AddListener(delegate {
                    OnClickChoiceButton(choice);
                });
            }
        }
        else
        {
            nextStepButton.SetActive(true);
        }
    }

    // When we click the choice button, tell the story to choose that choice!
    void OnClickChoiceButton(Choice choice)
    {
        story.ChooseChoiceIndex(choice.index);
        RefreshView();
    }

    // Creates a textbox showing the the line of text
    void CreateContentView(string text)
    {
        mainText.text = text;
    }

    void RemoveButtons()
    {
        int childCount = buttonsParents.transform.childCount;
        for (int i = childCount - 1; i >= 0; --i)
        {
            GameObject.Destroy(buttonsParents.transform.GetChild(i).gameObject);
        }
    }

    // Creates a button showing the choice text
    Button CreateChoiceView(string text)
    {
        // Creates the button from a prefab
        Button choice = Instantiate(buttonPrefab) as Button;
        choice.transform.SetParent(buttonsParents.transform, false);

        // Gets the text from the button prefab
        Text choiceText = choice.GetComponentInChildren<Text>();
        choiceText.text = text;

        // Make the button expand to fit the text
        HorizontalLayoutGroup layoutGroup = choice.GetComponent<HorizontalLayoutGroup>();
        layoutGroup.childForceExpandHeight = false;

        return choice;
    }
}
