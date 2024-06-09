using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;
using UnityEngine.ProBuilder;

public class ForceBraking : MonoBehaviour
{
    [SerializeField] PlayerControls player;

    void OnTriggerEnter(Collider collider)
    {
        player.ToggleForcedBraking();
    }
}
