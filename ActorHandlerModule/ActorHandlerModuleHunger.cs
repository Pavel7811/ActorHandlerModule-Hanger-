using System;
using ActorModule;
using OSMLSGlobalLibrary.Modules;
using NetTopologySuite.Geometries;  // Отсюда Point и другая геометрия
using NetTopologySuite.Mathematics; // Отсюда векторы
using NodaTime;
using NodaTime.Extensions;
using PathsFindingCoreModule;
using InitializeActorModule;
using ActorHandlerModule;

namespace ActorHandlerModuleHunger 
{
    public class ActorHandlerModuleHunger : OSMLSModule
    {
        public LocalTime HungerTimeStart;
        public LocalTime HungerTimeEnd;

        /// <summary>
        /// Инициализация модуля. В отладочной конфигурации выводит сообщение
        /// </summary>
        protected override void Initialize()
        {
            Console.WriteLine("ActorHandlerModuleHunger: Initialize");
        }
        /// <summary>
        /// Вызывает Update на всех акторах
        /// </summary>
        public override void Update(long elapsedMilliseconds)
        {
            var actors = MapObjects.GetAll<Actor>();

            foreach (var actor in actors)
            {
                //Установка приоритета
                int newPriority = 0;

                //установлена ли уже активность у актора
                bool isActivity = actor.Activity != null;
                //Является ли активность нашей
                bool isMovementActivityHunger = actor.Activity is MovementActivityHunger;
                bool isWaitingActivityHunger = actor.Activity is WaitingActivityHunger;


                Console.WriteLine($"Flags: IsActivity={isActivity} IsActivityMovement={isMovementActivityHunger} IsActivityWaiting={isWaitingActivityHunger}");
                //Если активность не установлена или приоритет Активностей выше
                if ((!isActivity) || (!isMovementActivityHunger && !isWaitingActivityHunger && newPriority > actor.Activity.Priority))
                {
                    // Назначить актору путь до места употребления пищи
                    actor.Activity = new MovementActivityHunger(newPriority, new TimeInterval(HungerTimeStart, HungerTimeEnd));
                    Console.WriteLine("Said actor go hunger\n");
                }
            }
        }
    }
}

