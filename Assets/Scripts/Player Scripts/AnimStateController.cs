using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimStateController : MonoBehaviour {

    Animator anim;

    private void Start() {

        anim = GetComponent<Animator>();
    }
}
