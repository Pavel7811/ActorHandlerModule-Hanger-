using System;
using ActorModule;
using OSMLSGlobalLibrary;
using NetTopologySuite.Geometries;  // Отсюда Point и другая геометрия
using NetTopologySuite.Mathematics; // Отсюда векторы
using NodaTime;
using PathsFindingCoreModule;
using InitializeActorModule;

namespace ActorHandlerModule
{

    public class MovementActivityHunger : IActivity
    {
        public Coordinate[] Path;
        public bool IsPath = true;
        public int i = 0;
        public int Priority { get; set; } = 0;
        public TimeInterval HungerTime { get; set; }
        // Точка назначения

        public MovementActivityHunger(int priority, TimeInterval hungerTime)
        {
            Priority = priority;
            HungerTime = hungerTime;
        }

        // Здесь происходит работа с актором
        public bool Update(Actor actor, double deltaTime)
        {

            // Расстояние, которое может пройти актор с заданной скоростью за прошедшее время
            double distance = actor.GetState<SpecState>().Speed * deltaTime;

            Console.WriteLine($"Health: {actor.GetState<SpecState>().Health}; Hunger: {actor.GetState<SpecState>().Hunger}; Fatigue: {actor.GetState<SpecState>().Fatigue}; Mood: {actor.GetState<SpecState>().Mood}");


            //Если путь еще не построен
            if (IsPath)
            {
                //Начальные координаты и координаты точки работы
                var firstCoordinate = new Coordinate(actor.X, actor.Y);
                var secondCoordinate = new Coordinate(actor.GetState<PlaceState>().Home.X, actor.GetState<PlaceState>().Home.Y);
                //Строим путь
                Path = PathsFinding.GetPath(firstCoordinate, secondCoordinate, "Walking").Result.Coordinates;
                IsPath = false;
            }

            Vector2D direction = new Vector2D(actor.Coordinate, Path[i]);
            // Проверка на перешагивание
            if (direction.Length() <= distance)
            {
                // Шагаем в точку, если она ближе, чем расстояние которое можно пройти
                actor.X = Path[i].X;
                actor.Y = Path[i].Y;
            }
            else
            {
                // Вычисляем новый вектор, с направлением к точке назначения и длинной в distance
                direction = direction.Normalize().Multiply(distance);

                // Смещаемся по вектору
                actor.X += direction.X;
                actor.Y += direction.Y;
            }
            //Если актор достиг следующей точки пути
            if (actor.X == Path[i].X && actor.Y == Path[i].Y && i < Path.Length - 1)
            {
                i++;
               
            }

            // Если в процессе шагания мы достигли точки назначения
            if (actor.X == Path[Path.Length - 1].X && actor.Y == Path[Path.Length - 1].Y)
            {
                Console.WriteLine("Start Waiting");
                Priority = 0;
                i = 0;
                IsPath = true;
                //Запуск активити ожидания(имитация поглощения пищи)
                actor.Activity = new WaitingActivityHunger(actor, Priority, HungerTime);
            }
            return false;
        }
    }
}