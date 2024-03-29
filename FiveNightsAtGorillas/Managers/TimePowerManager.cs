﻿using UnityEngine;
using System.Collections;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

namespace FiveNightsAtGorillas.Managers
{
    public class TimePowerManager : MonoBehaviour
    {
        public static TimePowerManager Data;
        public int CurrentPower { get; private set; } = 100;
        public int CurrentPowerDrainTime { get; private set; } = 10;
        public string CurrentTime { get; private set; } = "12AM";
        public bool AllowedToRunTime { get; private set; } = false;
        public bool AllowedToRunPower { get; private set; } = false;
        public int TimerDelay = 100;

        void Awake() { Data = this; }
        
        public void StopEverything() {
            AllowedToRunTime = false;
            AllowedToRunPower = false;
            CurrentTime = "12AM";
            CurrentPower = 100;
            RefreshText();
        }

        public void DingusThing() { if (!SandboxValues.Data.InfinitePower) { CurrentPower -= 3; RefreshText(); } }

        public void StopOnlyPower() {
            AllowedToRunPower = false;
            AllowedToRunTime = true;
            CurrentPower = 0;
            RefreshText();
        }

        public void StartEverything() {
            if (SandboxValues.Data.ShorterNight) { TimerDelay = 70; } else { TimerDelay = 100; }
            if (SandboxValues.Data.SlowPower) { CurrentPowerDrainTime = 20; } else { CurrentPowerDrainTime = 10; }
            if (SandboxValues.Data.FastPower) { CurrentPowerDrainTime = 7; } else { CurrentPowerDrainTime = 10; }
            if (SandboxValues.Data.LimitedPower) { CurrentPower = 70; } else { CurrentPower = 100; }
            AllowedToRunTime = true;
            AllowedToRunPower = true;
            CurrentTime = "12AM";
            CurrentPower = 100;
            RefreshText();
            StartCoroutine(PowerDelay());
            StartCoroutine(TimeDelay());
        }

        public void RefreshDrainTime() {
            if(DoorManager.Data.RightDoorOpen && !DoorManager.Data.LeftDoorOpen) {
                if (SandboxValues.Data.SlowPower) { CurrentPowerDrainTime = 16; } else if (SandboxValues.Data.FastPower) { CurrentPowerDrainTime = 3; } else { CurrentPowerDrainTime = 6; }
                return;
            }
            else if(!DoorManager.Data.RightDoorOpen && DoorManager.Data.LeftDoorOpen) {
                if (SandboxValues.Data.SlowPower) { CurrentPowerDrainTime = 16; } else if (SandboxValues.Data.FastPower) { CurrentPowerDrainTime = 3; } else { CurrentPowerDrainTime = 6; }
                return;
            }
            else if(DoorManager.Data.RightDoorOpen && DoorManager.Data.LeftDoorOpen) {
                if (SandboxValues.Data.SlowPower) { CurrentPowerDrainTime = 20; } else if (SandboxValues.Data.FastPower) { CurrentPowerDrainTime = 7; } else { CurrentPowerDrainTime = 10; }
                return;
            }
            else if(!DoorManager.Data.RightDoorOpen && !DoorManager.Data.LeftDoorOpen) {
                if (SandboxValues.Data.SlowPower) { CurrentPowerDrainTime = 8; } else if (SandboxValues.Data.FastPower) { CurrentPowerDrainTime = (int)0.5; } else { CurrentPowerDrainTime = 3; }
            }
        }

        public void RefreshText() {
            RefrenceManager.Data.CurrentPower.text = CurrentPower.ToString();
            RefrenceManager.Data.CurrentTime.text = CurrentTime;
        }

        IEnumerator IPowerDelay() {
            yield return new WaitForSeconds(1);
            StartCoroutine(PowerDelay());
        }
        IEnumerator PowerDelay() {
            yield return new WaitForSeconds(CurrentPowerDrainTime);
            if (AllowedToRunPower && !SandboxValues.Data.InfinitePower) {
                CurrentPower--;
                if (CurrentPower < 0)
                    FNAG.Data.Poweroutage();

                RefreshText();
                StartCoroutine(IPowerDelay());
            }
        }

        IEnumerator ITimeDelay() {
            yield return new WaitForSeconds(1);
            StartCoroutine(TimeDelay());
        }
        IEnumerator TimeDelay() {
            yield return new WaitForSeconds(TimerDelay);

            if (AllowedToRunTime) {
                if (CurrentTime == "5AM") {
                    CurrentTime = "6AM";
                    FNAG.Data.SixAM();
                    yield return "5AM";
                }
                else if (CurrentTime == "4AM") { CurrentTime = "5AM"; yield return "4AM"; }
                else if (CurrentTime == "3AM") { CurrentTime = "4AM"; yield return "3AM"; }
                else if (CurrentTime == "2AM") { CurrentTime = "3AM"; yield return "2AM"; }
                else if (CurrentTime == "1AM") { CurrentTime = "2AM"; yield return "1AM"; }
                else if (CurrentTime == "12AM") { CurrentTime = "1AM"; yield return "12AM"; }

                RefreshText();
                StartCoroutine(ITimeDelay());
            }
        }
    }
}
