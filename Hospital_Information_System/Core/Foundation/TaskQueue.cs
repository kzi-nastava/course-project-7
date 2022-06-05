using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HIS.Core.Foundation
{
	public class TaskQueue
	{
		private readonly IList<Task> _tasks;

		public TaskQueue()
		{
			_tasks = new List<Task>();
		}

		public void Add(Action action, DateTime when)
		{
			Task t = default;
			t = new Task(() =>
			{
				var dt = (int)(when - DateTime.Now).TotalMilliseconds;
				Thread.Sleep((int)Math.Max(dt, 0.0));
				action.Invoke();
				_tasks.Remove(t);
			});

			_tasks.Add(t);
			t.Start();
		}
	}
}
