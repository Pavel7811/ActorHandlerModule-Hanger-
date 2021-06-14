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
        public Point HungerPoint { get; set; }
        public List<OsmClosedWay> HungerPlace { get; set; }
        public double TimeUpdate = 0;
        public int i = 2;

        protected override void Initialize()
        {
            Console.WriteLine("ActorHandlerModuleHunger: Initialize");
        }

        public override void Update(long elapsedMilliseconds)
        {
            TimeUpdate += elapsedMilliseconds / 1000;
            //Парсинг объектов с карты с тегом shop 
            HungerPlace = MapObjects.GetAll<OsmClosedWay>().Where(x => x.Tags.ContainsKey("shop")).ToList();
            //Получаем список всех акторов
            var actors = MapObjects.GetAll<Actor>();
            //Вывод акторов
            //Console.WriteLine($"Got {actors.Count} actors\n");

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

                    HungerPoint = new Point(HungerPlace[0].Coordinate);
                    
                    //Проверка наличия активности
                    bool isActivity = actor.Activity != null;
                    //Наша ли активность
                    bool isHungerMovementActivity = actor.Activity is MovementActivityHunger;
                    bool isHungerWaitingActivity = actor.Activity is WaitingActivityHunger;

                    //Console.WriteLine($"Приоритет актора: {actor.Activity.Priority}\n");
                    //Console.WriteLine($"Flags: {isActivity} {isHungerMovementActivity} {isHungerWaitingActivity}");

                    //Если нет активности или активность не относится к голоду и приоритет активностей голода больше текущего приоритета
                    //Назначаем новую активность
                    if ((!isActivity) || (!isHungerMovementActivity && !isHungerWaitingActivity && (newPriority > actor.Activity.Priority)))
                    {
                        // Назначить актору путь до точки общепита
                        actor.Activity = new MovementActivityHunger(newPriority, HungerPoint);
                        ///Console.WriteLine($"Time Now: {DateTime.Now}\n");
                        Console.WriteLine("Said actor go Hunger\n");
                        //
                    }
                }
            }
        }
    }
}
