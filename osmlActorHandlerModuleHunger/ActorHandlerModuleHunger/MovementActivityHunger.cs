using System;
using ActorModule;
using InitializeActorModule;
using CityDataExpansionModule.OsmGeometries;
using PathsFindingCoreModule;
using NetTopologySuite.Geometries;
using NetTopologySuite.Mathematics;
using System.Collections.Generic;
using OSMLSGlobalLibrary.Map;
using NodaTime;

namespace ActorHandlerModuleHunger
{
    public class MovementActivityHunger : IActivity
    {
        //Точка в которой актор пополнит свою сытость
        public Point HungerPoint;
        public Coordinate[] HungerCoordinate;
        private double TimeUpdate = 0;
        //Проверка есть ли путь
        public bool IsPath = true;

        //Время на обновление параметров
        //private double TimeUpdate { get; set; }

        public int i = 0;
        
        public int Priority { get; set; } = 0;
        //Интервал времени на принятие пищи
        public TimeInterval HungerTime { get; set; }
         
        public MovementActivityHunger(int priority, Point hungerPoint)
        {
            Priority = priority;
            //HungerTime = hungerTime;
            HungerPoint = hungerPoint;
        }

        //Работа с катором
        public bool Update(Actor actor, double deltaTime)
        {
            TimeUpdate += deltaTime;

            //Уменьшаем статы акторы
            if (TimeUpdate >= 1)
            {
                TimeUpdate -= 1;

                //Голод
                if (actor.GetState<SpecState>().Satiety <= 0.1) 
                    actor.GetState<SpecState>().Satiety = 0;
                else 
                    actor.GetState<SpecState>().Satiety -= 0.001 * 100;
                //Усталость
                if (actor.GetState<SpecState>().Stamina <= 0.1) 
                    actor.GetState<SpecState>().Stamina = 0;
                else 
                    actor.GetState<SpecState>().Stamina -= 0.001 * 100;
                //Настроение(Падает медленее, чем другие)
                if (actor.GetState<SpecState>().Mood <= 0.1) 
                    actor.GetState<SpecState>().Mood = 0;
                else 
                    actor.GetState<SpecState>().Mood -= 0.0001 * 100;
            }

            // Расстояние, которое может пройти актор с заданной скоростью за прошедшее время
            double distance = actor.GetState<SpecState>().Speed * deltaTime;

            //Вывод в консоль состояний актора
            Console.WriteLine($"Health: {actor.GetState<SpecState>().Health}; " +
                $"Satiety: {actor.GetState<SpecState>().Satiety}; " +
                $"Stamina: {actor.GetState<SpecState>().Stamina}; " +
                $"Mood: {actor.GetState<SpecState>().Mood}");


            //Если путь еще не построен
            if (IsPath)
            {
                //Начальные координаты и координаты точки общепита
                var firstCoordinate = new Coordinate(actor.X, actor.Y);

                var secondCoordinate = new Coordinate(HungerPoint.X, HungerPoint.Y);
                //Строим путь
                HungerCoordinate = PathsFinding.GetPath(firstCoordinate, secondCoordinate, "Walking").Result.Coordinates;
                IsPath = false;
            }

            Vector2D direction = new Vector2D(actor.Coordinate, HungerCoordinate[i]);
            // Проверка на перешагивание
            if (direction.Length() <= distance)
            {
                // Шагаем в точку, если она ближе, чем расстояние которое можно пройти
                actor.X = HungerCoordinate[i].X;
                actor.Y = HungerCoordinate[i].Y;
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
            if (actor.X == HungerCoordinate[i].X && actor.Y == HungerCoordinate[i].Y && i < HungerPoint.Length - 1)
            {
                i++;
            }

            // Если в процессе шагания мы достигли точки назначения
            if (actor.X == HungerCoordinate[HungerCoordinate.Length - 1].X && actor.Y == HungerCoordinate[HungerCoordinate.Length - 1].Y)
            {
                string NowTime = DateTime.Now.ToString("HH:mm:ss");
                string EndTime = DateTime.Now.AddMinutes(10).ToString("HH:mm:ss");

                Console.WriteLine("Временной интервал");
                Console.WriteLine("NOW:  " + NowTime);
                Console.WriteLine("NEED:  " + EndTime);

                HungerTime = new TimeInterval(DateTime.Now.Hour, DateTime.Now.Minute, Convert.ToInt32(DateTime.Now.AddHours(0)), Convert.ToInt32(DateTime.Now.AddMinutes(10)));

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
