using System;
using OSMLSGlobalLibrary.Modules;
using ActorHandlerModuleHunger;
using ActorModule;
using CityDataExpansionModule.OsmGeometries;
using NetTopologySuite.Geometries;
using InitializeActorModule;
using System.Collections.Generic;
using System.Linq;

namespace ActorHandlerTestModuleHunger
{
    public class ActorHandlerTestModuleHunger : OSMLSModule
    {
        public Point HungerPoint { get; set; }
        public List<OsmClosedWay> AmenityFastFood { get; set; }

        protected override void Initialize()
        {
        }

        int count = 0;
        public override void Update(long elapsedMilliseconds)
        {
            //Парсинг объектов с карты с тегом shop 
            List<OsmClosedWay> AmenityFastFood = MapObjects.GetAll<OsmClosedWay>().Where(x => x.Tags.ContainsKey("shop")).ToList();

            var actors = MapObjects.GetAll<Actor>();
            foreach (var actor in actors)
            {

                bool isActivity = actor.Activity != null;

                bool goActivity = isActivity ? actor.Activity is MovementActivityHunger : false;
                bool timefal = isActivity ? actor.Activity is WaitingActivityHunger : false;



                //Console.WriteLine($"Flags: {isActivity} {goActivity} {timefal}");

                if (!isActivity && count != actors.Count)
                {
                    int Priority = 0;
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

                    Console.WriteLine("Назначение места употребления пищи");

                    HungerPoint = new Point(AmenityFastFood[1].Coordinate);

                    Console.WriteLine("Место употребления пищи назначено");

                    Console.WriteLine($"Координаты: {HungerPoint} ");

                    Console.WriteLine($"Приоритет: {Priority} ");

                    // Назначить актору путь до дома
                    actor.Activity = new MovementActivityHunger(Priority, HungerPoint);
                    Console.WriteLine("Said actor go home\n");
                    count++;
                }
            }
        }
    }
}
