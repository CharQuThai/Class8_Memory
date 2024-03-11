using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Card : MonoBehaviour
{
    [SerializeField] GameObject cardBack;
    [SerializeField] SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSprite(Sprite image)
    {
        spriteRenderer.sprite = image;
    }

    public Sprite GetSprite() 
    { 
        return spriteRenderer.sprite;
    }

    public void SetFaceVisible(bool faceVisible)
    {
        Debug.Log("sfv:" + faceVisible);
        cardBack.SetActive(!faceVisible);
        Debug.Log("card back active? " + cardBack.activeInHierarchy);
    }

    private void OnMouseDown()
    {
        //SetFaceVisible(true);
        Messenger<Card>.Broadcast(GameEvent.CARD_CLICKED, this);
    }
}
