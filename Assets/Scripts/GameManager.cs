using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject m_letterPrefab;
    [SerializeField] private Transform m_centre;
    [SerializeField] private Button m_alphabetButton;
    [SerializeField] private Transform m_buttonParent;
    [SerializeField] private TextMeshProUGUI m_currentTimeTMP;
    [SerializeField] private TextMeshProUGUI m_BestTimeTMP;
    [SerializeField] private TextMeshProUGUI m_AttemptsTMP;
    [SerializeField] private TextMeshProUGUI m_correctWordTMP;
    [SerializeField] private TextMeshProUGUI m_winCurrentTimeTMP;
    [SerializeField] private TextMeshProUGUI m_winBestTimeTMP;
    [SerializeField] private GameObject m_losePanel;
    [SerializeField] private GameObject m_winPanel;

    private string m_wordToGuess;
    private int m_lengthOfWordToGuess;
    private char[] m_lettersToGuess;
    private bool[] m_isLetterGuessed;

    private float m_time;
    private int m_noOfAttempts;
    private int m_maxAttempts = 10;

    
    void Start()
    {
        m_losePanel.SetActive(false);
        m_winPanel.SetActive(false);
        m_noOfAttempts = m_maxAttempts;
        m_AttemptsTMP.text = $"Attempts: {m_noOfAttempts}/{m_maxAttempts}";
        m_BestTimeTMP.text = "BestTime: "+PlayerPrefs.GetFloat("BestTime",0).ToString("00") + "s";

        InitGame();
        InitLetters();
        CreateKeyBoard();
    }

    void Update()
    {
        UpdateTime();
        //CheckKeyBoard();
        CheckPuzzleSolved();
    }

    private void InitLetters()
    {
        for (int i = 0; i < m_lengthOfWordToGuess; i++)
        {
            Vector3 newPos = new(m_centre.position.x + ((i - (m_lengthOfWordToGuess - 1) / 2) * 150f), m_centre.position.y, m_centre.position.z);
            GameObject letter = Instantiate(m_letterPrefab, newPos, Quaternion.identity);
            letter.name = "Letter " + (i + 1);
            letter.transform.SetParent(m_centre);
        }
    }

    private void InitGame()
    {
        m_wordToGuess = PickWordFromFile();
        m_wordToGuess = m_wordToGuess.ToUpper();
        m_lengthOfWordToGuess = m_wordToGuess.Length; Debug.Log(m_lengthOfWordToGuess);
        m_lettersToGuess = m_wordToGuess.ToCharArray();
        m_isLetterGuessed = new bool[m_lettersToGuess.Length];
    }

    private void CheckKeyBoard()
    {
        if (Input.anyKeyDown && !string.IsNullOrEmpty(Input.inputString))
        {
            char letterPressed = Input.inputString[0];
            letterPressed = char.ToUpper(letterPressed);
            int letterPressedAsInt = System.Convert.ToInt32(letterPressed);
            if (letterPressedAsInt >= 65 && letterPressedAsInt <= 90)
            {
                bool isCorrectGuess = false;
                for (int i = 0; i < m_lettersToGuess.Length; i++)
                {
                    if (!m_isLetterGuessed[i] && m_lettersToGuess[i] == letterPressed)
                    {
                        GameObject.Find("Letter " + (i + 1)).GetComponentInChildren<TextMeshProUGUI>().text = letterPressed.ToString();
                        m_isLetterGuessed[i] = true;
                        isCorrectGuess = true;
                        break;
                    }
                }
                if (!isCorrectGuess)
                {
                    UpdateAttempts();
                }
            }
        }
    }

    private void CreateKeyBoard()
    {
        for (char letter = 'A'; letter <= 'Z'; letter++)
        {
            Button alphabetBTN = Instantiate(m_alphabetButton, m_buttonParent);
            alphabetBTN.name = $"Button_{letter}";
            var buttonTMP = alphabetBTN.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonTMP != null)
                buttonTMP.text = letter.ToString();
            if (alphabetBTN != null)
            {
                char capturedLetter = letter;
                alphabetBTN.onClick.RemoveAllListeners();
                alphabetBTN.onClick.AddListener(()=>OnClickAlphabet(capturedLetter));
                
            }
        }
    }

    private void OnClickAlphabet(char letterPressed)
    {
        bool correctGuess = false;
        for (int i = 0; i < m_lengthOfWordToGuess; i++)
        {
            if (!m_isLetterGuessed[i] && m_lettersToGuess[i] == letterPressed)
            {
                GameObject.Find("Letter " + (i + 1)).GetComponentInChildren<TextMeshProUGUI>().text = letterPressed.ToString();
                m_isLetterGuessed[i] = true;
                correctGuess = true;
                break;
            }
        }
        if (!correctGuess)
        {
            UpdateAttempts();            
        }
    }

    private string PickWordFromFile()
    {
        TextAsset Word = Resources.Load("WordsFile") as TextAsset;
        string s = Word.text;
        string[] words = s.Split('\n').Select(w => w.Trim()).Where(w => !string.IsNullOrWhiteSpace(w)).ToArray();
        int randomWord = Random.Range(0, words.Length + 1); Debug.Log(words[randomWord]);
        return words[randomWord];
    }
    private void UpdateTime()
    {
        if (IsWordFound()) return;
        m_time += Time.deltaTime;
        m_currentTimeTMP.text = m_time.ToString("00") + "s";
    }
    private void UpdateAttempts()
    {
        m_noOfAttempts = Mathf.Max(m_noOfAttempts - 1, 0);
        m_AttemptsTMP.text = $"Attempts: {m_noOfAttempts}/{m_maxAttempts}";
        if (m_noOfAttempts <= 0)
        {
            EnableLosePanel();
        }
    }
    private bool IsWordFound()
    {
        for (int i = 0; i < m_isLetterGuessed.Length; i++)
        {
            if (!m_isLetterGuessed[i])
                return false;
        }
        return true;
    }
    private void EnableLosePanel()
    {
        StartCoroutine(nameof(LosePanel));
    }

    IEnumerator LosePanel()
    {
        yield return new WaitForSeconds(1f);
        m_losePanel.SetActive(true);
        m_correctWordTMP.text = "CorrectWord is: " + m_wordToGuess.ToString();
    }
    private void CheckPuzzleSolved()
    {
        if (IsWordFound())
        {
            StartCoroutine(nameof(EnableWinPanel));
        }
    }

    IEnumerator EnableWinPanel()
    {
        yield return new WaitForSeconds(1f);
        m_winPanel.SetActive(true);
        SaveBestTime();
    }

    private void SaveBestTime()
    {
        float bestTime = PlayerPrefs.GetFloat("BestTime", float.MaxValue);
        if (m_time < bestTime)
        {
            PlayerPrefs.SetFloat("BestTime", m_time);
        }
        m_winBestTimeTMP.text ="BestTime: "+ PlayerPrefs.GetFloat("BestTime").ToString("00") + "s";
        m_winCurrentTimeTMP.text = "CurrentTime: " + m_time.ToString("00")+"s";
    }
}
