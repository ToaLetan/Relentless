using UnityEngine;
using UnityEditor; //Required for tool development
using System.Collections;
using System.Collections.Generic;

[CustomEditor (typeof(AnimTool))]
public class AnimationPopulator : Editor 
{
    List<Object> spriteListTest = new List<Object>();

    string animName = "";
    string pathFolder = "";

    int numSprites = 0;
    int prevNumSprites = 0;
    int animSpeed = 6;

    bool loopAnim = false;

    public override void OnInspectorGUI() //Used to overwrite the UI in the Unity editor Inspector panel
    {
        GUILayout.BeginHorizontal();
        numSprites = EditorGUILayout.IntField("Num of Sprites:", numSprites);
        GUILayout.EndHorizontal();

        if (prevNumSprites != numSprites)
        {
            int diffNumSprites = numSprites - prevNumSprites;

            if (diffNumSprites > 0) //Add to the sprite list
            {
                for (int i = 0; i < diffNumSprites; i++)
                {
                    Object newSprite = null;
                    spriteListTest.Add(newSprite);
                }
            }
            else if (diffNumSprites < 0 && spriteListTest.Count > 0) //Subtract from the sprite list.
            {
                for (int j = 0; j < -diffNumSprites; j++)
                {
                    spriteListTest.RemoveAt(spriteListTest.Count - 1 - j);
                }
            }
            
            prevNumSprites = numSprites;
        }

        for (int j = 0; j < numSprites; j++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Sprite " + j + ": ");

            spriteListTest[j] = EditorGUILayout.ObjectField(spriteListTest[j], typeof(Sprite), false); //This lets the user select a sprite.

            GUILayout.EndHorizontal();
        }

        GUILayout.BeginHorizontal();
        GUILayout.Label("Animation Name: ");
        animName = EditorGUILayout.TextField(animName);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Path Folder: ");
        pathFolder = EditorGUILayout.TextField(pathFolder);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Animation Speed: ");
        animSpeed = EditorGUILayout.IntField(animSpeed);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Loop Animation: ");
        loopAnim = EditorGUILayout.Toggle(loopAnim);
        GUILayout.EndHorizontal();

        DrawCreateAnimButton();
        DrawClearSpritesButton();
    }

    private void DrawCreateAnimButton()
    {
        AnimTool animationTool = (AnimTool)target;

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Create Animation"))
        {
            animationTool.CreateAnimationClip(spriteListTest, animName, pathFolder, animSpeed, loopAnim);
        }

        GUILayout.EndHorizontal();
    }

    private void DrawClearSpritesButton()
    {
        AnimTool animationTool = (AnimTool)target;

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Clear All Sprites"))
        {
            numSprites = 0;
            spriteListTest.Clear();
        }

        GUILayout.EndHorizontal();
    }
}
