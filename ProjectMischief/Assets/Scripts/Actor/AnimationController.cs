using UnityEngine;
using System.Collections;

public class AnimationController : MonoBehaviour 
{
    Animator anim;
    public float speed = 1.2F;
    bool isWalking;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        anim.Play("idle");
    }

}
