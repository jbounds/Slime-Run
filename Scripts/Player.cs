using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Scripts.Enums;

namespace Scripts
{

    public class Player : KinematicBody2D
    {
        public TouchScreenButton ButtonLeft;
        public TouchScreenButton ButtonRight;
        public Goal CurrentGoalLabel;
        Control GameOverMenu;
        public Area2D ScoreArea;
        public Node2D SlimeContainer;
        public List<SlimeData> SlimeList;
        public Level LevelLabel;
        public Goal NextGoalLabel;
        public Vector2 PlayerStartPosition = new Vector2();
        public uint QuarterSecondsPassed = 0;
        public Score ScoreLabel;
        public int ScoreLabelPositionDifference = 0;
        public int SecondsFraction = 0;
        public int Speed = 300;
        public StrikesContainer StrikesContainer;
        public Vector2 Velocity = new Vector2();

        public SlimeData ChooseSlimeData()
        {
            return SlimeList[(int)(GD.Randi() % (SlimeList.FindLastIndex(a => a.AssociatedDifficulty == (GetTree().Root.GetNode("GlobalAttributes") as GlobalAttributes).Difficulty) + 1))];
        }

        public bool DenyOverlappingSpawn(uint xPosition, float yPosition)
        {
            var allSlimes = SlimeContainer.GetChildren();
            var slimeList = new List<Slime>();
            var slimeWidth = 70;

            for (int i = 0; i < allSlimes.Count; i++)
            {
                var slime = allSlimes[i] as Slime;
                if (slime.Position.y == yPosition)
                {
                    slimeList.Add(slime);
                }
            }

            // Don't spawn the slime if it is colliding with another slime (can maybe check collision method rather than this.)
            if (slimeList.Any(a => a.Position.x > xPosition - slimeWidth && a.Position.x < xPosition + slimeWidth))
            {
                return true;
            }
            return false;
        }

        private void GenerateSlimeLine()
        {
            // 2 or less enemies + a max of 1 more per 10 seconds.
            var slimeMax = 2 + (QuarterSecondsPassed / 40);

            var randomNumberOfEnemies = GD.Randi() % slimeMax;
            for (int i = 0; i < randomNumberOfEnemies; i++)
            {
                SpawnSlime();
            }

            if (SecondsFraction == 60)
            {
                SecondsFraction = 0;
            }
        }

        public void GetInput()
        {
            Velocity = new Vector2();
            var playerPosition = this.Position;
            var edgeOfScreenBuffer = 20;

            // Add in user movement
            if ((GetTree().Root.GetNode("GlobalAttributes") as GlobalAttributes).IsMobile &&
                (GetTree().Root.GetNode("GlobalAttributes") as GlobalAttributes).UseTiltControls)
            {
                var attemptedMovement = Input.GetAccelerometer().x * 0.5f;
                if (attemptedMovement > 0 && playerPosition.x <= GetViewport().Size.x - edgeOfScreenBuffer)
                {
                    Velocity.x = attemptedMovement;
                }
                if (attemptedMovement < 0 && playerPosition.x > 0 + edgeOfScreenBuffer)
                {
                    Velocity.x = attemptedMovement;
                }
            }
            else
            {
                if ((Input.IsActionPressed("ui_right") || ButtonRight.IsPressed()) && playerPosition.x <= GetViewport().Size.x - edgeOfScreenBuffer)
                {
                    Velocity.x += 1;
                }
                if ((Input.IsActionPressed("ui_left") || ButtonLeft.IsPressed()) && playerPosition.x > 0 + edgeOfScreenBuffer)
                {
                    Velocity.x -= 1;
                }
            }

            Velocity *= Speed;
        }

        public virtual void HandleHit(Slime body)
        {
            if (body.SlimeData.Slime == CurrentGoalLabel.SlimeData.Slime)
            {
                this.HandleHitScore();
            }
            else
            {
                this.HandleHitDamage();
            }
            body.GetParent().CallDeferred("remove_child", body);
        }

        private void HandleHitDamage()
        {
            if (StrikesContainer.Strikes > 0)
            {
                StrikesContainer.UpdateStrikes();
            }
            else
            {
                var highScoreForDifficulty = (GetTree().Root.GetNode("GlobalAttributes") as GlobalAttributes).HighScoreList.
                    First(a => a.Difficulty == (GetTree().Root.GetNode("GlobalAttributes") as GlobalAttributes).Difficulty).Score;

                if (ScoreLabel.CurrentScore > highScoreForDifficulty)
                {
                    (GetTree().Root.GetNode("GlobalAttributes") as GlobalAttributes).HighScoreList.
                    First(a => a.Difficulty == (GetTree().Root.GetNode("GlobalAttributes") as GlobalAttributes).Difficulty).Score = ScoreLabel.CurrentScore;
                }

                GetParent().AddChild(GameOverMenu);
                GetTree().Paused = true;
            }
        }

