using UnityEngine.UI;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using System.Collections;
using Newtonsoft.Json;

public class Menu : MonoBehaviour
{
    public enum Role { seeker, keeper, chaser1, chaser2, chaser3, beater1, beater2 }

    #region Panels
    //Panels and GUI
    private GameObject panelHauptmenue;
    private GameObject panelSettings;
    private GameObject panelHelp;
    private GameObject panelStart;
    private GameObject panelPlacePlayer;
    private GameObject panelGame;
    private GameObject panelSpectate;
    private GameObject panelError;
    private GameObject panelErrorReconnect;
    private GameObject panelScore;
    private GameObject panelWarning;
    private GameObject panelPause;
    private GameObject panelDefeat;
    private GameObject panelLoading;
    private GameObject panelSpectatePause;
    private GameObject panelMiniMap;
    private GameObject panelBalls;
    private GameObject panelPlayer;
    private GameObject panelHelpFans;
    private GameObject panelAnimation;

    public GameObject Spielfeld;
    
    #endregion

    #region Help Gui Elements
    //Help-Fan
    private Image imageHelpFans;
    private Text textFanName;
    private Text textFanDescription;
    public Sprite[] FanImages = new Sprite[5];
    private int activeFanHelp;
    private string[] FanNamen = new string[5];
    private string[] FanDescriptions = new string[5];

    //Help-Player    
    private GameObject panelHelpPlayer;
    private Image imageHelpPlayer;
    private Text textPlayerName;
    private Text textPlayerDescription;

    public Sprite[] PlayerImages = new Sprite[4];
    private int activePlayerHelp;
    private string[] PlayerNamen = new string[4];
    private string[] PlayerDescriptions = new string[4];

    //Help-HowToWin
    private GameObject panelHelpHWT;

    //Help-Fouls
    private GameObject panelHelpFouls;
    private Text textFoulName;
    private Text textFoulDescription;
    private int activeFoulHelp;
    private string[] FoulNamen = new string[5];
    private string[] FoulDescriptions = new string[5];
    #endregion

    #region Small windows Gui elements
    //Win Defeat
    private Text winnerIs;
    private Text scores;
    private Text reason;

    //PauseButton
    private GameObject pauseButton;

    //Warning
    private Text warningText;

    //Score GUI
    private Text ScoreLeft;
    private Text ScoreRight;
    //error text
    private Text errorText;

    private GameObject Background;
    private GameObject MenuBackground;
    private GameObject ueberschrift;
    public GameObject setPhase;

    //Input
    private InputField teamConfig;
    private InputField partieConfig;

    //ListView
    private GameObject ListView;
    private GameObject PrefabListView;

    //images
    public Sprite[] roleImage = new Sprite[4];

    //Volumen
    private Text volumenText;
    private Slider volumenSlider;
    private Text volumenEffectsText;
    private Slider volumenEffectsSlider;

    //Move Decision GUI
    private GameObject moveDecisionPanel;
    #endregion

    #region vars (GameLoop, Sound, Path, Prefab)

    private string teamPath;
    private string partiePath;
    public Player[] players { get; set; }
    public PartieConfig partieConfigDeserialized;
    GameObject[] listPrefabs;
    //animation
    private Animator timeoutAnimator;

    //Log
    private GameObject logText;
    private ArrayList logTextArray;

    //Sound
    AudioSource Music;
    public GameObject soundEffectsGameObject;

    //GameLoop Script connection
    public GameObject gLoop;
    public int lastSet;
    public TeamConfigJson sendToServer;

    //MiniMap
    private static readonly int scaleFactorX = 45;
    private static readonly int scaleFactorY = 45;
    public Image[] balls = new Image[4];
    public Image[] PlayerTeamLeft = new Image[7];
    public Image[] PlayerTeamRight = new Image[7];
    public Sprite toggleLeft;
    public Sprite toggleRight;
    private Button toggleButton;
    private bool ballsMinimap;
    //Spectate Back Button
    private GameObject spectateBackButton;
    #endregion

    #region fanButtons
    private Button nifflerButton;
    private Button goblinsButton;
    private Button elvesButton;
    private Button wombatsButton;
    private Button trollsButton;
    #endregion

    void Start()
    {
        InitialisePanels();
        InitialiseGuiElements();
        InitialisePath();
        InitialiseSoundOptions();
        InitialiseHelp();
        InitialiseStartPanel();
        InitialiseFan();
        InitialiseMinimap();
    }

