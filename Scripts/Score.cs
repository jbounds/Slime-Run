using Godot;
using System;

namespace Scripts
{

    public class Score : Label
    {
        public float CurrentScore = 0;

        public void UpdateScore()
        {
            this.Text = string.Format("Score: {0}", CurrentScore);
        }

        public void ResetScore()
        {
            CurrentScore = 0;
            this.UpdateScore();
        }
    }
}
