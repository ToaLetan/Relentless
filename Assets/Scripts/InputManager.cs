using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct Keybinds
{
    public KeyCode Key_Up;
    public KeyCode Key_Down;
    public KeyCode Key_Left;
    public KeyCode Key_Right;
    public KeyCode Key_Interact;
    public KeyCode Key_Menu;
}

public class InputManager
{
    public Dictionary<string, int> KeyIDs = new Dictionary<string, int>();

    public delegate void KeyPressedEvent(List<string> keysPressed);
    public delegate void KeyHeldEvent(List<string> keysHeld);
    public delegate void KeyReleasedEvent(List<string> keysReleased);

    public event KeyPressedEvent Key_Pressed;
    public event KeyHeldEvent Key_Held;
    public event KeyReleasedEvent Key_Released;

    public Keybinds PlayerKeybinds = new Keybinds();

    private static InputManager instance = null;

    public static InputManager Instance
    {
        get
        {
            if (instance == null)
                instance = new InputManager();
            return instance;
        }
    }

	// Use this for initialization
    private InputManager()
    {
        //Establish KeyCodes for player keybinds.
        PlayerKeybinds.Key_Up = KeyCode.W;
        PlayerKeybinds.Key_Down = KeyCode.S;
        PlayerKeybinds.Key_Left = KeyCode.A;
        PlayerKeybinds.Key_Right = KeyCode.D;
        PlayerKeybinds.Key_Interact = KeyCode.E;
        PlayerKeybinds.Key_Menu = KeyCode.Escape;

        //Populate the KeyIDs Dictionary with the respective KeyCode IDS
        KeyIDs.Add(PlayerKeybinds.Key_Up.ToString(), 0);
        KeyIDs.Add(PlayerKeybinds.Key_Down.ToString(), 1);
        KeyIDs.Add(PlayerKeybinds.Key_Left.ToString(), 2);
        KeyIDs.Add(PlayerKeybinds.Key_Right.ToString(), 3);
        KeyIDs.Add(PlayerKeybinds.Key_Interact.ToString(), 4);
        KeyIDs.Add(PlayerKeybinds.Key_Menu.ToString(), 5);
	}
	
	// Update is called once per frame
	public void Update () 
    {
        UpdateInput();
	}

    private void UpdateInput()
    {
        //=============================== PRESSED KEYS ==================================
        List<string> allPressedKeys = new List<string>();

        if (Input.GetKeyDown(PlayerKeybinds.Key_Up) )
            allPressedKeys.Add(PlayerKeybinds.Key_Up.ToString() );
        if (Input.GetKeyDown(PlayerKeybinds.Key_Down))
            allPressedKeys.Add(PlayerKeybinds.Key_Down.ToString());
        if (Input.GetKeyDown(PlayerKeybinds.Key_Left))
            allPressedKeys.Add(PlayerKeybinds.Key_Left.ToString());
        if (Input.GetKeyDown(PlayerKeybinds.Key_Right))
            allPressedKeys.Add(PlayerKeybinds.Key_Right.ToString());
        if (Input.GetKeyDown(PlayerKeybinds.Key_Interact))
            allPressedKeys.Add(PlayerKeybinds.Key_Interact.ToString());
        if (Input.GetKeyDown(PlayerKeybinds.Key_Menu))
            allPressedKeys.Add(PlayerKeybinds.Key_Menu.ToString());

        if (allPressedKeys.Count > 0)
        {
            if (Key_Pressed != null)
                Key_Pressed(allPressedKeys);
        }
        //============================================================================

        //=============================== HELD KEYS ==================================
        List<string> allHeldKeys = new List<string>();

        if (Input.GetKey(PlayerKeybinds.Key_Up))
            allHeldKeys.Add(PlayerKeybinds.Key_Up.ToString());
        if (Input.GetKey(PlayerKeybinds.Key_Down))
            allHeldKeys.Add(PlayerKeybinds.Key_Down.ToString());
        if (Input.GetKey(PlayerKeybinds.Key_Left))
            allHeldKeys.Add(PlayerKeybinds.Key_Left.ToString());
        if (Input.GetKey(PlayerKeybinds.Key_Right))
            allHeldKeys.Add(PlayerKeybinds.Key_Right.ToString());
        if (Input.GetKey(PlayerKeybinds.Key_Interact))
            allHeldKeys.Add(PlayerKeybinds.Key_Interact.ToString());
        if (Input.GetKey(PlayerKeybinds.Key_Menu))
            allHeldKeys.Add(PlayerKeybinds.Key_Menu.ToString());

        if (allHeldKeys.Count > 0)
        {
            if (Key_Held != null)
                Key_Held(allHeldKeys);
        }
        //================================================================================

        //=============================== RELEASED KEYS ==================================
        List<string> allReleasedKeys = new List<string>();

        if (!Input.GetKey(PlayerKeybinds.Key_Up))
            allReleasedKeys.Add(PlayerKeybinds.Key_Up.ToString());
        if (!Input.GetKey(PlayerKeybinds.Key_Down))
            allReleasedKeys.Add(PlayerKeybinds.Key_Down.ToString());
        if (!Input.GetKey(PlayerKeybinds.Key_Left))
            allReleasedKeys.Add(PlayerKeybinds.Key_Left.ToString());
        if (!Input.GetKey(PlayerKeybinds.Key_Right))
            allReleasedKeys.Add(PlayerKeybinds.Key_Right.ToString());
        if (!Input.GetKey(PlayerKeybinds.Key_Interact))
            allReleasedKeys.Add(PlayerKeybinds.Key_Interact.ToString());
        if (!Input.GetKey(PlayerKeybinds.Key_Menu))
            allReleasedKeys.Add(PlayerKeybinds.Key_Menu.ToString());

        if (allReleasedKeys.Count > 0)
        {
            if (Key_Released != null)
                Key_Released(allReleasedKeys);
        }
        //================================================================================
    }
}
