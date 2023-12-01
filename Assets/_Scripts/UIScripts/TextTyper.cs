using UnityEngine;
using UnityEngine.UI;

public class TextTyper : MonoBehaviour
{

	private static TextTyper instance;

	public static TextTyper Instance
	{
		get
		{
			if (instance == null)
			{
				Debug.LogError("TextTyper is null!");
			}
			return instance;
		}
	}
	[Header("Settings")]
	[SerializeField] private float typeSpeed = 0.1f;
	[SerializeField] private float startDelay = 0.3f;
	[SerializeField] private float volumeVariation = 0.1f;

	[Header("Components")]
	[SerializeField] private AudioSource mainAudioSource;

	private bool typing = false;
	private int counter = 0;
	public GameObject dialogPanel, namePanel;
	private Text dialogText, nameText;
	public string[] dialogLines;
	private int currentLine = 0, totalLineCount = 0;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}

		if (!mainAudioSource)
		{
			Debug.Log("No AudioSource has been set. Set one if you wish you use audio features.");
		}
	}

	private void Start()
	{
		dialogPanel = GameObject.Find("Dialog Panel");
		namePanel = GameObject.Find("Dialog Panel/Name Panel");

		dialogText = dialogPanel.GetComponentInChildren<Text>();
		nameText = namePanel.GetComponentInChildren<Text>();

		counter = 0;

		closeDialog();
	}

	public void startDialog(string[] newLines, bool isPerson)
	{
		dialogText.text = "";
		dialogLines = newLines;
		totalLineCount = dialogLines.Length;
		dialogPanel.SetActive(true);
		namePanel.SetActive(isPerson);
		currentLine = 0;
		counter = 0;

		StartTyping();
	}

	public void StartTyping()
	{
		if (!typing)
		{
			checkNameTag();
			InvokeRepeating("Type", startDelay, typeSpeed);
		}
		else
		{
			Debug.LogWarning(gameObject.name + " : Is already typing!");
		}
	}

	public void StopTyping()
	{
		counter = 0;
		typing = false;
		CancelInvoke("Type");
	}

	public bool UpdateText()
	{
		if (currentLine < totalLineCount - 1)
		{
			if (typing)
			{
				StopTyping();
			}
			dialogText.text = "";
			updateDialogLine();
			StartTyping();
			return true;
		}
        else
        {
			closeDialog();
			return false;
        }
	}

	public void QuickSkip()
	{
		if (typing)
		{
			StopTyping();
			dialogText.text = dialogLines[currentLine];
		}
	}

	private void Type()
	{
		typing = true;
		dialogText.text = dialogText.text + dialogLines[currentLine][counter];
		Debug.Log(currentLine);
		counter++;

		if (mainAudioSource)
		{
			mainAudioSource.Play();
			RandomiseVolume();
		}

		if (counter == dialogLines[currentLine].Length)
		{
			typing = false;
			CancelInvoke("Type");
		}
	}

	private void RandomiseVolume()
	{
		mainAudioSource.volume = Random.Range(1 - volumeVariation, volumeVariation + 1);
	}

	public void updateDialogLine()
	{
		StopTyping();
		currentLine++;
	}

	public void checkNameTag()
	{
		if (dialogLines[currentLine].StartsWith("n-"))
		{
			nameText.text = dialogLines[currentLine].Replace("n-", "");
			currentLine++;
		}
	}

	public void closeDialog()
	{
		StopTyping();
		dialogPanel.SetActive(false);
	}

	public bool IsTyping() { return typing; }
}