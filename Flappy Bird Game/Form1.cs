using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Wave;  
using System.IO;
using System.Text.Json;

namespace Flappy_Bird_Game
{
    public partial class Form1 : Form
    {
        // json file saving name 
        const string JSONfile = "PlayerData.json";

        List<PlayerScore> playerScores = new List<PlayerScore>();

        // for playing music
        System.Media.SoundPlayer die = new System.Media.SoundPlayer();
        System.Media.SoundPlayer wing = new System.Media.SoundPlayer();
        WaveOutEvent waveOutDevice = new WaveOutEvent();
        AudioFileReader audioFile = new AudioFileReader("birdsguitar.wav");

        // Variables
        // default pipe speed defined with an integer
        int pipeSpeed = 8;
        // default gravity speed defined with an integer
        double gravity = 0;
        // default score integer set to 0
        int score = 0;

        bool gameOver = false;
        // Track if player name is entered
        bool playerNameEntered = false;


        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true;

            // update the player score
            LoadPlayerScores();

            // song file location
            die.SoundLocation = "sfx_die.wav";
            wing.SoundLocation = "sfx_wing.wav";

        }

        /// <summary>
        /// Event handler for game timer tick event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gameTimerEvent(object sender, EventArgs e)
        {
            if (!playerNameEntered)
                return;

            // Increase gravity gradually to create smoother movement
            gravity += 1.0;

            // Adjust flappy bird's position based on gravity
            flappyBird.Top +=(int)gravity;
            // Move the pipes
            pipeBottom.Left -= pipeSpeed;
            pipeTop.Left -= pipeSpeed;
            pipeBottom3.Left -= pipeSpeed;
            // Update score display
            scoreText.Text = "Score: " + score;

            // Check if the bird has passed the pipes to increase score
            if (pipeBottom.Left < -150)
            {
                pipeBottom.Left = 800;
                score++;
            }
            if(pipeTop.Left < -180)
            {
                pipeTop.Left = 950;
                score++;
            }
            if(pipeBottom3.Left < - 150)
            {
                pipeBottom3.Left = 1100;
                score++;
            }
            // Check collision with pipes or ground
            if (flappyBird.Bounds.IntersectsWith(pipeBottom.Bounds) || 
                flappyBird.Bounds.IntersectsWith(pipeTop.Bounds) ||
                flappyBird.Bounds.IntersectsWith(pipeBottom3.Bounds) ||
                flappyBird.Bounds.IntersectsWith(ground.Bounds) || 
                flappyBird.Top < -25)
            {
                endGame();
            }
            // Increase pipe speed as the game progresses
            if (score > 5)
            {
                pipeSpeed = 15;
            }

            // Check if the bird has reached the top of the screen
            if (flappyBird.Top < -1)
            {
                endGame();
            }
            

        }

        /// <summary>
        /// // Event handler for key down event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gameKeydown(object sender, KeyEventArgs e)
        {
            // this is the game key is down event thats linked to the main form
            if (e.KeyCode == Keys.Space)
            {
                if (!playerNameEntered && !string.IsNullOrWhiteSpace(nameTextBox.Text))
                {
                   
                    // Start the game
                    gameTimer.Start();
                }
                if (!gameOver)
                {
                    wing.Play();
                }
                // if the space key is pressed then the gravity will be set to -15
                gravity = -10;
            }
            // Check if Enter key is pressed to exit the game
            else if (e.KeyCode == Keys.Enter)
            {
                ExitGame();
            }
            // Check if the R key is pressed and the game is over
            else if (e.KeyCode == Keys.R && gameOver)
            {
                // Restart the game
                RestartGame();
            }

        }

        /// <summary>
        /// // Event handler for key up event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gameKeyup(object sender, KeyEventArgs e)
        {
            // this is the game key is up event thats linked to the main form
            if (e.KeyCode == Keys.Space)
            {
                // if the space key is released then gravity is set back to 15
                // gravity -= 10;
            }
            if(e.KeyCode == Keys.R && gameOver)
            {
                
                // run the restart function
                RestartGame();
            }
        }

