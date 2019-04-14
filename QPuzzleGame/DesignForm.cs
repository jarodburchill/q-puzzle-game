/*
 * Jarod Burchill
 * PROG2370 Assignment 1
 * Written September 24th - September 28th
 */
using QPuzzleGame.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QPuzzleGame
{
    public partial class DesignForm : Form
    {
        //tile constants
        private const int START_TOP = 100;
        private const int START_LEFT = 150;
        private const int WIDTH = 50;
        private const int HEIGHT = 50;
        private const int GAP = 2;
        //global vars
        CurrentTool currentTool = new CurrentTool();
        int rows;
        int columns;
        /// <summary>
        /// tile color options
        /// </summary>
        public enum TileColors
        {
            blank,
            black,
            red,
            green,
            blue,
            yellow
        }
        /// <summary>
        /// tile type options
        /// </summary>
        public enum TileTypes
        {
            blank,
            wall,
            door,
            box
        }
        /// <summary>
        /// disable editing
        /// </summary>
        private void DisableToolbox()
        {
            btnNone.Enabled = false;
            btnWall.Enabled = false;
            btnBlueBox.Enabled = false;
            btnBlueDoor.Enabled = false;
            btnGreenBox.Enabled = false;
            btnGreenDoor.Enabled = false;
            btnRedBox.Enabled = false;
            btnRedDoor.Enabled = false;
            btnYellowBox.Enabled = false;
            btnYellowDoor.Enabled = false;

            btnGenerate.Enabled = true;
            txtColumns.Enabled = true;
            txtRows.Enabled = true;

            saveLevelToolStripMenuItem.Enabled = false;
            restartToolStripMenuItem.Enabled = false;
            loadLevelToolStripMenuItem.Enabled = true;

            txtColumns.Text = null;
            txtRows.Text = null;
        }
        /// <summary>
        /// enable editing
        /// </summary>
        private void EnableToolbox()
        {
            btnNone.Enabled = true;
            btnWall.Enabled = true;
            btnBlueBox.Enabled = true;
            btnBlueDoor.Enabled = true;
            btnGreenBox.Enabled = true;
            btnGreenDoor.Enabled = true;
            btnRedBox.Enabled = true;
            btnRedDoor.Enabled = true;
            btnYellowBox.Enabled = true;
            btnYellowDoor.Enabled = true;

            btnGenerate.Enabled = false;
            txtColumns.Enabled = false;
            txtRows.Enabled = false;

            saveLevelToolStripMenuItem.Enabled = true;
            restartToolStripMenuItem.Enabled = true;
            loadLevelToolStripMenuItem.Enabled = false;

            btnNone.PerformClick();
            btnNone.Select();
        }
        /// <summary>
        /// required
        /// </summary>
        public DesignForm()
        {
            InitializeComponent();
        }
        /// <summary>
        /// form load calls for editing to be disabled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DesignForm_Load(object sender, EventArgs e)
        {
            DisableToolbox();
        }
        /// <summary>
        /// checks rows and columns for valid input
        /// generates a grid of tiles with a delay animation if input is valid
        /// sends an error message if input is invalid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGenerate_Click(object sender, EventArgs e)
        {
            string errors = "";

            if (!int.TryParse(txtRows.Text, out rows))
            {
                errors += "Invalid Input: Numbers only when selecting rows.\n";
            }
            if (!int.TryParse(txtColumns.Text, out columns))
            {
                errors += "Invalid Input: Numbers only when selecting columns.\n";
            }

            if (errors == "")
            {
                int x = START_LEFT;
                int y = START_TOP;

                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        Tile tile = new Tile();
                        tile.Left = x;
                        tile.Top = y;

                        tile.Height = HEIGHT;
                        tile.Width = WIDTH;

                        tile.BackColor = Color.LightGray;

                        tile.tileColor = TileColors.blank.ToString();
                        tile.tileType = TileTypes.blank.ToString();

                        tile.Click += new EventHandler(Tile_Click);

                        this.Controls.Add(tile);

                        x += WIDTH + GAP;

                        Application.DoEvents();
                        Thread.Sleep(25);
                    }
                    y += HEIGHT + GAP;
                    x = START_LEFT;
                }
                EnableToolbox();
            }
            else
            {
                MessageBox.Show(errors, "Error");
            }
        }
        /// <summary>
        /// selects the current tool objects props through a switch statement of buttons
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolboxButtons_Click(object sender, EventArgs e)
        {
            switch (((Button)sender).Name)
            {
                case "btnNone":
                    currentTool.toolColor = TileColors.blank.ToString();
                    currentTool.toolType = TileTypes.blank.ToString();
                    break;
                case "btnWall":
                    currentTool.toolColor = TileColors.black.ToString();
                    currentTool.toolType = TileTypes.wall.ToString();
                    break;
                case "btnRedBox":
                    currentTool.toolColor = TileColors.red.ToString();
                    currentTool.toolType = TileTypes.box.ToString();
                    break;
                case "btnRedDoor":
                    currentTool.toolColor = TileColors.red.ToString();
                    currentTool.toolType = TileTypes.door.ToString();
                    break;
                case "btnGreenBox":
                    currentTool.toolColor = TileColors.green.ToString();
                    currentTool.toolType = TileTypes.box.ToString();
                    break;
                case "btnGreenDoor":
                    currentTool.toolColor = TileColors.green.ToString();
                    currentTool.toolType = TileTypes.door.ToString();
                    break;
                case "btnBlueBox":
                    currentTool.toolColor = TileColors.blue.ToString();
                    currentTool.toolType = TileTypes.box.ToString();
                    break;
                case "btnBlueDoor":
                    currentTool.toolColor = TileColors.blue.ToString();
                    currentTool.toolType = TileTypes.door.ToString();
                    break;
                case "btnYellowBox":
                    currentTool.toolColor = TileColors.yellow.ToString();
                    currentTool.toolType = TileTypes.box.ToString();
                    break;
                case "btnYellowDoor":
                    currentTool.toolColor = TileColors.yellow.ToString();
                    currentTool.toolType = TileTypes.door.ToString();
                    break;
                default:
                    MessageBox.Show(((Button)sender).Name);
                    break;
            }
        }
        /// <summary>
        /// assigns a picture to a tile based off of the tool selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tile_Click(object sender, EventArgs e)
        {
            Tile tile = new Tile();
            tile = (Tile)sender;
            tile.tileColor = currentTool.toolColor;
            tile.tileType = currentTool.toolType;

            string imageName = ($"{tile.tileColor} {tile.tileType}");

            switch (imageName)
            {
                case "blank blank":
                    tile.BackColor = Color.LightGray;
                    tile.Image = null;
                    break;
                case "black wall":
                    tile.Image = Resources.black_wall;
                    break;
                case "red door":
                    tile.Image = Resources.red_door;
                    break;
                case "red box":
                    tile.Image = Resources.red_box;
                    break;
                case "green door":
                    tile.Image = Resources.green_door;
                    break;
                case "green box":
                    tile.Image = Resources.green_box;
                    break;
                case "blue door":
                    tile.Image = Resources.blue_door;
                    break;
                case "blue box":
                    tile.Image = Resources.blue_box;
                    break;
                case "yellow door":
                    tile.Image = Resources.yellow_door;
                    break;
                case "yellow box":
                    tile.Image = Resources.yellow_box;
                    break;
                default:
                    tile.BackColor = Color.LightGray;
                    tile.Image = null;
                    break;
            }
        }
        /// <summary>
        /// loads a level
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openfile = new OpenFileDialog();
            openfile.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (openfile.ShowDialog() == DialogResult.OK)
            {
                var levelList = new List<string>();
                string record;
                using (StreamReader reader = new StreamReader(openfile.FileName))
                {
                    while ((record = reader.ReadLine()) != null)
                    {
                        levelList.Add(record);
                    }
                }
                string[] loadArray = levelList.ToArray();

                int x = START_LEFT;
                int y = START_TOP;

                rows = int.Parse(Regex.Match(loadArray[0], @"\d+").Value);
                columns = int.Parse(Regex.Match(loadArray[1], @"\d+").Value);
                int loadCounter = 1;

                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        loadCounter++;

                        Tile tile = new Tile();
                        tile.Left = x;
                        tile.Top = y;

                        tile.Height = HEIGHT;
                        tile.Width = WIDTH;

                        tile.Click += new EventHandler(Tile_Click);

                        switch (loadArray[loadCounter])
                        {
                            case "blank::blank":
                                tile.BackColor = Color.LightGray;
                                tile.tileColor = TileColors.blank.ToString();
                                tile.tileType = TileTypes.blank.ToString();
                                break;
                            case "black::wall":
                                tile.Image = Resources.black_wall;
                                tile.tileColor = TileColors.black.ToString();
                                tile.tileType = TileTypes.wall.ToString();
                                break;
                            case "red::box":
                                tile.Image = Resources.red_box;
                                tile.tileColor = TileColors.red.ToString();
                                tile.tileType = TileTypes.box.ToString();
                                break;
                            case "red::door":
                                tile.Image = Resources.red_door;
                                tile.tileColor = TileColors.red.ToString();
                                tile.tileType = TileTypes.door.ToString();
                                break;
                            case "green::box":
                                tile.Image = Resources.green_box;
                                tile.tileColor = TileColors.green.ToString();
                                tile.tileType = TileTypes.box.ToString();
                                break;
                            case "green::door":
                                tile.Image = Resources.green_door;
                                tile.tileColor = TileColors.green.ToString();
                                tile.tileType = TileTypes.door.ToString();
                                break;
                            case "blue::box":
                                tile.Image = Resources.blue_box;
                                tile.tileColor = TileColors.blue.ToString();
                                tile.tileType = TileTypes.box.ToString();
                                break;
                            case "blue::door":
                                tile.Image = Resources.blue_door;
                                tile.tileColor = TileColors.blue.ToString();
                                tile.tileType = TileTypes.door.ToString();
                                break;
                            case "yellow::box":
                                tile.Image = Resources.yellow_box;
                                tile.tileColor = TileColors.yellow.ToString();
                                tile.tileType = TileTypes.box.ToString();
                                break;
                            case "yellow::door":
                                tile.Image = Resources.yellow_door;
                                tile.tileColor = TileColors.yellow.ToString();
                                tile.tileType = TileTypes.door.ToString();
                                break;
                            default:
                                tile.BackColor = Color.LightGray;
                                tile.tileColor = TileColors.blank.ToString();
                                tile.tileType = TileTypes.blank.ToString();
                                break;
                        }

                        this.Controls.Add(tile);

                        x += WIDTH + GAP;

                        Application.DoEvents();
                        Thread.Sleep(25);
                    }
                    y += HEIGHT + GAP;
                    x = START_LEFT;
                }
                EnableToolbox();
            }
        }
        /// <summary>
        /// saves a level as a .txt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog savefile = new SaveFileDialog();
            savefile.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (savefile.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter writer = new StreamWriter(savefile.FileName, false))
                {
                    writer.WriteLine($"rows::{rows}");
                    writer.WriteLine($"columns::{columns}");

                }
                foreach (Tile tile in this.Controls.OfType<Tile>().ToArray())
                {
                    using (StreamWriter writer = new StreamWriter(savefile.FileName, true))
                    {
                        writer.WriteLine($"{tile.tileColor}::{tile.tileType}");
                    }
                }
                MessageBox.Show($"Level saved as:'{savefile.FileName}'","Saved");
            }
        }
        /// <summary>
        /// resets the form to its original state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void restartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Any unsaved progress will be lost if you continue",
                "Restart?", MessageBoxButtons.OKCancel);
            if (dialogResult == DialogResult.OK)
            {
                foreach (Tile tile in this.Controls.OfType<Tile>().ToArray())
                {
                    tile.Dispose();
                    this.Controls.Remove(tile);
                }
                DisableToolbox();
            }
        }
        /// <summary>
        /// hides design form and opens main menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mainMenuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MainMenu mainMenu = new MainMenu();
            mainMenu.Show();
            this.Hide();
        }
        /// <summary>
        /// exits application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmDesign_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}