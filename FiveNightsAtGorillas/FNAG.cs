﻿using BepInEx;
using FiveNightsAtGorillas.Managers;
using FiveNightsAtGorillas.Other;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using Utilla;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.Video;
using System.Collections.Generic;

namespace FiveNightsAtGorillas {
    [ModdedGamemode("fnag", "FNAG", Utilla.Models.BaseGamemode.Casual)]
    [BepInDependency("org.legoandmars.gorillatag.utilla")]
    [BepInPlugin(FNAGInfo.GUID, FNAGInfo.Name, FNAGInfo.Version)]
    public class FNAG : BaseUnityPlugin {
        public static FNAG Data;
        public bool RoundCurrentlyRunning;
        public bool LocalPlayingRound;
        public bool InCustomRoom { get; private set; }
        public int CurrentPage { get; set; } = 1;
        public bool TestMode { get; private set; } = false;
        public bool GameRunning { get; private set; }
        public int AmountOfPlayersPlaying { get; set; }
        public List<GameObject> triggeredObjects { get; set; }
        public bool beingJumpscared { get; set; }

        void Start() { Events.GameInitialized += OnGameInitialized; Data = this; }

        void OnEnable() { HarmonyPatches.ApplyHarmonyPatches(); }
        void OnDisable() { Data.enabled = true; HarmonyPatches.RemoveHarmonyPatches(); } //Sorry! I can't take the time to figure out how I'm going to disable this properly
        public void SkyColorFullBlack() { RenderSettings.ambientSkyColor = Color.black; }
        public void SkyColorGameBlack() { RenderSettings.ambientSkyColor = RefrenceManager.Data.GameSkyColor.color; }
        public void SkyColorWhite() { RenderSettings.ambientSkyColor = Color.white; }

        void OnGameInitialized(object sender, EventArgs e) {
            triggeredObjects = new List<GameObject>();

            var bundle = LoadAssetBundle("FiveNightsAtGorillas.Assets.fnag");
            var map = bundle.LoadAsset<GameObject>("FNAG MAP");
            var jumpscare = bundle.LoadAsset<GameObject>("Jumpscares");
            var menu = bundle.LoadAsset<GameObject>("Menu");
            var dark = bundle.LoadAsset<GameObject>("Darkness");

            GameObject.Find("BepInEx_Manager").AddComponent<RefrenceManager>();

            RefrenceManager.Data.Menu = Instantiate(menu);
            RefrenceManager.Data.FNAGMAP = Instantiate(map);
            RefrenceManager.Data.Jumpscares = Instantiate(jumpscare);
            RefrenceManager.Data.Darkness = Instantiate(dark);

            RefrenceManager.Data.SetRefrences();

            GameObject[] allobjs = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (GameObject objs in allobjs) {
                if (objs.name == "head_end") {
                    objs.layer = 10;
                    objs.AddComponent<SphereCollider>().isTrigger = true;
                    objs.AddComponent<HeadColliderDisableEvent>();
                }
            }

            SetupHitSounds();
            SetupComps();
            SetupEnemys();
            SetupMenu();
            StartCoroutine(GetInfoStuff());

            if (TestMode) {
                RefrenceManager.Data.Menu.SetActive(true);
                RefrenceManager.Data.FNAGMAP.SetActive(true);
            }
        }

        public bool IsLocalRigInGame() {
            foreach(GameObject objs in triggeredObjects) {
                if (GetNthParent(objs, 5).gameObject.name == "Local Gorilla Player")
                    return true;
                else 
                    return false;
            }

            return false;
        }

