namespace SpaceShip

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Input
open SpaceShip.Types

type Controller(texture) =
    let texture = texture
    let mutable timer = Timer 2.0
    let mutable maxTime = 2.0
    let mutable asteroids = List.empty
    let mutable nextSpeed = 250f;
    let mutable gameState = Menu
    let mutable totalTime: double = 0.0

    member __.update(gameTime: GameTime, player: outref<Ship>) =
        if gameState = Running then
            timer <- Timer.tick gameTime timer
            totalTime <- totalTime + gameTime.ElapsedGameTime.TotalSeconds
        else
            let kState = Keyboard.GetState()
            if kState.IsKeyDown(Keys.Enter) then
                gameState <- Running
                totalTime <- 0.0
                nextSpeed <- 250f
                timer <- Timer 2.0
                maxTime <- 2.0

        if Timer.isElapsed timer then
            asteroids <- (Asteroid.create texture nextSpeed) :: asteroids
            timer <- Timer maxTime
            maxTime <-  if maxTime > 0.5 then maxTime - 0.1 else maxTime
            nextSpeed <- if nextSpeed < 720f then nextSpeed + 4f else nextSpeed
        
        asteroids <- List.choose (fun x -> if x.position.X > -x.radius then Some (Asteroid.update gameTime x) else None) asteroids
        for asteroid in asteroids do
            let sum = asteroid.radius + player.radius
            if Vector2.Distance(asteroid.position, player.position) < sum then
                gameState <- Menu
                player <- { player with position = Vector2(1280f/2f, 720f/2f) }
                asteroids <- List.empty

    member __.getAsteroids() = asteroids
    member __.getState() = gameState
    member __.getTime() = totalTime

