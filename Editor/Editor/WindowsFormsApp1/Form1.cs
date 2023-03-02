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
        //errors
        List<string> problems = new List<string>();

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
            if (e.Button == System.Windows.Forms.MouseButtons.Left && WindowState == FormWindowState.Normal)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        /*Funktion zum bewegen des Fensters*/
        private void OnControlMouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left && WindowState == FormWindowState.Normal)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
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
            openedFilePartie = "";
            panelTeam.Visible = false;
            panelPartie.Visible = false;
            trikotColor = new Color[2];
            trikotColor[0] = Color.Black;
            trikotColor[1] = Color.Black;
            partieConfig = new PartieConfig();
            this.Icon = global::WindowsFormsApp1.Properties.Resources.stift;


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
            Fans fans = new Fans((int)this.numericUpDownGoblin.Value,(int)this.numericUpDownTroll.Value,(int)this.numericUpDownElf.Value,(int)this.numericUpDownNiffler.Value,(int)this.numericUpDownWombat.Value);

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
        /* Auswahl um Player zu bearbeiten */
        private void ButtonPlayer(int x){

            Button[] buttons = new Button[7];
            buttons[0] = this.buttonPlayer1;
            buttons[1] = this.buttonPlayer2;
            buttons[2] = this.buttonPlayer3;
            buttons[3] = this.buttonPlayer4;
            buttons[4] = this.buttonPlayer5;
            buttons[5] = this.buttonPlayer6;
            buttons[6] = this.buttonPlayer7;


            if (x == activePlayer)
            {
                return;
            }
            this.labelRolle.Text = "Seeker:";
            this.playerData.name[activePlayer] = this.textBoxName.Text ?? "";
            this.playerData.sex[activePlayer] = CheckRadioButtonGender();
            this.playerData.broom[activePlayer] = this.comboBox1.Text;
            this.textBoxName.Text = this.playerData.name[x];
            if (this.textBoxName.Text == "" || this.textBoxName.Text == "Insert Name")
            {
                this.textBoxName.Text = "Insert Name";
                this.textBoxName.ForeColor = Color.Gray;
            }
            else
            {
                this.textBoxName.ForeColor = Color.Black;
            }
            SetRadioButtonGender(this.playerData.sex[x]);
            this.comboBox1.Text = this.playerData.broom[x];

            if (playerData.sex[x] == "f")
            {
                buttons[x].BackgroundImage = global::WindowsFormsApp1.Properties.Resources.redKontaktFemale;
            }
            else
            {
                buttons[x].BackgroundImage = global::WindowsFormsApp1.Properties.Resources.redKontakt;
            }
            ChangeOldButtonImage(activePlayer);
            activePlayer = x;

        }
        /*Asuwahl um Player 1 zu bearbeiten*/
        private void buttonPlayer1_Click(object sender, EventArgs e)
        {
            ButtonPlayer(0);
        }
        /*Asuwahl um Player 2 zu bearbeiten*/
        private void ButtonPlayer2_Click(object sender, EventArgs e)
        {
            ButtonPlayer(1);
        }
        /*Asuwahl um Player 3 zu bearbeiten*/
        private void ButtonPlayer3_Click(object sender, EventArgs e)
        {
            ButtonPlayer(2);
        }
        /*Asuwahl um Player 4 zu bearbeiten*/
        private void ButtonPlayer4_Click(object sender, EventArgs e)
        {
            ButtonPlayer(3);
        }
        /*Asuwahl um Player 5 zu bearbeiten*/
        private void ButtonPlayer5_Click(object sender, EventArgs e)
        {
            ButtonPlayer(4);
        }
        /*Asuwahl um Player 6 zu bearbeiten*/
        private void ButtonPlayer6_Click(object sender, EventArgs e)
        {
            ButtonPlayer(5);
        }
        /*Asuwahl um Player 7 zu bearbeiten*/
        private void ButtonPlayer7_Click(object sender, EventArgs e)
        {
            ButtonPlayer(6);
        }
        /*speichern der Daten und anpassen der GUI*/
        private void ChangeOldButtonImage(int oldButton)
        {
            Image im;
            if (playerData.sex[oldButton] == "f")
            {
                im = global::WindowsFormsApp1.Properties.Resources.kontaktFemale;
            }
            else
            {
                im = global::WindowsFormsApp1.Properties.Resources.kontakt;
            }
            switch (oldButton)
            {
                case 0:
                    this.buttonPlayer1.BackgroundImage = im;
                    break;
                case 1:
                    this.buttonPlayer2.BackgroundImage = im;
                    break;
                case 2:
                    this.buttonPlayer3.BackgroundImage = im;
                    break;
                case 3:
                    this.buttonPlayer4.BackgroundImage = im;
                    break;
                case 4:
                    this.buttonPlayer5.BackgroundImage = im;
                    break;
                case 5:
                    this.buttonPlayer6.BackgroundImage = im;
                    break;
                case 6:
                    this.buttonPlayer7.BackgroundImage = im;
                    break;
            }
            
        }
        /*Dialog zum öffnen eines Bildes*/
        private void ButtonOpenImage_Click(object sender, EventArgs e)
        {
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
            this.numericUpDownElf.Value = data.Fans.elves;
            this.numericUpDownNiffler.Value = data.Fans.nifflers;
            this.numericUpDownWombat.Value = data.Fans.wombats;


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
        private void Button11_Click(object sender, EventArgs e)
        {

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
        private void Panel1_DragEnter(object sender, DragEventArgs e)
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
        private void Panel1_DragDrop(object sender, DragEventArgs e)
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
        //DragDrop Methode
        private void DragDropFile(DragEventArgs e)
        {
            string[] file = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            bool img = file[0].EndsWith("json");

            if (img)
            {
                e.Effect = DragDropEffects.Move;
            }
        }
        //DragEnterMethode
        private void DragEnterFile(DragEventArgs e) {
            string[] file = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            string filePath = file[0];
            loadFile(filePath);
        }
        /*Implementierung der Drag and Drop Funktion*/
        private void PanelTeam_DragEnter(object sender, DragEventArgs e)
        {
            DragDropFile(e);
        }
        /*Implementierung der Drag and Drop Funktion*/
        private void PanelTeam_DragDrop(object sender, DragEventArgs e)
        {
            DragEnterFile(e);
            
        }
        /*Implementierung der Drag and Drop Funktion*/
        private void PanelGeneral_DragEnter(object sender, DragEventArgs e)
        {
            DragDropFile(e);
        }
        /*Implementierung der Drag and Drop Funktion*/
        private void PanelGeneral_DragDrop(object sender, DragEventArgs e)
        {
            DragEnterFile(e);
        }
        /*ändern des Geschlechts und aktualisierung aller Geschlechter (Female)*/
        private void updateSex() {
            this.playerData.sex[activePlayer] = CheckRadioButtonGender();
            Image im;
            if (playerData.sex[activePlayer] == "f")
            {
                im = global::WindowsFormsApp1.Properties.Resources.redKontaktFemale;
            }
            else
            {
                im = global::WindowsFormsApp1.Properties.Resources.redKontakt;
            }
            switch (activePlayer)
            {
                case 0:
                    this.buttonPlayer1.BackgroundImage = im;
                    break;
                case 1:
                    this.buttonPlayer2.BackgroundImage = im;
                    break;
                case 2:
                    this.buttonPlayer3.BackgroundImage = im;
                    break;
                case 3:
                    this.buttonPlayer4.BackgroundImage = im;
                    break;
                case 4:
                    this.buttonPlayer5.BackgroundImage = im;
                    break;
                case 5:
                    this.buttonPlayer6.BackgroundImage = im;
                    break;
                case 6:
                    this.buttonPlayer7.BackgroundImage = im;
                    break;
            }

        }
        /*ändern des Geschlechts und aktualisierung aller Geschlechter (Female)*/
        private void RadioButtonMale_MouseClick(object sender, MouseEventArgs e)
        {
            updateSex();
        }
        /*ändern des Geschlechts und aktualisierung aller Geschlechter (Female)*/
        private void RadioButtonFemale_MouseClick(object sender, MouseEventArgs e)
        {
            updateSex();
        }
        /*Hilfe anzeigen beim hovern*/
        private void TextBoxName_Enter(object sender, EventArgs e)
        {
            this.textBoxName.Text = "";
            this.textBoxName.ForeColor = Color.Black;
        }
        /*Hilfe anzeigen beim hovern*/
        private void TextBoxName_Leave(object sender, EventArgs e)
        {
            if (this.textBoxName.Text == "")
            {
                this.textBoxName.Text = "Insert Name";
                this.textBoxName.ForeColor = Color.Gray;
            }
        }
        /*Hilfe anzeigen beim hovern*/
        private void TextBoxTeamname_Enter(object sender, EventArgs e)
        {
            if (this.textBoxTeamname.Text == "Insert Team-Name")
            {
                this.textBoxTeamname.Text = "";
                this.textBoxTeamname.ForeColor = Color.Black;
            }
        }
        /*Hilfe anzeigen beim hovern*/
        private void TextBoxTeamname_Leave(object sender, EventArgs e)
        {
            if (this.textBoxTeamname.Text == "")
            {
                this.textBoxTeamname.Text = "Insert Team-Name";
                this.textBoxTeamname.ForeColor = Color.Gray;
            }
        }
        /*Hilfe anzeigen beim hovern*/
        private void TextBoxMotto_Enter(object sender, EventArgs e)
        {
            if (this.textBoxMotto.Text == "Insert Motto")
            {
                this.textBoxMotto.Text = "";
                this.textBoxMotto.ForeColor = Color.Black;
            }
        }
        /*Hilfe anzeigen beim hovern*/
        private void TextBoxMotto_Leave(object sender, EventArgs e)
        {
            if (this.textBoxMotto.Text == "")
            {
                this.textBoxMotto.Text = "Insert Motto";
                this.textBoxMotto.ForeColor = Color.Gray;
            }
        }
        /*Event zum speichern der Json Datei*/
        private void Button3_Click(object sender, EventArgs e)
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
            c += (int)this.numericUpDownWombat.Value;
            if (c>7)
            {
                return false;
            }
            return true;
        }
        /*Event zum verändern der anzahl der Fans*/
        private void NumericUpDownElf_ValueChanged(object sender, EventArgs e)
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
        private void NumericUpDownNiffler_ValueChanged(object sender, EventArgs e)
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
        private void NumericUpDownGoblin_ValueChanged(object sender, EventArgs e)
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
        //test if valid(Teamname, Motto, Color, Fans, Image)
        private bool ValidateTextboxes() {
            bool valid = true;
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
            int fans = (int)this.numericUpDownElf.Value + (int)this.numericUpDownTroll.Value + (int)this.numericUpDownGoblin.Value + (int)this.numericUpDownNiffler.Value + (int)this.numericUpDownWombat.Value;
            if (fans != 7 || !CheckFansNumbers())
            {
                problems.Add("Wrong number of Fans");
                valid = false;
            }
            if (this.pictureBoxTeamImage.Image == null)
            {
                problems.Add("Missing Image");
                valid = false;
            }

            return valid;
        }
        //error Message
        private void SendErrorMessage(string message) {
            foreach (string s in problems)
            {
                message += "     " + s + "\n";
            }
            MessageBox.Show(message, "Teamconfiguration Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        /*Validierung der Teamconfigurtion und Fehlermeldung falls notwendig*/
        private bool ValidateConfig()
        {
            //save current active Player
            this.playerData.name[activePlayer] = this.textBoxName.Text ?? "";
            this.playerData.sex[activePlayer] = CheckRadioButtonGender();
            this.playerData.broom[activePlayer] = this.comboBox1.Text;


            problems.Clear();
            bool valid = ValidateTextboxes();
            
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
                    case "tinderblast":
                        brooms[0] = true;
                        break;
                    case "cleansweep11":
                        brooms[1] = true;
                        break;
                    case "comet260":
                        brooms[2] = true;
                        break;
                    case "nimbus2001":
                        brooms[3] = true;
                        break;
                    case "firebolt":
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
                SendErrorMessage(message);
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
            Color c1;
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
            Color c2;
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
                partieConfig.Timeouts.TeamFormationTimeout = int.Parse(textBoxTeamFormationTimeout.Text);
                partieConfig.Timeouts.PlayerTurnTimeout = int.Parse(textBoxPlayerTurnTimeout.Text);
                partieConfig.Timeouts.FanTurnTimeout = int.Parse(textBoxFanTurnTimeout.Text);
                partieConfig.Timeouts.UnbanTurnTimeout = int.Parse(textBoxUnbanTurnTimeout.Text);
                partieConfig.Timeouts.MinPlayerPhaseAnimationDuration = int.Parse(textBoxMinPlayer.Text);
                partieConfig.Timeouts.MinFanPhaseAnimationDuration = int.Parse(textBoxMinPhase.Text);
                partieConfig.Timeouts.MinBallPhaseAnimationDuration = int.Parse(textBoxBall.Text);
                partieConfig.Timeouts.MinUnbanPhaseAnimationDuration = int.Parse(textBoxUnban.Text);

                //propabilities
                partieConfig.Propabilities.Goal = float.Parse(textBoxGoal.Text);
                partieConfig.Propabilities.ThrowSuccess = float.Parse(textBoxThrowSucces.Text);
                partieConfig.Propabilities.KnockOut = float.Parse(textBoxKnockOut.Text);
                //Changed by commity
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
                partieConfig.FanFoulDetection.WombatPoo = float.Parse(textBoxWombatPoo.Text);

            }
            catch (FormatException)
            {
                MessageBox.Show("You can only enter numbers", "Partie-Configuration not valid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }
        //Validate Timeouts
        private bool ValidateTimeouts() {
            bool returnValue = true;
            if (partieConfig.MaxRounds <= 0) returnValue = false;
            if (partieConfig.Timeouts.TeamFormationTimeout <= 0) returnValue = false;
            if (partieConfig.Timeouts.PlayerTurnTimeout <= 0) returnValue = false;
            if (partieConfig.Timeouts.FanTurnTimeout <= 0) returnValue = false;
            if (partieConfig.Timeouts.UnbanTurnTimeout <= 0) returnValue = false;
            if (partieConfig.Timeouts.MinPlayerPhaseAnimationDuration <= 0) returnValue = false;
            if (partieConfig.Timeouts.MinFanPhaseAnimationDuration <= 0) returnValue = false;
            if (partieConfig.Timeouts.MinBallPhaseAnimationDuration <= 0) returnValue = false;
            if (partieConfig.Timeouts.MinUnbanPhaseAnimationDuration <= 0) returnValue = false;
            return returnValue;
        }
        //Validate Propabilities
        private bool ValidatePropabilities()
        {
            bool returnValue = true;
            if (partieConfig.Propabilities.Goal <= 0 || partieConfig.Propabilities.Goal > 1) returnValue = false;
            if (partieConfig.Propabilities.ThrowSuccess <= 0 || partieConfig.Propabilities.ThrowSuccess > 1) returnValue = false;
            if (partieConfig.Propabilities.KnockOut <= 0 || partieConfig.Propabilities.KnockOut > 1) returnValue = false;
            if (partieConfig.Propabilities.CatchSnitch <= 0 || partieConfig.Propabilities.CatchSnitch > 1) returnValue = false;
            if (partieConfig.Propabilities.CatchQuaffle <= 0 || partieConfig.Propabilities.CatchQuaffle > 1) returnValue = false;
            if (partieConfig.Propabilities.WrestQuaffle <= 0 || partieConfig.Propabilities.WrestQuaffle > 1) returnValue = false;

            return returnValue;

        }
        //Validate Propabilities ExtraMove
        private bool ValidateExtraMove()
        {
            bool returnValue = true;
            if (partieConfig.Propabilities.ExtraMove.thinderblast <= 0 || partieConfig.Propabilities.ExtraMove.thinderblast > 1) returnValue = false;
            if (partieConfig.Propabilities.ExtraMove.cleansweep11 <= 0 || partieConfig.Propabilities.ExtraMove.cleansweep11 > 1) returnValue = false;
            if (partieConfig.Propabilities.ExtraMove.comet260 <= 0 || partieConfig.Propabilities.ExtraMove.comet260 > 1) returnValue = false;
            if (partieConfig.Propabilities.ExtraMove.nimbus2001 <= 0 || partieConfig.Propabilities.ExtraMove.nimbus2001 > 1) returnValue = false;
            if (partieConfig.Propabilities.ExtraMove.firebolt <= 0 || partieConfig.Propabilities.ExtraMove.firebolt > 1) returnValue = false;
            return returnValue;
        }
        //Validate FoulDetection
        private bool ValidateFoulDetection() {
            bool returnValue = true;
            if (partieConfig.FoulDetection.Flacking <= 0 || partieConfig.FoulDetection.Flacking > 1) returnValue = false;
            if (partieConfig.FoulDetection.Haversacking <= 0 || partieConfig.FoulDetection.Haversacking > 1) returnValue = false;
            if (partieConfig.FoulDetection.Stooging <= 0 || partieConfig.FoulDetection.Stooging > 1) returnValue = false;
            if (partieConfig.FoulDetection.Blatching <= 0 || partieConfig.FoulDetection.Blatching > 1) returnValue = false;
            if (partieConfig.FoulDetection.Snitchnip <= 0 || partieConfig.FoulDetection.Snitchnip > 1) returnValue = false;
            return returnValue;
        }
        //Validate FanFoulDetection
        private bool ValidateFanFoulDetection() {
            bool returnValue = true;
            if (partieConfig.FanFoulDetection.ElfTeleportation <= 0 || partieConfig.FanFoulDetection.ElfTeleportation > 1) returnValue = false;
            if (partieConfig.FanFoulDetection.GoblinShock <= 0 || partieConfig.FanFoulDetection.GoblinShock > 1) returnValue = false;
            if (partieConfig.FanFoulDetection.TrollRoar <= 0 || partieConfig.FanFoulDetection.TrollRoar > 1) returnValue = false;
            if (partieConfig.FanFoulDetection.SnitchSnatch <= 0 || partieConfig.FanFoulDetection.SnitchSnatch > 1) returnValue = false;
            if (partieConfig.FanFoulDetection.WombatPoo <= 0 || partieConfig.FanFoulDetection.WombatPoo > 1) returnValue = false;
            return returnValue;
        }
        //Validate Partie Config
        private bool ValidatePartieConfig() {

            bool returnValue =  ValidateTimeouts();

            bool propabilities = ValidatePropabilities();

            bool extraMove = ValidateExtraMove();

            bool foulDetection = ValidateFoulDetection();

            bool fanFoulDetection = ValidateFanFoulDetection();


            if (!returnValue || !propabilities || !foulDetection || !fanFoulDetection || !extraMove)
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
            textBoxTeamFormationTimeout.Text = "" + partieConfig.Timeouts.TeamFormationTimeout;
            textBoxPlayerTurnTimeout.Text = "" + partieConfig.Timeouts.PlayerTurnTimeout;
            textBoxFanTurnTimeout.Text = "" + partieConfig.Timeouts.FanTurnTimeout;
            textBoxUnbanTurnTimeout.Text = "" + partieConfig.Timeouts.UnbanTurnTimeout;
            textBoxMinPlayer.Text = "" + partieConfig.Timeouts.MinPlayerPhaseAnimationDuration;
            textBoxMinPhase.Text = "" + partieConfig.Timeouts.MinFanPhaseAnimationDuration;
            textBoxBall.Text = "" + partieConfig.Timeouts.MinBallPhaseAnimationDuration;
            textBoxUnban.Text = "" + partieConfig.Timeouts.MinUnbanPhaseAnimationDuration;

            //propabilities
            textBoxGoal.Text = "" + partieConfig.Propabilities.Goal;
            textBoxThrowSucces.Text = "" + partieConfig.Propabilities.ThrowSuccess;
            textBoxKnockOut.Text = "" + partieConfig.Propabilities.KnockOut;
            //Changed by commity
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
            textBoxWombatPoo.Text = "" + partieConfig.FanFoulDetection.WombatPoo;

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
                PartieConfig dataFromFile;
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

        private void numericUpDownWombat_ValueChanged(object sender, EventArgs e)
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

    }
}
