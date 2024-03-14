using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SceneController : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform cardSpawnPoint;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] private Sprite[] cardImages;
    private List<Card> cards;

    Card card1 = null;
    Card card2 = null;
    private int score = 0;

    public string swapSortingLayerName = "Swap";

    private void Awake()
    {
        Messenger<Card>.AddListener(GameEvent.CARD_CLICKED, this.OnCardClicked);
    }

    private void OnDestroy()
    {
        Messenger<Card>.RemoveListener(GameEvent.CARD_CLICKED, this.OnCardClicked);
    }

    private void Start()
    {
        //Card card = CreateCard(cardSpawnPoint.position);
        //int imageIndex = Random.Range(0, cardImages.Length);
        //card.SetSprite(cardImages[imageIndex]);
        cards = CreateCards();
        AssignImagesToCards();
        Debug.Log("cardcount:" + cards.Count);
        foreach (Card card in cards)
        {
            card.SetFaceVisible(false);
        }
    }

    Card CreateCard(Vector3 pos)
    {
        GameObject obj = Instantiate(cardPrefab, pos, cardPrefab.transform.rotation);
        Card card = obj.GetComponent<Card>();
        return card;
    }

    public void OnCardClicked(Card card)
    {
        Debug.Log(this + ".OnCardClicked()");
        if (card1 == null)
        {
            card1 = card;
            card1.SetFaceVisible(true);
        }
        else if (card2 == null)
        {
            card2 = card;
            card2.SetFaceVisible(true);
            StartCoroutine(EvaluatePair());
            
        }
        else
        {
            Debug.Log("ignoring click");
        }
    }
    IEnumerator EvaluatePair()
    {
        if (card1.GetSprite() == card2.GetSprite())
        {
            Debug.Log("Match");
            score++;
            scoreText.text = score.ToString();
        }
        else
        {
            Debug.Log("Not a Match");

            SetSortingLayer(card1.gameObject, "Infront");
            SetSortingLayer(card2.gameObject, "Infront");

            iTween.MoveTo(card1.gameObject, iTween.Hash("position", card2.transform.position, "time", 1.5f, "easetype", iTween.EaseType.easeInOutBack));
            iTween.MoveTo(card2.gameObject, iTween.Hash("position", card1.transform.position, "time", 1.5f, "easetype", iTween.EaseType.easeInOutBack));

            yield return new WaitForSeconds(1.5f);

            SetSortingLayer(card1.gameObject, "Foreground");
            SetSortingLayer(card2.gameObject, "Foreground");

            card1.SetFaceVisible(false);
            card2.SetFaceVisible(false);
            //MoveCards();
        }
        card1 = null;
        card2 = null;
    }


    // Create (and return) a List of cards organized in a grid layout
    private List<Card> CreateCards()
    {
        List<Card> newCards = new List<Card>();
        int rows = 2;           // # of rows
        int cols = 4;           // # of columns
        float xOffset = 2f;     // # of units between cards horizontally
        float yOffset = -2.5f;  // # of units between cards vertically

        // Create cards and position on a grid
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                Vector3 offset = new Vector3(x * xOffset, y * yOffset, 0);    // calculate the offset                
                Card card = CreateCard(cardSpawnPoint.position + offset);     // create the card          
                newCards.Add(card);                                           // add the card to the list
            }
        }
        return newCards;
    }

    // Assign images to the cards in pairs
    private void AssignImagesToCards()
    {
        // create a list of paired image indices - the # of entries MUST match the # of cards.
        // eg: [0,0,1,1,2,2,3,3]
        List<int> imageIndices = new List<int>();
        for (int i = 0; i < cardImages.Length; i++)
        {
            imageIndices.Add(i);    // one index for the first card in the pair
            imageIndices.Add(i);    // one index for the second
        }

        //    // *** TODO: write code to shuffle the list of image indices
        ShuffleCards(imageIndices);


        //    // Go through each card in the game and assign it an image based on the (shuffled) list of indices.
        for (int i = 0; i < cards.Count; i++)
        {
            int imageIndex = imageIndices[i];           // use the card # to index into the imageIndices array
            cards[i].SetSprite(cardImages[imageIndex]); // set the image on the card
        }
    }


    private void ShuffleCards(List<int> list)
    {
        System.Random rng = new System.Random();

        int n = list.Count;
        while (n > 1)
        {
            n--;
            int r = rng.Next(n + 1);
            int temp = list[r];
            list[r] = list[n];
            list[n] = temp;
        }
    }

    public void OnResetButtonPressed()
    {
        Reset();
    }

    private void Reset()
    {
        score = 0;
        card1 = null;
        card2 = null;

        foreach (Card card in cards) 
        {
            card.SetFaceVisible(false);
        }

        AssignImagesToCards();
        scoreText.text = "0";
    }

    void SetSortingLayer(GameObject obj, string sortingLayerName)
    {
        SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingLayerName = sortingLayerName;
        }
    }

    //void MoveCards()
    //{
    //    iTween.MoveTo(this.gameObject, iTween.Hash("position", card2, "time", 2.5f, "easetype", iTween.EaseType.easeInOutSine));
    //}

}
