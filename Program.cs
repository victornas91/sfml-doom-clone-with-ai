using SFML.Graphics;
using SFML.Window;
using SFML.System;

namespace sfml_fps_game_1;

class Program
{
    private const int ScreenWidth = 800;
    private const int ScreenHeight = 600;
    private const int MapWidth = 24;
    private const int MapHeight = 24;

    private static readonly int[,] WorldMapTemplate = 
    {
      {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
      {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
      {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
      {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
      {1,0,0,0,2,2,2,2,2,0,0,0,0,3,0,3,0,3,0,0,0,0,0,1},
      {1,0,0,0,2,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
      {1,0,0,0,2,0,0,0,2,0,0,0,0,3,0,0,0,3,0,0,0,0,0,1},
      {1,0,0,0,2,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
      {1,0,0,0,2,2,0,2,2,0,0,0,0,3,0,3,0,3,0,0,0,0,0,1},
      {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
      {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
      {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
      {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
      {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
      {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
      {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
      {1,4,4,4,4,4,4,4,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
      {1,4,0,4,0,0,0,0,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
      {1,4,0,0,0,0,5,0,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
      {1,4,0,4,0,0,0,0,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
      {1,4,0,4,4,4,4,4,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
      {1,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
      {1,4,4,4,4,4,4,4,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
      {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1}
    };

    private static int[,] GenerateProceduralLevelMap(int level)
    {
        int[,] generated = (int[,])WorldMapTemplate.Clone();
        Random random = new Random(Guid.NewGuid().GetHashCode() + level * 997);

        for (int x = 1; x < MapWidth - 1; x++)
        {
            for (int y = 1; y < MapHeight - 1; y++)
            {
                int templateTile = WorldMapTemplate[x, y];

                if (templateTile > 0 && templateTile != 1)
                {
                    int chance = random.Next(100);
                    if (chance < 35)
                    {
                        generated[x, y] = templateTile;
                    }
                    else if (chance < 70)
                    {
                        generated[x, y] = 0;
                    }
                    else
                    {
                        generated[x, y] = RandomInteriorWallTile(random);
                    }
                }
                else if (templateTile == 0)
                {
                    generated[x, y] = random.Next(100) < 32 ? RandomInteriorWallTile(random) : 0;
                }
            }
        }

        int[,] smoothed = (int[,])generated.Clone();
        for (int x = 1; x < MapWidth - 1; x++)
        {
            for (int y = 1; y < MapHeight - 1; y++)
            {
                int wallNeighbors = 0;
                for (int nx = x - 1; nx <= x + 1; nx++)
                {
                    for (int ny = y - 1; ny <= y + 1; ny++)
                    {
                        if (nx == x && ny == y) continue;
                        if (generated[nx, ny] > 0) wallNeighbors++;
                    }
                }

                if (generated[x, y] == 0 && wallNeighbors >= 5 && random.Next(100) < 70)
                {
                    smoothed[x, y] = RandomInteriorWallTile(random);
                }
                else if (generated[x, y] > 0 && wallNeighbors <= 2 && random.Next(100) < 75)
                {
                    smoothed[x, y] = 0;
                }
            }
        }

        for (int x = 0; x < MapWidth; x++)
        {
            smoothed[x, 0] = 1;
            smoothed[x, MapHeight - 1] = 1;
        }
        for (int y = 0; y < MapHeight; y++)
        {
            smoothed[0, y] = 1;
            smoothed[MapWidth - 1, y] = 1;
        }

        ClearArea(smoothed, 22, 12, 1);
        ClearArea(smoothed, 10, 10, 1);

        return smoothed;
    }

    private static int RandomInteriorWallTile(Random random)
    {
        return random.Next(100) switch
        {
            < 35 => 2,
            < 60 => 3,
            < 85 => 4,
            _ => 5
        };
    }

    private static void ClearArea(int[,] map, int centerX, int centerY, int radius)
    {
        for (int x = centerX - radius; x <= centerX + radius; x++)
        {
            for (int y = centerY - radius; y <= centerY + radius; y++)
            {
                if (x <= 0 || x >= MapWidth - 1 || y <= 0 || y >= MapHeight - 1) continue;
                map[x, y] = 0;
            }
        }
    }

    class Enemy
    {
        public double X, Y;
        public int Health = 1;
        public double SpeedMultiplier = 1.0;
        public List<SpriteSegment> Segments = new List<SpriteSegment>();

        public void Update(double playerX, double playerY, double frameTime, int[,] worldMap)
        {
            double dirX = playerX - X;
            double dirY = playerY - Y;
            double dist = Math.Sqrt(dirX * dirX + dirY * dirY);
            if (dist > 0.1)
            {
                dirX /= dist;
                dirY /= dist;
                double moveSpeed = frameTime * 2.0 * SpeedMultiplier;
                if (worldMap[(int)(X + dirX * moveSpeed), (int)Y] == 0) X += dirX * moveSpeed;
                if (worldMap[(int)X, (int)(Y + dirY * moveSpeed)] == 0) Y += dirY * moveSpeed;
            }
        }
    }

    class SpriteSegment
    {
        public float RelativeY; // -0.5 to 0.5
        public float Height; // 0 to 1
        public Color Color;
        public float WidthScale; // 1.0 for full width of sprite
        public float RelativeX = 0; // -0.5 to 0.5
    }

    private static Enemy CreateZombie(double x, double y)
    {
        Enemy zombie = new Enemy { X = x, Y = y };
        zombie.Segments.Add(new SpriteSegment { RelativeY = 0, Height = 0.6f, Color = Color.Blue, WidthScale = 0.5f });
        zombie.Segments.Add(new SpriteSegment { RelativeY = -0.1f, Height = 0.2f, Color = Color.White, WidthScale = 0.55f });
        zombie.Segments.Add(new SpriteSegment { RelativeY = 0.35f, Height = 0.3f, Color = Color.White, WidthScale = 0.45f });
        zombie.Segments.Add(new SpriteSegment { RelativeY = -0.4f, Height = 0.2f, Color = Color.Blue, WidthScale = 0.3f });
        zombie.Segments.Add(new SpriteSegment { RelativeY = -0.42f, Height = 0.05f, Color = Color.Red, WidthScale = 0.05f, RelativeX = -0.1f });
        zombie.Segments.Add(new SpriteSegment { RelativeY = -0.42f, Height = 0.05f, Color = Color.Red, WidthScale = 0.05f, RelativeX = 0.1f });
        zombie.Segments.Add(new SpriteSegment { RelativeY = -0.1f, Height = 0.1f, Color = Color.Blue, WidthScale = 0.8f });
        return zombie;
    }

    private static Enemy CreateLevelTwoZombie(double x, double y)
    {
        Enemy zombie = new Enemy { X = x, Y = y, Health = 2, SpeedMultiplier = 1.5 };
        zombie.Segments.Add(new SpriteSegment { RelativeY = 0, Height = 0.6f, Color = Color.Red, WidthScale = 0.5f });
        zombie.Segments.Add(new SpriteSegment { RelativeY = -0.1f, Height = 0.2f, Color = Color.Green, WidthScale = 0.55f });
        zombie.Segments.Add(new SpriteSegment { RelativeY = 0.35f, Height = 0.3f, Color = Color.Green, WidthScale = 0.45f });
        zombie.Segments.Add(new SpriteSegment { RelativeY = -0.4f, Height = 0.2f, Color = Color.Red, WidthScale = 0.3f });
        zombie.Segments.Add(new SpriteSegment { RelativeY = -0.42f, Height = 0.05f, Color = Color.Red, WidthScale = 0.05f, RelativeX = -0.1f });
        zombie.Segments.Add(new SpriteSegment { RelativeY = -0.42f, Height = 0.05f, Color = Color.Red, WidthScale = 0.05f, RelativeX = 0.1f });
        zombie.Segments.Add(new SpriteSegment { RelativeY = -0.1f, Height = 0.1f, Color = Color.Red, WidthScale = 0.8f });
        return zombie;
    }

    static List<Enemy> Enemies = new List<Enemy>();

    static void Main(string[] args)
    {
        var window = new RenderWindow(new VideoMode(new Vector2u(ScreenWidth, ScreenHeight)), "SFML Doom Raycaster");
        window.SetVerticalSyncEnabled(true);
        window.Closed += (sender, e) => window.Close();

        // Loading Screen
        Font font;
        try
        {
            font = new Font(@"C:\Windows\Fonts\arial.ttf");
        }
        catch
        {
            // Fallback if font loading fails, just skip loading screen or use a blank screen
            font = null;
        }

        if (font != null)
        {
            Text welcomeText = new Text(font, "welcome to the arena", 50);
            welcomeText.FillColor = Color.White;
            FloatRect textRect = welcomeText.GetLocalBounds();
            welcomeText.Origin = new Vector2f(textRect.Left + textRect.Width / 2.0f, textRect.Top + textRect.Height / 2.0f);
            welcomeText.Position = new Vector2f(ScreenWidth / 2.0f, ScreenHeight / 2.0f);

            Clock loadingClock = new Clock();
            while (window.IsOpen && loadingClock.ElapsedTime.AsSeconds() < 3.0)
            {
                float elapsed = loadingClock.ElapsedTime.AsSeconds();
                byte alpha = 0;

                if (elapsed < 1.0f) // Fade in
                {
                    alpha = (byte)(elapsed * 255);
                }
                else if (elapsed < 2.0f) // Stay
                {
                    alpha = 255;
                }
                else // Fade out
                {
                    alpha = (byte)((3.0f - elapsed) * 255);
                }

                welcomeText.FillColor = new Color(255, 255, 255, alpha);

                window.DispatchEvents();
                window.Clear(Color.Black);
                window.Draw(welcomeText);
                window.Display();
            }
        }

        while (window.IsOpen)
        {
            // Menu Screen
            bool startGame = false;
            bool isLeftHanded = false;
            if (font != null)
            {
                Text startText = new Text(font, "Start", 40);
                Text exitText = new Text(font, "Exit", 40);
                
                int selectedOption = 0; // 0 for Start, 1 for Exit

                void UpdateMenuPositions()
                {
                    FloatRect startBounds = startText.GetLocalBounds();
                    startText.Origin = new Vector2f(startBounds.Left + startBounds.Width / 2.0f, startBounds.Top + startBounds.Height / 2.0f);
                    startText.Position = new Vector2f(ScreenWidth / 2.0f, ScreenHeight / 2.0f - 30);

                    FloatRect exitBounds = exitText.GetLocalBounds();
                    exitText.Origin = new Vector2f(exitBounds.Left + exitBounds.Width / 2.0f, exitBounds.Top + exitBounds.Height / 2.0f);
                    exitText.Position = new Vector2f(ScreenWidth / 2.0f, ScreenHeight / 2.0f + 30);
                }

                UpdateMenuPositions();

                bool menuRunning = true;
                bool upPressed = false;
                bool downPressed = false;
                bool enterPressed = false;

                while (window.IsOpen && menuRunning)
                {
                    window.DispatchEvents();

                    // Simple input handling for menu
                    if (Keyboard.IsKeyPressed(Keyboard.Key.Up))
                    {
                        if (!upPressed)
                        {
                            selectedOption = 0;
                            upPressed = true;
                        }
                    }
                    else upPressed = false;

                    if (Keyboard.IsKeyPressed(Keyboard.Key.Down))
                    {
                        if (!downPressed)
                        {
                            selectedOption = 1;
                            downPressed = true;
                        }
                    }
                    else downPressed = false;

                    if (Keyboard.IsKeyPressed(Keyboard.Key.Enter))
                    {
                        if (!enterPressed)
                        {
                            if (selectedOption == 0)
                            {
                                startGame = true;
                                menuRunning = false;
                            }
                            else
                            {
                                window.Close();
                                menuRunning = false;
                            }
                            enterPressed = true;
                        }
                    }
                    else enterPressed = false;

                    // Highlight selected option
                    startText.FillColor = (selectedOption == 0) ? Color.Red : Color.White;
                    exitText.FillColor = (selectedOption == 1) ? Color.Red : Color.White;

                    window.Clear(Color.Black);
                    window.Draw(startText);
                    window.Draw(exitText);
                    window.Display();
                }

                // Handedness Selection Screen
                if (startGame && window.IsOpen)
                {
                    Text promptText = new Text(font, "select left handed or right handed", 30);
                    Text leftText = new Text(font, "left-handed", 40);
                    Text rightText = new Text(font, "right-handed", 40);
                    
                    int selectedHand = 0; // 0 for Left, 1 for Right
                    bool handednessRunning = true;
                    
                    void UpdateHandednessPositions()
                    {
                        FloatRect promptBounds = promptText.GetLocalBounds();
                        promptText.Origin = new Vector2f(promptBounds.Left + promptBounds.Width / 2.0f, promptBounds.Top + promptBounds.Height / 2.0f);
                        promptText.Position = new Vector2f(ScreenWidth / 2.0f, ScreenHeight / 2.0f - 100);

                        FloatRect leftBounds = leftText.GetLocalBounds();
                        leftText.Origin = new Vector2f(leftBounds.Left + leftBounds.Width / 2.0f, leftBounds.Top + leftBounds.Height / 2.0f);
                        leftText.Position = new Vector2f(ScreenWidth / 2.0f, ScreenHeight / 2.0f);

                        FloatRect rightBounds = rightText.GetLocalBounds();
                        rightText.Origin = new Vector2f(rightBounds.Left + rightBounds.Width / 2.0f, rightBounds.Top + rightBounds.Height / 2.0f);
                        rightText.Position = new Vector2f(ScreenWidth / 2.0f, ScreenHeight / 2.0f + 60);
                    }

                    UpdateHandednessPositions();
                    
                    while (window.IsOpen && handednessRunning)
                    {
                        window.DispatchEvents();

                        if (Keyboard.IsKeyPressed(Keyboard.Key.Up))
                        {
                            if (!upPressed)
                            {
                                selectedHand = 0;
                                upPressed = true;
                            }
                        }
                        else upPressed = false;

                        if (Keyboard.IsKeyPressed(Keyboard.Key.Down))
                        {
                            if (!downPressed)
                            {
                                selectedHand = 1;
                                downPressed = true;
                            }
                        }
                        else downPressed = false;

                        if (Keyboard.IsKeyPressed(Keyboard.Key.Enter))
                        {
                            if (!enterPressed)
                            {
                                isLeftHanded = (selectedHand == 0);
                                handednessRunning = false;
                                enterPressed = true;
                            }
                        }
                        else enterPressed = false;

                        leftText.FillColor = (selectedHand == 0) ? Color.Red : Color.White;
                        rightText.FillColor = (selectedHand == 1) ? Color.Red : Color.White;

                        window.Clear(Color.Black);
                        window.Draw(promptText);
                        window.Draw(leftText);
                        window.Draw(rightText);
                        window.Display();
                    }
                }
            }
            else
            {
                // If font is missing, just start the game
                startGame = true;
            }

            if (!startGame || !window.IsOpen) continue;

            int currentLevel = 1;
            int[,] currentWorldMap = GenerateProceduralLevelMap(currentLevel);

            // Reset Enemies
            Enemies.Clear();
            // Spawn a zombie
            Enemies.Add(CreateZombie(10, 10));

            double posX = 22, posY = 12; // x and y start position
            double dirX = -1, dirY = 0; // initial direction vector
            double planeX = 0, planeY = 0.66; // the 2d raycaster version of camera plane

            Clock clock = new Clock();
            float bobTimer = 0;
            float bobSpeed = 10.0f;
            float bobIntensity = 10.0f;
            float currentBobOffset = 0;

            int playerHealth = 5;
            bool hPressed = false;
            bool spacePressed = false;
            Clock damageClock = new Clock();
            float damageFlashTimer = 0;
            float damageFlashDuration = 0.5f;

            bool projectileActive = false;
            double projectileX = 0;
            double projectileY = 0;
            double projectileDirX = 0;
            double projectileDirY = 0;
            float projectileLifeTimer = 0;
            const float projectileLifetime = 3.0f;
            const double projectileSpeed = 9.0;
            float zombieSpawnTimer = 0;
            const float zombieSpawnInterval = 5.0f;
            const int zombieKillTarget = 5;
            int defeatedZombies = 0;

            Enemy CreateEnemyForCurrentLevel(double x, double y)
            {
                return currentLevel == 1 ? CreateZombie(x, y) : CreateLevelTwoZombie(x, y);
            }

            void LoadLevel(int level)
            {
                currentLevel = level;
                currentWorldMap = GenerateProceduralLevelMap(currentLevel);
                Enemies.Clear();
                Enemies.Add(CreateEnemyForCurrentLevel(10, 10));
                defeatedZombies = 0;
                zombieSpawnTimer = 0;
                projectileActive = false;
                projectileLifeTimer = 0;
                posX = 22;
                posY = 12;
                dirX = -1;
                dirY = 0;
                planeX = 0;
                planeY = 0.66;
                bobTimer = 0;
                currentBobOffset = 0;
            }

            LoadLevel(currentLevel);

            // Create a red border for the damage effect
            RectangleShape topBorder = new RectangleShape(new Vector2f(ScreenWidth, 20)) { FillColor = new Color(255, 0, 0, 0) };
            RectangleShape bottomBorder = new RectangleShape(new Vector2f(ScreenWidth, 20)) { FillColor = new Color(255, 0, 0, 0), Position = new Vector2f(0, ScreenHeight - 20) };
            RectangleShape leftBorder = new RectangleShape(new Vector2f(20, ScreenHeight)) { FillColor = new Color(255, 0, 0, 0) };
            RectangleShape rightBorder = new RectangleShape(new Vector2f(20, ScreenHeight)) { FillColor = new Color(255, 0, 0, 0), Position = new Vector2f(ScreenWidth - 20, 0) };

            double[] zBuffer = new double[ScreenWidth];

            bool backToMenu = false;
            while (window.IsOpen && !backToMenu)
            {
            window.DispatchEvents();

            window.Clear(Color.Black);

            // Raycasting logic
            for (int x = 0; x < ScreenWidth; x++)
            {
                // calculate ray position and direction
                double cameraX = 2 * x / (double)ScreenWidth - 1; // x-coordinate in camera space
                double rayDirX = dirX + planeX * cameraX;
                double rayDirY = dirY + planeY * cameraX;

                // which box of the map we're in
                int mapX = (int)posX;
                int mapY = (int)posY;

                // length of ray from current position to next x or y-side
                double sideDistX;
                double sideDistY;

                // length of ray from one x or y-side to next x or y-side
                double deltaDistX = Math.Abs(1 / rayDirX);
                double deltaDistY = Math.Abs(1 / rayDirY);
                double perpWallDist;

                // what direction to step in x or y-direction (either +1 or -1)
                int stepX;
                int stepY;

                int hit = 0; // was there a wall hit?
                int side = 0; // was a NS or a EW wall hit?

                // calculate step and initial sideDist
                if (rayDirX < 0)
                {
                    stepX = -1;
                    sideDistX = (posX - mapX) * deltaDistX;
                }
                else
                {
                    stepX = 1;
                    sideDistX = (mapX + 1.0 - posX) * deltaDistX;
                }
                if (rayDirY < 0)
                {
                    stepY = -1;
                    sideDistY = (posY - mapY) * deltaDistY;
                }
                else
                {
                    stepY = 1;
                    sideDistY = (mapY + 1.0 - posY) * deltaDistY;
                }

                // perform DDA
                while (hit == 0)
                {
                    // jump to next map square, either in x-direction, or in y-direction
                    if (sideDistX < sideDistY)
                    {
                        sideDistX += deltaDistX;
                        mapX += stepX;
                        side = 0;
                    }
                    else
                    {
                        sideDistY += deltaDistY;
                        mapY += stepY;
                        side = 1;
                    }
                    // Check if ray has hit a wall
                    if (currentWorldMap[mapX, mapY] > 0) hit = 1;
                }

                // Calculate distance projected on camera direction (Euclidean distance will give fisheye effect!)
                if (side == 0) perpWallDist = (sideDistX - deltaDistX);
                else          perpWallDist = (sideDistY - deltaDistY);

                zBuffer[x] = perpWallDist; // Fill Z-Buffer

                // Calculate height of line to draw on screen
                int lineHeight = (int)(ScreenHeight / perpWallDist);

                // calculate lowest and highest pixel to fill in current stripe
                int drawStart = (int)(-lineHeight / 2 + ScreenHeight / 2 + currentBobOffset);
                if (drawStart < 0) drawStart = 0;
                int drawEnd = (int)(lineHeight / 2 + ScreenHeight / 2 + currentBobOffset);
                if (drawEnd >= ScreenHeight) drawEnd = ScreenHeight - 1;

                // choose wall color
                Color color;
                switch (currentWorldMap[mapX, mapY])
                {
                    case 1: color = Color.Red; break;
                    case 2: color = Color.Green; break;
                    case 3: color = Color.Blue; break;
                    case 4: color = Color.White; break;
                    default: color = Color.Yellow; break;
                }

                // give x and y sides different brightness
                if (side == 1)
                {
                    color = new Color((byte)(color.R / 2), (byte)(color.G / 2), (byte)(color.B / 2));
                }

                // draw the pixels of the stripe as a vertical line
                Vertex[] line = new Vertex[2];
                line[0] = new Vertex(new Vector2f(x, drawStart), color);
                line[1] = new Vertex(new Vector2f(x, drawEnd), color);
                window.Draw(line, PrimitiveType.Lines);
            }

            // Sprite Rendering
            foreach (var enemy in Enemies)
            {
                // Relative position to player
                double spriteX = enemy.X - posX;
                double spriteY = enemy.Y - posY;

                // Transform with inverse camera matrix
                double invDet = 1.0 / (planeX * dirY - dirX * planeY);
                double transformX = invDet * (dirY * spriteX - dirX * spriteY);
                double transformY = invDet * (-planeY * spriteX + planeX * spriteY);

                if (transformY <= 0) continue; // Behind player

                int spriteScreenX = (int)((ScreenWidth / 2) * (1 + transformX / transformY));
                int spriteHeight = Math.Abs((int)(ScreenHeight / transformY));
                int spriteWidth = Math.Abs((int)(ScreenHeight / transformY));

                int drawStartX = -spriteWidth / 2 + spriteScreenX;
                int drawEndX = spriteWidth / 2 + spriteScreenX;

                for (int stripe = drawStartX; stripe < drawEndX; stripe++)
                {
                    if (stripe < 0 || stripe >= ScreenWidth) continue;
                    if (transformY >= zBuffer[stripe]) continue; // Occluded by wall

                    foreach (var segment in enemy.Segments)
                    {
                        // Check segment width and horizontal position
                        float relativeStripe = (stripe - drawStartX) / (float)spriteWidth - 0.5f;
                        if (Math.Abs(relativeStripe - segment.RelativeX) > segment.WidthScale / 2.0f) continue;

                        int segHeight = (int)(spriteHeight * segment.Height);
                        // Fix: transformY was being added to ScreenHeight/2 incorrectly, and eyes needed horizontal offset
                        int segCenterY = (int)(ScreenHeight / 2 + segment.RelativeY * spriteHeight + currentBobOffset);
                        
                        int dStart = -segHeight / 2 + segCenterY;
                        int dEnd = segHeight / 2 + segCenterY;

                        if (dStart < 0) dStart = 0;
                        if (dEnd >= ScreenHeight) dEnd = ScreenHeight - 1;
                        if (dStart >= dEnd) continue;

                        Vertex[] segLine = new Vertex[2];
                        segLine[0] = new Vertex(new Vector2f(stripe, dStart), segment.Color);
                        segLine[1] = new Vertex(new Vector2f(stripe, dEnd), segment.Color);
                        window.Draw(segLine, PrimitiveType.Lines);
                    }
                }
            }

            if (projectileActive)
            {
                double spriteX = projectileX - posX;
                double spriteY = projectileY - posY;
                double invDet = 1.0 / (planeX * dirY - dirX * planeY);
                double transformX = invDet * (dirY * spriteX - dirX * spriteY);
                double transformY = invDet * (-planeY * spriteX + planeX * spriteY);

                if (transformY > 0)
                {
                    int projectileScreenX = (int)((ScreenWidth / 2) * (1 + transformX / transformY));
                    int projectileRadius = Math.Max(2, (int)(10 / transformY));

                    if (projectileScreenX >= -projectileRadius && projectileScreenX < ScreenWidth + projectileRadius)
                    {
                        CircleShape projectileShape = new CircleShape(projectileRadius);
                        projectileShape.FillColor = Color.Yellow;
                        projectileShape.Position = new Vector2f(
                            projectileScreenX - projectileRadius,
                            ScreenHeight / 2.0f + currentBobOffset - projectileRadius);
                        window.Draw(projectileShape);
                    }
                }
            }

            // Render Arm
            Color skinColor = new Color(224, 172, 105); // More natural skin tone
            Color sleeveColor = new Color(50, 50, 150); // A sleeve color for better realism

            // Arm Parts: Upper Arm (2 rectangles), Forearm (1 rectangle), and Hand (Circle)
            RectangleShape bicepPart1 = new RectangleShape();
            RectangleShape bicepPart2 = new RectangleShape();
            RectangleShape forearmRect = new RectangleShape();
            
            bicepPart1.FillColor = sleeveColor;
            bicepPart2.FillColor = sleeveColor;
            forearmRect.FillColor = sleeveColor;

            CircleShape hand = new CircleShape(30);
            hand.FillColor = skinColor;
            hand.OutlineThickness = 2;
            hand.OutlineColor = new Color((byte)(skinColor.R - 30), (byte)(skinColor.G - 30), (byte)(skinColor.B - 30));

            float armBaseY = ScreenHeight - 100 + currentBobOffset;
            float bicepWidth = 150;
            float bicepHeight = 80;
            float forearmWidth = 120;
            float forearmHeight = 70;

            if (isLeftHanded)
            {
                // Upper Arm (Bicep) - Horizontal from left
                bicepPart1.Size = new Vector2f(bicepWidth, bicepHeight);
                bicepPart1.Position = new Vector2f(-30, armBaseY);
                
                bicepPart2.Size = new Vector2f(bicepWidth - 20, bicepHeight + 20);
                bicepPart2.Position = new Vector2f(-10, armBaseY - 10);
                
                // Forearm - Connected to bicep, angled slightly up/in
                forearmRect.Size = new Vector2f(forearmWidth, forearmHeight);
                forearmRect.Rotation = -20;
                forearmRect.Position = new Vector2f(bicepWidth - 40, armBaseY + 10);

                // Hand at the end of forearm
                hand.Position = new Vector2f(bicepWidth + forearmWidth - 80, armBaseY - 40);
            }
            else
            {
                // Upper Arm (Bicep) - Horizontal from right
                bicepPart1.Size = new Vector2f(bicepWidth, bicepHeight);
                bicepPart1.Position = new Vector2f(ScreenWidth - bicepWidth + 30, armBaseY);
                
                bicepPart2.Size = new Vector2f(bicepWidth - 20, bicepHeight + 20);
                bicepPart2.Position = new Vector2f(ScreenWidth - bicepWidth + 10, armBaseY - 10);
                
                // Forearm - Connected to bicep, angled slightly up/in
                forearmRect.Size = new Vector2f(forearmWidth, forearmHeight);
                forearmRect.Rotation = 20;
                // For right handed, we need to adjust position because rotation is around top-left
                forearmRect.Position = new Vector2f(ScreenWidth - bicepWidth + 40, armBaseY + 10);

                // Hand at the end of forearm
                hand.Position = new Vector2f(ScreenWidth - bicepWidth - forearmWidth + 20, armBaseY - 40);
            }

            window.Draw(bicepPart1);
            window.Draw(bicepPart2);
            window.Draw(forearmRect);
            window.Draw(hand);

            // Render Damage Flash
            if (damageFlashTimer > 0)
            {
                byte alpha = (byte)(255 * (damageFlashTimer / damageFlashDuration));
                Color flashColor = new Color(255, 0, 0, alpha);
                topBorder.FillColor = flashColor;
                bottomBorder.FillColor = flashColor;
                leftBorder.FillColor = flashColor;
                rightBorder.FillColor = flashColor;
                window.Draw(topBorder);
                window.Draw(bottomBorder);
                window.Draw(leftBorder);
                window.Draw(rightBorder);
            }

            // Render Health
            float healthSquareSize = 20;
            float healthSpacing = 5;
            for (int i = 0; i < playerHealth; i++)
            {
                RectangleShape healthSquare = new RectangleShape(new Vector2f(healthSquareSize, healthSquareSize));
                healthSquare.FillColor = Color.Red;
                healthSquare.OutlineColor = Color.White;
                healthSquare.OutlineThickness = 1;
                healthSquare.Position = new Vector2f(10 + i * (healthSquareSize + healthSpacing), 10);
                window.Draw(healthSquare);
            }

            if (font != null)
            {
                string objectiveText = currentLevel == 1
                    ? $"derrote 5 inimigos: {defeatedZombies}/{zombieKillTarget}"
                    : $"derrote 5 inimigos para prosseguir: {defeatedZombies}/{zombieKillTarget}";
                Text scoreText = new Text(font, objectiveText, 28);
                scoreText.FillColor = Color.White;
                FloatRect scoreBounds = scoreText.GetLocalBounds();
                scoreText.Origin = new Vector2f(scoreBounds.Left + scoreBounds.Width / 2.0f, scoreBounds.Top + scoreBounds.Height / 2.0f);
                scoreText.Position = new Vector2f(ScreenWidth / 2.0f, 30);
                window.Draw(scoreText);
            }

            window.Display();

            // Timing for input and FPS counter
            float frameTime = clock.Restart().AsSeconds();
            bool isMoving = false;

            zombieSpawnTimer += frameTime;
            if (zombieSpawnTimer >= zombieSpawnInterval)
            {
                Enemies.Add(CreateEnemyForCurrentLevel(10, 10));
                zombieSpawnTimer = 0;
            }

            if (projectileActive)
            {
                projectileLifeTimer += frameTime;
                if (projectileLifeTimer >= projectileLifetime)
                {
                    projectileActive = false;
                }
                else
                {
                    projectileX += projectileDirX * projectileSpeed * frameTime;
                    projectileY += projectileDirY * projectileSpeed * frameTime;

                    int projectileMapX = (int)projectileX;
                    int projectileMapY = (int)projectileY;
                    if (projectileMapX <= 0 || projectileMapX >= MapWidth - 1 || projectileMapY <= 0 || projectileMapY >= MapHeight - 1 || currentWorldMap[projectileMapX, projectileMapY] > 0)
                    {
                        projectileActive = false;
                    }
                    else
                    {
                        for (int i = Enemies.Count - 1; i >= 0; i--)
                        {
                            double enemyDx = Enemies[i].X - projectileX;
                            double enemyDy = Enemies[i].Y - projectileY;
                            double enemyDist = Math.Sqrt(enemyDx * enemyDx + enemyDy * enemyDy);
                            if (enemyDist < 0.45)
                            {
                                Enemies[i].Health -= 1;
                                if (Enemies[i].Health <= 0)
                                {
                                    Enemies.RemoveAt(i);
                                    defeatedZombies++;
                                }
                                projectileActive = false;
                                break;
                            }
                        }
                    }
                }
            }

            // Update Enemies and check for damage
            foreach (var enemy in Enemies)
            {
                enemy.Update(posX, posY, frameTime, currentWorldMap);
                
                double dx = posX - enemy.X;
                double dy = posY - enemy.Y;
                double dist = Math.Sqrt(dx * dx + dy * dy);
                if (dist < 0.6) // Collision distance
                {
                    if (damageClock.ElapsedTime.AsSeconds() >= 1.0)
                    {
                        if (playerHealth > 0)
                        {
                            playerHealth--;
                            damageFlashTimer = damageFlashDuration;
                        }
                        damageClock.Restart();
                    }
                }
            }

            if (damageFlashTimer > 0)
            {
                damageFlashTimer -= frameTime;
                if (damageFlashTimer < 0) damageFlashTimer = 0;
            }

            if (playerHealth <= 0)
            {
                if (font != null)
                {
                    Text gameOverText = new Text(font, "GAME OVER", 80);
                    gameOverText.FillColor = Color.Red;
                    FloatRect goRect = gameOverText.GetLocalBounds();
                    gameOverText.Origin = new Vector2f(goRect.Left + goRect.Width / 2.0f, goRect.Top + goRect.Height / 2.0f);
                    gameOverText.Position = new Vector2f(ScreenWidth / 2.0f, ScreenHeight / 2.0f);
                    
                    window.Clear(Color.Black);
                    window.Draw(gameOverText);
                    window.Display();
                    
                    Clock endClock = new Clock();
                    while (endClock.ElapsedTime.AsSeconds() < 3.0)
                    {
                        window.DispatchEvents();
                    }
                }
                backToMenu = true;
                break;
            }

            if (defeatedZombies >= zombieKillTarget)
            {
                if (font != null)
                {
                    Text winText = new Text(font, "PARABENS", 70);
                    winText.FillColor = Color.Green;
                    FloatRect winRect = winText.GetLocalBounds();
                    winText.Origin = new Vector2f(winRect.Left + winRect.Width / 2.0f, winRect.Top + winRect.Height / 2.0f);
                    winText.Position = new Vector2f(ScreenWidth / 2.0f, ScreenHeight / 2.0f);

                    window.Clear(Color.Black);
                    window.Draw(winText);
                    window.Display();

                    Clock winClock = new Clock();
                    while (winClock.ElapsedTime.AsSeconds() < 3.0)
                    {
                        window.DispatchEvents();
                    }
                }

                if (currentLevel == 1)
                {
                    LoadLevel(2);
                    continue;
                }

                backToMenu = true;
                break;
            }

            // speed modifiers
            double moveSpeed = frameTime * 5.0; // the constant value is in squares/second
            double rotSpeed = frameTime * 3.0; // the constant value is in radians/second

            // move forward if no wall in front of you
            if (Keyboard.IsKeyPressed(Keyboard.Key.W))
            {
                if (currentWorldMap[(int)(posX + dirX * moveSpeed), (int)posY] == 0) posX += dirX * moveSpeed;
                if (currentWorldMap[(int)posX, (int)(posY + dirY * moveSpeed)] == 0) posY += dirY * moveSpeed;
                isMoving = true;
            }
            // move backwards if no wall behind you
            if (Keyboard.IsKeyPressed(Keyboard.Key.S))
            {
                if (currentWorldMap[(int)(posX - dirX * moveSpeed), (int)posY] == 0) posX -= dirX * moveSpeed;
                if (currentWorldMap[(int)posX, (int)(posY - dirY * moveSpeed)] == 0) posY -= dirY * moveSpeed;
                isMoving = true;
            }

            // Update bobbing
            if (isMoving)
            {
                bobTimer += frameTime * bobSpeed;
                currentBobOffset = (float)Math.Sin(bobTimer) * bobIntensity;
            }
            else
            {
                // Smoothly return to center
                bobTimer = 0;
                currentBobOffset *= 0.9f;
                if (Math.Abs(currentBobOffset) < 0.1f) currentBobOffset = 0;
            }
            // rotate to the right
            if (Keyboard.IsKeyPressed(Keyboard.Key.D))
            {
                // both camera direction and camera plane must be rotated
                double oldDirX = dirX;
                dirX = dirX * Math.Cos(-rotSpeed) - dirY * Math.Sin(-rotSpeed);
                dirY = oldDirX * Math.Sin(-rotSpeed) + dirY * Math.Cos(-rotSpeed);
                double oldPlaneX = planeX;
                planeX = planeX * Math.Cos(-rotSpeed) - planeY * Math.Sin(-rotSpeed);
                planeY = oldPlaneX * Math.Sin(-rotSpeed) + planeY * Math.Cos(-rotSpeed);
            }
            // rotate to the left
            if (Keyboard.IsKeyPressed(Keyboard.Key.A))
            {
                // both camera direction and camera plane must be rotated
                double oldDirX = dirX;
                dirX = dirX * Math.Cos(rotSpeed) - dirY * Math.Sin(rotSpeed);
                dirY = oldDirX * Math.Sin(rotSpeed) + dirY * Math.Cos(rotSpeed);
                double oldPlaneX = planeX;
                planeX = planeX * Math.Cos(rotSpeed) - planeY * Math.Sin(rotSpeed);
                planeY = oldPlaneX * Math.Sin(rotSpeed) + planeY * Math.Cos(rotSpeed);
            }

            // Damage test
            if (Keyboard.IsKeyPressed(Keyboard.Key.H))
            {
                if (!hPressed)
                {
                    if (playerHealth > 0)
                    {
                        playerHealth--;
                        damageFlashTimer = damageFlashDuration;
                    }
                    hPressed = true;
                }
            }
            else hPressed = false;

            if (Keyboard.IsKeyPressed(Keyboard.Key.Space))
            {
                if (!spacePressed && !projectileActive)
                {
                    projectileActive = true;
                    projectileLifeTimer = 0;
                    projectileDirX = dirX;
                    projectileDirY = dirY;
                    projectileX = posX + dirX * 0.5;
                    projectileY = posY + dirY * 0.5;
                }
                spacePressed = true;
            }
            else
            {
                spacePressed = false;
            }
            }
        }
    }
}