        #region EnemySetup
        void SetupEnemys() {
            foreach (GameObject obj in RefrenceManager.Data.gorilla) { obj.SetActive(false); }
            foreach(GameObject obj in RefrenceManager.Data.mingus) { obj.SetActive(false); }
            foreach(GameObject obj in RefrenceManager.Data.dingus) { obj.SetActive(false); }
            foreach(GameObject obj in RefrenceManager.Data.bob) { obj.SetActive(false); }
            RefrenceManager.Data.gorilla[0].SetActive(true);
            RefrenceManager.Data.mingus[0].SetActive(true);
            RefrenceManager.Data.dingus[0].SetActive(true);
            RefrenceManager.Data.bob[0].SetActive(true);
            RefrenceManager.Data.Jumpscare.SetActive(false);
            RefrenceManager.Data.StaticScreen.transform.parent = GorillaTagger.Instance.mainCamera.transform;
            RefrenceManager.Data.StaticScreen.transform.localPosition = new Vector3(0, 0, 0.6f);
            RefrenceManager.Data.StaticScreen.SetActive(false);
            RefrenceManager.Data.FNAGMAP.transform.position = new Vector3(-102.0151f, 23.7944f, -65.1198f);
            RefrenceManager.Data.Jumpscares.transform.localRotation = Quaternion.Euler(0, 180, 0);
            RefrenceManager.Data.Jumpscare.transform.parent = GorillaTagger.Instance.mainCamera.transform;
            RefrenceManager.Data.Jumpscare.transform.localPosition = new Vector3(0, -0.4f, 0.8f);
            RefrenceManager.Data.StaticScreen.transform.localScale = new Vector3(2, 1.3f, 0.3234f);
            RefrenceManager.Data.Darkness.transform.parent = GorillaTagger.Instance.mainCamera.transform;
            RefrenceManager.Data.Darkness.transform.localPosition = new Vector3(0, -0.2f, -0.1f);
            RefrenceManager.Data.Darkness.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
            RefrenceManager.Data.Darkness.SetActive(false);
        }
        #endregion
        #region SetupHitsounds
        void SetupHitSounds()
        {
            GameObject.Find($"{RefrenceManager.Data.FNAGMAP.name}/Office/Floor/Floor").AddComponent<GorillaSurfaceOverride>().overrideIndex = 0;
            GameObject.Find($"{RefrenceManager.Data.FNAGMAP.name}/Office/Walls/Office Walls").AddComponent<GorillaSurfaceOverride>().overrideIndex = 0;
            GameObject.Find($"{RefrenceManager.Data.FNAGMAP.name}/Office/Chair").AddComponent<GorillaSurfaceOverride>().overrideIndex = 3;
            GameObject.Find($"{RefrenceManager.Data.FNAGMAP.name}/TheRest/Deco/Monitors").AddComponent<GorillaSurfaceOverride>().overrideIndex = 146;
            GameObject p = GameObject.Find($"{RefrenceManager.Data.FNAGMAP.name}/Office/Desk/Parts");
            foreach (Transform child in p.transform) {
                child.gameObject.AddComponent<GorillaSurfaceOverride>().overrideIndex = 146;
            }
            GameObject p1 = GameObject.Find($"{RefrenceManager.Data.FNAGMAP.name}/TheRest/Deco/Monitors");
            foreach (Transform child in p1.transform) {
                child.gameObject.AddComponent<GorillaSurfaceOverride>().overrideIndex = 146;
            }
            RefrenceManager.Data.CameraScreen.AddComponent<GorillaSurfaceOverride>().overrideIndex = 29;
            RefrenceManager.Data.CameraScreen.AddComponent<BoxCollider>();
            GameObject.Find($"{RefrenceManager.Data.FNAGMAP.name}/Vent").AddComponent<GorillaSurfaceOverride>().overrideIndex = 18;
            GameObject.Find("RightDoor").AddComponent<GorillaSurfaceOverride>().overrideIndex = 18;
            GameObject.Find("LeftDoor").AddComponent<GorillaSurfaceOverride>().overrideIndex = 18;
        }
        #endregion
        #region SetupComps
        void SetupComps()
        {
            RefrenceManager.Data.LeftDoor.AddComponent<DoorButton>().isLeft = true;
            RefrenceManager.Data.RightDoor.AddComponent<DoorButton>().isLeft = false;
            RefrenceManager.Data.RightLight.AddComponent<LightButton>().isLeft = false;
            RefrenceManager.Data.LeftLight.AddComponent<LightButton>().isLeft = true;
            RefrenceManager.Data.ChainLoader.AddComponent<PhotonData>();
            RefrenceManager.Data.ChainLoader.AddComponent<DoorManager>();
            RefrenceManager.Data.ChainLoader.AddComponent<CameraManager>();
            RefrenceManager.Data.ChainLoader.AddComponent<TimePowerManager>();
            RefrenceManager.Data.ChainLoader.AddComponent<SandboxValues>();
            RefrenceManager.Data.ChainLoader.AddComponent<JoinedRoomChecker>();
            RefrenceManager.Data.gorillaParent.AddComponent<AIManager>().AIName = "gorilla";
            RefrenceManager.Data.mingusParent.AddComponent<AIManager>().AIName = "mingus";
            RefrenceManager.Data.bobParent.AddComponent<AIManager>().AIName = "bob";
            RefrenceManager.Data.dingusParent.AddComponent<AIManager>().AIName = "dingus";
            RefrenceManager.Data.gorillaParent.GetComponent<AIManager>().CamPos = "Cam11";
            RefrenceManager.Data.mingusParent.GetComponent<AIManager>().CamPos = "Cam11";
            RefrenceManager.Data.bobParent.GetComponent<AIManager>().CamPos = "Cam11";
            RefrenceManager.Data.dingusParent.GetComponent<AIManager>().CamPos = "Stage1";
            RefrenceManager.Data.gorillaBoopTrigger.AddComponent<Boop_>();
            RefrenceManager.Data.NearGameTrigger.AddComponent<MPDetectTrigger>();
            RefrenceManager.Data.NearGameTrigger.transform.parent = null;
            RefrenceManager.Data.NearGameTrigger.transform.position = new Vector3(-104.0056f, 23.8f, -65.6826f);

            RefrenceManager.Data.Cam1.AddComponent<CameraButton>().CameraButtonTrigger = "Cam1";
            RefrenceManager.Data.Cam2.AddComponent<CameraButton>().CameraButtonTrigger = "Cam2";
            RefrenceManager.Data.Cam3.AddComponent<CameraButton>().CameraButtonTrigger = "Cam3";
            RefrenceManager.Data.Cam4.AddComponent<CameraButton>().CameraButtonTrigger = "Cam4";
            RefrenceManager.Data.Cam5.AddComponent<CameraButton>().CameraButtonTrigger = "Cam5";
            RefrenceManager.Data.Cam6.AddComponent<CameraButton>().CameraButtonTrigger = "Cam6";
            RefrenceManager.Data.Cam7.AddComponent<CameraButton>().CameraButtonTrigger = "Cam7";
            RefrenceManager.Data.Cam8.AddComponent<CameraButton>().CameraButtonTrigger = "Cam8";
            RefrenceManager.Data.Cam9.AddComponent<CameraButton>().CameraButtonTrigger = "Cam9";
            RefrenceManager.Data.Cam10.AddComponent<CameraButton>().CameraButtonTrigger = "Cam10";
            RefrenceManager.Data.Cam11.AddComponent<CameraButton>().CameraButtonTrigger = "Cam11";

            RefrenceManager.Data.MenuIgnoreButton.AddComponent<IgnoreWarning>();
            RefrenceManager.Data.MenuStartButton[0].AddComponent<StartNight>().Night = 1;
            RefrenceManager.Data.MenuStartButton[1].AddComponent<StartNight>().Night = 2;
            RefrenceManager.Data.MenuStartButton[2].AddComponent<StartNight>().Night = 3;
            RefrenceManager.Data.MenuStartButton[3].AddComponent<StartNight>().Night = 4;
            RefrenceManager.Data.MenuStartButton[4].AddComponent<StartNight>().Night = 5;
            RefrenceManager.Data.MenuStartButton[5].AddComponent<StartNight>().Night = 6;
            RefrenceManager.Data.MenuStartButton[6].AddComponent<StartNight>().Night = 7;
            RefrenceManager.Data.MenuScrollLeftButton.AddComponent<MenuScroll>().isRight = false;
            RefrenceManager.Data.MenuScrollRightButton.AddComponent<MenuScroll>().isRight = true;

            RefrenceManager.Data.MenuCNAddGorilla.AddComponent<CNAdd>().IsGorilla = true;
            RefrenceManager.Data.MenuCNAddMingus.AddComponent<CNAdd>().IsMingus = true;
            RefrenceManager.Data.MenuCNAddBob.AddComponent<CNAdd>().IsBob = true;
            RefrenceManager.Data.MenuCNAddDingus.AddComponent<CNAdd>().IsDingus = true;

            RefrenceManager.Data.MenuCNSubGorilla.AddComponent<CNSub>().IsGorilla = true;
            RefrenceManager.Data.MenuCNSubMingus.AddComponent<CNSub>().IsMingus = true;
            RefrenceManager.Data.MenuCNSubBob.AddComponent<CNSub>().IsBob = true;
            RefrenceManager.Data.MenuCNSubDingus.AddComponent<CNSub>().IsDingus = true;

            RefrenceManager.Data.SwitchPageRight.AddComponent<SwitchPage>().isSub = false;
            RefrenceManager.Data.SwitchPageLeft.AddComponent<SwitchPage>().isSub = true;

            RefrenceManager.Data.BrightOffice.AddComponent<SandboxOption>().IsBrightOffice = true;
            RefrenceManager.Data.AutoCloseDoor.AddComponent<SandboxOption>().IsAutoCloseDoor = true;
            RefrenceManager.Data.AutoSwitchCamera.AddComponent<SandboxOption>().IsAutoSwitchCamera = true;
            RefrenceManager.Data.ShorterNight.AddComponent<SandboxOption>().IsShorterNight = true;
            RefrenceManager.Data.SlowPower.AddComponent<SandboxOption>().IsSlowPower = true;
            RefrenceManager.Data.FastPower.AddComponent<SandboxOption>().IsFastPower = true;
            RefrenceManager.Data.NoCamera.AddComponent<SandboxOption>().IsNoCamera = true;
            RefrenceManager.Data.PitchBlack.AddComponent<SandboxOption>().IsPitchBlack = true;
            RefrenceManager.Data.NoLights.AddComponent<SandboxOption>().IsNoLights = true;
            RefrenceManager.Data.LimitedPower.AddComponent<SandboxOption>().IsLimitedPower = true;
            RefrenceManager.Data.InfinitePower.AddComponent<SandboxOption>().IsInfinitePower = true;
        }
        #endregion
        #region MenuSetup
        void SetupMenu()
        {
            RefrenceManager.Data.Menu.transform.position = new Vector3(-63.078f, 12.4836f, -82.3281f);
            RefrenceManager.Data.Menu.transform.localRotation = Quaternion.Euler(0, 90, 0);

            RefrenceManager.Data.NightOneSelect.SetActive(true);
            RefrenceManager.Data.NightTwoSelect.SetActive(false);
            RefrenceManager.Data.NightThreeSelect.SetActive(false);
            RefrenceManager.Data.NightFourSelect.SetActive(false);
            RefrenceManager.Data.NightFiveSelect.SetActive(false);
            RefrenceManager.Data.NightSixSelect.SetActive(false);
            RefrenceManager.Data.CustomNightSelect.SetActive(false);

            RefrenceManager.Data.MenuScrollLeft.SetActive(false);
            RefrenceManager.Data.MenuScrollRight.SetActive(false);
            RefrenceManager.Data.MenuWarning.SetActive(true);
            RefrenceManager.Data.MenuIgnoreButton.SetActive(true);
            RefrenceManager.Data.MenuSelects.SetActive(false);
            RefrenceManager.Data.MenuRoundRunning.SetActive(false);

            RefrenceManager.Data.Menu.SetActive(false);
            RefrenceManager.Data.FNAGMAP.SetActive(false);

            RefrenceManager.Data.NearGameTrigger.layer = 18;
        }
        #endregion

