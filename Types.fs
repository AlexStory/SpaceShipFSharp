namespace SpaceShip.Types

open System
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input

type Ship = { position: Vector2 ; texture: Texture2D; speed: float32; radius: float32 }
type Asteroid = { position: Vector2 ; texture: Texture2D; speed: float32; radius: float32 }
type Timer = Timer of double
type GameState = Running | Menu

module Ship =
    let create (game: Game): Ship = {
        texture = game.Content.Load "ship"
        position = Vector2(1280f/2f, 720f/2f)
        speed = 180f
        radius = 30f}

    let draw (spriteBatch: SpriteBatch) (ship: Ship) =
        spriteBatch.Draw (ship.texture, Vector2(ship.position.X - float32 ship.texture.Width / 2f, ship.position.Y - float32 ship.texture.Height / 2f), Color.White)

    let private getMovement () =
        let keyboard = Keyboard.GetState()
        let keydown k = if keyboard.IsKeyDown k then 1f else 0f
        let r = keydown Keys.Right
        let l = keydown Keys.Left
        let u = keydown Keys.Up
        let d = keydown Keys.Down
        Vector2(r-l, d-u)

    let update (gameTime: GameTime) (ship: Ship) =
        let position = ship.position + getMovement() * ship.speed * float32 gameTime.ElapsedGameTime.TotalSeconds
        let minXClamp = float32 (ship.texture.Width/2)
        let maxXClamp = 1280f - float32 (ship.texture.Width/2)
        let minYClamp = float32 (ship.texture.Height/2)
        let maxYClamp = 720f - float32 (ship.texture.Height/2)
        let position = 
            Vector2(
                Math.Clamp(position.X, minXClamp, maxXClamp),
                Math.Clamp(position.Y, minYClamp, maxYClamp))
        { ship with 
            position = position
        }

module Asteroid =
    let create (texture: Texture2D) speed = {
        texture = texture
        position = 
            Vector2(
                1380f,
                float32 (Random().Next(texture.Height/2, 720 - texture.Height/2))
            )
        speed = speed
        radius = float32 (texture.Width / 2) }

    let draw (spriteBatch: SpriteBatch) asteroid = 
        spriteBatch.Draw (asteroid.texture, Vector2(asteroid.position.X - asteroid.radius, asteroid.position.Y - asteroid.radius), Color.White)

    let update (gameTime: GameTime) (asteroid: Asteroid) =
        { asteroid with
            position = asteroid.position - Vector2(asteroid.speed, 0f) * float32 gameTime.ElapsedGameTime.TotalSeconds
        }

module Timer =
    let tick (gameTime: GameTime) (Timer t) =
        t - gameTime.ElapsedGameTime.TotalSeconds |> Timer
    
    let isElapsed (Timer t) =
        t <= 0.0
