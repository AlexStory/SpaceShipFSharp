namespace SpaceShip

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input
open SpaceShip.Types

type Game1() as this =
    inherit Game ()
    let graphics = new GraphicsDeviceManager(this)
    let mutable spriteBatch = Unchecked.defaultof<_>

    let mutable asteroidSprite: Texture2D = Unchecked.defaultof<_>
    let mutable spaceSprite: Texture2D = Unchecked.defaultof<_>
    let mutable gameFont: SpriteFont = Unchecked.defaultof<_>
    let mutable timerFont: SpriteFont = Unchecked.defaultof<_>

    let mutable player = Unchecked.defaultof<_>
    let mutable controller = Unchecked.defaultof<_>
    

    do
        this.Content.RootDirectory <- "Content"
        this.IsMouseVisible <- true

    override _.Initialize () =
        graphics.PreferredBackBufferWidth <- 1280
        graphics.PreferredBackBufferHeight <- 720
        graphics.ApplyChanges()
        base.Initialize ()

    override _.LoadContent () =
        spriteBatch <- new SpriteBatch (this.GraphicsDevice)
        asteroidSprite <- this.Content.Load "asteroid"
        spaceSprite <- this.Content.Load "space"
        gameFont <- this.Content.Load "spaceFont"
        timerFont <- this.Content.Load "timerFont"
        player <- Ship.create this
        controller <- Controller (asteroidSprite)

    override _.Update (gameTime) =
        if GamePad.GetState(PlayerIndex.One).Buttons.Back = ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape) then
            this.Exit()

        if controller.getState() = Running then
            player <- Ship.update gameTime player
        controller.update(gameTime, &player)

        base.Update(gameTime)

    override _.Draw (gameTime) =
        this.GraphicsDevice.Clear(Color.CornflowerBlue)

        spriteBatch.Begin()
        spriteBatch.Draw(spaceSprite, Vector2.Zero, Color.White)
        Ship.draw spriteBatch player
        List.iter (Asteroid.draw spriteBatch) (controller.getAsteroids())

        if controller.getState() = Menu then
            let menuMessage = "Press ENTER to begin!"
            let textSize = gameFont.MeasureString menuMessage
            let halfWidth = graphics.PreferredBackBufferWidth / 2
            spriteBatch.DrawString(
                gameFont, 
                menuMessage, 
                Vector2(float32 halfWidth - textSize.X/2f, 720f/2f - textSize.X/2f),
                Color.White)

        spriteBatch.DrawString(timerFont, $"Time: %.1f{controller.getTime()}", Vector2(3f), Color.White)
        spriteBatch.End()

        base.Draw(gameTime)