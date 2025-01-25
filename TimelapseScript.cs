using System;
using System.Threading.Tasks;

namespace TimelapseApp
{
    public static class TimelapseScript
    {
        public static void Run(int day, int days, int delay)
        {
            if (day <= days)
            {
                Temp.Create();

                Task.Delay(delay > 3 ? (delay - 3) * 1000 : 0).Wait();
                
                int allTime = 16 * 60 * 60;
                int videoTime = (int)(allTime / days + Math.Round(allTime % days / (double)days));
                int minutes = videoTime / 60;
                int v = videoTime - minutes * 60;

                //System.Console.WriteLine((allTime / days) + " " + (allTime % days) + " " + ((allTime - allTime / days * days) / Convert.ToDouble(days)) + " " + (allTime % days / Convert.ToDouble(days)) + " " + videoTime + " " + (videoTime * days));
            }
        }
    }
}
