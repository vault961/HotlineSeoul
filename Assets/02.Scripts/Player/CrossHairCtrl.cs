﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossHairCtrl : MonoBehaviour {
    
	void Update () {
        transform.position = Input.mousePosition;
	}
}
