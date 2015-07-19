using UnityEngine;
using System.Collections;

public class ButtonScript : MonoBehaviour 
{
    private bool isMouseOver = false;

    public bool IsMouseOver
    {
        get { return isMouseOver; }
        set { isMouseOver = value; }
    }

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    private void OnMouseOver()
    {
        if(gameObject.GetComponent<SpriteRenderer>().enabled == true)
            isMouseOver = true;
    }

    private void OnMouseExit()
    {
        if (gameObject.GetComponent<SpriteRenderer>().enabled == true)
            isMouseOver = false;
    }
}
