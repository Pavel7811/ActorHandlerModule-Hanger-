﻿using System;
using System.Collections.Generic;
using System.Text;
using ActorModule;
using OSMLSGlobalLibrary.Modules;
using InitializeActorModule;
using CityDataExpansionModule.OsmGeometries;
using System.Linq;
using NetTopologySuite.Geometries;

namespace ActorHandlerModuleHunger
{
    class ActorHandlerModuleHunger : OSMLSModule
    {
        //bool isPointHunger = true;
        public Point HungerPoint;
        //public TimeInterval hungerTime;
        /// <summary>
        /// Инициализация модуля. В отладочной конфигурации выводит сообщение
        /// </summary>
        protected override void Initialize()
        {
            List<OsmClosedWay> AmenityFastFood = MapObjects.GetAll<OsmClosedWay>().Where(x => x.Tags.ContainsKey("amenity:fast_food")).ToList();
            HungerPoint = new Point(AmenityFastFood[1].Coordinate);
            //Вывод сообщения о инициализации модуля
            Console.WriteLine("ActorHandlerModuleHungere: Initialize");
        }
        

        /// <summary>
        /// Вызывает Update на всех акторах
        /// </summary>
        public override void Update(long elapsedMilliseconds)
        {
           /* if (isPointHunger)
            {
                List<OsmClosedWay> PointHunger = MapObjects.GetAll<OsmClosedWay>().Where(x => x.Tags.ContainsKey("amenity:fast_food")).ToList();
                isPointHunger = false;
            }*/
            
            //Получаем список всех акторов
            var actors = MapObjects.GetAll<Actor>();
            //Вывод акторов
            Console.WriteLine($"Got {actors.Count} actors\n");

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
                    //hungerTime 
                    // Назначить актору путь до точки общепита
                    actor.Activity = new MovementActivityHunger(newPriority, HungerPoint);
                    Console.WriteLine("Said actor go Hunger\n");
                }
            }
        }
    }
}