    #region Initialise all
        //Initialise Canvas Components
        private void InitialisePanels()
        {
            panelHauptmenue = transform.Find("Panel_Hauptmenu").gameObject;
            panelSettings = transform.Find("Panel_Settings").gameObject;
            panelHelp = transform.Find("Panel_Help").gameObject;
            panelStart = transform.Find("Panel_Start").gameObject;
            panelPlacePlayer = transform.Find("Panel_PlaceGUI").gameObject;
            panelGame = transform.Find("Panel_Game").gameObject;
            panelSpectate = transform.Find("Panel_Spectate").gameObject;
            panelError = transform.Find("Panel_Error").gameObject;
            panelErrorReconnect = transform.Find("Panel_Error_Disconnect").gameObject;
            panelScore = transform.Find("Panel_Score").gameObject;
            panelWarning = transform.Find("Panel_Warning").gameObject;
            panelPause = transform.Find("Panel_Pause").gameObject;
            panelDefeat = transform.Find("Panel_Defeat").gameObject;
            panelLoading = transform.Find("Panel_Loading").gameObject;
            panelSpectatePause = transform.Find("Panel_SpectatePause").gameObject;
            panelMiniMap = transform.Find("Panel_Minimap").gameObject;
            panelBalls = panelMiniMap.transform.Find("Map").transform.Find("Panel_Balls").gameObject;
            panelPlayer = panelMiniMap.transform.Find("Map").transform.Find("Panel_Player").gameObject;
            panelHelpFans = panelHelp.transform.Find("Panel_Fans").gameObject;
            Background = transform.Find("background").gameObject;
            MenuBackground = transform.Find("menu").gameObject;
            ueberschrift = transform.Find("TextUeberschrift").gameObject;
            panelHelpPlayer = panelHelp.transform.Find("Panel_Player").gameObject;
            panelHelpHWT = panelHelp.transform.Find("Panel_HWT").gameObject;
            panelHelpFouls = panelHelp.transform.Find("Panel_Fouls").gameObject;

    }
        //Initialise help Gui Elements
        private void InitialiseGuiElements()
        {
            //pause Button
            pauseButton = panelGame.transform.Find("StartPauseButton").gameObject;
            //Player
            imageHelpPlayer = panelHelpPlayer.transform.Find("ImagePlayer").gameObject.GetComponent<Image>();
            textPlayerName = panelHelpPlayer.transform.Find("TextPlayerName").gameObject.GetComponent<Text>();
            textPlayerDescription = panelHelpPlayer.transform.Find("TextPlayerDescription").gameObject.GetComponent<Text>();
            //Fan
            imageHelpFans = panelHelpFans.transform.Find("ImageFan").gameObject.GetComponent<Image>();
            textFanName = panelHelpFans.transform.Find("TextFanName").gameObject.GetComponent<Text>();
            textFanDescription = panelHelpFans.transform.Find("TextFanDescription").gameObject.GetComponent<Text>();
            //Fouls
            textFoulName = panelHelpFouls.transform.Find("TextFoulName").gameObject.GetComponent<Text>();
            textFoulDescription = panelHelpFouls.transform.Find("TextFoulDescription").gameObject.GetComponent<Text>();
            //Win Defeat
            winnerIs = panelDefeat.transform.Find("TextWinnerIs").gameObject.GetComponent<Text>();
            scores = panelDefeat.transform.Find("TextScore").gameObject.GetComponent<Text>();
            reason = panelDefeat.transform.Find("TextReason").gameObject.GetComponent<Text>();
            //Warning
            warningText = panelWarning.transform.Find("TextWarning").gameObject.GetComponent<Text>();
            //Scores
            ScoreLeft = panelScore.transform.Find("TextLeft").gameObject.GetComponent<Text>();
            ScoreRight = panelScore.transform.Find("TextRight").gameObject.GetComponent<Text>();
            //error text
            errorText = panelError.transform.Find("TextError").gameObject.GetComponent<Text>();
            //start input
            teamConfig = panelStart.transform.Find("InputFieldTeamConfig").gameObject.GetComponent<InputField>();
            partieConfig = panelStart.transform.Find("InputFieldPartieConfig").gameObject.GetComponent<InputField>();
            //placeing
            ListView = transform.Find("Panel_PlaceGUI").transform.Find("Listview").gameObject;
            PrefabListView = transform.Find("Panel_PlaceGUI").transform.Find("ListPrefab").gameObject;
            //Volumen
            volumenText = panelSettings.transform.Find("TextVolumenSlider").gameObject.GetComponent<Text>();
            volumenSlider = panelSettings.transform.Find("SliderVolume").gameObject.GetComponent<Slider>();
            volumenEffectsText = panelSettings.transform.Find("TextVolumenSliderEffects").gameObject.GetComponent<Text>();
            volumenEffectsSlider = panelSettings.transform.Find("SliderVolumeEffects").gameObject.GetComponent<Slider>();
            //move decision handle
            moveDecisionPanel = panelGame.transform.Find("Panel_MakeDecision").gameObject;
            //SpectateButton
            spectateBackButton = panelScore.transform.Find("ButtonBackToMenu").gameObject;
            //minimap
            toggleButton = panelMiniMap.transform.Find("Toggle_Ball_Player").gameObject.GetComponent<Button>();
            //Fan Buttons
            nifflerButton = panelGame.transform.Find("NifflerFanButton").gameObject.GetComponent<Button>();
            goblinsButton = panelGame.transform.Find("GoblinFanButton").gameObject.GetComponent<Button>();
            elvesButton = panelGame.transform.Find("ElveFanButton").gameObject.GetComponent<Button>();
            wombatsButton = panelGame.transform.Find("WombatFanButton").gameObject.GetComponent<Button>();
            trollsButton = panelGame.transform.Find("TrollFanButton").gameObject.GetComponent<Button>();
            //panel timeout
            panelAnimation = transform.Find("Panel_Animation").gameObject;
            timeoutAnimator = panelAnimation.GetComponent<Animator>();
            //log text
            logText = panelScore.transform.Find("Logging").gameObject;
            logTextArray = new ArrayList();
        }

