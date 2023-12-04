using UnityEngine;
using System.Collections.Generic;
using TMPro;

namespace FiveNightsAtGorillas.Managers.Refrences
{
    public class RefrenceManager : MonoBehaviour
    {
        bool AlreadySetRefrences;

        public static RefrenceManager Data;

        public GameObject ChainLoader { get; private set; }
        public GameObject FNAGMAP { get; set; }
        public GameObject _Jumpscares { get; set; }
        public GameObject Menu { get; set; }

        public Material GameSkyColor { get; private set; }
        public Material RedRefrence { get; private set; }
        public Material WhiteRefrence { get; private set; }
        public GameObject NearGameTrigger { get; private set; }

        public GameObject RightLight { get; private set; }
        public GameObject LeftLight { get; private set; }
        public GameObject RightDoor { get; private set; }
        public GameObject LeftDoor { get; private set; }
        public AudioSource LeftLightSound { get; private set; }
        public AudioSource RightLightSound { get; private set; }
        public AudioSource RightDoorFailSound{ get; private set; }
        public AudioSource LeftDoorFailSound { get; private set; }
        public AudioSource RightDoorSound { get; private set; }
        public AudioSource LeftDoorSound { get; private set; }
        public List<GameObject> CameraButtons { get; private set; }
        public GameObject Cam1 { get; private set; }
        public GameObject Cam2 { get; private set; }
        public GameObject Cam3 { get; private set; }
        public GameObject Cam4 { get; private set; }
        public GameObject Cam5 { get; private set; }
        public GameObject Cam6 { get; private set; }
        public GameObject Cam7 { get; private set; }
        public GameObject Cam8 { get; private set; }
        public GameObject Cam9 { get; private set; }
        public GameObject Cam10 { get; private set; }
        public GameObject Cam11 { get; private set; }

        public GameObject NightOneSelect { get; private set; }
        public GameObject NightTwoSelect { get; private set; }
        public GameObject NightThreeSelect { get; private set; }
        public GameObject NightFourSelect { get; private set; }
        public GameObject NightFiveSelect { get; private set; }
        public GameObject NightSixSelect { get; private set; }
        public GameObject CustomNightSelect { get; private set; }

        public Material Cam1Nothing { get; private set; }
        public Material Cam2Nothing { get; private set; }
        public Material Cam3Nothing { get; private set; }
        public Material Cam4Nothing { get; private set; }
        public Material Cam5Credits { get; private set; }
        public Material Cam6Nothing { get; private set; }
        public Material Cam7Nothing { get; private set; }
        public Material Cam9Nothing { get; private set; }
        public Material Cam10Nothing { get; private set; }
        public Material Cam11Nothing { get; private set; }
        public Material Cam1Mingus { get; private set; }
        public Material Cam2Mingus { get; private set; }
        public Material Cam3Bob { get; private set; }
        public Material Cam3Gorilla { get; private set; }
        public Material Cam4Bob { get; private set; }
        public Material Cam4Gorilla { get; private set; }
        public Material Cam6Bob { get; private set; }
        public Material Cam7Mingus { get; private set; }
        public Material Cam8Dingus1 { get; private set; }
        public Material Cam8Dingus2 { get; private set; }
        public Material Cam8Dingus3 { get; private set; }
        public Material Cam8Dingus4 { get; private set; }
        public Material Cam8Dingus5 { get; private set; }
        public Material Cam8Dingus6 { get; private set; }
        public Material Cam9Mingus { get; private set; }
        public Material Cam10All { get; private set; }
        public Material Cam10Bob { get; private set; }
        public Material Cam10BobGorilla { get; private set; }
        public Material Cam10BobMingus { get; private set; }
        public Material Cam10Gorilla { get; private set; }
        public Material Cam10GorillaMingus { get; private set; }
        public Material Cam10Mingus { get; private set; }
        public Material Cam11All { get; private set; }
        public Material Cam11Bob { get; private set; }
        public Material Cam11Gorilla { get; private set; }
        public Material Cam11GorillaBob { get; private set; }
        public Material Cam11GorillaMingus { get; private set; }
        public Material Cam11Mingus { get; private set; }
        public Material Cam11MingusBob { get; private set; }

