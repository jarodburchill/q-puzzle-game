/*
 * Jarod Burchill
 * PROG2370 Assignment 1
 * Written September 24th - September 28th
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QPuzzleGame
{
    /// <summary>
    /// tile props
    /// </summary>
    public class Tile : PictureBox
    {
        public string tileColor { get; set; }
        public string tileType { get; set; }
        public bool selected { get; set; }
    }
}