        //Initialise all Help GUI
        private void InitialiseHelp()
            {
                //Fan text
                activeFanHelp = 0;
                FanNamen[0] = "Niffler:";
                FanNamen[1] = "Goblin:";
                FanNamen[2] = "Troll.";
                FanNamen[3] = "Elf.";
                FanNamen[4] = "Wombat:";

                FanDescriptions[0] = "They like shiny thinks, he can make the snitch jump to a neighbor field.";
                FanDescriptions[1] = "They can stun player, if this player has the quaffel, he will drop it and he will stumble on a neighbor available field.";
                FanDescriptions[2] = "They can roar scary, and the Player with the quaffel will drop it.";
                FanDescriptions[3] = "They can teleport a random enemy or teammate to an empty field, they can only teleport the Player not the balls.";
                FanDescriptions[4] = "They can shit on a field and this field is blocked for the next round.";

                //Player text
                activePlayerHelp = 0;
                PlayerNamen[0] = "Seeker:";
                PlayerNamen[1] = "Keeper:";
                PlayerNamen[2] = "Chaser:";
                PlayerNamen[3] = "Beater:";

                PlayerDescriptions[0] = "He has one job, to catch the snitch, if they can win with 30 points.";
                PlayerDescriptions[1] = "He protects the gates, they can take the quaffel and drow them but they can't score goals.";
                PlayerDescriptions[2] = "They try to throw the quaffel to the gates and score a goal. Only one Chaser can be in the enemys devense zone.";
                PlayerDescriptions[3] = "They got a club, to defend the teammates from the bludger. They can walk on a bludger and trhow him in the next round.";

                //Foul text
                activeFoulHelp = 0;
                FoulNamen[0] = "flacking:";
                FoulNamen[1] = "haversacking:";
                FoulNamen[2] = "stooging:";
                FoulNamen[3] = "blatching:";
                FoulNamen[4] = "snitchnip:";

                FoulDescriptions[0] = "All Players can stand on a gate and defend a goal, if the quaffel would go into the gate, it will drop on a free neighboring field.";
                FoulDescriptions[1] = "A chaser can fly into a gate with the quaffel, he will score a goal, but he makes a foul with that move.";
                FoulDescriptions[2] = "If a second Chaser flys into the enemys defense zone, its a foul. You can only have two Chaser in the enemys defense zone.";
                FoulDescriptions[3] = "If you go on a field with a Player on it, this player will loose his ball and he will be repressed to a neighbor field.";
                FoulDescriptions[4] = "If you aren't a seeker, dont touch the snitch.";
            }
        //Set correct panel at teh start
        private void InitialiseStartPanel()
        {
            panelPlacePlayer.SetActive(false);
            panelGame.SetActive(false);
            panelScore.SetActive(false);
            Spielfeld.SetActive(false);
            panelErrorReconnect.SetActive(false);
            panelWarning.SetActive(false);
            panelPause.SetActive(false);
            panelDefeat.SetActive(false);
            panelSettings.SetActive(false);
            panelHelp.SetActive(false);
            panelStart.SetActive(false);
            panelSpectate.SetActive(false);
            panelError.SetActive(false);
            panelLoading.SetActive(false);
            spectateBackButton.SetActive(false);
            panelMiniMap.SetActive(false);
            panelPlayer.SetActive(false);
            panelBalls.SetActive(true);
            Background.SetActive(true);
            MenuBackground.SetActive(true);
            ueberschrift.SetActive(true);
            panelHauptmenue.SetActive(true);
        }
        //Initialise Sound Vars
        private void InitialiseSoundOptions()
        {
            Music = gameObject.GetComponent<AudioSource>();

            volumenSlider.value = LoadSound();
            volumenEffectsSlider.value = LoadVolumenEffects();
            UpdateVolumen();
        }
        //Initialise Path value
        private void InitialisePath()
        {
            teamPath = "";
            partiePath = "";
        }
        //Initialise Fans value
        private void InitialiseFan()
        {
            nifflerButton.interactable = false;
            trollsButton.interactable = false;
            elvesButton.interactable = false;
            wombatsButton.interactable = false;
            goblinsButton.interactable = false;
        }
        //Initialise minimap
        private void InitialiseMinimap()
        {
            ballsMinimap = true;
        }
        
    #endregion

    #region Main Menu
    //get the right panels in the foreground
    public void StartButton()
    {
        panelStart.SetActive(true);
        panelHauptmenue.SetActive(false);
        ueberschrift.GetComponent<Text>().text = "Start";
    }
    //get to the Spectate Menu
    public void SpectateButton()
    {
        panelSpectate.SetActive(true);
        panelHauptmenue.SetActive(false);
        ueberschrift.GetComponent<Text>().text = "Spectate";
    }
    //get the right panels in the foreground
    public void SettingsButton()
    {
        panelHauptmenue.SetActive(false);
        panelSettings.SetActive(true);
        ueberschrift.GetComponent<Text>().text = "Settings";
    }
    //get the right panels in the foreground and go back to main menu
    public void BackButton()
    {
        panelSpectate.SetActive(false);
        panelSettings.SetActive(false);
        panelHelp.SetActive(false);
        panelStart.SetActive(false);
        panelHauptmenue.SetActive(true);
        SaveSound(volumenSlider.value);
        SaveVolumenEffects(volumenEffectsSlider.value);
        ueberschrift.GetComponent<Text>().text = "Menu";
    }
    //exit the game
    public void ExitButton()
    {
        Application.Quit();
    }
    #endregion

