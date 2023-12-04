using BepInEx;
using FiveNightsAtGorillas.Managers.Refrences;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using Utilla;
using System.Collections;
using FiveNightsAtGorillas.Managers.Teleport;
using FiveNightsAtGorillas.Other.Door;
using FiveNightsAtGorillas.Managers.AI;
using FiveNightsAtGorillas.Managers.DoorAndLight;
using FiveNightsAtGorillas.Managers.NetworkedData;
using FiveNightsAtGorillas.Other.Light;
using FiveNightsAtGorillas.Other.Camera;
using FiveNightsAtGorillas.Managers.Cameras;
using FiveNightsAtGorillas.Managers.TimePower;
using FiveNightsAtGorillas.Other.PlayerDetecter;
using FiveNightsAtGorillas.Other.Ignore;
using FiveNightsAtGorillas.Other.NightStart;
using FiveNightsAtGorillas.Other.Scroll;
using FiveNightsAtGorillas.Other.CustomNightAdd;
using FiveNightsAtGorillas.Other.CustomNightSub;
using FiveNightsAtGorillas.Other.BOOP;

namespace FiveNightsAtGorillas
{
    [ModdedGamemode("fnag", "FNAG", Utilla.Models.BaseGamemode.Casual)]
    [BepInDependency("org.legoandmars.gorillatag.utilla")]
    [BepInPlugin(FNAGInfo.GUID, FNAGInfo.Name, FNAGInfo.Version)]
    public class FNAG : BaseUnityPlugin
    {
        public int Version { get; private set; } = 100;

        public static FNAG Data;
        public bool RoundCurrentlyRunning;
        public bool LocalPlayingRound;
        public bool InCustomRoom { get; private set; }
        public int CurrentPage { get; private set; } = 0;
        public bool TestMode { get; private set; } = false;

        void Start() { Events.GameInitialized += OnGameInitialized; Data = this; }

        void OnEnable() { HarmonyPatches.ApplyHarmonyPatches(); }
        void OnDisable() { this.enabled = true; HarmonyPatches.RemoveHarmonyPatches(); } //Sorry! I can't take the time to figure out how I'm going to disable this properly
        void MakeFog() { RenderSettings.fog = true; RenderSettings.fogStartDistance = 7; RenderSettings.fogColor = Color.black; }
        void MakePoweroutFog() { RenderSettings.fog = true; RenderSettings.fogStartDistance = 0.3f; RenderSettings.fogColor = Color.black; }
        void RemoveFog() { RenderSettings.fog = false; }
        public void SkyColorFullBlack() { RenderSettings.ambientSkyColor = Color.black; MakeFog(); }
        public void SkyColorGameBlack() { RenderSettings.ambientSkyColor = RefrenceManager.Data.GameSkyColor.color; MakeFog(); }
        public void SkyColorWhite() { RenderSettings.ambientSkyColor = Color.white; RemoveFog(); }

        void OnGameInitialized(object sender, EventArgs e)
        {
            GameObject[] allObjs = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (GameObject obj in allObjs)
            {
                if(obj.name == "head_end")
                {
                    obj.AddComponent<BoxCollider>().isTrigger = true;
                    obj.layer = 10;
                    obj.GetComponent<BoxCollider>().center = new Vector3(0, 0, 0);
                }
            }

            var bundle = LoadAssetBundle("FiveNightsAtGorillas.Assets.fnag");
            var map = bundle.LoadAsset<GameObject>("FNAG MAP");
            var jumpscare = bundle.LoadAsset<GameObject>("_Jumpscares");
            var menu = bundle.LoadAsset<GameObject>("Menu");

            gameObject.AddComponent<RefrenceManager>();

            RefrenceManager.Data.Menu = Instantiate(menu);
            RefrenceManager.Data.FNAGMAP = Instantiate(map);
            RefrenceManager.Data._Jumpscares = Instantiate(jumpscare);

            RefrenceManager.Data.SetRefrences();

            SetupHitSounds();
            SetupComps();
            SetupEnemys();
            SetupMenu();

            if (TestMode)
            {
                RefrenceManager.Data.Menu.SetActive(true);
                RefrenceManager.Data.FNAGMAP.SetActive(true);
            }
        }

