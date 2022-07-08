using Godot;
using System;

namespace Scripts
{
    public class StrikesContainer : Control
    {
        public Sprite Strike1;
        public Sprite Strike2;
        public Sprite Strike3;
        public int Strikes = 3;

        public override void _Ready()
        {
            Strike1 = (Sprite)this.GetNode("Strike1");
            Strike2 = (Sprite)this.GetNode("Strike2");
            Strike3 = (Sprite)this.GetNode("Strike3");
        }

         public void UpdateStrikes()
         {
            Strikes--;

            if (Strikes == 2)
            {
                Strike1.Frame = 2;
            }
            else if (Strikes == 1)
            {
                Strike2.Frame = 2;
            }
            else if (Strikes == 0)
            {
                Strike3.Frame = 2;
            }
         }

         public void ResetStrikes()
         {
            Strikes = 3;

            Strike1.Frame = 0;
            Strike2.Frame = 0;
            Strike3.Frame = 0;
         }
    }
}