    #region Help Menu

    //Fouls
    #region
    //Foul Help Button
    public void FoulButton()
    {
        panelHelpFouls.SetActive(true);
        panelHelpHWT.SetActive(false);
        panelHelpPlayer.SetActive(false);
        panelHelpFans.SetActive(false);
        UpdateFoulHelp();
    }
    //Update Foul infos
    public void UpdateFoulHelp()
    {
        textFoulName.text = FoulNamen[activeFoulHelp];
        textFoulDescription.text = FoulDescriptions[activeFoulHelp];
    }
    //Foul right click
    public void FoulRightButton()
    {
        activeFoulHelp++;
        if (activeFoulHelp > 4) { activeFoulHelp = 0; }
        UpdateFoulHelp();
    }
    //Foul left click
    public void FoulLeftButton()
    {
        activeFoulHelp--;
        if (activeFoulHelp < 0) { activeFoulHelp = 4; }
        UpdateFoulHelp();
    }
    #endregion
    //Fan
    #region
    //Fan Help Button
    public void FanButton()
    {
        panelHelpPlayer.SetActive(false);
        panelHelpFouls.SetActive(false);
        panelHelpHWT.SetActive(false);
        panelHelpFans.SetActive(true);
        UpdateFanHelp();

    }
    //Update Fan infos
    public void UpdateFanHelp()
    {
        textFanName.text = FanNamen[activeFanHelp];
        textFanDescription.text = FanDescriptions[activeFanHelp];
        imageHelpFans.sprite = FanImages[activeFanHelp];
    }
    //Fan right click
    public void FanRightButton()
    {
        activeFanHelp++;
        if (activeFanHelp > 4) { activeFanHelp = 0; }
        UpdateFanHelp();
    }
    //Fan left click
    public void FanLeftButton()
    {
        activeFanHelp--;
        if (activeFanHelp < 0) { activeFanHelp = 4; }
        UpdateFanHelp();
    }
    #endregion

    //Player
    #region
    //Player Help Button
    public void PlayerButton()
    {
        panelHelpFouls.SetActive(false);
        panelHelpHWT.SetActive(false);
        panelHelpPlayer.SetActive(true);
        panelHelpFans.SetActive(false);
        UpdatePlayerHelp();

    }
    //Update Player infos
    public void UpdatePlayerHelp()
    {
        textPlayerName.text = PlayerNamen[activePlayerHelp];
        textPlayerDescription.text = PlayerDescriptions[activePlayerHelp];
        imageHelpPlayer.sprite = PlayerImages[activePlayerHelp];
    }
    //Player right click
    public void PlayerRightButton()
    {
        activePlayerHelp++;
        if (activePlayerHelp > 3) { activePlayerHelp = 0; }
        UpdatePlayerHelp();
    }
    //Player left click
    public void PlayerLeftButton()
    {
        activePlayerHelp--;
        if (activePlayerHelp < 0) { activePlayerHelp = 3; }
        UpdatePlayerHelp();
    }
    #endregion

    //How To Win
    #region
    //HWT Button
    public void HWTButton()
    {
        panelHelpFouls.SetActive(false);
        panelHelpHWT.SetActive(true);
        panelHelpPlayer.SetActive(false);
        panelHelpFans.SetActive(false);
    }
    #endregion

    //HelpButton
    public void HelpButton()
    {
        panelHauptmenue.SetActive(false);
        panelHelp.SetActive(true);
        panelHelpFans.SetActive(true);
        panelHelpPlayer.SetActive(false);
        panelHelpFouls.SetActive(false);
        panelHelpHWT.SetActive(false);
        ueberschrift.GetComponent<Text>().text = "Help";
        UpdateFanHelp();
    }

    #endregion

