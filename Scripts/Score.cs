using Godot;
using System;

namespace Scripts
{

    public class Score : Label
    {
        public float score = 0;

        public void UpdateScore()
        {
            this.Text = string.Format("Score: {0}", score);
        }

        public void ResetScore()
        {
            score = 0;
        }
    }
}
