using System;
using ActorModule;
using InitializeActorModule;
using NetTopologySuite.Geometries;
using PathsFindingCoreModule;
using NetTopologySuite.Mathematics;
using System.Threading.Tasks;

namespace ActorHandlerModuleHunger
{
    public class MovementActivityHunger : IActivity
    {
        //Координаты пути до точки общепита
        public Coordinate[] Path;
        public int i = 0;
        //Флаг-путь построен.
        public bool IsPath = true;
        //Время на обновление
        public double TimeUpdate = 0;

        public bool IsCompleted { get; set; }
        //Точка питания
        public Point HungerPoint { get; set; }

        // Приоритет делаем авто-свойством, со значением по умолчанию
        public int Priority { get; set; } = 0;
        //Интервал врмени на питание
        public TimeInterval HungerTime { get; set; }

        public MovementActivityHunger(int priority, Point hungerPoint)
        {
            Priority = priority;
            HungerPoint = hungerPoint;
        }

        // Здесь происходит работа с актором
        public bool Update(Actor actor, double deltaTime)
        {
            TimeUpdate += deltaTime;
            // Расстояние, которое может пройти актор с заданной скоростью за прошедшее время
            double distance = actor.GetState<SpecState>().Speed * deltaTime;
            //Уменьшаем статы акторы раз в секунду
            if (TimeUpdate >= 1)
            {
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
                //Настроение
                if (actor.GetState<SpecState>().Mood <= 0.1)
                    actor.GetState<SpecState>().Mood = 0;
                else
                    actor.GetState<SpecState>().Mood -= 0.0001 * 100;

                TimeUpdate -= 1;
            }

            //Вывод состояний актора
            //Console.WriteLine($"Health: {actor.GetState<SpecState>().Health}; Hunger: {actor.GetState<SpecState>().Satiety}; Fatigue: {actor.GetState<SpecState>().Stamina}; Mood: {actor.GetState<SpecState>().Mood}");
            
            if (IsPath)
            {
                var firstCoordinate = new Coordinate(actor.X, actor.Y);
                var secondCoordinate = new Coordinate(HungerPoint.X, HungerPoint.Y);
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

            if (actor.X == Path[i].X && actor.Y == Path[i].Y && i < Path.Length - 1)
            {
                i++;
                //Console.WriteLine(i);
                //Console.WriteLine(Path.Length);
            }

            // Если в процессе шагания мы достигли точки назначения
            if (actor.X == Path[Path.Length - 1].X && actor.Y == Path[Path.Length - 1].Y)
            {
                //Установка времени на принятие пищи
                HungerTime = new TimeInterval(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second,
                    Convert.ToInt32(DateTime.Now.AddHours(0)),
                    Convert.ToInt32(DateTime.Now.AddMinutes(0)),
                    Convert.ToInt32(DateTime.Now.AddSeconds(10)));

                Console.WriteLine("Start Waiting");

                //Присваиваем приоритет в зависимости от сытости
                //Если сытость [100-80)% то приоритет
                if ((actor.GetState<SpecState>().Satiety <= 100) && (actor.GetState<SpecState>().Satiety > (0.8 * 100)))
                    Priority = 4;
                else
                //Если сытость [80-60)% то приоритет
                if ((actor.GetState<SpecState>().Satiety <= (0.8 * 100)) && (actor.GetState<SpecState>().Satiety > (0.6 * 100)))
                    Priority = 24;
                else
                //Если сытость [60-40)% то приоритет
                if ((actor.GetState<SpecState>().Satiety <= (0.6 * 100)) && (actor.GetState<SpecState>().Satiety > (0.4 * 100)))
                    Priority = 44;
                else
                //Если сытость [40-20)% то приоритет
                if ((actor.GetState<SpecState>().Satiety <= (0.4 * 100)) && (actor.GetState<SpecState>().Satiety > (0.2 * 100)))
                    Priority = 64;
                else
                //Если сытость [20-5)% то приоритет
                if ((actor.GetState<SpecState>().Satiety <= (0.2 * 100)) && (actor.GetState<SpecState>().Satiety > (0.05 * 100)))
                    Priority = 84;
                else
                //Если сытость <=5% то приоритет
                if (actor.GetState<SpecState>().Satiety <= (0.05 * 100))
                    Priority = 94;

                i = 0;

                IsPath = true;

                //Запуск ожидания
                actor.Activity = new WaitingActivityHunger(Priority, HungerTime);
            }

            return false;
        }
    }
}
        
