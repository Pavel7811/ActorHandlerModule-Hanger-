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
        public string EndTime;
        public string NowTime;
        public WaitingActivityHunger(int priority, DateTime timeend)
        {
            Priority = priority;
            EndTime = timeend.ToString("HH:mm:ss");
            Console.WriteLine("Start WaitingActivity(Hunger)");
        }

        //Update-работает постоянно, пока не return true
        public bool Update(Actor actor, double deltaTime)
        {
            TimeUpdate += deltaTime;
            //Работа со статами акторы раз в секунду
            if ((TimeUpdate >= 1) && (actor.GetState<SpecState>().Satiety < 100))
            {
                TimeUpdate -= 1;
                //Голод
                actor.GetState<SpecState>().Satiety += 0.1 * 100;
                actor.GetState<SpecState>().Money -= 0.1;
                if (actor.GetState<SpecState>().Satiety > 100)
                {
                    actor.GetState<SpecState>().Satiety = 100;
                }
            }
            //Текущее время, переведенное в строку
            NowTime = DateTime.Now.ToString("HH:mm:ss");
            if (NowTime == EndTime)
            {
                //Console.WriteLine("Выход из ожидания.");
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
                Console.WriteLine("Stats actors:");
                Console.WriteLine($"Health: {actor.GetState<SpecState>().Health}; " +
                $"Hunger: {actor.GetState<SpecState>().Satiety}; " +
                $"Fatigue: {actor.GetState<SpecState>().Stamina}; " +
                $"Mood: {actor.GetState<SpecState>().Mood} " + 
                $"Money: {actor.GetState<SpecState>().Money} ");
                return true;//Выходим из активити
            }
            return false;
        }
    }
}
