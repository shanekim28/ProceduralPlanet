﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        transform.Translate(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"), Space.World);
    }
}