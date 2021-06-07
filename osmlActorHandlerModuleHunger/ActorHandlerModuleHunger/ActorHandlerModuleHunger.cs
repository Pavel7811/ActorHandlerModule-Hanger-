using System;
using OSMLSGlobalLibrary.Modules;
using NetTopologySuite.Geometries;
using System.Linq;
using CityDataExpansionModule.OsmGeometries;
using System.Collections.Generic;
using ActorModule;
using InitializeActorModule;

namespace ActorHandlerModuleHunger
{
    public class ActorHandlerModuleHunger : OSMLSModule
    {
        //Точка общепита
        public Point HungerPoint { get; set; }
        public List<OsmClosedWay> AmenityFastFood { get; set; }
        public double TimeUpdate = 0;
        /// <summary>
        /// Инициализация модуля. В отладочной конфигурации выводит сообщение
        /// </summary>
        protected override void Initialize()
        {
            //Вывод сообщения о инициализации модуля
            Console.WriteLine("ActorHandlerModuleHungere: Initialize");
        }

        /// <summary>
        /// Вызывает Update на всех акторах
        /// </summary>
        public override void Update(long elapsedMilliseconds)
        {
            TimeUpdate += elapsedMilliseconds;
            //Парсинг объектов с карты с тегом shop 
            List<OsmClosedWay> AmenityFastFood = MapObjects.GetAll<OsmClosedWay>().Where(x => x.Tags.ContainsKey("shop")).ToList();

            //Получаем список всех акторов
            var actors = MapObjects.GetAll<Actor>();
            //Вывод акторов
            Console.WriteLine($"Got {actors.Count} actors\n");

            if (TimeUpdate >= 1) 
            {
                TimeUpdate -= 1;
                // Для каждого актора проверяем условия и назначаем новую активность если нужно
                foreach (var actor in actors)
                {
                    int newPriority = 0;

                    //Присваиваем приоритет в зависимости от сытости
                    //Если сытость [100-80)% то приоритет
                    if ((actor.GetState<SpecState>().Satiety <= 100) && (actor.GetState<SpecState>().Satiety > (0.8 * 100)))
                        newPriority = 4;
                    else
                    //Если сытость [80-60)% то приоритет
                    if ((actor.GetState<SpecState>().Satiety <= (0.8 * 100)) && (actor.GetState<SpecState>().Satiety > (0.6 * 100)))
                        newPriority = 24;
                    else
                    //Если сытость [60-40)% то приоритет
                    if ((actor.GetState<SpecState>().Satiety <= (0.6 * 100)) && (actor.GetState<SpecState>().Satiety > (0.4 * 100)))
                        newPriority = 44;
                    else
                    //Если сытость [40-20)% то приоритет
                    if ((actor.GetState<SpecState>().Satiety <= (0.4 * 100)) && (actor.GetState<SpecState>().Satiety > (0.2 * 100)))
                        newPriority = 64;
                    else
                    //Если сытость [20-5)% то приоритет
                    if ((actor.GetState<SpecState>().Satiety <= (0.2 * 100)) && (actor.GetState<SpecState>().Satiety > (0.05 * 100)))
                        newPriority = 84;
                    else
                    //Если сытость <=5% то приоритет
                    if (actor.GetState<SpecState>().Satiety <= (0.05 * 100))
                        newPriority = 94;


                    //Console.WriteLine("Назначение места употребления пищи");

                    HungerPoint = new Point(AmenityFastFood[1].Coordinate);

                    //Console.WriteLine("Место употребления пищи назначено");

                    //Console.WriteLine($"Координаты: {HungerPoint} ");

                    Console.WriteLine($"Приоритет: {newPriority} ");

                    //Проверка наличия активности
                    bool isActivity = actor.Activity != null;
                    //Наша ли активность
                    bool isHungerMovementActivity = actor.Activity is MovementActivityHunger;
                    bool isHungerWaitingActivity = actor.Activity is WaitingActivityHunger;


                    Console.WriteLine($"Flags: {isActivity} {isHungerMovementActivity} {isHungerWaitingActivity}");

                    //Если нет активности или активность не относится к голоду и приоритет активностей голода больше текущего приоритета
                    //Назначаем новую активность
                    if ((!isActivity) || (!isHungerMovementActivity && !isHungerWaitingActivity && newPriority > actor.Activity.Priority))
                    {
                        // Назначить актору путь до точки общепита
                        actor.Activity = new MovementActivityHunger(newPriority, HungerPoint);
                        Console.WriteLine("Said actor go Hunger\n");
                    }
                }
            }
        }
    }
}