        #region EnemySetup
        void SetupEnemys()
        {
            foreach (GameObject obj in RefrenceManager.Data.gorilla) { obj.SetActive(false); }
            foreach(GameObject obj in RefrenceManager.Data.mingus) { obj.SetActive(false); }
            foreach(GameObject obj in RefrenceManager.Data.dingus) { obj.SetActive(false); }
            foreach(GameObject obj in RefrenceManager.Data.bob) { obj.SetActive(false); }
            RefrenceManager.Data.gorilla[0].SetActive(true);
            RefrenceManager.Data.mingus[0].SetActive(true);
            RefrenceManager.Data.dingus[0].SetActive(true);
            RefrenceManager.Data.bob[0].SetActive(true);
            RefrenceManager.Data.SixAM.SetActive(false);
            foreach (GameObject obj in RefrenceManager.Data.Jumpscares.Values)
            {
                obj.SetActive(false);
            }
            RefrenceManager.Data._Jumpscares.SetActive(false);
            RefrenceManager.Data.FNAGMAP.transform.position = new Vector3(-102.0151f, 23.7944f, -65.1198f);
            RefrenceManager.Data._Jumpscares.transform.localRotation = Quaternion.Euler(0, 180, 0);
            RefrenceManager.Data.JumpscareParent.transform.parent = GorillaTagger.Instance.mainCamera.transform;
            RefrenceManager.Data.SixAM.transform.parent = GorillaTagger.Instance.mainCamera.transform;
            RefrenceManager.Data.JumpscareParent.transform.localPosition = new Vector3(0, -0.4f, 0.8f);
            RefrenceManager.Data.SixAM.transform.localPosition = new Vector3(0, 0, 0.1f);
            RefrenceManager.Data.SixAM.transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
        #endregion
        #region SetupHitsounds
        void SetupHitSounds()
        {
            GameObject.Find($"{RefrenceManager.Data.FNAGMAP.name}/Office/Floor/Floor").AddComponent<GorillaSurfaceOverride>().overrideIndex = 0;
            GameObject.Find($"{RefrenceManager.Data.FNAGMAP.name}/Office/Walls/Office Walls").AddComponent<GorillaSurfaceOverride>().overrideIndex = 0;
            GameObject.Find($"{RefrenceManager.Data.FNAGMAP.name}/Office/Chair/Cylinder (1)").AddComponent<GorillaSurfaceOverride>().overrideIndex = 3;
            GameObject.Find($"{RefrenceManager.Data.FNAGMAP.name}/TheRest/Deco/Monitors").AddComponent<GorillaSurfaceOverride>().overrideIndex = 146;
            RefrenceManager.Data.CameraScreen.AddComponent<GorillaSurfaceOverride>().overrideIndex = 29;
            RefrenceManager.Data.CameraScreen.AddComponent<BoxCollider>();
            GameObject.Find($"{RefrenceManager.Data.FNAGMAP.name}/Office/Desk/Collider").AddComponent<GorillaSurfaceOverride>().overrideIndex = 146;
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
            RefrenceManager.Data.gorillaParent.AddComponent<AIManager>().AIName = "gorilla";
            RefrenceManager.Data.mingusParent.AddComponent<AIManager>().AIName = "mingus";
            RefrenceManager.Data.bobParent.AddComponent<AIManager>().AIName = "bob";
            RefrenceManager.Data.dingusParent.AddComponent<AIManager>().AIName = "dingus";
            RefrenceManager.Data.gorillaParent.GetComponent<AIManager>().CamPos = "Cam11";
            RefrenceManager.Data.mingusParent.GetComponent<AIManager>().CamPos = "Cam11";
            RefrenceManager.Data.bobParent.GetComponent<AIManager>().CamPos = "Cam11";
            RefrenceManager.Data.dingusParent.GetComponent<AIManager>().CamPos = "Stage1";
            RefrenceManager.Data.NearGameTrigger.AddComponent<PlayersInRound>();
            RefrenceManager.Data.gorillaBoopTrigger.AddComponent<Boop_>();

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
            RefrenceManager.Data.MenuScrollRight.SetActive(true);
            RefrenceManager.Data.MenuWarning.SetActive(false);
            RefrenceManager.Data.MenuSelects.SetActive(true);
            RefrenceManager.Data.MenuRoundRunning.SetActive(false);

            RefrenceManager.Data.Menu.SetActive(false);
            RefrenceManager.Data.FNAGMAP.SetActive(false);
        }
        #endregion

        AssetBundle LoadAssetBundle(string path)
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            AssetBundle bundle = AssetBundle.LoadFromStream(stream);
            stream.Close();
            return bundle;
        }

