using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimationScript : MonoBehaviour 
{
    public enum ResolutionType { Stop, Destroy, Loop, FireEvent_Stop, FireEvent_Destroy };
    public ResolutionType animResolutionType = ResolutionType.Stop;

    public delegate void ResolvedAnimEvent();
    public event ResolvedAnimEvent Anim_Complete;

    public bool CanBePaused = false;

    private AnimationClip[] animationArray;
    private List<string> animationNamesList = new List<string>();

    private string currentAnimName = "";

    private GameManager gameManager = null;

    private Animator objectAnimator = null;
    private AnimatorStateInfo animatorState;

    private bool hasAnimationStopped = false;

    public AnimationClip[] AnimationArray
    { get { return animationArray; } }

    public List<string> AnimationNamesList
    { get { return animationNamesList; } }

    public GameManager Game_Manager
    {
        get { return gameManager; }
        set { gameManager = value; }
    }

    public bool HasAnimationStopped
    {
        get { return hasAnimationStopped; }
        set { hasAnimationStopped = value; }
    }

	// Use this for initialization
	void Start () 
    {
        if (CanBePaused)
            gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        objectAnimator = gameObject.GetComponent<Animator>();

        if (objectAnimator != null)
        {
            RuntimeAnimatorController animController = objectAnimator.runtimeAnimatorController;

            animationArray = animController.animationClips;

            for (int i = 0; i < animationArray.Length; i++)
            {
                animationNamesList.Add(animationArray[i].name);
            }

            currentAnimName = animationNamesList[0];
        }
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (CanBePaused && gameManager != null) //If the game is paused, pause the animation.
        {
            if (gameManager.CurrentGameState == GameManager.GameState.Paused)
                objectAnimator.enabled = false;
            else
            {
                if (!hasAnimationStopped) //Make sure to re-enable animations on any animators that are disabled.
                {
                    if (objectAnimator.enabled == false)
                        objectAnimator.enabled = true;
                }
            }
        }
	}

    private void OnAnimationComplete() //Use this for Animation timeline events, perform an action based on the resolution type.
    {
        switch(animResolutionType)
        {
            case ResolutionType.Stop:
                hasAnimationStopped = true;
                objectAnimator.enabled = false;
                break;
            case ResolutionType.Destroy:
                Destroy(gameObject);
                break;
            case ResolutionType.Loop:
                break;
            case ResolutionType.FireEvent_Stop:
                if (Anim_Complete != null)
                    Anim_Complete();
                objectAnimator.enabled = false;
                break;
            case ResolutionType.FireEvent_Destroy:
                if (Anim_Complete != null)
                    Anim_Complete();
                hasAnimationStopped = true;
                Destroy(gameObject);
                break;
        }
    }

    public void ResetAnimator()
    {
        objectAnimator.enabled = true;
        hasAnimationStopped = false;
    }

    public void PlayAnimation(string animationName) //Play an animation, making sure it's not already being played.
    {
        ResetAnimator();

        animatorState = objectAnimator.GetCurrentAnimatorStateInfo(0);

        if (animatorState.IsName(animationName) == false)
            objectAnimator.Play(animationName);

        currentAnimName = animationName;
    }

    public void PlayDirectionalAnimation(string animationName) //Play an animation at the time of the current animation, used by walk anims
    {
        ResetAnimator();

        float currentAnimTime = 0.0f;

        animatorState = objectAnimator.GetCurrentAnimatorStateInfo(0);

        if (animatorState.IsName(animationName) == false)
        {
            currentAnimTime = animatorState.normalizedTime % 1;

            objectAnimator.Play(animationName, -1, currentAnimTime);
        }
        currentAnimName = animationName;
    }
}
