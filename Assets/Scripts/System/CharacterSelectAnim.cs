using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectAnim : MonoBehaviour
{
    public Animator anim;

    public void PlayGoku()
    {
        anim.Play("goku");
    }

    public void PlayVegeta()
    {
        anim.Play("vegeta");
    }

    public void PlayFide()
    {
        anim.Play("fide");
    }
}