        AssetBundle LoadAssetBundle(string path) {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            AssetBundle bundle = AssetBundle.LoadFromStream(stream);
            stream.Close();
            return bundle;
        }

        public void Refresh() {
            if (AmountOfPlayersPlaying > 0) {
                RefrenceManager.Data.MenuRoundRunning.SetActive(true);
                RefrenceManager.Data.MenuWarning.SetActive(false);
                RefrenceManager.Data.MenuIgnoreButton.SetActive(false);
                RefrenceManager.Data.MenuSelects.SetActive(false);
                RefrenceManager.Data.MenuScrollLeft.SetActive(false);
                RefrenceManager.Data.MenuScrollRight.SetActive(false);
                CurrentPage = 1;
            }
            else if (AmountOfPlayersPlaying == 0) {
                RefrenceManager.Data.MenuRoundRunning.SetActive(false);
                RefrenceManager.Data.MenuWarning.SetActive(true);
                RefrenceManager.Data.MenuIgnoreButton.SetActive(true);
                RefrenceManager.Data.MenuSelects.SetActive(false);
                RefrenceManager.Data.MenuScrollLeft.SetActive(false);
                RefrenceManager.Data.MenuScrollRight.SetActive(false);
            }
        }

        public void ChangeCurrentPage(bool isSub) {
            foreach (GameObject obj in RefrenceManager.Data.MenuNights) {
                obj.SetActive(false);
            }

            if (isSub) {
                CurrentPage--;
                RefrenceManager.Data.MenuNights[CurrentPage].SetActive(true);
                if (RefrenceManager.Data.CustomNightSelect.activeSelf) { RefrenceManager.Data.MenuScrollRight.SetActive(false); RefrenceManager.Data.MenuScrollLeft.SetActive(true); } else { RefrenceManager.Data.MenuScrollRight.SetActive(true); RefrenceManager.Data.MenuScrollLeft.SetActive(true); }
                if (RefrenceManager.Data.NightOneSelect.activeSelf) { RefrenceManager.Data.MenuScrollRight.SetActive(true); RefrenceManager.Data.MenuScrollLeft.SetActive(false); } else { RefrenceManager.Data.MenuScrollRight.SetActive(true); RefrenceManager.Data.MenuScrollLeft.SetActive(true); }
            }
            else {
                CurrentPage++; RefrenceManager.Data.MenuNights[CurrentPage].SetActive(true);
                if (RefrenceManager.Data.NightOneSelect.activeSelf) { RefrenceManager.Data.MenuScrollRight.SetActive(true); RefrenceManager.Data.MenuScrollLeft.SetActive(false); } else { RefrenceManager.Data.MenuScrollRight.SetActive(true); RefrenceManager.Data.MenuScrollLeft.SetActive(true); }
                if (RefrenceManager.Data.CustomNightSelect.activeSelf) { RefrenceManager.Data.MenuScrollRight.SetActive(false); RefrenceManager.Data.MenuScrollLeft.SetActive(true); } else { RefrenceManager.Data.MenuScrollRight.SetActive(true); RefrenceManager.Data.MenuScrollLeft.SetActive(true); }
            }
        }

