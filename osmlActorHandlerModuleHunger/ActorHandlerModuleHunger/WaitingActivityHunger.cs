using System;
using InitializeActorModule;
using ActorModule;

namespace ActorHandlerModuleHunger
{
    public class WaitingActivityHunger : IActivity
    {
        // Приоритет делаем авто-свойством, со значением по умолчанию
        public int Priority { get; set; } = 1;
        public double TimeUpdate = 0;
        //Интервал времени на питание
        public TimeInterval HungerTime { get; set; }

        public WaitingActivityHunger(int priority, TimeInterval hungerTime)
        {
            Priority = priority;
            HungerTime = hungerTime;
        }
        //Update-работает постоянно, пока не return true
        public bool Update(Actor actor, double deltaTime)
        {
            Console.WriteLine("Start WaitingActivityHunger");

            TimeUpdate += deltaTime;
            //Работа со статами акторы раз в секунду
            if (TimeUpdate >= 1)
            {
                TimeUpdate -= 1;
                //Голод
                actor.GetState<SpecState>().Satiety += 0.1 * 100;
                actor.GetState<SpecState>().Money -= 1;
            }

            //Текущее время, переведенное в строку
            string NowTime = DateTime.Now.ToString("HH:mm:ss");

            //Console.WriteLine("NOW:  " + NowTime);
            //Console.WriteLine("NEED:  " + HungerTime.End.ToString());


            if (NowTime == HungerTime.End.ToString())
            {
                Priority = 0;

                return true;//Выходим из активити
            }
            return false;
        }
    }
}