        private void HandleHitScore()
        {
            ScoreLabel.CurrentScore += 1;
            ScoreLabel.UpdateScore();

            CurrentGoalLabel.SlimeData = NextGoalLabel.SlimeData;
            CurrentGoalLabel.UpdateGoal();

            NextGoalLabel.SlimeData = ChooseSlimeData();
            NextGoalLabel.UpdateGoal();
        }

        private void MovePlayer(float delta)
        {
            // Don't allow any vertical movement.
            Velocity.y = 0;
            this.Position += Velocity * delta;
        }

        public override void _PhysicsProcess(float delta)
        {
            SecondsFraction += 1;
            GetInput();
            MovePlayer(delta);

            if (SecondsFraction % 15 == 0)
            {
                QuarterSecondsPassed++;
            }
            else if (SecondsFraction % 20 == 0)
            {
                GenerateSlimeLine();
                RemoveAllEnemies(true);
            }
        }

        public void RemoveAllEnemies(bool singleRow = false)
        {
            var allNodes = SlimeContainer.GetChildren();
            var pixelFallOffPoint = 500;
            for (int i = 0; i < allNodes.Count; i++)
            {
                if (allNodes[i] is Slime)
                {
                    var slime = allNodes[i] as Slime;
                    if (singleRow && this.Position.y + pixelFallOffPoint < slime.Position.y)
                    {
                        SlimeContainer.CallDeferred("remove_child", (Node)allNodes[i]);
                    }
                    else if (!singleRow)
                    {
                        SlimeContainer.CallDeferred("remove_child", (Node)allNodes[i]);
                    }
                }
            }
        }

        public override void _Ready()
        {
            ButtonLeft = (TouchScreenButton)this.GetNode("CanvasLayer").GetNode("ButtonLeft");
            ButtonRight = (TouchScreenButton)this.GetNode("CanvasLayer").GetNode("ButtonRight");
            SlimeList = SlimeData.GetSlimeList();
            LevelLabel = (Level)this.GetNode("LevelLabel");
            LevelLabel.levelData = LevelData.GetLevelList();
            PlayerStartPosition = this.Position;
            ScoreLabel = (Score)GetParent().GetNode("StaticGUI/ScoreLabel");

            CurrentGoalLabel = (Goal)GetParent().GetNode("StaticGUI/CurrentGoalLabel");
            NextGoalLabel = (Goal)GetParent().GetNode("StaticGUI/NextGoalLabel");
            StrikesContainer = (StrikesContainer)GetParent().GetNode("StaticGUI/StrikesContainer");

            SlimeContainer = (Node2D)GetParent().GetNode("SlimeContainer");

            ScoreLabelPositionDifference = (int)(PlayerStartPosition.x - ScoreLabel.RectGlobalPosition.x);

            ScoreArea = (Area2D)this.GetNode("ScoreArea");

            var gameOverScene = GD.Load("res://Scenes/GameOver.tscn") as PackedScene;
            GameOverMenu = gameOverScene.Instance() as Control;

            base._Ready();
        }

        private void SetSlimeSprite(Slime slime)
        {
            var slimeSprite = (slime.GetNode("Sprite") as Sprite);
            slimeSprite.Texture = (Texture)GD.Load("res://Texture/Slimes/" + slime.SlimeData.Slime + ".png");
        }

        public void SpawnSlime()
        {
            // 32 pixel buffer for edges of screen.
            var randomX = (uint)((GD.Randi() % (GetViewport().Size.x - 32)) + 32);
            var newSlimePixelDistanceAhead = GetViewport().Size.y;

            if (DenyOverlappingSpawn(randomX, this.Position.y - newSlimePixelDistanceAhead))
            {
                return;
            }
            else
            {
                var newSlimeScene = GD.Load("res://Scenes//Slime.tscn") as PackedScene;
                var newSlime = newSlimeScene.Instance() as Slime;

                var slimeData = ChooseSlimeData();
                newSlime.SlimeData = slimeData;
                SetSlimeSprite(newSlime);

                SlimeContainer.AddChild(newSlime);

                newSlime.MoveLocalX(randomX);
                newSlime.MoveLocalY(this.Position.y - newSlimePixelDistanceAhead);
            }
        }
    }
}
