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
        // Will be updated on menu in future.
        public DifficultyTypes Difficulty = DifficultyTypes.Easy;
        public List<SlimeData> EnemyList;
        public Level LevelLabel;
        public Goal NextGoalLabel;
        public Vector2 PlayerStartPosition = new Vector2();
        public uint QuarterSecondsPassed = 0;
        public Score ScoreLabel;
        public int ScoreLabelPositionDifference = 0;
        public int SecondsFraction = 0;
        public int Speed = 200;
        public StrikesContainer StrikesContainer;
        public Vector2 Velocity = new Vector2();

        public SlimeData ChooseEnemyData()
        {
            return EnemyList[(int)(GD.Randi() % (EnemyList.FindLastIndex(a => a.AssociatedDifficulty == Difficulty) + 1))];
        }

        public bool DenyOverlappingSpawn(uint xPosition, float yPosition)
        {
            var allNodes = GetParent().GetChildren();
            var enemyList = new List<Slime>();

            for (int i = 0; i < allNodes.Count; i++)
            {
                if (allNodes[i] is Slime)
                {
                    var enemy = allNodes[i] as Slime;
                    if (enemy.Position.y == yPosition)
                    {
                        enemyList.Add(enemy);
                    }
                }
            }

            // Don't spawn the enemy if it is colliding with another enemy (can maybe check collision method rather than this.)
            if (enemyList.Any(a => a.Position.x > xPosition - 32 && a.Position.x < xPosition + 32))
            {
                return true;
            }
            return false;
        }

        private void GenerateEnemyLine()
        {
            QuarterSecondsPassed++;

            // Less than 10 enemies + 1 more per 5 seconds
            var enemyMax = 5 + (QuarterSecondsPassed / 20);

            var randomNumberOfEnemies = GD.Randi() % enemyMax;
            for (int i = 0; i < randomNumberOfEnemies; i++)
            {
                SpawnEnemy();
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

            // Add in user movement
            if ((Input.IsActionPressed("ui_right") || ButtonRight.IsPressed()) && playerPosition.x <= GetViewport().Size.x)
            {
                Velocity.x += 1;
            }
            if ((Input.IsActionPressed("ui_left") || ButtonLeft.IsPressed()) && playerPosition.x > 0)
            {
                Velocity.x -= 1;
            }

            Velocity *= Speed;
        }

        public Vector2 GetSubTileWithPriority(TileMap tileMap, int id)
        {
            var tiles = tileMap.TileSet;
            var rect = tileMap.TileSet.TileGetRegion(id);
            var sizeX = rect.Size.x / tiles.AutotileGetSize(id).x;
            var sizeY = rect.Size.y / tiles.AutotileGetSize(id).y;
            var tileList = new List<Vector2>();

            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    var priority = tiles.AutotileGetSubtilePriority(id, new Vector2(x, y));
                    for (int p = 0; p < priority; p++)
                    {
                        tileList.Add(new Vector2(x, y));
                    }
                }
            }

            // Select a random tile from the list of all tiles.
            return tileList[0];
        }

        public List<Vector2> GetTileBoundary(Vector2 playerPosition)
        {
            // X and Y are camera size dependent - //? can I pull current view size?
            //? Card here: https://trello.com/c/TN08plk9
            var xMaxValue = 36;
            var yMaxValue = 36;
            var index = 0;
            var boundaries = new List<Vector2>();
            boundaries.Add(playerPosition);

            for (int x = 0; x <= xMaxValue; x++)
            {
                for (int y = 0; y < yMaxValue; y++)
                {
                    index++;
                    boundaries.Add(new Vector2(playerPosition.x + x, playerPosition.y));
                    boundaries.Add(new Vector2(playerPosition.x - x, playerPosition.y - y));
                    boundaries.Add(new Vector2(playerPosition.x + x, playerPosition.y - y));
                    boundaries.Add(new Vector2(playerPosition.x - x, playerPosition.y));
                }
            }

            return boundaries;
        }

        public virtual void handle_hit()
        {
            ScoreLabel.CurrentScore += 1;
            ScoreLabel.UpdateScore();

            CurrentGoalLabel.SlimeData = NextGoalLabel.SlimeData;
            CurrentGoalLabel.UpdateGoal();

            NextGoalLabel.SlimeData = ChooseEnemyData();
            NextGoalLabel.UpdateGoal();
        }

        public virtual void handle_hit_death()
        {
            if (StrikesContainer.Strikes > 0)
            {
                StrikesContainer.UpdateStrikes();
            }
            else
            {
                ScoreLabel.ResetScore();
                QuarterSecondsPassed = 0;
                this.Position = PlayerStartPosition;

                RemoveAllEnemies();

                // Reset the seconds tracker to refresh the map.
                SecondsFraction = -1;
                StrikesContainer.ResetStrikes();
            }
        }

        private void MovePlayer()
        {
            AnimationTree myAnimTree = GetNode<AnimationTree>("AnimationTree");
            AnimationNodeStateMachinePlayback stateMachinePlayback = (AnimationNodeStateMachinePlayback)myAnimTree.Get("parameters/playback");

            if (Velocity == Vector2.Zero)
            {
                stateMachinePlayback.Travel("Idle");
            }
            else
            {
                stateMachinePlayback.Travel("Walking");
                myAnimTree.Set("parameters/Idle/blend_position", Velocity);
                myAnimTree.Set("parameters/Walking/blend_position", Velocity);
                MoveAndSlide(Velocity);
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            SecondsFraction += 1;
            GetInput();
            MovePlayer();

            if (SecondsFraction % 15 == 0)
            {
                GenerateEnemyLine();
                RemoveAllEnemies(true);
            }
        }

        public void RemoveAllEnemies(bool singleRow = false)
        {
            var allNodes = GetParent().GetChildren();
            var pixelFallOffPoint = 500;
            for (int i = 0; i < allNodes.Count; i++)
            {
                if (allNodes[i] is Slime)
                {
                    var enemy = allNodes[i] as Slime;
                    if (singleRow && this.Position.y + pixelFallOffPoint < enemy.Position.y)
                    {
                        GetParent().CallDeferred("remove_child", (Node)allNodes[i]);
                    }
                    else if (!singleRow)
                    {
                        GetParent().CallDeferred("remove_child", (Node)allNodes[i]);
                    }
                }
            }
        }

        public override void _Ready()
        {
            ButtonLeft = (TouchScreenButton)this.GetNode("CanvasLayer").GetNode("ButtonLeft");
            ButtonRight = (TouchScreenButton)this.GetNode("CanvasLayer").GetNode("ButtonRight");
            EnemyList = SlimeData.GetEnemyList();
            LevelLabel = (Level)this.GetNode("LevelLabel");
            LevelLabel.levelData = LevelData.GetLevelList();
            PlayerStartPosition = this.Position;
            ScoreLabel = (Score)GetParent().GetNode("StaticGUI/ScoreLabel");

            CurrentGoalLabel = (Goal)GetParent().GetNode("StaticGUI/CurrentGoalLabel");
            NextGoalLabel = (Goal)GetParent().GetNode("StaticGUI/NextGoalLabel");
            StrikesContainer = (StrikesContainer)GetParent().GetNode("StaticGUI/StrikesContainer");

            ScoreLabelPositionDifference = (int)(PlayerStartPosition.x - ScoreLabel.RectGlobalPosition.x);
            base._Ready();
        }

        private void SetEnemySprite(Slime enemy)
        {
            var enemySprite = (enemy.GetNode("Sprite") as Sprite);
            enemySprite.Texture = (Texture)GD.Load("res://Texture/Slimes/" + enemy.SlimeData.Slime + ".png");

            if (enemy.SlimeData.Slime == SlimeTypes.LavaSlime)
            {
                enemySprite.Hframes = 8;
            }
        }

        public void SpawnEnemy()
        {
            // 32 pixel buffer for edges of screen.
            var randomX = (uint)((GD.Randi() % (GetViewport().Size.x - 32)) + 32);
            var newEnemyPixelDistanceAhead = GetViewport().Size.y;

            if (DenyOverlappingSpawn(randomX, this.Position.y - newEnemyPixelDistanceAhead))
            {
                return;
            }
            else
            {
                var newEnemyScene = GD.Load("res://Scenes//Enemy.tscn") as PackedScene;
                var newEnemy = newEnemyScene.Instance() as Slime;

                var enemyData = ChooseEnemyData();
                newEnemy.SlimeData = enemyData;
                SetEnemySprite(newEnemy);

                GetParent().AddChild(newEnemy);

                newEnemy.MoveLocalX(randomX);
                newEnemy.MoveLocalY(this.Position.y - newEnemyPixelDistanceAhead);
            }
        }
    }
}
