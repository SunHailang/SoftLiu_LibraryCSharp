﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TFramework.Singleton;

namespace TFramework.Utils
{
    public class ThreadPoolManager : AutoGeneratedSingleton<ThreadPoolManager>, IDisposable
    {
        //注意：线程池中启动的线程都是后台线程

        //线程池有如下优点：
        //1、缩短应用程序的响应时间。因为在线程池中有线程的线程处于等待分配任务状态（只要没有超过线程池的最大上限），无需创建线程。
        //2、不必管理和维护生存周期短暂的线程，不用在创建时为其分配资源，在其执行完任务之后释放资源。
        //3、线程池会根据当前系统特点对池内的线程进行优化处理。
        //ThreadPool不支持线程的取消、完成、失败通知等交互性操作；
        //ThreadPool不支持线程执行的先后次序；


        //GetAvailableThreads(Int32, Int32)
        //线程池中空闲线程数

        //GetMaxThreads(Int32, Int32)
        //检索可以同时处于活动状态的线程池请求的数目。 所有大于此数目的请求将保持排队状态，直到线程池线程变为可用。

        //GetMinThreads(Int32, Int32)
        //发出新的请求时，在切换到管理线程创建和销毁的算法之前检索线程池按需创建的线程的最小数量。

        //QueueUserWorkItem(WaitCallback)
        //将方法排入队列以便执行。 此方法在有线程池线程变得可用时执行。

        //QueueUserWorkItem(WaitCallback, Object)
        //将方法排入队列以便执行，并指定包含该方法所用数据的对象。 此方法在有线程池线程变得可用时执行。

        //SetMaxThreads(Int32, Int32)
        //设置可以同时处于活动状态的线程池的请求数目。 所有大于此数目的请求将保持排队状态，直到线程池线程变为可用。

        //SetMinThreads(Int32, Int32)
        //发出新的请求时，在切换到管理线程创建和销毁的算法之前设置线程池按需创建的线程的最小数量。


        public ThreadPoolManager()
        {
            //使用Task.Factory.StartNew来处理这个队列，也就是所有数据使用一个线程处理
            //Task.Factory.StartNew
            //直接使用ThreadPool.QueueUserWorkItem来处理每条数据,这种方法的处理速度更快，但是因为使用的是多个线程，有时候执行的顺序并不是传入的顺序
        }
        /// <summary>
        /// 初始化  默认线程池 最大：10 最小：2
        /// </summary>
        public void Init()
        {

            ThreadPool.SetMaxThreads(10, 10);
            ThreadPool.SetMinThreads(2, 2);
        }
        /// <summary>
        /// 设置 最大，最小线程数
        /// </summary>
        /// <param name="max">最大值</param>
        /// <param name="min">最小值</param>
        public void Init(int max, int min)
        {
            ThreadPool.SetMaxThreads(max, max);
            ThreadPool.SetMinThreads(min, min);
        }

        public void TaskNewStart(object state, Action<object> action, TaskCreationOptions option)
        {
            Task.Factory.StartNew(action, state, TaskCreationOptions.LongRunning);
        }

        public void QueueUserWorkItem()
        {
            ThreadPool.QueueUserWorkItem((state) => { });
        }

        public void Dispose()
        {

        }
    }
}