        IEnumerator GetInfoStuff() {
            yield return new WaitForSeconds(10);
            RefrenceManager.Data.Version.text = FNAGInfo.Version;
            RefrenceManager.Data.HasUpdater.text = "NOT SUPPORTED ANYMORE";
            RefrenceManager.Data.TestMode.text = TestMode.ToString();
            UnityWebRequest www = UnityWebRequest.Get("https://raw.githubusercontent.com/MrBanana01/Five-Nights-At-Gorillas/master/News");
            yield return www.SendWebRequest();
            RefrenceManager.Data.RecentNews.text = www.downloadHandler.text;
        }

        [ModdedGamemodeJoin]
        void OnJoin(string gamemode)  { 
            InCustomRoom = true;
            RefrenceManager.Data.Menu.SetActive(true);
            RefrenceManager.Data.FNAGMAP.SetActive(true);
        }

        [ModdedGamemodeLeave]
        void OnLeave(string gamemode)  {
            InCustomRoom = false;
            StopGame();
            RefrenceManager.Data.Menu.SetActive(false);
            RefrenceManager.Data.FNAGMAP.SetActive(false);
        }

        public void StopGame() {
            #region ResetAI
            foreach (GameObject ai in RefrenceManager.Data.gorilla) { ai.gameObject.SetActive(false); }
            foreach (GameObject ai in RefrenceManager.Data.mingus) { ai.gameObject.SetActive(false); }
            foreach (GameObject ai in RefrenceManager.Data.bob) { ai.gameObject.SetActive(false); }
            foreach (GameObject ai in RefrenceManager.Data.dingus) { ai.gameObject.SetActive(false); }
            RefrenceManager.Data.gorilla[0].SetActive(true);
            RefrenceManager.Data.mingus[0].SetActive(true);
            RefrenceManager.Data.bob[0].SetActive(true);
            RefrenceManager.Data.dingus[0].SetActive(true);
            AIManager[] AI = Resources.FindObjectsOfTypeAll<AIManager>();
            foreach(AIManager ai in AI) { ai.Difficulty = 0; ai.StopAI(); }
            #endregion
            #region ResetDoors
            DoorManager.Data.ResetDoors();
            #endregion
            #region ResetMenu
            RefrenceManager.Data.MenuRoundRunning.SetActive(false);
            RefrenceManager.Data.MenuWarning.SetActive(true);
            RefrenceManager.Data.MenuIgnoreButton.SetActive(true);
            RefrenceManager.Data.MenuSelects.SetActive(false);
            RefrenceManager.Data.MenuScrollLeft.SetActive(false);
            RefrenceManager.Data.MenuScrollRight.SetActive(false);
            #endregion
            CameraManager.Data.RefreshCamera();
            GameRunning = false;
            beingJumpscared = false;
            RefrenceManager.Data.Darkness.SetActive(false);
            StopCoroutine(PoweroutageDelay());
            AmountOfPlayersPlaying = 0;
            CurrentPage = 1;
            Refresh();
            SandboxValues.Data.ResetAllValues();
        }

