using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartScript : MonoBehaviour
{
    // Start is called before the first frame update
    public Sprite[] SpriteList;
    public int index = 0;

    public void NextSprite()
    {
        index++;
        gameObject.GetComponent<SpriteRenderer>().sprite = SpriteList[index];
    }
}
