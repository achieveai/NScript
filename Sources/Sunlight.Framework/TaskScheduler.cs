//-----------------------------------------------------------------------
// <copyright file="TaskScheduler.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework
{
    using System;

    public class TaskHandle
    {
        int taskId;
    }

<<<<<<< HEAD
	public enum NativeTimerHandleType
	{
		None,
		Timeout,
		Intervale,
		Immediate
	}
=======
    public enum NativeTimerHandleType
    {
        None,
        Timeout,
        Intervale,
        Immediate
    }
>>>>>>> 67c21b7702788ea6d42f3a2e0c4b588ee67d594e

    public enum TaskState
    {
        Waiting,
        Canceled,
        Completed,
        Running
    }

    public class Task
    {
        int nativeTimerId;
        TaskState state;
        NativeTimerHandleType nativeTimerType;
        Task parentTask;
        Action work;

        public Task(Action work)
        {
            this.work = work;
        }

        public Task(
            int nativeTimerId,
            NativeTimerHandleType nativeTimerType,
            Action work)
        {
            this.nativeTimerId = nativeTimerId;
            this.nativeTimerType = nativeTimerType;
            this.work = work;
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

        public TaskScheduler(int workQuanta, int idleTimeOut)
        {
        }

        public static TaskScheduler Instance
        {
            get
            {
                if (TaskScheduler.instance != null)
                {
                    TaskScheduler.instance = new TaskScheduler(16, 25);
                }

                return TaskScheduler.instance;
            }
        }

        public TaskHandle EnqueHighPriTask(Action work, string traceId)
        {
            return null;
        }

        public TaskHandle EnqueueTask(Action work, string traceId)
        {
            return null;
        }

        public TaskHandle EnqueueLowPriTask(Action work, string traceId)
        {
            return null;
        }
    }
}
