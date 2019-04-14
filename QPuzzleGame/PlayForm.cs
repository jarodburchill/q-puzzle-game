/*
 * Jarod Burchill
 * PROG2370 Assignment 2
 * Written October 2018 - November 2018
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
    public partial class PlayForm : Form
    {
        //tile constants
        private const int START_TOP = 30;
        private const int START_LEFT = 175;
        private const int WIDTH = 50;
        private const int HEIGHT = 50;
        private const int GAP = 2;
        //global vars
        Tile currentTile = new Tile();
        int rows;
        int columns;
        int moves = 0;
        bool correctDoor = false;
        bool isBox = false;
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
        /// required
        /// </summary>
        public PlayForm()
        {
            InitializeComponent();
            DoubleBuffered = true;
        }
        /// <summary>
        /// Enables controls
        /// </summary>
        public void EnableControls()
        {
            btnUp.Enabled = true;
            btnDown.Enabled = true;
            btnLeft.Enabled = true;
            btnRight.Enabled = true;
            loadLevelToolStripMenuItem.Enabled = false;
            restartToolStripMenuItem.Enabled = true;
        }
        /// <summary>
        /// disables controls
        /// </summary>
        public void DisableControls()
        {
            btnUp.Enabled = false;
            btnDown.Enabled = false;
            btnLeft.Enabled = false;
            btnRight.Enabled = false;
            loadLevelToolStripMenuItem.Enabled = true;
            restartToolStripMenuItem.Enabled = false;
        }
        /// <summary>
        /// checks for interseting tiles
        /// </summary>
        /// <returns></returns>
        public bool Intersection(string direction)
        {
            foreach (Tile tile in this.Controls.OfType<Tile>().ToArray())
            {
                switch (direction)
                {
                    case "up":
                        if (currentTile.Left == tile.Left && currentTile.Top == (tile.Bottom + GAP) && !tile.selected)
                        {
                            if (tile.tileType == TileTypes.door.ToString() && currentTile.tileColor == tile.tileColor)
                            {
                                correctDoor = true;
                            }
                            return true;
                        }
                        break;
                    case "down":
                        if (currentTile.Left == tile.Left && currentTile.Bottom == (tile.Top - GAP) && !tile.selected)
                        {
                            if (tile.tileType == TileTypes.door.ToString() && currentTile.tileColor == tile.tileColor)
                            {
                                correctDoor = true;
                            }
                            return true;
                        }
                        break;
                    case "left":
                        if (currentTile.Left == (tile.Right + GAP) && currentTile.Top == tile.Top && !tile.selected)
                        {
                            if (tile.tileType == TileTypes.door.ToString() && currentTile.tileColor == tile.tileColor)
                            {
                                correctDoor = true;
                            }
                            return true;
                        }
                        break;
                    case "right":
                        if (currentTile.Right == (tile.Left - GAP) && currentTile.Top == tile.Top && !tile.selected)
                        {
                            if (tile.tileType == TileTypes.door.ToString() && currentTile.tileColor == tile.tileColor)
                            {
                                correctDoor = true;
                            }
                            return true;
                        }
                        break;
                }
            }
            correctDoor = false;
            return false;
        }
        /// <summary>
        /// checks to see if all box type tiles are gone
        /// </summary>
        /// <returns></returns>
        public bool CheckWin()
        {
            foreach (Tile tile in this.Controls.OfType<Tile>().ToArray())
            {
                if (tile.tileType == TileTypes.box.ToString())
                {
                    return false;
                }
            }
            MessageBox.Show("You win.");
            restartToolStripMenuItem.PerformClick();
            return true;
        }
        /// <summary>
        /// selects a tile and returns if it is a box or not
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tile_Click(object sender, EventArgs e)
        {
            foreach (Tile tile in this.Controls.OfType<Tile>().ToArray())
            {
                tile.selected = false;
            }

            currentTile.Padding = new System.Windows.Forms.Padding(all: 0);
            currentTile.BackColor = Color.Transparent;

            if (((Tile)sender).tileType == TileTypes.box.ToString())
            {
                isBox = true;

                currentTile.Padding = new System.Windows.Forms.Padding(all: 0);
                currentTile.BackColor = Color.Transparent;

                currentTile = (Tile)sender;
                currentTile.selected = true;

                currentTile.Padding = new System.Windows.Forms.Padding(all: 2);
                currentTile.BackColor = Color.Blue;
            }
            else
            {
                isBox = false;
            }
        }
        /// <summary>
        /// form load events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DesignForm_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;
            DisableControls();
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
                try
                {
                    using (StreamReader reader = new StreamReader(openfile.FileName))
                    {
                        while ((record = reader.ReadLine()) != null)
                        {
                            levelList.Add(record);
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Something went wrong loading.");
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

                        tile.SizeMode = PictureBoxSizeMode.StretchImage;

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
                        Thread.Sleep(10);
                    }
                    y += HEIGHT + GAP;
                    x = START_LEFT;
                }

                foreach (Tile tile in this.Controls.OfType<Tile>().ToArray())
                {
                    if (tile.tileType == TileTypes.blank.ToString())
                    {
                        this.Controls.Remove(tile);
                        Application.DoEvents();
                        Thread.Sleep(10);
                    }
                }
                EnableControls();
            }
        }
        /// <summary>
        /// resets the form to its original state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void restartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Tile tile in this.Controls.OfType<Tile>().ToArray())
            {
                tile.Dispose();
                this.Controls.Remove(tile);
            }
            moves = 0;
            txtMoves.Text = moves.ToString();
            DisableControls();
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
        /// <summary>
        /// uses the w-a-s-d keys to perform up down left and right controls
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    e.Handled = true;
                    btnUp.PerformClick();
                    break;
                case Keys.S:
                    e.Handled = true;
                    btnDown.PerformClick();
                    break;
                case Keys.A:
                    e.Handled = true;
                    btnLeft.PerformClick();
                    break;
                case Keys.D:
                    e.Handled = true;
                    btnRight.PerformClick();
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// moves box up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUp_Click(object sender, EventArgs e)
        {
            if (isBox)
            {
                lblMoves.Select();
                moves++;
                txtMoves.Text = moves.ToString();
                DisableControls();
                while (!Intersection("up"))
                {
                    if (chkAnimations.Checked)
                    {
                        currentTile.Top -= GAP;
                        Application.DoEvents();
                        Thread.Sleep(5);
                    }
                    else
                    {
                        currentTile.Top -= HEIGHT + GAP;
                    }
                }
                EnableControls();
                if (correctDoor)
                {
                    this.Controls.Remove(currentTile);
                    
                }
            }
            else
            {
                MessageBox.Show("Please select a box first.");
            }
            CheckWin();
        }
        /// <summary>
        /// moves box down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDown_Click(object sender, EventArgs e)
        {
            if (isBox)
            {
                lblMoves.Select();
                moves++;
                txtMoves.Text = moves.ToString();
                DisableControls();
                while (!Intersection("down"))
                {
                    if (chkAnimations.Checked)
                    {
                        currentTile.Top += GAP;
                        Application.DoEvents();
                        Thread.Sleep(5);
                    }
                    else
                    {
                        currentTile.Top += HEIGHT + GAP;
                    }
                }
                EnableControls();
                if (correctDoor)
                {
                    this.Controls.Remove(currentTile);
                }
            }
            else
            {
                MessageBox.Show("Please select a box first.");
            }
            CheckWin();
        }
        /// <summary>
        /// moves box left
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLeft_Click(object sender, EventArgs e)
        {
            if (isBox)
            {
                lblMoves.Select();
                moves++;
                txtMoves.Text = moves.ToString();
                DisableControls();
                while (!Intersection("left"))
                {
                    if (chkAnimations.Checked)
                    {
                        currentTile.Left -= GAP;
                        Application.DoEvents();
                        Thread.Sleep(5);
                    }
                    else
                    {
                        currentTile.Left -= WIDTH + GAP;
                    }
                }
                EnableControls();
                if (correctDoor)
                {
                    this.Controls.Remove(currentTile);
                }
            }
            else
            {
                MessageBox.Show("Please select a box first.");
            }
            CheckWin();
        }
        /// <summary>
        /// moves box right
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRight_Click(object sender, EventArgs e)
        {
            if (isBox)
            {
                lblMoves.Select();
                moves++;
                txtMoves.Text = moves.ToString();
                DisableControls();
                while (!Intersection("right"))
                {
                    if (chkAnimations.Checked)
                    {
                        currentTile.Left += GAP;
                        Application.DoEvents();
                        Thread.Sleep(5);
                    }
                    else
                    {
                        currentTile.Left += WIDTH + GAP;
                    }
                }
                EnableControls();
                if (correctDoor)
                {
                    this.Controls.Remove(currentTile);
                }
            }
            else
            {
                MessageBox.Show("Please select a box first.");
            }
            CheckWin();
        }
    }
}