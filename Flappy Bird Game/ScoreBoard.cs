using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using static Flappy_Bird_Game.Form1;

namespace Flappy_Bird_Game
{
    public partial class ScoreBoard : Form
    {
        List<PlayerScore> playerScores = new List<PlayerScore>();

        void UpdateScoreBoardGUI()
        {
            // create a new table object
            DataTable scoreBoard = new DataTable();

            // add 3 colums
            scoreBoard.Columns.Add("Ranking");
            scoreBoard.Columns.Add("User Name");
            scoreBoard.Columns.Add("Score");

            // Add 1 row per score board in the game
            int rank = 1;
            foreach (var playerScore in playerScores.OrderByDescending(p => p.Score))
            {
                scoreBoard.Rows.Add(rank++, playerScore.Name, playerScore.Score);
            }


            // update the datagrid
            dataGridView1.DataSource = scoreBoard;

            // Resize the columns
            dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
        }

        public ScoreBoard()
        {
            InitializeComponent();
            UpdateScoreBoardGUI();
            LoadJSON();
        }
        void LoadJSON()
        {
            const string JSONfile = "PlayerData.json";

            string json = File.ReadAllText(JSONfile);
            playerScores = JsonSerializer.Deserialize<List<PlayerScore>>(json);

            UpdateScoreBoardGUI();

        }
    }
}