        public void ChangeCurrentPage(bool isSub)
        {
            foreach (GameObject obj in RefrenceManager.Data.MenuNights)
            {
                obj.SetActive(false);
            }

            if (isSub)
            {
                CurrentPage--;
                RefrenceManager.Data.MenuNights[CurrentPage].SetActive(true);
                if (RefrenceManager.Data.CustomNightSelect.activeSelf) { RefrenceManager.Data.MenuScrollRight.SetActive(false); RefrenceManager.Data.MenuScrollLeft.SetActive(true); } else { RefrenceManager.Data.MenuScrollRight.SetActive(true); RefrenceManager.Data.MenuScrollLeft.SetActive(true); }
                if (RefrenceManager.Data.NightOneSelect.activeSelf) { RefrenceManager.Data.MenuScrollRight.SetActive(true); RefrenceManager.Data.MenuScrollLeft.SetActive(false); } else { RefrenceManager.Data.MenuScrollRight.SetActive(true); RefrenceManager.Data.MenuScrollLeft.SetActive(true); }
            }
            else 
            { 
                CurrentPage++; RefrenceManager.Data.MenuNights[CurrentPage].SetActive(true);
                if (RefrenceManager.Data.NightOneSelect.activeSelf) { RefrenceManager.Data.MenuScrollRight.SetActive(true); RefrenceManager.Data.MenuScrollLeft.SetActive(false); } else { RefrenceManager.Data.MenuScrollRight.SetActive(true); RefrenceManager.Data.MenuScrollLeft.SetActive(true); }
                if (RefrenceManager.Data.CustomNightSelect.activeSelf) { RefrenceManager.Data.MenuScrollRight.SetActive(false); RefrenceManager.Data.MenuScrollLeft.SetActive(true); } else { RefrenceManager.Data.MenuScrollRight.SetActive(true); RefrenceManager.Data.MenuScrollLeft.SetActive(true); }
            }
        }

        [ModdedGamemodeJoin]
        void OnJoin(string gamemode) 
        { 
            InCustomRoom = true; 
            RefrenceManager.Data.Menu.SetActive(true);
            RefrenceManager.Data.FNAGMAP.SetActive(true);
        }

        [ModdedGamemodeLeave]
        void OnLeave(string gamemode) 
        {
            InCustomRoom = false;
            StopGame();
            RefrenceManager.Data.Menu.SetActive(false);
            RefrenceManager.Data.FNAGMAP.SetActive(false);
        }

        public void StopGame()
        {
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
            CameraManager.Data.RefreshCamera();
        }

        public void StartGame(int Night, string GD, string MD, string BD, string DD)
        {
            #region StartGame
            if (Night == 1)
            {
                RefrenceManager.Data.gorillaParent.GetComponent<AIManager>().Difficulty = 0;
                RefrenceManager.Data.mingusParent.GetComponent<AIManager>().Difficulty = 2;
                RefrenceManager.Data.bobParent.GetComponent<AIManager>().Difficulty = 2;
                RefrenceManager.Data.dingusParent.GetComponent<AIManager>().Difficulty = 0;
            }
            if (Night == 2)
            {
                RefrenceManager.Data.gorillaParent.GetComponent<AIManager>().Difficulty = 0;
                RefrenceManager.Data.mingusParent.GetComponent<AIManager>().Difficulty = 2;
                RefrenceManager.Data.bobParent.GetComponent<AIManager>().Difficulty = 3;
                RefrenceManager.Data.dingusParent.GetComponent<AIManager>().Difficulty = 1;
            }
            if (Night == 3)
            {
                RefrenceManager.Data.gorillaParent.GetComponent<AIManager>().Difficulty = 1;
                RefrenceManager.Data.mingusParent.GetComponent<AIManager>().Difficulty = 5;
                RefrenceManager.Data.bobParent.GetComponent<AIManager>().Difficulty = 4;
                RefrenceManager.Data.dingusParent.GetComponent<AIManager>().Difficulty = 2;
            }
            if (Night == 4)
            {
                RefrenceManager.Data.gorillaParent.GetComponent<AIManager>().Difficulty = 2;
                RefrenceManager.Data.mingusParent.GetComponent<AIManager>().Difficulty = 7;
                RefrenceManager.Data.bobParent.GetComponent<AIManager>().Difficulty = 3;
                RefrenceManager.Data.dingusParent.GetComponent<AIManager>().Difficulty = 6;
            }
            if (Night == 5)
            {
                RefrenceManager.Data.gorillaParent.GetComponent<AIManager>().Difficulty = 5;
                RefrenceManager.Data.mingusParent.GetComponent<AIManager>().Difficulty = 7;
                RefrenceManager.Data.bobParent.GetComponent<AIManager>().Difficulty = 6;
                RefrenceManager.Data.dingusParent.GetComponent<AIManager>().Difficulty = 6;
            }
            if (Night == 6)
            {
                RefrenceManager.Data.gorillaParent.GetComponent<AIManager>().Difficulty = 8;
                RefrenceManager.Data.mingusParent.GetComponent<AIManager>().Difficulty = 12;
                RefrenceManager.Data.bobParent.GetComponent<AIManager>().Difficulty = 10;
                RefrenceManager.Data.dingusParent.GetComponent<AIManager>().Difficulty = 16;
            }
            if (Night == 7)
            {
                RefrenceManager.Data.gorillaParent.GetComponent<AIManager>().Difficulty = int.Parse(GD);
                RefrenceManager.Data.mingusParent.GetComponent<AIManager>().Difficulty = int.Parse(MD);
                RefrenceManager.Data.bobParent.GetComponent<AIManager>().Difficulty = int.Parse(BD);
                RefrenceManager.Data.dingusParent.GetComponent<AIManager>().Difficulty = int.Parse(DD);
            }
            TimePowerManager.Data.StartEverything();
            RefrenceManager.Data.gorillaParent.GetComponent<AIManager>().StartAI();
            RefrenceManager.Data.mingusParent.GetComponent<AIManager>().StartAI();
            RefrenceManager.Data.bobParent.GetComponent<AIManager>().StartAI();
            RefrenceManager.Data.dingusParent.GetComponent<AIManager>().StartAI();
            if (FNAG.Data.TestMode != true) { FNAG.Data.SkyColorGameBlack(); }
            FNAG.Data.TeleportPlayerToGame();
            #endregion
        }

