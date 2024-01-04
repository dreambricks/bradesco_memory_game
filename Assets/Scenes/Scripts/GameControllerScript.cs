using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
//using UDPSender;

public class Kalman
{
    private float m_x0;
    private float m_P0;
    private float m_R0;
    private float m_Q0;
    private float m_x;
    private float m_P;
    private float m_R;
    private float m_Q;
    private float m_maxv;

    public Kalman(float x0, float P0, float R, float Q, float maxv = -1)
    {
        m_x0 = x0;
        m_P0 = P0;
        m_R0 = R;
        m_Q0 = Q;
        m_x = m_x0;
        m_P = m_P0;
        m_R = m_R0;
        m_Q = m_Q0;
        m_maxv = maxv;
        if (maxv == -1)
            m_maxv = x0 * 2;
    }

    public void reset()
    {
        m_x = m_x0;
        m_P = m_P0;
        m_R = m_R0;
        m_Q = m_Q0;
    }

    public float filter(float z)
    {
        if (z > m_maxv)
            z = m_maxv;

        float K = m_P / (m_P + m_R);
        m_x = m_x + K * (z - m_x);
        m_P = (1 - K) * m_P + m_Q;


        return m_x;
    }
}

public class GameControllerScript : MonoBehaviour
{
    enum Status
    {
        CallToAction,
        WaittingToStart,
        Show,
        Playing,
        Fail,
        Finished
    };

    [SerializeField] private float TIME_CARDS_OPEN;// = 4.0f;
    [SerializeField] private float TIME_GUESS_OPEN;// = 0.6f;
    [SerializeField] private float TIME_PLAY;// = 30.0f;

    private const int NUM_COLUMNS = 4;
    private const int NUM_ROWS = 4;
    private const int NUM_CARDS = (NUM_COLUMNS * NUM_ROWS) / 2;

    public const float SPACE_X = 2.2f;
    public const float SPACE_Y = -2.2f;

    //[SerializeField] private UDPSender udpSender;

    private MainImage[] allCards;
    private MainImage cardOpened1;
    private MainImage cardOpened2;
    private int score;
    private int attempts;
    private float timeLeft;
    private Status status;

    //private Kalman fpsK;

    [SerializeField] private MainImage card;
    [SerializeField] private Sprite[] images;

    //private MainImage firstOpen;
    //private MainImage secondOpen;

    //private int score = 0;
    //private int attempts = 0;

    //[SerializeField] private TextMesh scoreText;
    [SerializeField] private TMPro.TextMeshPro scoreText;
    [SerializeField] private TMPro.TextMeshPro attemptsText;
    [SerializeField] private TMPro.TextMeshPro timerText;
    [SerializeField] private TMPro.TextMeshPro buttonText;

    [SerializeField] private GameObject callToActionScreen;
    [SerializeField] private GameObject prepareGame;
    [SerializeField] private GameObject failScreen;
    [SerializeField] private GameObject qrCodeScreen;
    [SerializeField] private ParticleSystem particles;

    private void Awake()
    {
        particles.Stop();
    }

