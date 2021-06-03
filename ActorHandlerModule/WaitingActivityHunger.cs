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

    public class WaitingActivityHunger : IActivity
    {
        // Приоритет делаем авто-свойством, со значением по умолчанию
        public int Priority { get; set; } = 1;
        //Интервал времени на работу
        public TimeInterval HungerTime { get; set; }

        public WaitingActivityHunger(Actor actor, int priority, TimeInterval hungerTime)
        {
            Priority = priority;
            HungerTime = hungerTime;
        }
        //Update-работает постоянно, пока не return true
        public bool Update(Actor actor, double deltaTime)
        {
            
            // Увеличивается голод, настроение
            //if (actor.GetState<SpecState>().Hunger <= 0.1) actor.GetState<SpecState>().Hunger += 0.01;
            //else actor.GetState<SpecState>().Hunger += 0.01;

            Console.WriteLine($"Health: {actor.GetState<SpecState>().Health}; Hunger: {actor.GetState<SpecState>().Hunger}; Fatigue: {actor.GetState<SpecState>().Fatigue}; Mood: {actor.GetState<SpecState>().Mood}");


            //Текущее время, переведенное в строку
            string NowTime = DateTime.Now.ToString("HH:mm:ss");

            Console.WriteLine("NOW:  " + NowTime);
            Console.WriteLine("NEED:  " + HungerTime.End.ToString());

            
            if (NowTime == HungerTime.End.ToString())
            {
                Priority = 0;
                actor.GetState<SpecState>().Hunger = 100;
                return true;//Выходим из активити
            }
            return false;
        }
    }

}