        public Transform GetNthParent(GameObject obj, int n) {
            Transform parent = obj.transform;
            for (int i = 0; i < n - 1; i++) {
                if (parent.parent != null) {
                    parent = parent.parent;
                }
                else {
                    return null;
                }
            }
            return parent;
        }

        public void StartGame(byte Night, byte GD, byte MD, byte BD, byte DD) {
            #region StartGame
            beingJumpscared = false;
            GameRunning = true;
            TimePowerManager.Data.StartEverything();
            if (!SandboxValues.Data.BrightOffice) { SkyColorGameBlack(); }
            if (SandboxValues.Data.PitchBlack) { SkyColorFullBlack(); RefrenceManager.Data.Darkness.SetActive(true); }
            if (SandboxValues.Data.NoCamera) { RefrenceManager.Data.CameraScreen.GetComponent<Renderer>().material = RefrenceManager.Data.Cam11Nothing; }
            if (Night == 1) {
                RefrenceManager.Data.gorillaParent.GetComponent<AIManager>().Difficulty = 0;
                RefrenceManager.Data.mingusParent.GetComponent<AIManager>().Difficulty = 2;
                RefrenceManager.Data.bobParent.GetComponent<AIManager>().Difficulty = 2;
                RefrenceManager.Data.dingusParent.GetComponent<AIManager>().Difficulty = 0;
            }
            else if (Night == 2) {
                RefrenceManager.Data.gorillaParent.GetComponent<AIManager>().Difficulty = 0;
                RefrenceManager.Data.mingusParent.GetComponent<AIManager>().Difficulty = 2;
                RefrenceManager.Data.bobParent.GetComponent<AIManager>().Difficulty = 3;
                RefrenceManager.Data.dingusParent.GetComponent<AIManager>().Difficulty = 1;
            }
            else if (Night == 3) {
                RefrenceManager.Data.gorillaParent.GetComponent<AIManager>().Difficulty = 1;
                RefrenceManager.Data.mingusParent.GetComponent<AIManager>().Difficulty = 5;
                RefrenceManager.Data.bobParent.GetComponent<AIManager>().Difficulty = 4;
                RefrenceManager.Data.dingusParent.GetComponent<AIManager>().Difficulty = 2;
            }
            else if (Night == 4) {
                RefrenceManager.Data.gorillaParent.GetComponent<AIManager>().Difficulty = 2;
                RefrenceManager.Data.mingusParent.GetComponent<AIManager>().Difficulty = 7;
                RefrenceManager.Data.bobParent.GetComponent<AIManager>().Difficulty = 3;
                RefrenceManager.Data.dingusParent.GetComponent<AIManager>().Difficulty = 6;
            }
            else if (Night == 5) {
                RefrenceManager.Data.gorillaParent.GetComponent<AIManager>().Difficulty = 5;
                RefrenceManager.Data.mingusParent.GetComponent<AIManager>().Difficulty = 7;
                RefrenceManager.Data.bobParent.GetComponent<AIManager>().Difficulty = 6;
                RefrenceManager.Data.dingusParent.GetComponent<AIManager>().Difficulty = 6;
            }
            else if (Night == 6) {
                RefrenceManager.Data.gorillaParent.GetComponent<AIManager>().Difficulty = 8;
                RefrenceManager.Data.mingusParent.GetComponent<AIManager>().Difficulty = 12;
                RefrenceManager.Data.bobParent.GetComponent<AIManager>().Difficulty = 10;
                RefrenceManager.Data.dingusParent.GetComponent<AIManager>().Difficulty = 16;
            }
            else if (Night == 7) {
                RefrenceManager.Data.gorillaParent.GetComponent<AIManager>().Difficulty = GD;
                RefrenceManager.Data.mingusParent.GetComponent<AIManager>().Difficulty = MD;
                RefrenceManager.Data.bobParent.GetComponent<AIManager>().Difficulty = BD;
                RefrenceManager.Data.dingusParent.GetComponent<AIManager>().Difficulty = DD;
            }
            StartCoroutine(MapUnloadDelay(true));
            Vector3 Back = new Vector3(-103.5589f, 24.4809f, -66.4852f); Teleport.TeleportPlayer(Back, 90, true);
            #endregion
        }