        public void TeleportPlayerBack()
        { 
            Vector3 Back = new Vector3(-66.3163f, 12.9148f, -82.4704f); Teleport.TeleportPlayer(Back, 90f, true);
        }

        public void TeleportPlayerToGame()
        { 
            Vector3 Back = new Vector3(-103.6259f, 24.8166f, -66.3371f); Teleport.TeleportPlayer(Back, 90f, true);
        }

        public void Jumpscare(string ai)
        {
            RefrenceManager.Data.Jumpscares[ai].SetActive(true);
            RefrenceManager.Data.JumpscareAnimation.Play("Jumpscares");
            RefrenceManager.Data.JumpscareSound.Play();
            StartCoroutine(JumpscareDelay());
            StopGame();
        }

        public void Poweroutage()
        {
            if (!DoorManager.Data.RightDoorOpen)
            {
                DoorManager.Data.UseLocalDoor(true);
            }
            if (!DoorManager.Data.LeftDoorOpen)
            {
                DoorManager.Data.UseLocalDoor(false);
            }
            if (DoorManager.Data.LeftLightOn)
            {
                DoorManager.Data.UseLight(false);
            }
            if (DoorManager.Data.RightLightOn)
            {
                DoorManager.Data.UseLight(true);
            }
            StopGame();
            DoorManager.Data.PowerOutage();
            RefrenceManager.Data.Poweroutage.Play();
            TimePowerManager.Data.StopOnlyPower();
            SkyColorFullBlack();
            StartCoroutine(PoweroutageDelay());
        }

        public void SixAM()
        {
            StopGame();
            RefrenceManager.Data.SixAM.SetActive(true);
            RefrenceManager.Data.SixAMSound.Play();
            RefrenceManager.Data.Poweroutage.Stop();
            StartCoroutine(SixAMDelay());
        }

        public void DingusRun()
        {
            RefrenceManager.Data.DingusRunning.Play();
            foreach (GameObject D in RefrenceManager.Data.dingus)
            {
                D.SetActive(false);
            }
            StartCoroutine(DingusRunDelay());
        }

        IEnumerator SixAMDelay()
        {
            yield return new WaitForSeconds(10);
            RefrenceManager.Data.SixAM.SetActive(false);
            TeleportPlayerBack();
            SkyColorWhite();
            TimePowerManager.Data.StopEverything();
        }

        IEnumerator PoweroutageDelay()
        {
            yield return new WaitForSeconds(67.8f);
            Jumpscare("gorilla");
        }

        IEnumerator DingusRunDelay()
        {
            yield return new WaitForSeconds(2);
            if (DoorManager.Data.LeftDoorOpen)
            {
                Jumpscare("bingus");
            }
            else
            {
                RefrenceManager.Data.dingusParent.GetComponent<AIManager>().ResetDingus();
                RefrenceManager.Data.DingusScrapingSound.Play();
                TimePowerManager.Data.DingusThing();
            }
        }

        IEnumerator JumpscareDelay()
        {
            yield return new WaitForSeconds(1.5f);
            RefrenceManager.Data.FNAGMAP.SetActive(false);
            TeleportPlayerBack();
            TimePowerManager.Data.StopEverything();
            SkyColorWhite();
            foreach (GameObject obj in RefrenceManager.Data.Jumpscares.Values)
            {
                obj.SetActive(false);
            }
            RefrenceManager.Data.JumpscareAnimation.StopPlayback();
            RefrenceManager.Data.JumpscareSound.Stop();
            SkyColorWhite();
        }
    }
}