using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum CardStatus
{
    Opening,
    Closing,
    Still
}

public class MainImage : MonoBehaviour
{
    //[SerializeField] private GameObject image_unknown;
    //[SerializeField] private GameObject image_unknown2;
    [SerializeField] private GameControllerScript gameController;
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private Sprite imageBack1;
    [SerializeField] private Sprite imageBack2;
    [SerializeField] private float flipSpeed;

    private Sprite imageFront;
    private Sprite imageBack;
    private SpriteRenderer spriteRenderer;
    private bool isOpen;
    private bool flipHorizontal;
    private float zFlip;
    private float flipAngle;

    private bool useFirst = true;

    private CardStatus status = CardStatus.Still;

    private void Awake()
    {
        particles.Stop();
        //spriteRenderer.sprite = imageBack;
    }

    void Start()
    {
        //particles = gameObject.GetComponent<ParticleSystem>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        isOpen = false;
        zFlip = 0f;
    }

    void Update()
    {
        switch(status)
        {
            case CardStatus.Opening:
                flipAngle += 180f * flipSpeed * Time.deltaTime;

                if (spriteRenderer.sprite != imageFront && flipAngle >= 90f)
                {
                    spriteRenderer.sprite = imageFront;
                    if (!flipHorizontal)
                        zFlip = 180f;
                }

                if (flipAngle >= 180f)
                {
                    flipAngle = 180f;
                    status = CardStatus.Still;
                }

                if (flipHorizontal)
                    transform.rotation = Quaternion.Euler(0f, flipAngle, zFlip);
                else
                    transform.rotation = Quaternion.Euler(flipAngle, 0f, zFlip);

                break;

            case CardStatus.Closing:
                flipAngle -= 180f * flipSpeed * Time.deltaTime;

                if (spriteRenderer.sprite == imageFront && flipAngle <= 90f)
                {
                    spriteRenderer.sprite = imageBack;
                    if (!flipHorizontal)
                        zFlip = 0f;
                }

                if (flipAngle <= 0f)
                {
                    flipAngle = 0f;
                    status = CardStatus.Still;
                }

                if (flipHorizontal)
                    transform.rotation = Quaternion.Euler(0f, flipAngle, zFlip);
                else
                    transform.rotation = Quaternion.Euler(flipAngle, 0f, zFlip);

                break;

            default:
                break;
        }
    }

    public void SetCardToUse(bool value)
    {
        this.useFirst = value;
        if (useFirst)
        {
            //GameObject temp = image_unknown;
            //image_unknown = image_unknown2;
            //image_unknown2 = temp;
            imageBack = imageBack1;
        }
        else
        {
            imageBack = imageBack2;
        }

        //spriteRenderer = image_unknown.GetComponent<SpriteRenderer>();
        //imageBack = image_unknown.GetComponent<SpriteRenderer>().sprite;
        GetComponent<SpriteRenderer>().sprite = imageBack;

        //image_unknown.SetActive(false);
        //image_unknown2.SetActive(false);
    }

    public void OnMouseDown()
    {
        if (gameController.canOpen && !isOpen && status != CardStatus.Opening) //image_unknown.activeSelf)
        {
            Open();
            gameController.SetCardOpened(this);
        }		
    }

    private int _spriteId;

    public int spriteId
    {
        get { return _spriteId; }
    }

    public void ChangeSprite(int id, Sprite image)
    {
        _spriteId = id;
        imageFront = image;
        //spriteRenderer.sprite = image;
    }

    public void Burst()
    {
        particles.Play();
    }

    public void Open()
    {
        //image_unknown.SetActive(false);
        isOpen = true;
        StartCoroutine(RotateOpen());
    }
	
    public void Close()
    {
        //image_unknown.SetActive(true);
        isOpen = false;
        StartCoroutine(RotateClose());
    }

    private IEnumerator RotateOpen()
    {
        flipHorizontal = Random.value < 0.5f;
        status = CardStatus.Opening;
        flipAngle = 0f;
        yield return 0;
        /*

        for (float i = 0f; i < 180f; i += 10f)
        {
            if (i == 90f)
            {
                spriteRenderer.sprite = imageFront;
                if (!flipHorizontal)
                    zFlip = 180f;
            }

            if (flipHorizontal)
                transform.rotation = Quaternion.Euler(0f, i, zFlip);
            else
                transform.rotation = Quaternion.Euler(i, 0f, zFlip);

            yield return new WaitForSeconds(0.015f);
        }
        */
    }

    private IEnumerator RotateClose()
    {
        status = CardStatus.Closing;
        flipAngle = 180f;
        yield return 0;
        /*
        for (float i = 180f; i >= 0f; i -= 10f)
        {
            if (flipHorizontal)
                transform.rotation = Quaternion.Euler(0f, i, zFlip);
            else
                transform.rotation = Quaternion.Euler(i, 0f, zFlip);

            if (i == 90f)
            {
                spriteRenderer.sprite = imageBack;
                if (!flipHorizontal)
                    zFlip = 0f;
            }
            yield return new WaitForSeconds(0.015f);
        }
        */
    }
}
