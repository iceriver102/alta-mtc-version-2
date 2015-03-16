using System.Collections;
using System;
using System.Threading;
namespace Alta.Class
{
    public class EasyTimer
    {
        private static Thread t;
        public static IDisposable SetInterval(Action method, int delayInMilliseconds)
        {
            System.Timers.Timer timer = new System.Timers.Timer(delayInMilliseconds);
            timer.Elapsed += (source, e) =>
            {
                method();
            };
            timer.Enabled = true;
            timer.Start();

            // Returns a stop handle which can be used for stopping
            // the timer, if required
            return timer as IDisposable;
        }
        public static void SetInterval(Action menthod, int time, ApartmentState state)
        {
            if (t != null)
            {
                throw new Exception("Không thể khởi tạo thread");
            }
            t = new Thread(() => Run(menthod, time * 1000,2));
            t.SetApartmentState(state);
            t.IsBackground = true;
            t.Start();
        }

        public static void SetTimeout(Action menthod, int time, ApartmentState state)
        {
            if (t != null)
            {
                throw new Exception("Không thể khởi tạo thread");
            }
            t = new Thread(()=>Run(menthod,time*1000));
            t.SetApartmentState(state);
            t.IsBackground = true;
            t.Start();
        }
        private static void Run(Action menthod, int time, int mode =1)
        {
            while (mode>0)
            {
                Thread.Sleep(time);
                menthod();
                if (mode == 1)
                {
                    mode = 0;

                    try
                    {
                        t.Abort();
                    }
                    catch (Exception)
                    {
                    }
                    finally
                    {
                        t = null;
                    }
                }
               
            }
        }

        public static void Cancel()
        {
            if (t != null)
            {
                try
                {
                    t.Abort();
                }
                catch (Exception)
                {

                }
                finally
                {
                    t = null;
                }
            }
        }

        public static IDisposable SetTimeout(Action method, int delayInMilliseconds)
        {
            System.Timers.Timer timer = new System.Timers.Timer(delayInMilliseconds);
            timer.Elapsed += (source, e) =>
            {
                method();
            };

            timer.AutoReset = false;
            timer.Enabled = true;
            timer.Start();

            // Returns a stop handle which can be used for stopping
            // the timer, if required
            return timer as IDisposable;
        }

    }
}