    private void Start()
    {
        Debug.Log("Memory Game");
        //fpsK = new Kalman(60f, 1f, 1f, 1f);

        //udpSender = new UDPSender();
        //udpSender.init();
        //udpSender.sendString("M");

        status = Status.CallToAction;
        cardOpened1 = null;
        cardOpened2 = null;
        score = 0;
        attempts = 0;
        timeLeft = 0;
        allCards = new MainImage[NUM_COLUMNS * NUM_ROWS];
        failScreen.SetActive(false);
        qrCodeScreen.SetActive(false);

        int[] locations = GenerateRandomLocations(NUM_CARDS);

        Vector3 startPosition = card.transform.position;

        for (int i = 0; i < NUM_COLUMNS; i++)
        {
            for (int j = 0; j < NUM_ROWS; j++)
            {
                MainImage newCard;
                if (i == 0 && j == 0)
                {
                    newCard = card;
                }
                else
                {
                    newCard = Instantiate(card) as MainImage;
                }

                int index = j * NUM_COLUMNS + i;
                allCards[index] = newCard;
                int id = locations[index];
                //newCard.SetId(id);
                newCard.ChangeSprite(id, images[id]);
                float positionX = (SPACE_X * i) + startPosition.x;
                float positionY = (SPACE_Y * j) + startPosition.y;

                newCard.transform.position = new Vector3(positionX, positionY, startPosition.z);
                if ((i+j) % 2 == 1)
                {
                    newCard.SetCardToUse(false);
                }
                else
                {
                    newCard.SetCardToUse(true);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //float fps = fpsK.filter(1.0f / Time.deltaTime);
       // attemptsText.text = string.Format("{0:00}", fps);

        switch (status)
        {
            case Status.Playing:
                timeLeft += Time.deltaTime;

                if (timeLeft >= TIME_PLAY)
                {
                    status = Status.Fail;
                    StartCoroutine(GameFailed());
                }
                timerText.text = FormatarTempo(timeLeft);
                break;

            case Status.Finished:
                timeLeft -= Time.deltaTime;
                if (timeLeft <= 0.0f)
                {
                   Restart();
                   Debug.Log("ACABOU");
                }
                buttonText.text = string.Format("{0:00}", timeLeft);
                break;

            default:
                break;
        }
    }

    string FormatarTempo(float tempoEmSegundos)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(tempoEmSegundos);
        return string.Format("{0:00}:{1:00}", timeSpan.Minutes, timeSpan.Seconds);
    }

    private void Restart()
    {
        SceneManager.LoadScene("MainScene");
    }

    private void ResetTimer()
    {
        timeLeft = 20f;
    }

    public bool canOpen
    {
        get { return (status == Status.Playing || status == Status.WaittingToStart) && cardOpened2 == null; }
    }

    public void StartGame()
    {
        prepareGame.SetActive(false);
        Debug.Log("Starting Game");
        status = Status.Show;
        StartCoroutine(ShowCards());
    }

    public void PrepareGame()
    {
        Debug.Log("Preparing Game");
        callToActionScreen.SetActive(false);
        prepareGame.SetActive(true);
    }

    public void SetCardOpened(MainImage opened)
    {
        switch(status)
        {
            case Status.WaittingToStart:
                Debug.Log("Starting Game");
                status = Status.Show;
                StartCoroutine(ShowCards());
                return;

            default:
                break;
        }
        if (cardOpened1 == null)
        {
            cardOpened1 = opened;
        }
        else
        {
            cardOpened2 = opened;
            StartCoroutine(CheckGuessed());
        }
    }

    private void OpenAllCards()
    {
        for(int i = 0; i < NUM_COLUMNS * NUM_ROWS; i++)
        {
            allCards[i].Open();
        }
    }
    private void CloseAllCards()
    {
        for (int i = 0; i < NUM_COLUMNS * NUM_ROWS; i++)
        {
            allCards[i].Close();
        }
    }

    private IEnumerator ShowCards()
    {
        yield return new WaitForSeconds(0.5f);

        OpenAllCards();
        yield return new WaitForSeconds(TIME_CARDS_OPEN);

        CloseAllCards();
        yield return new WaitForSeconds(0.5f);

        status = Status.Playing;
    }

    private IEnumerator GameFailed()
    {
        yield return new WaitForSeconds(1.0f);
        failScreen.SetActive(true);


        yield return new WaitForSeconds(10.0f);
        SceneManager.LoadScene("MainScene");
    }

    private IEnumerator GameSuccess()
    {
        
        yield return new WaitForSeconds(1.0f);
        qrCodeScreen.SetActive(true);
        particles.Play();

        yield return new WaitForSeconds(10.0f);
        SceneManager.LoadScene("MainScene");

    }



    private IEnumerator CheckGuessed()
    {
        if (cardOpened1.spriteId == cardOpened2.spriteId)
        {
            cardOpened1.Burst();
            cardOpened2.Burst();
            score++;
            scoreText.text = "Pontos: " + score;
            //scoreTextPro.text = "Score: " + score;
            if (score == NUM_CARDS)
            {
                status = Status.Finished;
                ResetTimer();
                StartCoroutine(GameSuccess());
            }
        }
        else
        {
            yield return new WaitForSeconds(TIME_GUESS_OPEN);

            cardOpened1.Close();
            cardOpened2.Close();
        }

        attempts++;
        //attemptsText.text = "Tentativas: " + attempts;

        cardOpened1 = null;
        cardOpened2 = null;
    }

    private int[] GenerateRandomLocations(int numCards)
    {
        return Randomiser(GenerateLocations(numCards));
    }

    private int[] GenerateLocations(int numCards)
    {
        int[] result = new int[numCards * 2];
        for (int i = 0; i < numCards; i++)
        {
            result[2 * i + 0] = i;
            result[2 * i + 1] = i;
        }

        return result;
    }

    private int[] Randomiser(int[] locations)
    {
        int[] array = locations.Clone() as int[];
        for (int i = 0; i < array.Length; i++)
        {
            int newArray = array[i];
            int j = UnityEngine.Random.Range(i, array.Length);
            array[i] = array[j];
            array[j] = newArray;
        }
        return array;
    }

    public void imageOpened(MainImage startObject)
    {
        if (cardOpened1 == null)
        {
            cardOpened1 = startObject;
        }
        else
        {
            cardOpened2 = startObject;
            StartCoroutine(CheckGuessed());
        }
    }

}
