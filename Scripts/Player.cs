using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Scripts.Enums;

namespace Scripts
{

    public class Player : KinematicBody2D
    {
        public TileMap backgroundTiles = null;
        public TouchScreenButton buttonLeft;
        public TouchScreenButton buttonRight;
        public List<SlimeData> enemyList;
        public Level levelLabel = null;
        public Vector2 playerStartPosition = new Vector2();
        public Score scoreLabel = null;
        public Goal currentGoalLabel = null;
        public Goal nextGoalLabel = null;
        public int scoreLabelPositionDifference = 0;
        public int secondsFraction = 0;
        public int speed = 200;
        public Vector2 velocity = new Vector2();
        public uint quarterSecondsPassed = 0;

        public SlimeData ChooseEnemyData()
        {
            return enemyList[(int)(GD.Randi() % enemyList.Count)];
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
            var playerPosition = backgroundTiles.WorldToMap(this.Position);
            var boundaries = GetTileBoundary(playerPosition);

            for (int i = 0; i < boundaries.Count; i++)
            {
                var tile = backgroundTiles.GetCell((int)boundaries[i].x, (int)boundaries[i].y);

                // If the Tile doesn't exist, create a new tile.
                // Use my game's X camera range. A little lazy - could do soemthing in GetTileBoundary().
                if (tile == Godot.TileMap.InvalidCell &&
                    boundaries[i].x >= -2 && boundaries[i].x < 32)
                {
                    SetCell(backgroundTiles, (int)boundaries[i].x, (int)boundaries[i].y, 0);
                }
            }
        }

        private void GenerateEnemyLine()
        {
            quarterSecondsPassed++;

            // Less than 10 enemies + 1 more per 5 seconds
            var enemyMax = 10 + (quarterSecondsPassed / 20);

            var randomNumberOfEnemies = GD.Randi() % enemyMax;
            for (int i = 0; i < randomNumberOfEnemies; i++)
            {
                SpawnEnemy();
            }

            if (secondsFraction == 60)
            {
                secondsFraction = 0;
            }
        }

        public void GetInput()
        {
            velocity = new Vector2();
            var playerPosition = backgroundTiles.WorldToMap(this.Position);

            // Force movement up.
            velocity.y -= 1.5F;

            // Add in user movement
            if ((Input.IsActionPressed("ui_right") || buttonRight.IsPressed()) && playerPosition.x <= 32)
            {
                velocity.x += 1;
            }
            if ((Input.IsActionPressed("ui_left") || buttonLeft.IsPressed()) && playerPosition.x > 0)
            {
                velocity.x -= 1;
            }
            if (Input.IsActionPressed("ui_down"))
            {
                velocity.y += 1;
            }
            if (Input.IsActionPressed("ui_up"))
            {
                velocity.y -= 1;
            }

            velocity *= speed;
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
            scoreLabel.score += 1;
            scoreLabel.UpdateScore();

            currentGoalLabel.slimeData = nextGoalLabel.slimeData;
            currentGoalLabel.UpdateGoal();

            nextGoalLabel.slimeData = ChooseEnemyData();
            nextGoalLabel.UpdateGoal();
        }

        public virtual void handle_hit_death()
        {
            scoreLabel.ResetScore();
            quarterSecondsPassed = 0;
            this.Position = playerStartPosition;

            RemoveAllEnemies();

            // Reset the seconds tracker to refresh the map.
            secondsFraction = -1;
        }

        private void MovePlayer()
        {
            AnimationTree myAnimTree = GetNode<AnimationTree>("AnimationTree");
            AnimationNodeStateMachinePlayback stateMachinePlayback = (AnimationNodeStateMachinePlayback)myAnimTree.Get("parameters/playback");

            if (velocity == Vector2.Zero)
            {
                stateMachinePlayback.Travel("Idle");
            }
            else
            {
                stateMachinePlayback.Travel("Walking");
                myAnimTree.Set("parameters/Idle/blend_position", velocity);
                myAnimTree.Set("parameters/Walking/blend_position", velocity);
                MoveAndSlide(velocity);

                // Janky way of making label not move on screen with Player.
                if (this.Position.x <= 280)
                {
                    scoreLabel.RectGlobalPosition = new Vector2(Math.Max(this.Position.x - scoreLabelPositionDifference, 10), scoreLabel.RectGlobalPosition.y);
                }
                // Screen max size - 1 screensize with zoom.
                else if (this.Position.x >= (1024 - 280))
                {
                    scoreLabel.RectGlobalPosition = new Vector2(Math.Min(this.Position.x - scoreLabelPositionDifference, (1024 - 280)), scoreLabel.RectGlobalPosition.y);
                }
                else
                {
                    scoreLabel.RectGlobalPosition = new Vector2(this.Position.x - scoreLabelPositionDifference, scoreLabel.RectGlobalPosition.y);
                }
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            secondsFraction += 1;
            GetInput();
            MovePlayer();

            if (secondsFraction % 15 == 0)
            {
                DrawBackgroundTiles();
                RemoveOldBackgroundTiles();
                backgroundTiles.UpdateBitmaskRegion();

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
            backgroundTiles = (TileMap)GetParent().GetNode("StonePath");
            buttonLeft = (TouchScreenButton)this.GetNode("CanvasLayer").GetNode("ButtonLeft");
            buttonRight = (TouchScreenButton)this.GetNode("CanvasLayer").GetNode("ButtonRight");
            enemyList = SlimeData.GetEnemyList();
            levelLabel = (Level)this.GetNode("LevelLabel");
            levelLabel.levelData = LevelData.GetLevelList();
            playerStartPosition = this.Position;
            scoreLabel = (Score)this.GetNode("UserInterface/ScoreLabel");

            currentGoalLabel = (Goal)this.GetNode("UserInterface/CurrentGoalLabel");
            nextGoalLabel = (Goal)this.GetNode("UserInterface/NextGoalLabel");

            scoreLabelPositionDifference = (int)(playerStartPosition.x - scoreLabel.RectGlobalPosition.x);
            base._Ready();
        }

        public void RemoveOldBackgroundTiles()
        {
            var playerPosition = backgroundTiles.WorldToMap(this.Position);
            var xMaxValue = 36;

            for (int i = -2; i < xMaxValue; i++)
            {
                backgroundTiles.SetCell(i, (int)playerPosition.y + 5, -1);
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
            enemySprite.Texture = (Texture)GD.Load("res://Texture/Slimes/" + enemy.EnemyData.Slime + ".png");

            if (enemy.EnemyData.Slime == SlimeTypes.LavaSlime)
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
                newEnemy.EnemyData = enemyData;
                SetEnemySprite(newEnemy);

                GetParent().AddChild(newEnemy);

                newEnemy.MoveLocalX(randomX);
                newEnemy.MoveLocalY(this.Position.y - newEnemyPixelDistanceAhead);
            }
        }
    }
}