        public GameObject RightDoorObject { get; private set; }
        public GameObject LeftDoorObject { get; private set; }
        public GameObject LeftDoorVoid { get; private set; }
        public GameObject RightDoorVoid { get; private set; }

        public GameObject CameraScreen { get; private set; }
        public TextMeshPro CurrentTime { get; private set; }
        public TextMeshPro CurrentPower { get; private set; }

        public List<GameObject> gorilla { get; private set; }
        public List<GameObject> bob { get; private set; }
        public List<GameObject> mingus { get; private set; }
        public List<GameObject> dingus { get; private set; }

        public GameObject dingusParent { get; private set; }
        public GameObject gorillaParent { get; private set; }
        public GameObject mingusParent { get; private set; }
        public GameObject bobParent { get; private set; }

        public GameObject JumpscareParent;
        public Dictionary<string, GameObject> Jumpscares { get; private set; }
        public Animator JumpscareAnimation { get; private set; }
        public GameObject SixAM { get; private set; }

        public AudioSource DingusRunning { get; private set; }
        public AudioSource JumpscareSound { get; private set; }
        public AudioSource SixAMSound { get; private set; }
        public AudioSource Poweroutage { get; private set; }
        public AudioSource CameraButtonPressSound { get; private set; }
        public AudioSource DingusScrapingSound { get; private set; }
        public AudioSource GorillaBoopSound { get; private set; }
        public GameObject gorillaBoopTrigger { get; private set; }

        public GameObject MenuIgnoreButton { get; private set; }
        public GameObject MenuWarning { get; private set; }
        public GameObject MenuRoundRunning { get; private set; }
        public GameObject MenuSelects { get; private set; }
        public List<GameObject> MenuNights { get; private set; }
        public GameObject MenuScrollLeft { get; private set; }
        public GameObject MenuScrollRight { get; private set; }
        public GameObject MenuScrollRightButton { get; private set; }
        public GameObject MenuScrollLeftButton { get; private set; }
        public List<GameObject> MenuStartButton { get; private set; }
        public GameObject MenuCNAddGorilla { get; private set; }
        public GameObject MenuCNAddMingus { get; private set; }
        public GameObject MenuCNAddBob { get; private set; }
        public GameObject MenuCNAddDingus { get; private set; }
        public GameObject MenuCNSubDingus { get; private set; }
        public GameObject MenuCNSubGorilla { get; private set; }
        public GameObject MenuCNSubBob { get; private set; }
        public GameObject MenuCNSubMingus { get; private set; }

        public TextMeshPro GD { get; private set; }
        public TextMeshPro BD { get; private set; }
        public TextMeshPro DD { get; private set; }
        public TextMeshPro MD { get; private set; }