        public void TeleportPlayerBack() {
            StartCoroutine(MapUnloadDelay(false));
            Vector3 Back = new Vector3(-66.3163f, 12.9148f, -82.4704f); Teleport.TeleportPlayer(Back, 90, true);
        }

        public void TeleportPlayerToBox() {
            StartCoroutine(MapUnloadDelay(false));
            Teleport.TeleportPlayer(RefrenceManager.Data.BlackBoxTeleport.transform.position, 90, true);
        }

        public void Jumpscare() {
            if (!beingJumpscared && GameRunning) {
                beingJumpscared = true;
                RefrenceManager.Data.Jumpscare.SetActive(true);
                RefrenceManager.Data.JumpscareAnimation.Play("Jumpscare");
                RefrenceManager.Data.JumpscareSound.Play();
                TimePowerManager.Data.StopEverything();
                StartCoroutine(JumpscareDelay());
                StopGame();
            }
        }

        public void Poweroutage() {
            if (GameRunning) {
                if (!DoorManager.Data.RightDoorOpen) {
                    DoorManager.Data.UseLocalDoor(true);
                }
                if (!DoorManager.Data.LeftDoorOpen) {
                    DoorManager.Data.UseLocalDoor(false);
                }
                if (DoorManager.Data.LeftLightOn) {
                    DoorManager.Data.UseLight(false);
                }
                if (DoorManager.Data.RightLightOn) {
                    DoorManager.Data.UseLight(true);
                }
                DoorManager.Data.PowerOutage();
                RefrenceManager.Data.Poweroutage.Play();
                TimePowerManager.Data.StopOnlyPower();
                SkyColorFullBlack();
                foreach (GameObject ai in RefrenceManager.Data.gorilla) { ai.gameObject.SetActive(false); }
                foreach (GameObject ai in RefrenceManager.Data.mingus) { ai.gameObject.SetActive(false); }
                foreach (GameObject ai in RefrenceManager.Data.bob) { ai.gameObject.SetActive(false); }
                foreach (GameObject ai in RefrenceManager.Data.dingus) { ai.gameObject.SetActive(false); }
                RefrenceManager.Data.gorilla[0].SetActive(true);
                RefrenceManager.Data.mingus[0].SetActive(true);
                RefrenceManager.Data.bob[0].SetActive(true);
                RefrenceManager.Data.dingus[0].SetActive(true);
                AIManager[] AI = Resources.FindObjectsOfTypeAll<AIManager>();
                foreach (AIManager ai in AI) { ai.Difficulty = 0; ai.StopAI(); }
                StartCoroutine(PoweroutageDelay());
                RefrenceManager.Data.Darkness.SetActive(true);
            }
        }

