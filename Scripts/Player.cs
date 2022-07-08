using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Scripts.Enums;

namespace Scripts
{

    public class Player : KinematicBody2D
    {
        public TileMap BackgroundTiles = null;
        public TouchScreenButton ButtonLeft;
        public TouchScreenButton ButtonRight;
        public Goal CurrentGoalLabel = null;
        // Will be updated on menu in future.
        public DifficultyTypes Difficulty = DifficultyTypes.Easy;
        public List<SlimeData> EnemyList;
        public Level LevelLabel = null;
        public Goal NextGoalLabel = null;
        public Vector2 PlayerStartPosition = new Vector2();
        public uint QuarterSecondsPassed = 0;
        public Score ScoreLabel = null;
        public int ScoreLabelPositionDifference = 0;
        public int SecondsFraction = 0;
        public int Speed = 200;
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

        public void DrawBackgroundTiles()
        {
            var playerPosition = BackgroundTiles.WorldToMap(this.Position);
            var boundaries = GetTileBoundary(playerPosition);

            for (int i = 0; i < boundaries.Count; i++)
            {
                var tile = BackgroundTiles.GetCell((int)boundaries[i].x, (int)boundaries[i].y);

                // If the Tile doesn't exist, create a new tile.
                // Use my game's X camera range. A little lazy - could do soemthing in GetTileBoundary().
                if (tile == Godot.TileMap.InvalidCell &&
                    boundaries[i].x >= -2 && boundaries[i].x < 32)
                {
                    SetCell(BackgroundTiles, (int)boundaries[i].x, (int)boundaries[i].y, 0);
                }
            }
        }

        private void GenerateEnemyLine()
        {
            QuarterSecondsPassed++;

            // Less than 10 enemies + 1 more per 5 seconds
            var enemyMax = 10 + (QuarterSecondsPassed / 20);

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
            var playerPosition = BackgroundTiles.WorldToMap(this.Position);

            // Force movement up.
            Velocity.y -= 1.5F;

            // Add in user movement
            if ((Input.IsActionPressed("ui_right") || ButtonRight.IsPressed()) && playerPosition.x <= 32)
            {
                Velocity.x += 1;
            }
            if ((Input.IsActionPressed("ui_left") || ButtonLeft.IsPressed()) && playerPosition.x > 0)
            {
                Velocity.x -= 1;
            }
            if (Input.IsActionPressed("ui_down"))
            {
                Velocity.y += 1;
            }
            if (Input.IsActionPressed("ui_up"))
            {
                Velocity.y -= 1;
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
            ScoreLabel.ResetScore();
            QuarterSecondsPassed = 0;
            this.Position = PlayerStartPosition;

            RemoveAllEnemies();

            // Reset the seconds tracker to refresh the map.
            SecondsFraction = -1;
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

                // Janky way of making label not move on screen with Player.
                if (this.Position.x <= 280)
                {
                    ScoreLabel.RectGlobalPosition = new Vector2(Math.Max(this.Position.x - ScoreLabelPositionDifference, 10), ScoreLabel.RectGlobalPosition.y);
                }
                // Screen max size - 1 screensize with zoom.
                else if (this.Position.x >= (1024 - 280))
                {
                    ScoreLabel.RectGlobalPosition = new Vector2(Math.Min(this.Position.x - ScoreLabelPositionDifference, (1024 - 280)), ScoreLabel.RectGlobalPosition.y);
                }
                else
                {
                    ScoreLabel.RectGlobalPosition = new Vector2(this.Position.x - ScoreLabelPositionDifference, ScoreLabel.RectGlobalPosition.y);
                }
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            SecondsFraction += 1;
            GetInput();
            MovePlayer();

            if (SecondsFraction % 15 == 0)
            {
                DrawBackgroundTiles();
                RemoveOldBackgroundTiles();
                BackgroundTiles.UpdateBitmaskRegion();

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
            BackgroundTiles = (TileMap)GetParent().GetNode("StonePath");
            ButtonLeft = (TouchScreenButton)this.GetNode("CanvasLayer").GetNode("ButtonLeft");
            ButtonRight = (TouchScreenButton)this.GetNode("CanvasLayer").GetNode("ButtonRight");
            EnemyList = SlimeData.GetEnemyList();
            LevelLabel = (Level)this.GetNode("LevelLabel");
            LevelLabel.levelData = LevelData.GetLevelList();
            PlayerStartPosition = this.Position;
            ScoreLabel = (Score)this.GetNode("UserInterface/ScoreLabel");

            CurrentGoalLabel = (Goal)this.GetNode("UserInterface/CurrentGoalLabel");
            NextGoalLabel = (Goal)this.GetNode("UserInterface/NextGoalLabel");

            ScoreLabelPositionDifference = (int)(PlayerStartPosition.x - ScoreLabel.RectGlobalPosition.x);
            base._Ready();
        }

        public void RemoveOldBackgroundTiles()
        {
            var playerPosition = BackgroundTiles.WorldToMap(this.Position);
            var xMaxValue = 36;

            for (int i = -2; i < xMaxValue; i++)
            {
                BackgroundTiles.SetCell(i, (int)playerPosition.y + 5, -1);
            }
        }

        public void SetCell(TileMap tileMap, int x, int y, int id)
        {
            // An index of -1 clears the cell. This is not used here.
            tileMap.SetCell(x, y, id, false, false, false, GetSubTileWithPriority(tileMap, id));
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
            // 1 Box (32 pixels) can't be used, shift to right 1 box.
            var randomX = (GD.Randi() % 1024) + 32;
            var newEnemyPixelDistanceAhead = 800;

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