        void Awake() { Data = this; MenuStartButton = new List<GameObject>(); MenuNights = new List<GameObject>(); gorilla = new List<GameObject>(); bob = new List<GameObject>(); mingus = new List<GameObject>(); dingus = new List<GameObject>(); CameraButtons = new List<GameObject>(); }
        public void SetRefrences()
        {
            if (!AlreadySetRefrences)
            {
                ChainLoader = gameObject;
            GameSkyColor = GameObject.Find("GameSkyRefrence").GetComponent<Renderer>().material;
            LeftDoorVoid = GameObject.Find("BLACK VOID L");
            RightDoorVoid = GameObject.Find("BLACK VOID R");
            RightDoorObject = GameObject.Find("RightDoor");
            LeftDoorObject = GameObject.Find("LeftDoor");
            RightDoor = GameObject.Find("CloseRightDoor");
            LeftDoor = GameObject.Find("CloseLeftDoor");
            LeftLight = GameObject.Find("LightLeftDoor");
            RightLight = GameObject.Find("LightRightDoor");
            Cam1 = GameObject.Find("CameraButton1");
            Cam2 = GameObject.Find("CameraButton2");
            Cam3 = GameObject.Find("CameraButton3");
            Cam4 = GameObject.Find("CameraButton4");
            Cam5 = GameObject.Find("CameraButton5");
            Cam6 = GameObject.Find("CameraButton6");
            Cam7 = GameObject.Find("CameraButton7");
            Cam8 = GameObject.Find("CameraButton8");
            Cam9 = GameObject.Find("CameraButton9");
            Cam10 = GameObject.Find("CameraButton10");
            Cam11 = GameObject.Find("CameraButton11");
                CameraButtons.Add(Cam1);
                CameraButtons.Add(Cam2);
                CameraButtons.Add(Cam3);
                CameraButtons.Add(Cam4);
                CameraButtons.Add(Cam5);
                CameraButtons.Add(Cam6);
                CameraButtons.Add(Cam7);
                CameraButtons.Add(Cam8);
                CameraButtons.Add(Cam9);
                CameraButtons.Add(Cam10);
                CameraButtons.Add(Cam11);
            CameraScreen = GameObject.Find($"{FNAGMAP.name}/Office/Cameras/Monitor/Screen/CameraScreen");
            CurrentTime = GameObject.Find("CurrentTime").GetComponent<TextMeshPro>();
            CurrentPower = GameObject.Find("CurrentPower").GetComponent<TextMeshPro>();
            gorilla.Add(GameObject.Find("gorilla(CAM11)"));
            gorilla.Add(GameObject.Find("gorilla(CAM10)"));
            gorilla.Add(GameObject.Find("gorilla(CAM5)"));
            gorilla.Add(GameObject.Find("gorilla(CAM4)"));
            gorilla.Add(GameObject.Find("gorilla(CAM3)"));
            mingus.Add(GameObject.Find("mingus(CAM11)"));
            mingus.Add(GameObject.Find("mingus(CAM10)"));
            mingus.Add(GameObject.Find("mingus(CAM9)"));
            mingus.Add(GameObject.Find("mingus(CAM7)"));
            mingus.Add(GameObject.Find("mingus(CAM1)"));
            mingus.Add(GameObject.Find("mingus(CAM2)"));
            dingus.Add(GameObject.Find("dingus(state1)"));
            dingus.Add(GameObject.Find("dingus(state2)"));
            dingus.Add(GameObject.Find("dingus(state3)"));
            dingus.Add(GameObject.Find("dingus(state4)"));
            dingus.Add(GameObject.Find("dingus(state5)"));
            dingus.Add(GameObject.Find("dingus(state6)"));
            bob.Add(GameObject.Find("bob(CAM11)"));
            bob.Add(GameObject.Find("bob(CAM10)"));
            bob.Add(GameObject.Find("bob(CAM6)"));
            bob.Add(GameObject.Find("bob(CAM4)"));
            bob.Add(GameObject.Find("bob(CAM3)"));

                JumpscareParent = GameObject.Find("gorillaJS-PARENT");
                //add all the jumpscare gameobjets like this with the "ai name" as the first bit
                Jumpscares.Add("gorilla", JumpscareParent.transform.GetChild(0).gameObject);

                JumpscareAnimation = GameObject.Find($"{_Jumpscares.name}/gorillaJS-PARENT/gorillaJS").GetComponent<Animator>();
            SixAM = GameObject.Find($"{_Jumpscares.name}/6AM");
                DingusRunning = GameObject.Find("Dingus Running").GetComponent<AudioSource>();
                JumpscareSound = GameObject.Find("Jumpscares").GetComponent<AudioSource>();
                SixAMSound = GameObject.Find($"{FNAGMAP.name}/Audio/6AM").GetComponent<AudioSource>();
                Poweroutage = GameObject.Find("PowerOutage").GetComponent<AudioSource>();
                MenuIgnoreButton = GameObject.Find("Menu(Clone)/Warning/IgnoreWarning/Button");
                MenuWarning = GameObject.Find("Warning");
                MenuScrollLeftButton = GameObject.Find($"{Menu.name}/Select left/Button");
                MenuScrollRightButton = GameObject.Find($"{Menu.name}/Select right/Button");
                MenuScrollLeft = GameObject.Find($"{Menu.name}/Select left/");
                MenuScrollRight = GameObject.Find($"{Menu.name}/Select right/");
                MenuStartButton.Add(GameObject.Find("START(n1)"));
                MenuStartButton.Add(GameObject.Find("START(n2)"));
                MenuStartButton.Add(GameObject.Find("START(n3)"));
                MenuStartButton.Add(GameObject.Find("START(n4)"));
                MenuStartButton.Add(GameObject.Find("START(n5)"));
                MenuStartButton.Add(GameObject.Find("START(n6)"));
                MenuStartButton.Add(GameObject.Find("START(cn)"));
                MenuCNAddBob = GameObject.Find("B plus CN");
                MenuCNAddDingus = GameObject.Find("D plus CN");
                MenuCNAddGorilla = GameObject.Find("G plus CN");
                MenuCNAddMingus = GameObject.Find("M plus CN");
                MenuCNSubBob = GameObject.Find("B minus CN");
                MenuCNSubDingus = GameObject.Find("D minus CN");
                MenuCNSubGorilla = GameObject.Find("G minus CN");
                MenuCNSubMingus = GameObject.Find("M minus CN");
                NearGameTrigger = GameObject.Find("MPDetectTrigger");
                LeftLightSound = GameObject.Find("LeftDoorLightAudio").GetComponent<AudioSource>();
                RightLightSound = GameObject.Find("RightDoorLightAudio").GetComponent<AudioSource>();
                RightDoorSound = GameObject.Find("RightDoorCloseAndOpenAudio").GetComponent<AudioSource>();
                LeftDoorSound = GameObject.Find("LeftDoorCloseAndOpenAudio").GetComponent<AudioSource>();
                dingusParent = GameObject.Find("Dingus");
                gorillaParent = GameObject.Find("Gorilla");
                mingusParent = GameObject.Find("Mingus");
                bobParent = GameObject.Find("Bob");
                Cam1Nothing = GameObject.Find("CameraTexture(1-nothing)").GetComponent<Renderer>().material;
                Cam1Mingus = GameObject.Find("CameraTexture(1-mingus)").GetComponent<Renderer>().material;
                Cam2Mingus = GameObject.Find("CameraTexture(2-mingus)").GetComponent<Renderer>().material;
                Cam2Nothing = GameObject.Find("CameraTexture(2-nothing)").GetComponent<Renderer>().material;
                Cam3Bob = GameObject.Find("CameraTexture(3-bob)").GetComponent<Renderer>().material;
                Cam3Gorilla = GameObject.Find("CameraTexture(3-gorilla)").GetComponent<Renderer>().material;
                Cam3Nothing = GameObject.Find("CameraTexture(3-nothing)").GetComponent<Renderer>().material;
                Cam4Bob = GameObject.Find("CameraTexture(4-bob)").GetComponent<Renderer>().material;
                Cam4Gorilla = GameObject.Find("CameraTexture(4-gorilla)").GetComponent<Renderer>().material;
                Cam4Nothing = GameObject.Find("CameraTexture(4-nothing)").GetComponent<Renderer>().material;
                Cam5Credits = GameObject.Find("CameraTexture(5-credits)").GetComponent<Renderer>().material;
                Cam6Bob = GameObject.Find("CameraTexture(6-bob)").GetComponent<Renderer>().material;
                Cam6Nothing = GameObject.Find("CameraTexture(6-nothing)").GetComponent<Renderer>().material;
                Cam7Mingus = GameObject.Find("CameraTexture(7-mingus)").GetComponent<Renderer>().material;
                Cam7Nothing = GameObject.Find("CameraTexture(7-nothing)").GetComponent<Renderer>().material;
                Cam8Dingus1 = GameObject.Find("CameraTexture(8-dingus-1)").GetComponent<Renderer>().material;
                Cam8Dingus2 = GameObject.Find("CameraTexture(8-dingus-2)").GetComponent<Renderer>().material;
                Cam8Dingus3 = GameObject.Find("CameraTexture(8-dingus-3)").GetComponent<Renderer>().material;
                Cam8Dingus4 = GameObject.Find("CameraTexture(8-dingus-4)").GetComponent<Renderer>().material;
                Cam8Dingus5 = GameObject.Find("CameraTexture(8-dingus-5)").GetComponent<Renderer>().material;
                Cam8Dingus6 = GameObject.Find("CameraTexture(8-dingus-6)").GetComponent<Renderer>().material;
                Cam9Mingus = GameObject.Find("CameraTexture(9-mingus)").GetComponent<Renderer>().material;
                Cam9Nothing = GameObject.Find("CameraTexture(9-nothing)").GetComponent<Renderer>().material;
                Cam10All = GameObject.Find("CameraTexture(10-all)").GetComponent<Renderer>().material;
                Cam10Bob = GameObject.Find("CameraTexture(10-bob)").GetComponent<Renderer>().material;
                Cam10BobGorilla = GameObject.Find("CameraTexture(10-bob-gorilla)").GetComponent<Renderer>().material;
                Cam10BobMingus = GameObject.Find("CameraTexture(10-bob-mingus)").GetComponent<Renderer>().material;
                Cam10Gorilla = GameObject.Find("CameraTexture(10-gorilla)").GetComponent<Renderer>().material;
                Cam10GorillaMingus = GameObject.Find("CameraTexture(10-gorilla-mingus)").GetComponent<Renderer>().material;
                Cam10Mingus = GameObject.Find("CameraTexture(10-mingus)").GetComponent<Renderer>().material;
                Cam10Nothing = GameObject.Find("CameraTexture(10-nothing)").GetComponent<Renderer>().material;
                Cam11All = GameObject.Find("CameraTexture(11-all)").GetComponent<Renderer>().material;
                Cam11Bob = GameObject.Find("CameraTexture(11-bob)").GetComponent<Renderer>().material;
                Cam11Gorilla = GameObject.Find("CameraTexture(11-gorilla)").GetComponent<Renderer>().material;
                Cam11GorillaBob = GameObject.Find("CameraTexture(11-gorilla-bob)").GetComponent<Renderer>().material;
                Cam11GorillaMingus = GameObject.Find("CameraTexture(11-gorilla-mingus)").GetComponent<Renderer>().material;
                Cam11Mingus = GameObject.Find("CameraTexture(11-mingus)").GetComponent<Renderer>().material;
                Cam11MingusBob = GameObject.Find("CameraTexture(11-mingus-bob)").GetComponent<Renderer>().material;
                Cam11Nothing = GameObject.Find("CameraTexture(11-nothing)").GetComponent<Renderer>().material;
                MenuSelects = GameObject.Find("Selects");
                NightOneSelect = GameObject.Find("Night One Select");
                NightTwoSelect = GameObject.Find("Night Two Select");
                NightThreeSelect = GameObject.Find("Night Three Select");
                NightFourSelect = GameObject.Find("Night Four Select");
                NightFiveSelect = GameObject.Find("Night Five Select");
                NightSixSelect = GameObject.Find("Night Six Select");
                CustomNightSelect = GameObject.Find("Custom Night Select");
                GD = GameObject.Find("GD(cn)").GetComponent<TextMeshPro>();
                DD = GameObject.Find("DD(cn)").GetComponent<TextMeshPro>();
                BD = GameObject.Find("BD(cn)").GetComponent<TextMeshPro>();
                MD = GameObject.Find("MD(cn)").GetComponent<TextMeshPro>();
                WhiteRefrence = GameObject.Find("WhiteRefrence").GetComponent<Renderer>().material;
                RedRefrence = GameObject.Find("RedRefrence").GetComponent<Renderer>().material;
                MenuRoundRunning = GameObject.Find("RoundRunning");
                RightDoorFailSound = GameObject.Find("RightDoorFailAudio").GetComponent<AudioSource>();
                LeftDoorFailSound = GameObject.Find("LeftDoorFailAudio").GetComponent<AudioSource>();
                CameraButtonPressSound = GameObject.Find("Camera Button Sound").GetComponent<AudioSource>();
                DingusScrapingSound = GameObject.Find("DingusScrapingDoor").GetComponent<AudioSource>();
                GorillaBoopSound = GameObject.Find("GorillaBoopSound").GetComponent<AudioSource>();
                gorillaBoopTrigger = GameObject.Find("gorilla Boop Trigger");
                MenuNights.Add(NightOneSelect);
                MenuNights.Add(NightTwoSelect);
                MenuNights.Add(NightThreeSelect);
                MenuNights.Add(NightFourSelect);
                MenuNights.Add(NightFiveSelect);
                MenuNights.Add(NightSixSelect);
                MenuNights.Add(CustomNightSelect);

                AlreadySetRefrences = true;
            }
            else
            {
                Debug.LogError("Refrences have already been successfully set!");
            }
        }
    }
}