        public void SixAM() {
            RefrenceManager.Data.Darkness.SetActive(false);
            SkyColorWhite();
            TeleportPlayerToBox();
            StartCoroutine(SixAMDelay());
        }

        public void DingusRun() {
            if (SandboxValues.Data.AutoCloseDoor) { if (DoorManager.Data.LeftDoorOpen) { DoorManager.Data.UseLocalDoor(false); } }
            RefrenceManager.Data.DingusRunning.Play();
            foreach (GameObject D in RefrenceManager.Data.dingus) {
                D.SetActive(false);
            }
            StartCoroutine(DingusRunDelay());
        }

        IEnumerator SixAMDelay() {
            yield return new WaitForSeconds(0.3f);
            RefrenceManager.Data.SixAMSound.Play();
            RefrenceManager.Data.SixAM.GetComponent<VideoPlayer>().Play();
            RefrenceManager.Data.Poweroutage.Stop();
            TimePowerManager.Data.StopEverything();
            yield return new WaitForSeconds(10);
            StopGame();
            TeleportPlayerBack();
            Refresh();
        }

        IEnumerator PoweroutageDelay() {
            yield return new WaitForSeconds(68);
            Jumpscare();
        }

        IEnumerator DingusRunDelay() {
            yield return new WaitForSeconds(2);
            if (DoorManager.Data.LeftDoorOpen) {
                Jumpscare();
            }
            else {
                RefrenceManager.Data.dingusParent.GetComponent<AIManager>().ResetDingus();
                RefrenceManager.Data.DingusScrapingSound.Play();
                TimePowerManager.Data.DingusThing();
            }
        }

