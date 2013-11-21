//-----------------------------------------------------------------------
// <copyright file="TaskScheduler.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.Runtime.CompilerServices;

    public class TaskHandle
    {
        public readonly int TaskId;

        public TaskHandle(int taskId)
        {
            this.TaskId = taskId;
        }
    }

	public enum NativeTimerHandleType
	{
		None,
		Timeout,
		Intervale,
		Immediate
	}

    public enum TaskState
    {
        Waiting,
        Canceled,
        Completed,
        Running
    }

    public class Task
    {
        public Number NativeTimerId;
        public TaskState State;
        public NativeTimerHandleType NativeTimerType;
        public Task ParentTask;
        public Action Work;
        public Number TaskId;

        public Task(int taskId, Action work)
        {
            this.TaskId = taskId;
            this.Work = work;
        }

        public Task(
            int taskId,
            int nativeTimerId,
            NativeTimerHandleType nativeTimerType,
            Action work)
        {
            this.TaskId = taskId;
            this.NativeTimerId = nativeTimerId;
            this.NativeTimerType = nativeTimerType;
            this.Work = work;
        }
    }

    public interface IWindowTimer
    {
        int SetImmediate(Action action);

        int SetTimeout(Action action, int timoutTime);

        int SetInterval(Action action, int intervalTime);

        void ClearTimeout(int timeoutHandle);

        void ClearInterval(int intervalHandle);
    }

    public class WindowTimer: IWindowTimer
    {
        /// <summary>
        /// Sets an immediate.
        /// </summary>
        /// <param name="action"> The action. </param>
        /// <returns>
        /// handle
        /// </returns>
        [Script(
            @"if (typeof @{[System.Web]System.Web.Globals::SetImmediate([mscorlib]System.Action)} != 'undefined')
                return @{[System.Web]System.Web.Globals::SetImmediate([mscorlib]System.Action)}(action);
            return @{[System.Web]System.Web.Globals::SetTimeout([mscorlib]System.Action, [mscorlib]System.Int32)}(action, 0);")]
        public extern int SetImmediate(Action action);

        /// <summary>
        /// Sets a timeout.
        /// </summary>
        /// <param name="action">        The action. </param>
        /// <param name="timeoutHandle"> Handle of the timeout. </param>
        /// <returns>
        /// handle
        /// </returns>
        public int SetTimeout(Action action, int timeoutHandle)
        {
            if (timeoutHandle == 0)
            {
                return this.SetImmediate(action);
            }

            return Globals.SetTimeout(action, timeoutHandle);
        }

        [Script(
            @"if (typeof @{[System.Web]System.Web.Globals::ClearImmediate([mscorlib]System.Int32)} != 'undefined')
                @{[System.Web]System.Web.Globals::ClearImmediate([mscorlib]System.Int32)}(timeoutHandle);
            @{[System.Web]System.Web.Globals::ClearTimeout([mscorlib]System.Int32)}(timeoutHandle);")]
        public extern void ClearTimeout(int timeoutHandle);

        /// <summary>
        /// Sets an interval.
        /// </summary>
        /// <param name="action">       The action. </param>
        /// <param name="intervalTime"> Time of the interval. </param>
        /// <returns>
        /// Interval handle.
        /// </returns>
        public int SetInterval(Action action, int intervalTime)
        {
            return Globals.SetInterval(action, intervalTime);
        }

        /// <summary>
        /// Clears the interval described by intervalHandle.
        /// </summary>
        /// <param name="intervalHandle"> Handle of the interval. </param>
        public void ClearInterval(int intervalHandle)
        {
            Globals.ClearInterval(intervalHandle);
        }
    }

    public class TestWindowTimer : IWindowTimer
    {
        public int SetImmediate(Action action)
        {
            action();
            return 0;
        }

        public int SetTimeout(Action action, int timoutTime)
        {
            action();
            return 0;
        }

        public int SetInterval(Action action, int intervalTime)
        {
            throw new NotImplementedException();
        }

        public void ClearTimeout(int timeoutHandle)
        {
        }

        public void ClearInterval(int intervalHandle)
        {
        }
    }

    /// <summary>
    /// Definition for TaskScheduler
    /// </summary>
    public class TaskScheduler
    {
        private static int taskId;
        private static TaskScheduler instance;

        private Task currentTask;
        private int currentContextId;
        private int workQuanta;
        private int idleTimeout;
        private int nextTimerId;

        private IWindowTimer windowTimer;
        private Queue<Task> hiPriTasks, lowPriTasks, idleTasks;
        private NumberDictionary<Task> tasks;
        private int timerId = -1;
        private bool highPriSetup;

        public TaskScheduler(
            IWindowTimer windowTimer,
            int workQuanta,
            int idleTimeout)
        {
            this.tasks = new NumberDictionary<Task>();
            this.windowTimer = windowTimer;
            this.workQuanta = workQuanta;
            this.idleTimeout = idleTimeout;
            this.hiPriTasks = new Queue<Task>();
            this.lowPriTasks = new Queue<Task>();
            this.idleTasks = new Queue<Task>();
        }

        public static TaskScheduler Instance
        {
            get
            {
                if (TaskScheduler.instance == null)
                {
                    TaskScheduler.instance = new TaskScheduler(
                        new WindowTimer(),
                        16,
                        25);
                }

                return TaskScheduler.instance;
            }

            set { TaskScheduler.instance = value; }
        }

        public TaskHandle EnqueueOnTimeout(Action work, string traceId, int timeout)
        {
            var task = new Task(
                this.nextTimerId++,
                this.windowTimer.SetTimeout(work, timeout),
                NativeTimerHandleType.Timeout,
                work);

            this.tasks.Add(task.TaskId, task);
            return new TaskHandle(task.TaskId);
        }

        public TaskHandle EnqueHighPriTask(Action work, string traceId)
        {
            var task = new Task(
                    this.nextTimerId++,
                    work);
            this.hiPriTasks.Enqueue(task);
            this.tasks.Add(task.TaskId, task);
            this.ScheduleQuanta(false);
            return new TaskHandle(task.TaskId);
        }

        public TaskHandle EnqueueTask(Action work, string traceId)
        {
            var task = new Task(
                    this.nextTimerId++,
                    work);
            this.lowPriTasks.Enqueue(task);
            this.tasks.Add(task.TaskId, task);
            this.ScheduleQuanta(false);
            return new TaskHandle(task.TaskId);
        }

        public TaskHandle EnqueueLowPriTask(Action work, string traceId)
        {
            var task = new Task(
                    this.nextTimerId++,
                    work);
            this.idleTasks.Enqueue(task);
            this.tasks.Add(task.TaskId, task);
            this.ScheduleQuanta(false);
            return new TaskHandle(task.TaskId);
        }

        private void RunQuanta()
        {
            if (this.hiPriTasks.Count > 0)
            {
                this.FlushQueue(this.hiPriTasks, DateTime.Now + this.workQuanta);
            }
            else if (this.idleTasks.Count > 0)
            {
                this.FlushQueue(this.idleTasks, DateTime.Now + this.workQuanta / 2);
            }
            else if (this.lowPriTasks.Count > 0)
            {
                this.FlushQueue(this.lowPriTasks, DateTime.Now + this.workQuanta / 2);
            }

            this.timerId = -1;
            this.ScheduleQuanta(true);
        }

        private void FlushQueue(Queue<Task> taskQueue, DateTime endBy)
        {
            while(taskQueue.Count > 0)
            {
                Task task = taskQueue.Dequeue();
                this.ExecuteTask(task);
                DateTime now = DateTime.Now;
                if (endBy < now)
                {
                    return;
                }
            }
        }

        private void ExecuteTask(Task task)
        {
            if (task.State == TaskState.Waiting)
            {
                try
                {
                    this.currentTask = task;
                    task.State = TaskState.Running;
                    task.Work();
                }
                catch
                {
                }

                this.currentTask = null;
            }

            task.State = TaskState.Completed;
            this.tasks.Remove(task.TaskId);
        }

        private void ScheduleQuanta(bool force)
        {
            if (force || (!this.highPriSetup && this.hiPriTasks.Count > 0))
            {
                this.windowTimer.ClearTimeout(this.timerId);
                this.timerId = -1;
            }

            if (this.timerId != -1)
            {
                return;
            }

            if (this.hiPriTasks.Count > 0)
            {
                this.highPriSetup = true;
                this.timerId = this.windowTimer.SetImmediate(this.RunQuanta);
            }
            else if (this.idleTasks.Count + this.lowPriTasks.Count > 0)
            {
                this.highPriSetup = false;
                this.timerId = this.windowTimer.SetTimeout(this.RunQuanta, this.idleTimeout);
            }
        }
    }
}