    #region Sound
    //refresh GUI for volumen
    public void UpdateVolumen()
    {
        volumenText.text = "" + (int)(volumenSlider.value * 100) + " %";
        volumenEffectsText.text = "" + (int)(volumenEffectsSlider.value * 100) + " %";
        Music.volume = volumenSlider.value;
    }
    //load sound settings from file
    float LoadSound()
    {
        string destination = Application.persistentDataPath + "/volumenMusic.dat";
        FileStream file;
        float returnSound;
        if (File.Exists(destination))
        {
            file = File.OpenRead(destination);
            BinaryFormatter bf = new BinaryFormatter();
            returnSound = (float)bf.Deserialize(file);
            file.Close();
        }
        else
        {
            return 0.5f;
        }
        return returnSound;
    }
    //save sound settings to file
    void SaveSound(float s)
    {
        string destination = Application.persistentDataPath + "/volumenMusic.dat";
        FileStream file;
        if (File.Exists(destination))
        {
            file = File.OpenWrite(destination);
        }
        else
        {
            file = File.Create(destination);
        }

        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, s);
        file.Close();
    }
    //load sound settings from file
    float LoadVolumenEffects()
    {
        string destination = Application.persistentDataPath + "/volumenEffectes.dat";
        FileStream file;
        float returnSound;
        if (File.Exists(destination))
        {
            file = File.OpenRead(destination);
            BinaryFormatter bf = new BinaryFormatter();
            returnSound = (float)bf.Deserialize(file);
            file.Close();
        }
        else
        {
            return 0.5f;
        }
        return returnSound;
    }
    //save sound settings to file
    void SaveVolumenEffects(float s)
    {
        string destination = Application.persistentDataPath + "/volumenEffectes.dat";
        FileStream file;
        if (File.Exists(destination))
        {
            file = File.OpenWrite(destination);
        }
        else
        {
            file = File.Create(destination);
        }

        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, s);
        file.Close();
    }

    #endregion

    #region Placing
    //OK Button after placing
    public void OKButtonPlaceing()
    {
        if (setPhase.GetComponent<SetPhase>().AllSet())
        {
            Spielfeld.SetActive(true);
            panelScore.SetActive(true);
            panelGame.SetActive(true);
            panelPlacePlayer.SetActive(false);
            spectateBackButton.SetActive(false);
            ChangeScore(0, 0);
        }
    }

    //unban phase ended GUI
    public void TurnOffPlacing()
    {
        panelPlacePlayer.SetActive(false);
        spectateBackButton.SetActive(false);

        panelPlacePlayer.transform.Find("ButtonPlacenOk").gameObject.SetActive(true);
        panelPlacePlayer.transform.Find("StandardformationButton").gameObject.SetActive(true);
        panelGame.transform.Find("StartPauseButton").gameObject.SetActive(true);
        panelGame.transform.Find("ButtonMiniMap").gameObject.SetActive(true);
        panelGame.transform.Find("SkipButton").gameObject.SetActive(true);


    }

    //start unban phase GUI
    public void UnbanPlacing()
    {
        panelPlacePlayer.SetActive(true);

        panelPlacePlayer.transform.Find("ButtonPlacenOk").gameObject.SetActive(false);
        panelPlacePlayer.transform.Find("StandardformationButton").gameObject.SetActive(false);
        panelGame.transform.Find("StartPauseButton").gameObject.SetActive(false);
        panelGame.transform.Find("ButtonMiniMap").gameObject.SetActive(false);
        panelGame.transform.Find("SkipButton").gameObject.SetActive(false);

    }

    //update List view (ListViewAdapter)
    public void ChangeViewState(int index, bool visible)
    {
        listPrefabs[index].SetActive(visible);
    }

    //if Item "index" is clicked
    public void ListItemClicked(int index)
    {
        lastSet = index;
    }
    //get Team informations from TeamConfig.json
    public Player[] DeserializeTeamConfig()
    {
        string dataAsJson = File.ReadAllText(teamPath);
        Debug.Log(dataAsJson);
        sendToServer = JsonConvert.DeserializeObject<TeamConfigJson>(dataAsJson);
        TeamConfigAdapter tca = JsonConvert.DeserializeObject<TeamConfigAdapter>(dataAsJson);
        return tca.GetPlayerArray();
    }
    //deserialize Partie Config
    public PartieConfig DeserializePartieConfig()
    {

        string dataAsJson = File.ReadAllText(partiePath);
        Debug.Log(dataAsJson);
        PartieConfig pc = JsonConvert.DeserializeObject<PartieConfig>(dataAsJson);
        return pc;

    }
    //OK button is pressed => start placing phase
    public void OkButton()
    {
        teamPath = teamConfig.text;
        partiePath = partieConfig.text;
        Debug.Log(teamPath);
        try
        {
            players = DeserializeTeamConfig();
        }
        catch (Exception e)
        {
            SendError("Something went wrong, please verify your input.");
            Debug.Log("failed deserialize");
            Debug.Log(e);

            return;
        }
        try
        {
            partieConfigDeserialized = DeserializePartieConfig();
            Debug.Log(partieConfigDeserialized);
        }
        catch (Exception e)
        {
            Debug.Log("failed deserialize Partie Config");
        }
        //loading sound
        soundEffectsGameObject.GetComponent<SoundEffects>().StartLoadingMusic();
        StartLoading();
        panelPlacePlayer.SetActive(true);
        panelGame.SetActive(false);
        MenuBackground.SetActive(false);
        panelSettings.SetActive(false);
        panelHelp.SetActive(false);
        panelHauptmenue.SetActive(false);
        panelStart.SetActive(false);
        ueberschrift.SetActive(false);
        spectateBackButton.SetActive(false);

        listPrefabs = new GameObject[7];


        for (int x = 0; x < 7; x++)
        {
            GameObject temp = Instantiate(PrefabListView, new Vector3(0, 0, 0), Quaternion.identity);
            temp.transform.parent = ListView.transform;
            temp.GetComponent<RectTransform>().sizeDelta = new Vector2(232, 61);
            temp.transform.localScale = (new Vector3(1, 1, 1));
            Transform name = temp.transform.GetChild(0);
            name.GetComponent<Text>().text = players[x].name;
            Transform broom = temp.transform.GetChild(1);
            broom.GetComponent<Text>().text = players[x].broom;
            Transform sex = temp.transform.GetChild(2);
            int a = x;
            temp.AddComponent<Button>().onClick.AddListener(() => ListItemClicked(a));
            Transform image = temp.transform.GetChild(3);

            switch (x)
            {
                case 0:
                    image.GetComponent<Image>().sprite = roleImage[0];
                    break;
                case 1:
                    image.GetComponent<Image>().sprite = roleImage[1];
                    break;
                case 2:
                    image.GetComponent<Image>().sprite = roleImage[2];
                    break;
                case 3:
                    image.GetComponent<Image>().sprite = roleImage[2];
                    break;
                case 4:
                    image.GetComponent<Image>().sprite = roleImage[2];
                    break;
                case 5:
                    image.GetComponent<Image>().sprite = roleImage[3];
                    break;
                case 6:
                    image.GetComponent<Image>().sprite = roleImage[3];
                    break;
            }

            if (players[x].sex == "m")
            {
                sex.GetComponent<Text>().text = "Male";
            }
            else
            {
                sex.GetComponent<Text>().text = "Female";
            }

            listPrefabs[x] = temp;
        }
        Spielfeld.SetActive(true);
    }
    #endregion

    #region Ingame
    //set Position of MoveDecisionGUI
    public void SetPositionMoveDecision(Vector3 pos)
    {
        moveDecisionPanel.GetComponent<RectTransform>().localPosition = pos;
    }

    public void SetActiveMoveDecision(bool activ)
    {
        moveDecisionPanel.SetActive(activ);
    }

    public void MoveDecision(bool activ, Vector3 pos)
    {
        SetActiveMoveDecision(activ);
        SetPositionMoveDecision(pos);
    }

    public void ChangeScore(int left, int right)
    {
        ScoreLeft.text = "" + left;
        ScoreRight.text = "" + right;
    }
    //Add log
    public void AddLog(string text)
    {
        if (logTextArray.Count == 6)
        {
            logTextArray.RemoveAt(0);
        }
        logTextArray.Add(text+"\n");
        UpdateLog();
    }
    //Update Log
    public void UpdateLog() {
        string s2 = "";

        foreach (string s in logTextArray)
        {
            s2 += s;
        }

        logText.GetComponent<Text>().text = s2;

    }

    //Set Fan active
    public void SetActiveFan(string fanName)
    {
        DeactivateAllFans();


        switch (fanName)
        {
            case "rightNiffler":
            case "leftNiffler":
                nifflerButton.interactable = true;
                break;
            case "rightGoblin":
            case "leftGoblin":
                goblinsButton.interactable = true;
                break;
            case "rightElf":
            case "leftElf":
                elvesButton.interactable = true;
                break;
            case "rightTroll":
            case "leftTroll":
                trollsButton.interactable = true;
                break;
            case "rightWombat":
            case "leftWombat":
                wombatsButton.interactable = true;
                break;
        }
    }

    public void DeactivateAllFans()
    {
        nifflerButton.interactable = false;
        trollsButton.interactable = false;
        elvesButton.interactable = false;
        wombatsButton.interactable = false;
        goblinsButton.interactable = false;
    }
    #endregion

    #region Warnings
    public void ShowWarning(string warning)
    {
        warningText.text = warning;
        panelWarning.SetActive(true);
    }

    public void CloseWarning()
    {
        panelWarning.SetActive(true);
    }
    #endregion

    #region Errors
    //close error message
    public void CloseError()
    {
        panelError.SetActive(false);
    }
    //From disconnect error to mainmenu
    public void DisconnectToMainMenu()
    {
        panelMiniMap.SetActive(false);
        Spielfeld.SetActive(false);
        MenuBackground.SetActive(true);
        panelScore.SetActive(false);
        panelHauptmenue.SetActive(true);
        ueberschrift.SetActive(true);
        panelPause.SetActive(false);
        panelGame.SetActive(false);
        panelLoading.SetActive(false);
        panelErrorReconnect.SetActive(false);
        Spielfeld.SetActive(false);
        ueberschrift.GetComponent<Text>().text = "Menu";
    }
    //Generate Error
    public void SendError(string error)
    {
        panelError.SetActive(true);
        errorText.text = error;
    }
    //Disconnect Error
    public void ShowDisconnectError()
    {
        panelErrorReconnect.SetActive(true);
    }
    #endregion

    #region Pause
    //Pause
    public void Pause()
    {
        panelPause.SetActive(true);
        pauseButton.SetActive(false);
    }
    //Unpause
    public void Unpause()
    {
        pauseButton.SetActive(true);
        panelPause.SetActive(false);
    }
    #endregion

    #region Win or Defeat
    public void WinOrDefeat(int team1, int team2, string victoryReason, string winnerName)
    {
        panelDefeat.SetActive(true);
        reason.text = victoryReason;
        scores.text = "" + team1 + " : " + team2;
        winnerIs.text = "The winner is " + winnerName + ".";
    }
    //Defeat to Mainmenu
    public void DefeatToMainmenu()
    {
        panelErrorReconnect.SetActive(false);
        Spielfeld.SetActive(false);
        panelHauptmenue.SetActive(false);
        panelDefeat.SetActive(false);
        panelSpectate.SetActive(false);
        panelSettings.SetActive(false);
        panelHelp.SetActive(false);
        panelStart.SetActive(false);
        panelScore.SetActive(false);
        panelPause.SetActive(false);
        panelGame.SetActive(false);
        panelMiniMap.SetActive(false);
        panelPlacePlayer.SetActive(false);
        spectateBackButton.SetActive(false);

        MenuBackground.SetActive(true);
        ueberschrift.SetActive(true);
        panelHauptmenue.SetActive(true);
        SaveSound(volumenSlider.value);
        SaveVolumenEffects(volumenEffectsSlider.value);
        ueberschrift.GetComponent<Text>().text = "Menu";
    }
    #endregion

    #region Loading

    public void StartLoading()
    {
        soundEffectsGameObject.GetComponent<SoundEffects>().StartLoadingMusic();
        panelLoading.SetActive(true);
    }

    public void StopLoading()
    {
        soundEffectsGameObject.GetComponent<SoundEffects>().StoppLoadingMusic();
        panelLoading.SetActive(false);
    }

    #endregion

    #region spectate
    //Sets field active for spectator
    private void Spectating()
    {
        spectateBackButton.SetActive(true);

        Spielfeld.SetActive(true);
        panelScore.SetActive(true);
        ChangeScore(0, 0);

        panelSpectate.SetActive(false);
        panelPlacePlayer.SetActive(false);
        panelGame.SetActive(false);
        panelPlacePlayer.SetActive(false);
        MenuBackground.SetActive(false);
        panelSettings.SetActive(false);
        panelHelp.SetActive(false);
        panelHauptmenue.SetActive(false);
        panelStart.SetActive(false);
        ueberschrift.SetActive(false);
    }


    //Start spectating
    public void StartSpectate()
    {
        StartLoading();
        Spectating();

    }
    //Start Spectate Pause
    public void SpectatePause()
    {
        panelSpectatePause.SetActive(true);
    }
    //Unpause
    public void SpectateUnpause()
    {
        panelSpectatePause.SetActive(false);
    }

    #endregion

    #region minimap

        //Back Button
        public void MinimapBackButton()
        {
            panelMiniMap.SetActive(false);
        }

        //Activate minimap
        public void ShowMinimap()
        {
            panelMiniMap.SetActive(true);
        }

       
        //Update Player Positions
        public void UpdatePlayerMinimap(PlayersSnapshotJson teamLeft, PlayersSnapshotJson teamRight)
        {
            UpdateTeamRightMinimap(teamRight);
            UpdateTeamLeftMinimap(teamLeft);
        }

    public void UpdateBallsMinimap(BallsJson blj)
    {
        balls[0].transform.localPosition = new Vector3((blj.bludger1.xPos - 8) * scaleFactorX, -(blj.bludger1.yPos - 6) * scaleFactorY, 0);
        balls[1].transform.localPosition = new Vector3((blj.bludger2.xPos - 8) * scaleFactorX, -(blj.bludger2.yPos - 6) * scaleFactorY, 0);
        balls[2].transform.localPosition = new Vector3((blj.quaffle.xPos - 8) * scaleFactorX, -(blj.quaffle.yPos - 6) * scaleFactorY, 0);

        if (blj.snitch.xPos == 0 && blj.snitch.yPos == 0)
        {
            balls[3].transform.localPosition = new Vector3((-20 - 8) * scaleFactorX, -(-20 - 6) * scaleFactorY, 0);
        }
        else
        {
            balls[3].transform.localPosition = new Vector3((blj.snitch.xPos - 8) * scaleFactorX, -(blj.snitch.yPos - 6) * scaleFactorY, 0);
        }

    }
    private void UpdateTeamLeftMinimap(PlayersSnapshotJson teamLeft)
    {


        if (teamLeft.seeker.xPos == 0 && teamLeft.seeker.yPos == 0) { PlayerTeamLeft[0].transform.localPosition = new Vector3((-28) * scaleFactorX, -(-26) * scaleFactorY, 0); }
        else
        {
            PlayerTeamLeft[0].transform.localPosition = new Vector3((teamLeft.seeker.xPos - 8) * scaleFactorX, -(teamLeft.seeker.yPos - 6) * scaleFactorY, 0);
        }
        if (teamLeft.keeper.xPos == 0 && teamLeft.keeper.yPos == 0) { PlayerTeamLeft[1].transform.localPosition = new Vector3((-28) * scaleFactorX, -(-26) * scaleFactorY, 0); }
        else
        {
            PlayerTeamLeft[1].transform.localPosition = new Vector3((teamLeft.keeper.xPos - 8) * scaleFactorX, -(teamLeft.keeper.yPos - 6) * scaleFactorY, 0);
        }
        if (teamLeft.beater1.xPos == 0 && teamLeft.beater1.yPos == 0) { PlayerTeamLeft[2].transform.localPosition = new Vector3((-28) * scaleFactorX, -(-26) * scaleFactorY, 0); }
        else
        {
            PlayerTeamLeft[2].transform.localPosition = new Vector3((teamLeft.beater1.xPos - 8) * scaleFactorX, -(teamLeft.beater1.yPos - 6) * scaleFactorY, 0);
        }
        if (teamLeft.beater2.xPos == 0 && teamLeft.beater2.yPos == 0) { PlayerTeamLeft[3].transform.localPosition = new Vector3((-28) * scaleFactorX, -(-26) * scaleFactorY, 0); }
        else
        {
            PlayerTeamLeft[3].transform.localPosition = new Vector3((teamLeft.beater2.xPos - 8) * scaleFactorX, -(teamLeft.beater2.yPos - 6) * scaleFactorY, 0);
        }
        if (teamLeft.chaser1.xPos == 0 && teamLeft.chaser1.yPos == 0) { PlayerTeamLeft[4].transform.localPosition = new Vector3((-28) * scaleFactorX, -(-26) * scaleFactorY, 0); }
        else
        {
            PlayerTeamLeft[4].transform.localPosition = new Vector3((teamLeft.chaser1.xPos - 8) * scaleFactorX, -(teamLeft.chaser1.yPos - 6) * scaleFactorY, 0);
        }
        if (teamLeft.chaser2.xPos == 0 && teamLeft.chaser2.yPos == 0) { PlayerTeamLeft[5].transform.localPosition = new Vector3((-28) * scaleFactorX, -(-26) * scaleFactorY, 0); }
        else
        {
            PlayerTeamLeft[5].transform.localPosition = new Vector3((teamLeft.chaser2.xPos - 8) * scaleFactorX, -(teamLeft.chaser2.yPos - 6) * scaleFactorY, 0);
        }
        if (teamLeft.chaser3.xPos == 0 && teamLeft.chaser3.yPos == 0) { PlayerTeamLeft[6].transform.localPosition = new Vector3((-28) * scaleFactorX, -(-26) * scaleFactorY, 0); }
        else
        {
            PlayerTeamLeft[6].transform.localPosition = new Vector3((teamLeft.chaser3.xPos - 8) * scaleFactorX, -(teamLeft.chaser3.yPos - 6) * scaleFactorY, 0);
        }

    }
    private void UpdateTeamRightMinimap(PlayersSnapshotJson teamRight)
    {
        if (teamRight.seeker.xPos == 0 && teamRight.seeker.yPos == 0) { PlayerTeamRight[0].transform.localPosition = new Vector3((-28) * scaleFactorX, -(-26) * scaleFactorY, 0); }
        else
        {
            PlayerTeamRight[0].transform.localPosition = new Vector3((teamRight.seeker.xPos - 8) * scaleFactorX, -(teamRight.seeker.yPos - 6) * scaleFactorY, 0);
        }
        if (teamRight.keeper.xPos == 0 && teamRight.keeper.yPos == 0) { PlayerTeamRight[1].transform.localPosition = new Vector3((-28) * scaleFactorX, -(-26) * scaleFactorY, 0); }
        else
        {
            PlayerTeamRight[1].transform.localPosition = new Vector3((teamRight.keeper.xPos - 8) * scaleFactorX, -(teamRight.keeper.yPos - 6) * scaleFactorY, 0);
        }
        if (teamRight.beater1.xPos == 0 && teamRight.beater1.yPos == 0) { PlayerTeamRight[2].transform.localPosition = new Vector3((-28) * scaleFactorX, -(-26) * scaleFactorY, 0); }
        else
        {
            PlayerTeamRight[2].transform.localPosition = new Vector3((teamRight.beater1.xPos - 8) * scaleFactorX, -(teamRight.beater1.yPos - 6) * scaleFactorY, 0);
        }
        if (teamRight.beater2.xPos == 0 && teamRight.beater2.yPos == 0) { PlayerTeamRight[3].transform.localPosition = new Vector3((-28) * scaleFactorX, -(-26) * scaleFactorY, 0); }
        else
        {
            PlayerTeamRight[3].transform.localPosition = new Vector3((teamRight.beater2.xPos - 8) * scaleFactorX, -(teamRight.beater2.yPos - 6) * scaleFactorY, 0);
        }
        if (teamRight.chaser1.xPos == 0 && teamRight.chaser1.yPos == 0) { PlayerTeamRight[4].transform.localPosition = new Vector3((-28) * scaleFactorX, -(-26) * scaleFactorY, 0); }
        else
        {
            PlayerTeamRight[4].transform.localPosition = new Vector3((teamRight.chaser1.xPos - 8) * scaleFactorX, -(teamRight.chaser1.yPos - 6) * scaleFactorY, 0);
        }
        if (teamRight.chaser2.xPos == 0 && teamRight.chaser2.yPos == 0) { PlayerTeamRight[5].transform.localPosition = new Vector3((-28) * scaleFactorX, -(-26) * scaleFactorY, 0); }
        else
        {
            PlayerTeamRight[5].transform.localPosition = new Vector3((teamRight.chaser2.xPos - 8) * scaleFactorX, -(teamRight.chaser2.yPos - 6) * scaleFactorY, 0);
        }
        if (teamRight.chaser3.xPos == 0 && teamRight.chaser3.yPos == 0) { PlayerTeamRight[6].transform.localPosition = new Vector3((-28) * scaleFactorX, -(-26) * scaleFactorY, 0); }
        else
        {
            PlayerTeamRight[6].transform.localPosition = new Vector3((teamRight.chaser3.xPos - 8) * scaleFactorX, -(teamRight.chaser3.yPos - 6) * scaleFactorY, 0);
        }
    }


    //Switch between Balls and Player
    public void ChangeBallPlayerMinimap()
        {
            ballsMinimap = !ballsMinimap;
            panelBalls.SetActive(ballsMinimap);
            panelPlayer.SetActive(!ballsMinimap);
            if (ballsMinimap)
            {
                toggleButton.GetComponent<Image>().sprite = toggleLeft;
            } else {
                toggleButton.GetComponent<Image>().sprite = toggleRight;
            }
        }

    #endregion

    #region Animation Timeout

    public void Animation(string text)
    {
        panelAnimation.transform.Find("TextAnim").gameObject.GetComponent<Text>().text = text;
        timeoutAnimator.Play("TimeoutAnimation");
    }

    #endregion
}