using UnityEngine;
using UnityEditor; //Used to access the Asset Database
using System.Collections;
using System.Collections.Generic;

public class AnimTool : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    public void CreateAnimationClip(List<Object> sprites, string animationName, string pathFolder, int animSpeed, bool loopAnim)
    {
        int numEmptySprites = 0;

        if(sprites.Count > 0)
        {
            for(int i = 0; i < sprites.Count; i++)
            {
                if (sprites[i] == null)
                    numEmptySprites++;
            }

            if(numEmptySprites == 0) //Populate the animation if all sprites have been set
            {
                AnimationClip newClip = new AnimationClip();
                AnimationCurve clipCurve = new AnimationCurve();

                EditorCurveBinding animatorCurve = new EditorCurveBinding(); //Used to animate Objects (in this case, Sprites)
                animatorCurve.type = typeof(SpriteRenderer); //Set the binder type.
                animatorCurve.path = "";
                animatorCurve.propertyName = "m_Sprite"; //Set the binder property.

                ObjectReferenceKeyframe[] animationFrames = new ObjectReferenceKeyframe[sprites.Count];

                newClip.frameRate = animSpeed;

                //Determine the number of decimal places for keyframe timing. NOTE THAT THIS DOESN'T WORK ANYWAYS.
                /*int numDigits = Mathf.FloorToInt(Mathf.Log10(animSpeed) + 1);
                float timingDivision = Mathf.Pow(10, numDigits);


                float animationTimingModifier = ((animSpeed / timingDivision) % 10) / animSpeed;
                float currentFrameTime = 0.00f;*/

                for (int j = 0; j < sprites.Count; j++)
                {
                    animationFrames[j] = new ObjectReferenceKeyframe();

                    animationFrames[j].value = sprites[j];

                    animationFrames[j].time = j; //TODO: FIX THIS, CURRENTLY ARRANGES FRAMES AT 1 PER SECOND
                }

                if(loopAnim == true) //NOTE: This doesn't work, toggle Loop Time manually.
                    newClip.wrapMode = WrapMode.Loop;

                AnimationUtility.SetObjectReferenceCurve(newClip, animatorCurve, animationFrames);

                if (pathFolder != "") //If the specified folder doesn't exist, create it.
                {
                    if (AssetDatabase.IsValidFolder("Assets/Resources/Animations/" + pathFolder) == false)
                    {
                        string newFolder = AssetDatabase.CreateFolder("Assets/Resources/Animations", pathFolder);
                        string newFolderPath = AssetDatabase.GUIDToAssetPath(newFolder);

                        AssetDatabase.SaveAssets(); //Make sure to save every time you change the Asset Database

                        Debug.Log(pathFolder + " did not exist, created new folder in Resources/Animations!");
                    }


                    if (AssetDatabase.FindAssets("Assets/Resources/Animations/" + pathFolder + "/" + animationName).Length == 0) //If the asset doesn't already exist, create a new one
                    {
                        AssetDatabase.CreateAsset(newClip, "Assets/Resources/Animations/" + pathFolder + "/" + animationName + ".anim");
                        AssetDatabase.SaveAssets();
                    }
                    else //NOTE: This currently never gets called
                        Debug.Log("Animation not created, asset name already in use at " + pathFolder + "!");
                    
                }
                else //Otherwise if no folder was specified, throw an error.
                    Debug.Log("Animation not created, no path folder specified!"); 
            }
            else //If there are any blank sprites, throw an error.
                Debug.Log("Animation not created, some sprites are not set!"); 
        }
        else //If the sprites list is empty, throw an error.
            Debug.Log("Animation not created, sprites list is empty!"); 
    }
}
