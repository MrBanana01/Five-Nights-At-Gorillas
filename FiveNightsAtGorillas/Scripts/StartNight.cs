﻿using FiveNightsAtGorillas.Managers;
using Photon.Pun;
using UnityEngine;

namespace FiveNightsAtGorillas.Other
{
    public class StartNight : MonoBehaviour
    {
        public int Night;

        void Awake() { PhotonNetwork.AddCallbackTarget(this); gameObject.layer = 18; }

        void OnTriggerEnter(Collider other)
        {
            if (other.name == "LeftHandTriggerCollider" || other.name == "RightHandTriggerCollider")
            {
                if (other.name == "LeftHandTriggerCollider")
                {
                    GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength / 2, GorillaTagger.Instance.tapHapticDuration);
                }
                else if (other.name == "RightHandTriggerCollider")
                {
                    GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tapHapticStrength / 2, GorillaTagger.Instance.tapHapticDuration);
                }

                if (Night == 7 && RefrenceManager.Data.GD.text == "1" && RefrenceManager.Data.BD.text == "9" && RefrenceManager.Data.DD.text == "8" && RefrenceManager.Data.MD.text == "7")
                {
                    FNAG.Data.Jumpscare();
                    return;
                }
                else
                {
                    FNAG.Data.StartGame(Night, RefrenceManager.Data.GD.text.ToString(), RefrenceManager.Data.MD.text.ToString(), RefrenceManager.Data.BD.text.ToString(), RefrenceManager.Data.DD.text.ToString());
                }
            }
        }
    }
}