using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour 
{
    private InputManager inputManager = null;

    private Camera mainCamera = null;

    private GameObject crosshair = null;

	// Use this for initialization
	void Start () 
    {
        Cursor.visible = false; //Hide the mouse cursor

        inputManager = InputManager.Instance;

        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        inputManager.Mouse_Moved += ProcessMousePosition;

        crosshair = GameObject.Find("Crosshair"); 
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    private void ProcessMousePosition(Vector3 mousePosition)
    {
        float crosshairX = 0;
        float crosshairY = 0;

        //float screenPercentX = (mousePosition.x - Screen.width / 2) / Screen.width;
        //float screenPercentY = (mousePosition.y - Screen.height / 2) / Screen.height;

        Vector3 newMousePos = mainCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 1) );

        crosshair.transform.position = newMousePos;
    }
}