        /// <summary>
        // Method to handle game over state
        /// </summary>
        private void endGame()
        {
            die.Play(); 
            // Stop the game timer
            gameTimer.Stop();
            // Display game over message
            scoreText.Text += " Game Over!! Press R to Retry";
            // Set game over flag
            gameOver = true;

            // save the score by player name and there highest score
            var playerName = nameTextBox.Text;
            var playerScore = playerScores.FirstOrDefault(p => p.Name == playerName);

            if (playerScore == null)
            {
                playerScores.Add(new PlayerScore { Name = playerName, Score = score });
            }
            else if (score > playerScore.Score)
            {
                playerScore.Score = score;
            }

            SavePlayerScores();
        }

        /// <summary>
        /// // Method to restart the game
        /// </summary>
        private void RestartGame()
        {
            // Reset game variables
            gameOver = false;

            flappyBird.Location = new Point(15, 192);
            pipeTop.Left = 800;
            pipeBottom.Left = 1200;
            pipeBottom3.Left = 1600;

            score = 0;
            pipeSpeed = 8;
            gravity = 0; // reset gravity
            scoreText.Text = "Score: 0";
            // restart the game timer
            gameTimer.Start();

            SavePlayerScores();
        }

        /// <summary>
        /// Event handler for form load event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            // Set volume levels
            audioFile.Volume = 0.1f; // Adjust this value for the desired volume level
            
            // Start playing background music
            waveOutDevice.Init(audioFile);
            waveOutDevice.Play();
            this.KeyDown += gameKeydown;
        }
        /// <summary>
        /// exit the game method
        /// </summary>
        private void ExitGame()
        {
            // close the form-po
            Close();
        }

        /// <summary>
        /// how it save the name and score in file
        /// </summary>
        public class PlayerScore
        {
            public string Name { get; set; }
            public int Score { get; set; }
        }


        /// <summary>
        /// enter button for saving and starting the game 
        /// after player enter there name press enter to start the game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void enterButton_Click(object sender, EventArgs e)
        {
            // Check if the name is entered
            if (!string.IsNullOrWhiteSpace(nameTextBox.Text))
            {
                playerNameEntered = true; // Set the flag to indicate that the player's name is entered
                nameTextBox.Enabled = false; // Disable name input textbox
                enterButton.Enabled = false; // Disable enter button

                // Start the game timer
                gameTimer.Start();
            }
            else
            {
                MessageBox.Show("Please enter your name."); // Show a message if the name is not entered
            }
        }

        /// <summary>
        /// this for the sound 
        /// when pressing space button it don't need to make enter button sound
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Space)
            {
                // Handle the space key press here
                gameKeydown(this, new KeyEventArgs(Keys.Space));
                return true; // Mark the key as handled
            }
            else if (keyData == Keys.Enter)
            {
                // Handle the enter key press here
                ExitGame();
                return true; // Mark the key as handled
            }

            // Allow other keys to be processed normally
            return base.ProcessCmdKey(ref msg, keyData);
        }

        /// <summary>
        ///  save the player score using Json
        /// </summary>
        private void SavePlayerScores()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(playerScores, options);            
                File.WriteAllText(JSONfile, jsonString);
            
           

        }

        /// <summary>
        /// it gonna load the player score 
        /// </summary>
        private void LoadPlayerScores()
        {
          
            string json = File.ReadAllText(JSONfile);
            playerScores = JsonSerializer.Deserialize<List<PlayerScore>>(json);
          
        }

        private void scoreBoardbutton_Click(object sender, EventArgs e)
        {
            // create a new object of the scoreboardform
            ScoreBoard scoreBoard = new ScoreBoard();
            scoreBoard.ShowDialog();
        }
    }
}

