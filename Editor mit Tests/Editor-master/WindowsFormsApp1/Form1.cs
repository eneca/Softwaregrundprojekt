using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        //Vars
        int activePlayer;
        Data playerData;
        string openedFile;
        string openedFilePartie;
        Color[] trikotColor;
        PartieConfig partieConfig;

        //dragging
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        /*DLL fürs bewegen der Fenster*/
        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();
        /*Funktion zum bewegen des Fensters*/
        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (WindowState == FormWindowState.Normal)
                {
                    ReleaseCapture();
                    SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                }
            }
        }
        /*Funktion zum bewegen des Fensters*/
        private void OnControlMouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (WindowState == FormWindowState.Normal)
                {
                    ReleaseCapture();
                    SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                }
            }
        }
        /*Funktion zum bewegen des Fensters*/
        private void AddDraggingControl(System.Windows.Forms.Control theControl)
        {
            theControl.MouseMove += new System.Windows.Forms.MouseEventHandler(OnControlMouseMove);
        }


        /*initialisieren der Windows-Form*/
        public Form1()
        {
            InitializeComponent();

            AddDraggingControl(this.panelTop);
            AddDraggingControl(this.panelTopLeft);
            //initialize Players
            this.AllowDrop = true;
            activePlayer = 0;
            playerData = new Data();
            openedFile = "";
            panelTeam.Visible = false;
            panelPartie.Visible = false;
            trikotColor = new Color[2];
            trikotColor[0] = Color.Black;
            trikotColor[1] = Color.Black;
            partieConfig = new PartieConfig();
        }
        /*add Tooltips für verschiedene Funktionen*/
        private void Form1_Load(object sender, EventArgs e)
        {
            ToolTip toolTip = new ToolTip();
            toolTip.AutoPopDelay = 3050;
            toolTip.InitialDelay = 700;
            toolTip.ReshowDelay = 500;
            toolTip.ShowAlways = true;
            toolTip.SetToolTip(this.pictureBoxTeamImage, "Image 256 x 256");
            toolTip.SetToolTip(this.labelFans,"You need seven Fans");
            toolTip.SetToolTip(this.numericUpDownElf, "You need seven Fans");
            toolTip.SetToolTip(this.numericUpDownNiffler, "You need seven Fans");
            toolTip.SetToolTip(this.numericUpDownGoblin, "You need seven Fans");
            toolTip.SetToolTip(this.numericUpDownTroll, "You need seven Fans");
            toolTip.SetToolTip(this.labelFan1, "You need seven Fans");
            toolTip.SetToolTip(this.labelFan2, "You need seven Fans");
            toolTip.SetToolTip(this.labelFan3, "You need seven Fans");
            toolTip.SetToolTip(this.labelFan4, "You need seven Fans");
            toolTip.SetToolTip(this.textBoxMotto, "Insert a Motto for your Team");
            toolTip.SetToolTip(this.textBoxTeamname, "Insert a Teamname");
            toolTip.SetToolTip(this.buttonPrimaryColor, "Change Trikot Color");
            toolTip.SetToolTip(this.buttonRandomize, "Generate a random Team");
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        /*Event zum minimieren des Fenster*/
        private void button1_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        /*Event zum wechseln der Kategorie zu General*/
        private void buttonGeneral_Click(object sender, EventArgs e)
        {
            if (!InsertPartieConfigToClass())
            {
                return;
            }
            panelRedButtons.Location = new Point(0,65);
            panelTeam.Visible = false;
            panelPartie.Visible = false;
            buttonGeneral.ForeColor = ColorTranslator.FromHtml("#ba1a3b");
            buttonTeam.ForeColor = Color.White;
            buttonPartie.ForeColor = Color.White;
        }
        /*Event zum wechseln der Kategorie zu Team*/
        private void buttonTeam_Click(object sender, EventArgs e)
        {
            if (!InsertPartieConfigToClass())
            {
                return;
            }
            panelRedButtons.Location = new Point(0, 117);
            panelTeam.Visible = true;
            panelPartie.Visible = false;
            buttonTeam.ForeColor = ColorTranslator.FromHtml("#ba1a3b");
            buttonGeneral.ForeColor = Color.White;
            buttonPartie.ForeColor = Color.White;
        }
        /*Event für Partie Optionen*/
        private void buttonPartie_Click(object sender, EventArgs e)
        {

            if (!InsertPartieConfigToClass())
            {
                return;
            }
            panelTeam.Visible = true;
            panelPartie.Visible = true;
            panelRedButtons.Location = new Point(0,221);
            buttonTeam.ForeColor = Color.White;
            buttonGeneral.ForeColor = Color.White;
            buttonPartie.ForeColor = ColorTranslator.FromHtml("#ba1a3b");

            buttonRounds.BackColor = Color.White;
            buttonRounds.ForeColor = ColorTranslator.FromHtml("#ba1a3b");
            buttonPropabilities.BackColor = ColorTranslator.FromHtml("#ba1a3b");
            buttonPropabilities.ForeColor = Color.White;
            buttonFouls.BackColor = ColorTranslator.FromHtml("#ba1a3b");
            buttonFouls.ForeColor = Color.White;
            panelRounds.Visible = true;
            panelPropabilities.Visible = false;
            panelFouls.Visible = false;
            

        }
        private void buttonRounds_Click(object sender, EventArgs e)
        {
            buttonRounds.BackColor = Color.White;
            buttonRounds.ForeColor = ColorTranslator.FromHtml("#ba1a3b");
            buttonPropabilities.BackColor = ColorTranslator.FromHtml("#ba1a3b");
            buttonPropabilities.ForeColor = Color.White;
            buttonFouls.BackColor = ColorTranslator.FromHtml("#ba1a3b");
            buttonFouls.ForeColor = Color.White;
            panelRounds.Visible = true;
            panelPropabilities.Visible = false;
            panelFouls.Visible = false;
        }

        private void buttonPropabilities_Click(object sender, EventArgs e)
        {
            buttonPropabilities.BackColor = Color.White;
            buttonPropabilities.ForeColor = ColorTranslator.FromHtml("#ba1a3b");
            buttonRounds.BackColor = ColorTranslator.FromHtml("#ba1a3b");
            buttonRounds.ForeColor = Color.White;
            buttonFouls.BackColor = ColorTranslator.FromHtml("#ba1a3b");
            buttonFouls.ForeColor = Color.White;
            panelRounds.Visible = false;
            panelPropabilities.Visible = true;
            panelFouls.Visible = false;
        }

        private void buttonFouls_Click(object sender, EventArgs e)
        {
            buttonFouls.BackColor = Color.White;
            buttonFouls.ForeColor = ColorTranslator.FromHtml("#ba1a3b");
            buttonRounds.BackColor = ColorTranslator.FromHtml("#ba1a3b");
            buttonRounds.ForeColor = Color.White;
            buttonPropabilities.BackColor = ColorTranslator.FromHtml("#ba1a3b");
            buttonPropabilities.ForeColor = Color.White;
            panelRounds.Visible = false;
            panelPropabilities.Visible = false;
            panelFouls.Visible = true;
        }
        /*Event zum ändern der Farbe eines Trikos*/
        private void buttonPrimaryColor_Click(object sender, EventArgs e)
        {
            ColorDialog MyDialog = new ColorDialog();
            MyDialog.AllowFullOpen = true;
            MyDialog.Color = buttonPrimaryColor.BackColor;
            if (MyDialog.ShowDialog() == DialogResult.OK)
            {
                trikotColor[0] = MyDialog.Color;
                trikotColor[1] = GetKomplementaerColor(trikotColor[0]);
                UpdateTrikos();
            }
        }

        //save Json
        private void buttonSave_Click(object sender, EventArgs e)
        {
            Save();
            /*
            this.playerData.name[activePlayer] = this.textBoxName.Text ?? "";
            this.playerData.sex[activePlayer] = CheckRadioButtonGender();
            this.playerData.broom[activePlayer] = this.comboBox1.Text;

            if (!ValidateConfig())
            {
                return;
            }

            if (openedFile!="")
            {
                FileStream fs = new FileStream(openedFile, FileMode.Create);
                JsonSerializer serializer = new JsonSerializer();
                using (StreamWriter sw = new StreamWriter(fs))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    OutputData od = GenerateOutputData();
                    serializer.Serialize(writer, od);
                }
                fs.Close();
                return;
            }

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Json|*.json";
            saveFileDialog1.Title = "Save an Json File";
            saveFileDialog1.ShowDialog();

            // If the file name is not an empty string open it for saving.  
            if (saveFileDialog1.FileName != "")
            { 
                FileStream fs = (System.IO.FileStream)saveFileDialog1.OpenFile();
                JsonSerializer serializer = new JsonSerializer();
                using (StreamWriter sw = new StreamWriter(fs))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    OutputData od = GenerateOutputData();
                    serializer.Serialize(writer, od);
                }
                fs.Close();
            }*/
        }
        /*erstellen der Klasse Outputdata zum serializen*/
        private OutputData GenerateOutputData()
        {
            string name = textBoxTeamname.Text;
            string motto = textBoxMotto.Text;
            Bitmap image = new Bitmap(this.pictureBoxTeamImage.Image);
            string base64;
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Png);
                byte[] imageBytes = ms.ToArray();

                // Convert byte[] to Base64 String
                base64 = Convert.ToBase64String(imageBytes);
                ms.Close();
            }
            Colors colors = new Colors(trikotColor[0],trikotColor[1]);
            Fans fans = new Fans((int)this.numericUpDownGoblin.Value,(int)this.numericUpDownTroll.Value,(int)this.numericUpDownElf.Value,(int)this.numericUpDownNiffler.Value);

            Players players = new Players(playerData);
            return new OutputData(name, motto, colors, base64, fans, players);
        }

        //Check Radio Button Gender
        private string CheckRadioButtonGender() {
            string s;
            if (this.radioButtonMale.Checked)
            {
                s = "m";
            }
            else if (this.radioButtonFemale.Checked)
            {
                s = "f";
            }
            else
            {
                s = "";
            }
            return s;
        }
        //Set Radio Button Gender
        private void SetRadioButtonGender(string s)
        {
            switch (s) {  
                
                case "f":
                    radioButtonMale.Checked = false;
                    radioButtonFemale.Checked = true;
                    break;
                case "m":
                    radioButtonMale.Checked = true;
                    radioButtonFemale.Checked = false;
                    break;
                default:
                    radioButtonMale.Checked = false;
                    radioButtonFemale.Checked = false;
                    break;
             }
        }
        /*Asuwahl um Player 1 zu bearbeiten*/
        private void buttonPlayer1_Click(object sender, EventArgs e)
        {
            if (0 == activePlayer)
            {
                return;
            }
            this.labelRolle.Text = "Seeker:";
            this.playerData.name[activePlayer] = this.textBoxName.Text ?? "";
            this.playerData.sex[activePlayer] = CheckRadioButtonGender();
            this.playerData.broom[activePlayer] = this.comboBox1.Text;
            this.textBoxName.Text = this.playerData.name[0];
            if (this.textBoxName.Text == "" || this.textBoxName.Text == "Insert Name")
            {
                this.textBoxName.Text = "Insert Name";
                this.textBoxName.ForeColor = Color.Gray;
            }
            else
            {
                this.textBoxName.ForeColor = Color.Black;
            }
            SetRadioButtonGender(this.playerData.sex[0]);
            this.comboBox1.Text = this.playerData.broom[0];
            if (playerData.sex[0] == "f")
            {
                this.buttonPlayer1.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.redKontaktFemale;
            }
            else
            {
                this.buttonPlayer1.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.redKontakt;
            }
            ChangeOldButtonImage(activePlayer);
            activePlayer = 0;
        }
        /*Asuwahl um Player 2 zu bearbeiten*/
        private void buttonPlayer2_Click(object sender, EventArgs e)
        {
            if (1 == activePlayer)
            {
                return;
            }
            this.labelRolle.Text = "Keeper:";
            this.playerData.name[activePlayer] = this.textBoxName.Text ?? "";
            this.playerData.sex[activePlayer] = CheckRadioButtonGender();
            this.playerData.broom[activePlayer] = this.comboBox1.Text;
            this.textBoxName.Text = this.playerData.name[1];
            if (this.textBoxName.Text == "" || this.textBoxName.Text == "Insert Name")
            {
                this.textBoxName.Text = "Insert Name";
                this.textBoxName.ForeColor = Color.Gray;
            }
            else
            {
                this.textBoxName.ForeColor = Color.Black;
            }
            SetRadioButtonGender(this.playerData.sex[1]);
            this.comboBox1.Text = this.playerData.broom[1];
            if (playerData.sex[1] == "f")
            {
                this.buttonPlayer2.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.redKontaktFemale;
            }
            else
            {
                this.buttonPlayer2.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.redKontakt;
            }
            ChangeOldButtonImage(activePlayer);
            activePlayer = 1;
        }
        /*Asuwahl um Player 3 zu bearbeiten*/
        private void buttonPlayer3_Click(object sender, EventArgs e)
        {
            if (2 == activePlayer)
            {
                return;
            }
            this.labelRolle.Text = "Chaser:";
            this.playerData.name[activePlayer] = this.textBoxName.Text ?? "";
            this.playerData.sex[activePlayer] = CheckRadioButtonGender();
            this.playerData.broom[activePlayer] = this.comboBox1.Text;
            this.textBoxName.Text = this.playerData.name[2];
            if (this.textBoxName.Text == "" || this.textBoxName.Text == "Insert Name")
            {
                this.textBoxName.Text = "Insert Name";
                this.textBoxName.ForeColor = Color.Gray;
            }
            else
            {
                this.textBoxName.ForeColor = Color.Black;
            }
            SetRadioButtonGender(this.playerData.sex[2]);
            this.comboBox1.Text = this.playerData.broom[2];
            if (playerData.sex[2] == "f")
            {
                this.buttonPlayer3.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.redKontaktFemale;
            }
            else
            {
                this.buttonPlayer3.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.redKontakt;
            }
            ChangeOldButtonImage(activePlayer);
            activePlayer = 2;
        }
        /*Asuwahl um Player 4 zu bearbeiten*/
        private void buttonPlayer4_Click(object sender, EventArgs e)
        {
            if (3 == activePlayer)
            {
                return;
            }
            this.labelRolle.Text = "Chaser:";
            this.playerData.name[activePlayer] = this.textBoxName.Text ?? "";
            this.playerData.sex[activePlayer] = CheckRadioButtonGender();
            this.playerData.broom[activePlayer] = this.comboBox1.Text;
            this.textBoxName.Text = this.playerData.name[3];
            if (this.textBoxName.Text == "" || this.textBoxName.Text == "Insert Name")
            {
                this.textBoxName.Text = "Insert Name";
                this.textBoxName.ForeColor = Color.Gray;
            }
            else
            {
                this.textBoxName.ForeColor = Color.Black;
            }
            SetRadioButtonGender(this.playerData.sex[3]);
            this.comboBox1.Text = this.playerData.broom[3];
            if (playerData.sex[3] == "f")
            {
                this.buttonPlayer4.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.redKontaktFemale;
            }
            else
            {
                this.buttonPlayer4.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.redKontakt;
            }
            ChangeOldButtonImage(activePlayer);
            activePlayer = 3;
        }
        /*Asuwahl um Player 5 zu bearbeiten*/
        private void buttonPlayer5_Click(object sender, EventArgs e)
        {
            if (4 == activePlayer)
            {
                return;
            }
            this.labelRolle.Text = "Chaser:";
            this.playerData.name[activePlayer] = this.textBoxName.Text ?? "";
            this.playerData.sex[activePlayer] = CheckRadioButtonGender();
            this.playerData.broom[activePlayer] = this.comboBox1.Text;
            this.textBoxName.Text = this.playerData.name[4];
            if (this.textBoxName.Text == "" || this.textBoxName.Text == "Insert Name")
            {
                this.textBoxName.Text = "Insert Name";
                this.textBoxName.ForeColor = Color.Gray;
            }
            else
            {
                this.textBoxName.ForeColor = Color.Black;
            }
            SetRadioButtonGender(this.playerData.sex[4]);
            this.comboBox1.Text = this.playerData.broom[4];
            if (playerData.sex[4] == "f")
            {
                this.buttonPlayer5.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.redKontaktFemale;
            }
            else
            {
                this.buttonPlayer5.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.redKontakt;
            }
            ChangeOldButtonImage(activePlayer);
            activePlayer = 4;
        }
        /*Asuwahl um Player 6 zu bearbeiten*/
        private void buttonPlayer6_Click(object sender, EventArgs e)
        {
            if (5 == activePlayer)
            {
                return;
            }
            this.labelRolle.Text = "Beater:";
            this.playerData.name[activePlayer] = this.textBoxName.Text ?? "";
            this.playerData.sex[activePlayer] = CheckRadioButtonGender();
            this.playerData.broom[activePlayer] = this.comboBox1.Text;
            this.textBoxName.Text = this.playerData.name[5];
            if (this.textBoxName.Text == "" || this.textBoxName.Text == "Insert Name")
            {
                this.textBoxName.Text = "Insert Name";
                this.textBoxName.ForeColor = Color.Gray;
            }
            else
            {
                this.textBoxName.ForeColor = Color.Black;
            }
            SetRadioButtonGender(this.playerData.sex[5]);
            this.comboBox1.Text = this.playerData.broom[5];
            if (playerData.sex[5] == "f")
            {
                this.buttonPlayer6.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.redKontaktFemale;
            }
            else
            {
                this.buttonPlayer6.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.redKontakt;
            }
            ChangeOldButtonImage(activePlayer);
            activePlayer = 5;
        }
        /*Asuwahl um Player 7 zu bearbeiten*/
        private void buttonPlayer7_Click(object sender, EventArgs e)
        {
            if (6 == activePlayer)
            {
                return;
            }
            this.labelRolle.Text = "Beater:";
            this.playerData.name[activePlayer] = this.textBoxName.Text ?? "";
            this.playerData.sex[activePlayer] = CheckRadioButtonGender();
            this.playerData.broom[activePlayer] = this.comboBox1.Text;
            this.textBoxName.Text = this.playerData.name[6];
            if (this.textBoxName.Text == "" || this.textBoxName.Text == "Insert Name")
            {
                this.textBoxName.Text = "Insert Name";
                this.textBoxName.ForeColor = Color.Gray;
            }
            else
            {
                this.textBoxName.ForeColor = Color.Black;
            }
            SetRadioButtonGender(this.playerData.sex[6]);
            this.comboBox1.Text = this.playerData.broom[6];
            if (playerData.sex[6] == "f")
            {
                this.buttonPlayer7.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.redKontaktFemale;
            }
            else
            {
                this.buttonPlayer7.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.redKontakt;
            }
            ChangeOldButtonImage(activePlayer);
            activePlayer = 6;
        }
        /*speichern der Daten und anpassen der GUI*/
        private void ChangeOldButtonImage(int oldButton)
        {
            switch (oldButton)
            {

                case 0:
                    if (playerData.sex[0]=="f")
                    {
                        this.buttonPlayer1.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.kontaktFemale;
                    }
                    else
                    {
                        this.buttonPlayer1.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.kontakt;
                    }
                    break;
                case 1:
                    if (playerData.sex[1] == "f")
                    {
                        this.buttonPlayer2.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.kontaktFemale;
                    }
                    else
                    {
                        this.buttonPlayer2.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.kontakt;
                    }
                    break;
                case 2:
                    if (playerData.sex[2] == "f")
                    {
                        this.buttonPlayer3.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.kontaktFemale;
                    }
                    else
                    {
                        this.buttonPlayer3.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.kontakt;
                    }
                    break;
                case 3:
                    if (playerData.sex[3] == "f")
                    {
                        this.buttonPlayer4.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.kontaktFemale;
                    }
                    else
                    {
                        this.buttonPlayer4.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.kontakt;
                    }
                    break;
                case 4:
                    if (playerData.sex[4] == "f")
                    {
                        this.buttonPlayer5.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.kontaktFemale;
                    }
                    else
                    {
                        this.buttonPlayer5.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.kontakt;
                    }
                    break;
                case 5:
                    if (playerData.sex[5] == "f")
                    {
                        this.buttonPlayer6.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.kontaktFemale;
                    }
                    else
                    {
                        this.buttonPlayer6.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.kontakt;
                    }
                    break;
                case 6:
                    if (playerData.sex[6] == "f")
                    {
                        this.buttonPlayer7.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.kontaktFemale;
                    }
                    else
                    {
                        this.buttonPlayer7.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.kontakt;
                    }
                    break;
            }


        }
        /*Dialog zum öffnen eines Bildes*/
        private void buttonOpenImage_Click(object sender, EventArgs e)
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;
            Bitmap image;
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;

                    image = new Bitmap(filePath);
                    image = CropImage(image);
                    this.pictureBoxTeamImage.Image = image;
                }
            }

        }

        private Bitmap CropImage(Bitmap bmp) {
            return new Bitmap(bmp,256,256);
        }


        /*dezerializieren der Input File und zuweisen zu passenden Datenstruktur*/
        private void InsertData(OutputData data) {
            textBoxTeamname.Text = data.Name;
            textBoxMotto.Text = data.Motto;

            string base64 = data.Image;
            Bitmap bmp;
            using (var ms = new MemoryStream(Convert.FromBase64String(base64)))
            {
                bmp = new Bitmap(ms);
            }
            this.pictureBoxTeamImage.Image = bmp;

            trikotColor[0] = data.color.GetPrimaryColor();
            trikotColor[1] = data.color.GetSecondaryColor();

            this.numericUpDownGoblin.Value = data.Fans.goblins;
            this.numericUpDownTroll.Value = data.Fans.trolls;
            this.numericUpDownElf.Value = data.Fans.elfs;
            this.numericUpDownNiffler.Value = data.Fans.nifflers;



            playerData = data.Players.getData();

            this.textBoxName.Text = this.playerData.name[0];
            SetRadioButtonGender(this.playerData.sex[0]);
            this.comboBox1.Text = this.playerData.broom[0];
            this.buttonPlayer1.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.redKontakt;
            ChangeOldButtonImage(1);
            ChangeOldButtonImage(2);
            ChangeOldButtonImage(3);
            ChangeOldButtonImage(4);
            ChangeOldButtonImage(5);
            ChangeOldButtonImage(6);
            activePlayer = 0;

        }
        /*Event zum öffnen einer Json File*/
        private void button11_Click(object sender, EventArgs e)
        {


            var fileContent = string.Empty;
            var filePath = string.Empty;
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "Json|*.json";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                   
                    filePath = openFileDialog.FileName;
                    OutputData dataFromFile = JsonConvert.DeserializeObject<OutputData>(File.ReadAllText(filePath));
                    if (dataFromFile == null)
                    {
                        MessageBox.Show("Something went wrong while trying to open File. Pleas check File.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    this.openedFile = filePath;
                    InsertData(dataFromFile);
                    UpdateTrikos();
                }
            }

        }

        /*Implementierung der Drag and Drop Funktion (des Bildes)*/
        private void panel1_DragEnter(object sender, DragEventArgs e)
        {
            string[] file = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            bool img = file[0].EndsWith("png") || file[0].EndsWith("jpg") || file[0].EndsWith("jpeg");

            if (img)
            {
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }
        /*Implementierung der Drag and Drop Funktion (des Bildes)*/
        private void panel1_DragDrop(object sender, DragEventArgs e)
        {
            string[] file = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            bool img = file[0].EndsWith("png") || file[0].EndsWith("jpg") || file[0].EndsWith("jpeg");

            if (img)
            {
                Bitmap image = new Bitmap(file[0]);
                image = CropImage(image);
                this.pictureBoxTeamImage.Image = image;
            }
        }
        /*Implementierung der Drag and Drop Funktion*/
        private void panelTeam_DragEnter(object sender, DragEventArgs e)
        {
            string[] file = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            bool img = file[0].EndsWith("json");

            if (img)
            {
                e.Effect = DragDropEffects.Move;
            }
        }
        /*Implementierung der Drag and Drop Funktion*/
        private void panelTeam_DragDrop(object sender, DragEventArgs e)
        {
            string[] file = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            string filePath = file[0];
            loadFile(filePath);
            
        }
        /*Implementierung der Drag and Drop Funktion*/
        private void panelGeneral_DragEnter(object sender, DragEventArgs e)
        {
            string[] file = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            bool img = file[0].EndsWith("json");

            if (img)
            {
                e.Effect = DragDropEffects.Move;
            }
        }
        /*Implementierung der Drag and Drop Funktion*/
        private void panelGeneral_DragDrop(object sender, DragEventArgs e)
        {
            string[] file = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            string filePath = file[0];

            loadFile(filePath);
            /*
            OutputData dataFromFile = JsonConvert.DeserializeObject<OutputData>(File.ReadAllText(filePath));
            if (dataFromFile == null)
            {
                MessageBox.Show("Something went wrong while trying to open File. Pleas check File.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            this.openedFile = filePath;
            InsertData(dataFromFile);
            UpdateTrikos();*/
        }
        /*ändern des Geschlechts und aktualisierung aller Geschlechter (Female)*/
        private void radioButtonMale_MouseClick(object sender, MouseEventArgs e)
        {
            this.playerData.sex[activePlayer] = CheckRadioButtonGender();
            switch (activePlayer)
            {

                case 0:
                    if (playerData.sex[0] == "f")
                    {
                        this.buttonPlayer1.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.redKontaktFemale;
                    }
                    else
                    {
                        this.buttonPlayer1.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.redKontakt;
                    }
                    break;
                case 1:
                    if (playerData.sex[1] == "f")
                    {
                        this.buttonPlayer2.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.redKontaktFemale;
                    }
                    else
                    {
                        this.buttonPlayer2.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.redKontakt;
                    }
                    break;
                case 2:
                    if (playerData.sex[2] == "f")
                    {
                        this.buttonPlayer3.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.redKontaktFemale;
                    }
                    else
                    {
                        this.buttonPlayer3.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.redKontakt;
                    }
                    break;
                case 3:
                    if (playerData.sex[3] == "f")
                    {
                        this.buttonPlayer4.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.redKontaktFemale;
                    }
                    else
                    {
                        this.buttonPlayer4.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.redKontakt;
                    }
                    break;
                case 4:
                    if (playerData.sex[4] == "f")
                    {
                        this.buttonPlayer5.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.redKontaktFemale;
                    }
                    else
                    {
                        this.buttonPlayer5.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.redKontakt;
                    }
                    break;
                case 5:
                    if (playerData.sex[5] == "f")
                    {
                        this.buttonPlayer6.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.redKontaktFemale;
                    }
                    else
                    {
                        this.buttonPlayer6.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.redKontakt;
                    }
                    break;
                case 6:
                    if (playerData.sex[6] == "f")
                    {
                        this.buttonPlayer7.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.redKontaktFemale;
                    }
                    else
                    {
                        this.buttonPlayer7.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.redKontakt;
                    }
                    break;
            }
        }
        /*ändern des Geschlechts und aktualisierung aller Geschlechter (Female)*/
        private void radioButtonFemale_MouseClick(object sender, MouseEventArgs e)
        {
            this.playerData.sex[activePlayer] = CheckRadioButtonGender();
            switch (activePlayer)
            {
                case 0:
                    if (playerData.sex[0] == "f")
                    {
                        this.buttonPlayer1.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.redKontaktFemale;
                    }
                    else
                    {
                        this.buttonPlayer1.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.redKontakt;
                    }
                    break;
                case 1:
                    if (playerData.sex[1] == "f")
                    {
                        this.buttonPlayer2.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.redKontaktFemale;
                    }
                    else
                    {
                        this.buttonPlayer2.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.redKontakt;
                    }
                    break;
                case 2:
                    if (playerData.sex[2] == "f")
                    {
                        this.buttonPlayer3.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.redKontaktFemale;
                    }
                    else
                    {
                        this.buttonPlayer3.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.redKontakt;
                    }
                    break;
                case 3:
                    if (playerData.sex[3] == "f")
                    {
                        this.buttonPlayer4.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.redKontaktFemale;
                    }
                    else
                    {
                        this.buttonPlayer4.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.redKontakt;
                    }
                    break;
                case 4:
                    if (playerData.sex[4] == "f")
                    {
                        this.buttonPlayer5.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.redKontaktFemale;
                    }
                    else
                    {
                        this.buttonPlayer5.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.redKontakt;
                    }
                    break;
                case 5:
                    if (playerData.sex[5] == "f")
                    {
                        this.buttonPlayer6.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.redKontaktFemale;
                    }
                    else
                    {
                        this.buttonPlayer6.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.redKontakt;
                    }
                    break;
                case 6:
                    if (playerData.sex[6] == "f")
                    {
                        this.buttonPlayer7.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.redKontaktFemale;
                    }
                    else
                    {
                        this.buttonPlayer7.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.redKontakt;
                    }
                    break;
            }

        }
        /*Hilfe anzeigen beim hovern*/
        private void textBoxName_Enter(object sender, EventArgs e)
        {
            this.textBoxName.Text = "";
            this.textBoxName.ForeColor = Color.Black;
        }
        /*Hilfe anzeigen beim hovern*/
        private void textBoxName_Leave(object sender, EventArgs e)
        {
            if (this.textBoxName.Text == "")
            {
                this.textBoxName.Text = "Insert Name";
                this.textBoxName.ForeColor = Color.Gray;
            }
        }
        /*Hilfe anzeigen beim hovern*/
        private void textBoxTeamname_Enter(object sender, EventArgs e)
        {
            if (this.textBoxTeamname.Text == "Insert Team-Name")
            {
                this.textBoxTeamname.Text = "";
                this.textBoxTeamname.ForeColor = Color.Black;
            }
        }
        /*Hilfe anzeigen beim hovern*/
        private void textBoxTeamname_Leave(object sender, EventArgs e)
        {
            if (this.textBoxTeamname.Text == "")
            {
                this.textBoxTeamname.Text = "Insert Team-Name";
                this.textBoxTeamname.ForeColor = Color.Gray;
            }
        }
        /*Hilfe anzeigen beim hovern*/
        private void textBoxMotto_Enter(object sender, EventArgs e)
        {
            if (this.textBoxMotto.Text == "Insert Motto")
            {
                this.textBoxMotto.Text = "";
                this.textBoxMotto.ForeColor = Color.Black;
            }
        }
        /*Hilfe anzeigen beim hovern*/
        private void textBoxMotto_Leave(object sender, EventArgs e)
        {
            if (this.textBoxMotto.Text == "")
            {
                this.textBoxMotto.Text = "Insert Motto";
                this.textBoxMotto.ForeColor = Color.Gray;
            }
        }
        /*Event zum speichern der Json Datei*/
        private void button3_Click(object sender, EventArgs e)
        {
            SaveAs();
        }
        /*checken ob Anzahl der Fans valide ist*/
        private bool CheckFansNumbers()
        {
            int c = 0;
            c += (int)this.numericUpDownElf.Value;
            c += (int)this.numericUpDownTroll.Value;
            c += (int)this.numericUpDownNiffler.Value;
            c += (int)this.numericUpDownGoblin.Value;
            if (c>7)
            {
                return false;
            }
            return true;
        }
        /*Event zum verändern der anzahl der Fans*/
        private void numericUpDownElf_ValueChanged(object sender, EventArgs e)
        {
            if (CheckFansNumbers())
            {
                this.labelTooManyFans.Visible = false;
            }
            else
            {
                this.labelTooManyFans.Visible = true;
            }
        }
        /*Event zum verändern der anzahl der Fans*/
        private void numericUpDownNiffler_ValueChanged(object sender, EventArgs e)
        {
            if (CheckFansNumbers())
            {
                this.labelTooManyFans.Visible = false;
            }
            else
            {
                this.labelTooManyFans.Visible = true;
            }
        }
        /*Event zum verändern der anzahl der Fans*/
        private void numericUpDownGoblin_ValueChanged(object sender, EventArgs e)
        {
            if (CheckFansNumbers())
            {
                this.labelTooManyFans.Visible = false;
            }
            else
            {
                this.labelTooManyFans.Visible = true;
            }
        }
        /*Event zum verändern der anzahl der Fans*/
        private void numericUpDownTroll_ValueChanged(object sender, EventArgs e)
        {
            if (CheckFansNumbers())
            {
                this.labelTooManyFans.Visible = false;
            }
            else
            {
                this.labelTooManyFans.Visible = true;
            }
        }

        /*Validierung der Teamconfigurtion und Fehlermeldung falls notwendig*/
        private bool ValidateConfig()
        {
            //save current active Player
            this.playerData.name[activePlayer] = this.textBoxName.Text ?? "";
            this.playerData.sex[activePlayer] = CheckRadioButtonGender();
            this.playerData.broom[activePlayer] = this.comboBox1.Text;


            bool valid = true;
            List<string> problems = new List<string>();

            if (this.textBoxTeamname.Text == "" || this.textBoxTeamname.Text == "Insert Team-Name" || this.textBoxTeamname.Text.Length < 4)
            {
                problems.Add("Missing Team-Name");
                valid = false;
            }
            if (this.textBoxMotto.Text == "" || this.textBoxMotto.Text == "Insert Motto" || this.textBoxMotto.Text.Length < 4)
            {
                problems.Add("Missing Motto");
                valid = false;
            }
            if (trikotColor[0].Equals(Color.Black) && trikotColor[1].Equals(Color.Black))
            {
                problems.Add("Missing Color");
                valid = false;
            }
            int fans = (int)this.numericUpDownElf.Value + (int)this.numericUpDownTroll.Value + (int)this.numericUpDownGoblin.Value + (int)this.numericUpDownNiffler.Value;
            if (fans!=7 || !CheckFansNumbers())
            {
                problems.Add("Wrong number of Fans");
                valid = false;
            }
            if (this.pictureBoxTeamImage.Image == null)    
            {
                problems.Add("Missing Image");
                valid = false;
            }
            int gender = 0;
            bool[] brooms = new bool[5];
            for(int x=0; x<7; x++)
            {
                if (playerData.sex[x]=="f")
                {
                    gender--;
                }
                else if(playerData.sex[x] == "m")
                {
                    gender++;
                }else
                {
                    int c = x + 1;
                    problems.Add("Missing Gender at Player: "+c);
                    valid = false;
                }
                if (playerData.name[x] == "" || playerData.name[x] == "Insert Name" || playerData.name[x].Length < 3)
                {
                    int c = x + 1;
                    problems.Add("Wrong or None Name at Player: " + c);
                    valid = false;
                }

                switch (playerData.broom[x]) {
                    case "Zunderfauch":
                        brooms[0] = true;
                        break;
                    case "Sauberwisch 11":
                        brooms[1] = true;
                        break;
                    case "Komet 2-60":
                        brooms[2] = true;
                        break;
                    case "Nimbus 2001":
                        brooms[3] = true;
                        break;
                    case "Feuerblitz":
                        brooms[4] = true;
                        break;
                    case "":
                        int c = x + 1;
                        problems.Add("Missing Broom at Player: " + c);
                        valid = false;
                        break;
                }
            }
            if (!(brooms[0] && brooms[1] && brooms[2] && brooms[3] && brooms[4]))
            {
                problems.Add("You need each broom at least one time");
                valid = false;
            }

            if (gender < -1)
            {
                problems.Add("Too many Females");
                valid = false;
            }
            if (gender > 1)
            {
                problems.Add("Too many Males");
                valid = false;
            }

            string message = "Your Configuration wasn't valid:\n";

            if (!valid)
            {
                foreach (string s in problems)
                {
                    message += "     "+s + "\n";
                }
                MessageBox.Show(message, "Teamconfiguration Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return valid;
        }
        /*Hilfsmethode zum ersetzen von farben in einem Bitmap*/
        private Bitmap ChangeColor(Bitmap bm, Color c)
        {
            for (int x = 0; x < 100; x++)
            {
                for (int y = 0; y < 100; y++)
                {
                    Color c2 = Color.FromArgb(bm.GetPixel(x, y).A, c.R, c.G, c.B);
                    bm.SetPixel(x, y, c2);
                }
            }
            return bm;
        }
        /*erstellen der komplementär Farbe*/
        private Color GetKomplementaerColor(Color c)
        {
            return Color.FromArgb(c.A,255 - c.R,255 - c.G, 255 - c.B);
        }

        /*Anpassung der Trikotfarbe, mit komplementär Farbe*/
        private void UpdateTrikotBackground()
        {
            Bitmap bm = new Bitmap(this.pictureBoxTrikotPrimary.BackgroundImage);
            int t = trikotColor[0].R + trikotColor[0].G + trikotColor[0].B;
            Color c1 = Color.FromArgb(255,0,0,0);
            if (t < 380)
            {
                c1 = Color.FromArgb(255, 0, 0, 0);
            }
            else
            {
                c1 = Color.FromArgb(255, 255,255,255);
            }

            for (int x = 0; x < 100; x++)
            {
                for (int y = 0; y < 100; y++)
                {
                   
                    bm.SetPixel(x, y, c1);
                }
            }
            this.pictureBoxTrikotSecondary.BackgroundImage = bm;

            Bitmap bm2 = new Bitmap(this.pictureBoxTrikotPrimary.BackgroundImage);
            int t2 = trikotColor[1].R + trikotColor[1].G + trikotColor[1].B;
            Color c2 = Color.FromArgb(255, 0, 0, 0);
            if (t2 < 380)
            {
                c2 = Color.FromArgb(255, 0, 0, 0);
            }
            else
            {
                c2 = Color.FromArgb(255, 255, 255, 255);
            }

            for (int x = 0; x < 100; x++)
            {
                for (int y = 0; y < 100; y++)
                {

                    bm2.SetPixel(x, y, c2);
                }
            }
            this.pictureBoxTrikotPrimary.BackgroundImage = bm2;
        }
        /*neu laden der Trikos der GUI*/
        private void UpdateTrikos()
        {
            Bitmap bm1 = new Bitmap(this.pictureBoxTrikotPrimary.Image);
            bm1 = ChangeColor(bm1, trikotColor[0]);
            this.pictureBoxTrikotPrimary.Image = bm1;

            Bitmap bm2 = new Bitmap(this.pictureBoxTrikotSecondary.Image);
            bm1 = ChangeColor(bm2, trikotColor[1]);
            this.pictureBoxTrikotSecondary.Image = bm2;

            UpdateTrikotBackground();
        }

        /*Event für drücken des random Buttons*/
        private void button4_Click(object sender, EventArgs e)
        {
            RandomTeam rt = new RandomTeam();
            InsertData(rt.RandomTeamComposition());
            UpdateTrikos();
            this.textBoxName.ForeColor = Color.Black;
        }

        private bool InsertPartieConfigToClass()
        {
            try
            {
                //max rounds and timeouts
                partieConfig.MaxRounds = int.Parse(textBoxMaxRounds.Text);
                partieConfig.Timeouts.PlayerTurnTimeout = int.Parse(textBoxPlayerTurnTimeout.Text);
                partieConfig.Timeouts.FanTurnTimeout = int.Parse(textBoxFanTurnTimeout.Text);
                partieConfig.Timeouts.PlayerPhaseTime = int.Parse(textBoxPlayerPhaseTime.Text);
                partieConfig.Timeouts.FanPhaseTime = int.Parse(textBoxFanPhaseTime.Text);
                partieConfig.Timeouts.BallPhaseTime = int.Parse(textBoxBallPhaseTime.Text);

                //propabilities
                partieConfig.Propabilities.Goal = float.Parse(textBoxGoal.Text);
                partieConfig.Propabilities.ThrowSuccess = float.Parse(textBoxThrowSucces.Text);
                partieConfig.Propabilities.KnockOut = float.Parse(textBoxKnockOut.Text);
                partieConfig.Propabilities.FoolAway = float.Parse(textBoxfoolAway.Text);
                partieConfig.Propabilities.CatchSnitch = float.Parse(textBoxCatchSnitch.Text);
                partieConfig.Propabilities.CatchQuaffle = float.Parse(textBoxCatchQuaffle.Text);
                partieConfig.Propabilities.WrestQuaffle = float.Parse(textBoxWrestQuaffle.Text);

                //extraMove
                partieConfig.Propabilities.ExtraMove.thinderblast = float.Parse(textBoxThinderblast.Text);
                partieConfig.Propabilities.ExtraMove.cleansweep11 = float.Parse(textBoxCleansweep11.Text);
                partieConfig.Propabilities.ExtraMove.comet260 = float.Parse(textBoxComet260.Text);
                partieConfig.Propabilities.ExtraMove.nimbus2001 = float.Parse(textBoxNimbus2001.Text);
                partieConfig.Propabilities.ExtraMove.firebolt = float.Parse(textBoxFirebolt.Text);

                //foul Detection
                partieConfig.FoulDetection.Flacking = float.Parse(textBoxFlacking.Text);
                partieConfig.FoulDetection.Haversacking = float.Parse(textBoxHaversacking.Text);
                partieConfig.FoulDetection.Stooging = float.Parse(textBoxStooging.Text);
                partieConfig.FoulDetection.Blatching = float.Parse(textBoxBlatching.Text);
                partieConfig.FoulDetection.Snitchnip = float.Parse(textBoxSnitchnip.Text);

                //fan foul Detection
                partieConfig.FanFoulDetection.ElfTeleportation = float.Parse(textBoxElfTeleportation.Text);
                partieConfig.FanFoulDetection.GoblinShock = float.Parse(textBoxGoblinShock.Text);
                partieConfig.FanFoulDetection.TrollRoar = float.Parse(textBoxTrollRoar.Text);
                partieConfig.FanFoulDetection.SnitchSnatch = float.Parse(textBoxSnitchSnatch.Text);
            }
            catch (FormatException e)
            {
                MessageBox.Show("You can only enter numbers", "Partie-Configuration not valid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private bool ValidatePartieConfig() {
            bool returnValue = true;

            if (partieConfig.MaxRounds == 0) returnValue = false;
            if (partieConfig.Timeouts.PlayerTurnTimeout == 0) returnValue = false;
            if (partieConfig.Timeouts.FanTurnTimeout == 0) returnValue = false;
            if (partieConfig.Timeouts.PlayerPhaseTime == 0) returnValue = false;
            if (partieConfig.Timeouts.FanPhaseTime == 0) returnValue = false;
            if (partieConfig.Timeouts.BallPhaseTime == 0) returnValue = false;

            if (partieConfig.Propabilities.Goal == 0 || partieConfig.Propabilities.Goal>1) returnValue = false;
            if (partieConfig.Propabilities.ThrowSuccess == 0 || partieConfig.Propabilities.ThrowSuccess > 1) returnValue = false;
            if (partieConfig.Propabilities.KnockOut == 0 || partieConfig.Propabilities.KnockOut > 1) returnValue = false;
            if (partieConfig.Propabilities.FoolAway == 0 || partieConfig.Propabilities.FoolAway > 1) returnValue = false;
            if (partieConfig.Propabilities.CatchSnitch == 0 || partieConfig.Propabilities.CatchSnitch > 1) returnValue = false;
            if (partieConfig.Propabilities.CatchQuaffle == 0 || partieConfig.Propabilities.CatchQuaffle > 1) returnValue = false;
            if (partieConfig.Propabilities.WrestQuaffle == 0 || partieConfig.Propabilities.WrestQuaffle > 1) returnValue = false;

            if (partieConfig.Propabilities.ExtraMove.thinderblast == 0 || partieConfig.Propabilities.ExtraMove.thinderblast > 1) returnValue = false;
            if (partieConfig.Propabilities.ExtraMove.cleansweep11 == 0 || partieConfig.Propabilities.ExtraMove.cleansweep11 > 1) returnValue = false;
            if (partieConfig.Propabilities.ExtraMove.comet260 == 0 || partieConfig.Propabilities.ExtraMove.comet260 > 1) returnValue = false;
            if (partieConfig.Propabilities.ExtraMove.nimbus2001 == 0 || partieConfig.Propabilities.ExtraMove.nimbus2001 > 1) returnValue = false;
            if (partieConfig.Propabilities.ExtraMove.firebolt == 0 || partieConfig.Propabilities.ExtraMove.firebolt > 1) returnValue = false;

            if (partieConfig.FoulDetection.Flacking == 0 || partieConfig.FoulDetection.Flacking > 1) returnValue = false;
            if (partieConfig.FoulDetection.Haversacking == 0 || partieConfig.FoulDetection.Haversacking > 1 ) returnValue = false;
            if (partieConfig.FoulDetection.Stooging == 0 || partieConfig.FoulDetection.Stooging > 1) returnValue = false;
            if (partieConfig.FoulDetection.Blatching == 0 || partieConfig.FoulDetection.Blatching > 1) returnValue = false;
            if (partieConfig.FoulDetection.Snitchnip == 0 || partieConfig.FoulDetection.Snitchnip > 1) returnValue = false;

            if (partieConfig.FanFoulDetection.ElfTeleportation == 0 || partieConfig.FanFoulDetection.ElfTeleportation > 1) returnValue = false;
            if (partieConfig.FanFoulDetection.GoblinShock == 0 || partieConfig.FanFoulDetection.GoblinShock > 1) returnValue = false;
            if (partieConfig.FanFoulDetection.TrollRoar == 0 || partieConfig.FanFoulDetection.TrollRoar > 1) returnValue = false;
            if (partieConfig.FanFoulDetection.SnitchSnatch == 0 || partieConfig.FanFoulDetection.SnitchSnatch > 1) returnValue = false;

            if (!returnValue)
            {
                MessageBox.Show("Your input is invalid.", "Partie-Configuration not valid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return returnValue;
        }
        //insert Partie Data
        private void InsertPartieData(PartieConfig pc)
        {
            partieConfig = pc;
        }

        private void UpdatePartieData()
        {
            //max rounds and timeouts
            textBoxMaxRounds.Text = ""+partieConfig.MaxRounds;
            textBoxPlayerTurnTimeout.Text =""+partieConfig.Timeouts.PlayerTurnTimeout;
            textBoxFanTurnTimeout.Text = "" +partieConfig.Timeouts.FanTurnTimeout;
            textBoxPlayerPhaseTime.Text = "" +partieConfig.Timeouts.PlayerPhaseTime;
            textBoxFanPhaseTime.Text = "" +partieConfig.Timeouts.FanPhaseTime;
            textBoxBallPhaseTime.Text = "" +partieConfig.Timeouts.BallPhaseTime;

            //propabilities
            textBoxGoal.Text = "" + partieConfig.Propabilities.Goal;
            textBoxThrowSucces.Text = "" + partieConfig.Propabilities.ThrowSuccess;
            textBoxKnockOut.Text = "" + partieConfig.Propabilities.KnockOut;
            textBoxfoolAway.Text = "" + partieConfig.Propabilities.FoolAway;
            textBoxCatchSnitch.Text = "" + partieConfig.Propabilities.CatchSnitch;
            textBoxCatchQuaffle.Text = "" + partieConfig.Propabilities.CatchQuaffle;
            textBoxWrestQuaffle.Text = "" + partieConfig.Propabilities.WrestQuaffle;

            //extraMove
            textBoxThinderblast.Text = "" + partieConfig.Propabilities.ExtraMove.thinderblast;
            textBoxCleansweep11.Text = "" + partieConfig.Propabilities.ExtraMove.cleansweep11;
            textBoxComet260.Text = "" + partieConfig.Propabilities.ExtraMove.comet260;
            textBoxNimbus2001.Text = "" + partieConfig.Propabilities.ExtraMove.nimbus2001;
            textBoxFirebolt.Text = "" + partieConfig.Propabilities.ExtraMove.firebolt;

            //foul Detection
            textBoxFlacking.Text = "" + partieConfig.FoulDetection.Flacking;
            textBoxHaversacking.Text = "" + partieConfig.FoulDetection.Haversacking;
            textBoxStooging.Text = "" + partieConfig.FoulDetection.Stooging;
            textBoxBlatching.Text = "" + partieConfig.FoulDetection.Blatching;
            textBoxSnitchnip.Text = "" + partieConfig.FoulDetection.Snitchnip;

            //fan foul Detection
            textBoxElfTeleportation.Text = "" + partieConfig.FanFoulDetection.ElfTeleportation;
            textBoxGoblinShock.Text = "" + partieConfig.FanFoulDetection.GoblinShock;
            textBoxTrollRoar.Text = "" + partieConfig.FanFoulDetection.TrollRoar;
            textBoxSnitchSnatch.Text = "" + partieConfig.FanFoulDetection.SnitchSnatch;

        }
        //loadFile
        private void loadFile(string filePath)
        {
            bool loadPartie = false;

            string text = File.ReadAllText(filePath);
            if (text.Length < 3)
            {
                MessageBox.Show("Something went wrong while trying to open File. Pleas check File.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (text[2]=='m')
            {
                loadPartie = true;
            }

            if (loadPartie)
            {
                PartieConfig dataFromFile = new PartieConfig();
                try
                {
                    dataFromFile = JsonConvert.DeserializeObject<PartieConfig>(File.ReadAllText(filePath));
                }
                catch
                {
                    MessageBox.Show("Something went wrong while trying to open File. Pleas check File.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (dataFromFile == null)
                {
                    MessageBox.Show("Something went wrong while trying to open File. Pleas check File.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                this.openedFilePartie = filePath;
                InsertPartieData(dataFromFile);
                UpdatePartieData();
            }
            else
            {
                OutputData dataFromFile = JsonConvert.DeserializeObject<OutputData>(File.ReadAllText(filePath));
                if (dataFromFile == null)
                {
                    MessageBox.Show("Something went wrong while trying to open File. Pleas check File.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                this.openedFile = filePath;
                InsertData(dataFromFile);
                UpdateTrikos();
            }
        }

        private void SaveAs()
        {
            //save active player Data
            this.playerData.name[activePlayer] = this.textBoxName.Text ?? "";
            this.playerData.sex[activePlayer] = CheckRadioButtonGender();
            this.playerData.broom[activePlayer] = this.comboBox1.Text;

            if (panelPartie.Visible)
            {
                if (!InsertPartieConfigToClass())
                {
                    return;
                }
                if (!ValidatePartieConfig())
                {
                    return;
                }

                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "Json|*.json";
                saveFileDialog1.Title = "Save an Json File";
                saveFileDialog1.ShowDialog();

                if (saveFileDialog1.FileName != "")
                {
                    FileStream fs = (System.IO.FileStream)saveFileDialog1.OpenFile();
                    JsonSerializer serializer = new JsonSerializer();
                    using (StreamWriter sw = new StreamWriter(fs))
                    using (JsonWriter writer = new JsonTextWriter(sw))
                    {
                        serializer.Serialize(writer, partieConfig);
                    }
                    fs.Close();
                }

            }
            else
            {
                if (!ValidateConfig())
                {
                    return;
                }

                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "Json|*.json";
                saveFileDialog1.Title = "Save an Json File";
                saveFileDialog1.ShowDialog();

                // If the file name is not an empty string open it for saving.  
                if (saveFileDialog1.FileName != "")
                {
                    FileStream fs = (System.IO.FileStream)saveFileDialog1.OpenFile();
                    JsonSerializer serializer = new JsonSerializer();
                    using (StreamWriter sw = new StreamWriter(fs))
                    using (JsonWriter writer = new JsonTextWriter(sw))
                    {
                        OutputData od = GenerateOutputData();
                        serializer.Serialize(writer, od);
                    }
                    fs.Close();
                }
            }
        }
        //Save unterschied ob Partie oder Team Config speichern
        private void Save()
        {
            this.playerData.name[activePlayer] = this.textBoxName.Text ?? "";
            this.playerData.sex[activePlayer] = CheckRadioButtonGender();
            this.playerData.broom[activePlayer] = this.comboBox1.Text;


            if (panelPartie.Visible)
            {
                if (!InsertPartieConfigToClass())
                {
                    return;
                }
                if (!ValidatePartieConfig())
                {
                    return;
                }

                if (openedFilePartie != "")
                {
                    FileStream fs = new FileStream(openedFilePartie, FileMode.Create);
                    JsonSerializer serializer = new JsonSerializer();
                    using (StreamWriter sw = new StreamWriter(fs))
                    using (JsonWriter writer = new JsonTextWriter(sw))
                    {
                        serializer.Serialize(writer, partieConfig);
                    }
                    fs.Close();
                    return;
                }
            }
            else
            {
                if (!ValidateConfig())
                {
                    return;
                }

                if (openedFile != "")
                {
                    FileStream fs = new FileStream(openedFile, FileMode.Create);
                    JsonSerializer serializer = new JsonSerializer();
                    using (StreamWriter sw = new StreamWriter(fs))
                    using (JsonWriter writer = new JsonTextWriter(sw))
                    {
                        OutputData od = GenerateOutputData();
                        serializer.Serialize(writer, od);
                    }
                    fs.Close();
                    return;
                }
            }
            SaveAs();
        }
    }
}
