using System.Collections;
using UnityEngine;

public class TrocaSprites : MonoBehaviour
{
    public SpriteRenderer spriteRenderer1;
    public SpriteRenderer spriteRenderer2;
    public float duracaoFade = 1f;
    public float tempoDeEspera = 1f;

    void Start()
    {
        // Inicialize a visibilidade das sprites
        spriteRenderer1.color = new Color(1f, 1f, 1f, 1f);
        spriteRenderer2.color = new Color(1f, 1f, 1f, 0f);

        // Inicie o loop de fade
        StartCoroutine(LoopFade());
    }

    IEnumerator LoopFade()
    {
        while (true)
        {
            yield return StartCoroutine(FadeSprites(spriteRenderer1, spriteRenderer2));
            yield return new WaitForSeconds(tempoDeEspera);

            yield return StartCoroutine(FadeSprites(spriteRenderer2, spriteRenderer1));
            yield return new WaitForSeconds(tempoDeEspera);
        }
    }

    IEnumerator FadeSprites(SpriteRenderer spriteSaindo, SpriteRenderer spriteEntrando)
    {
        float tempoDecorrido = 0f;

        while (tempoDecorrido < duracaoFade)
        {
            spriteSaindo.color = new Color(1f, 1f, 1f, Mathf.Lerp(1f, 0f, tempoDecorrido / duracaoFade));
            spriteEntrando.color = new Color(1f, 1f, 1f, Mathf.Lerp(0f, 1f, tempoDecorrido / duracaoFade));

            tempoDecorrido += Time.deltaTime;
            yield return null;
        }

        // Garanta que a transição seja concluída corretamente
        spriteSaindo.color = new Color(1f, 1f, 1f, 0f);
        spriteEntrando.color = new Color(1f, 1f, 1f, 1f);
    }
}