        IEnumerator MapUnloadDelay(bool StartAI) {
            RefrenceManager.Data.FNAGMAP.SetActive(false);
            yield return new WaitForSeconds(0.1f);
            RefrenceManager.Data.FNAGMAP.SetActive(true);
            if (StartAI) {
                RefrenceManager.Data.gorillaParent.GetComponent<AIManager>().StartAI();
                RefrenceManager.Data.mingusParent.GetComponent<AIManager>().StartAI();
                RefrenceManager.Data.bobParent.GetComponent<AIManager>().StartAI();
                RefrenceManager.Data.dingusParent.GetComponent<AIManager>().StartAI();
            }
        }

        IEnumerator JumpscareDelay() {
            yield return new WaitForSeconds(1.5f);
            TeleportPlayerToBox();
            SkyColorWhite();
            RefrenceManager.Data.Jumpscare.SetActive(false);
            RefrenceManager.Data.JumpscareSound.Stop();
            RefrenceManager.Data.Darkness.SetActive(false);
            RefrenceManager.Data.StaticScreen.SetActive(true);
            RefrenceManager.Data.StaticScreen.GetComponent<VideoPlayer>().Play();
            yield return new WaitForSeconds(7);
            RefrenceManager.Data.StaticScreen.SetActive(false);
            TeleportPlayerBack();
            TimePowerManager.Data.StopEverything();
            RefrenceManager.Data.Jumpscare.SetActive(false);
            RefrenceManager.Data.JumpscareAnimation.StopPlayback();
            RefrenceManager.Data.JumpscareSound.Stop();
            beingJumpscared = false;
            Refresh();
            StopGame();
        }
    }
}