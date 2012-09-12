using System;
using System.Collections.Generic;
using System.Threading;
using VersionOne.SDK.ObjectModel;
using VersionOne.SDK.ObjectModel.Filters;

namespace V1TortoiseSVN
{
	public class WorkitemRetriever
	{
		private V1Instance _v1;
		AutoResetEvent _reset = new AutoResetEvent(false);
		ManualResetEvent _stop = new ManualResetEvent(false);

		public WorkitemRetriever(V1Instance v1)
		{
			_v1 = v1;

			Thread workerThread = new Thread(BackgroundLoop);
			workerThread.IsBackground = true;
			workerThread.Name = "Workitem Downloader Thread";
			workerThread.Start();
		}

		/// <summary>
		/// Begins the Async retrieval or Workitems.  WorkitemsReady is raised when completed, WorkitemsError on error.
		/// </summary>
		public void BeginRetrieveWorkitems()
		{
			_reset.Set();
		}

		public void ShutDown()
		{
			_stop.Set();
			_reset.Set();
		}

		private void BackgroundLoop()
		{
			while (!_stop.WaitOne(0, false))
			{
				_reset.WaitOne();

				if (_stop.WaitOne(0, false))
					return;

				GetWorkitems();
			}
		}

		private void GetWorkitems()
		{
			try
			{
				Dictionary<Workitem, List<Task>> workitems = new Dictionary<Workitem, List<Task>>();

				Member currentUser = _v1.LoggedInMember;
				PrimaryWorkitemFilter storyFilter = new PrimaryWorkitemFilter();
				storyFilter.State.Add(State.Active);

				storyFilter.Owners.Add(currentUser);

				IterationFilter iterationFilter = new IterationFilter();
				iterationFilter.State.Add(IterationState.Active);
				foreach (Iteration iteration in _v1.Get.Iterations(iterationFilter))
					storyFilter.Iteration.Add(iteration);

				ICollection<Workitem> ownedStories = _v1.Get.Workitems(storyFilter);
				foreach (PrimaryWorkitem story in ownedStories)
				{
					if (!workitems.ContainsKey(story))
						workitems.Add(story, new List<Task>());

					// We will pass a TaskFilter to GetSecondaryWorkitems so we only get Tasks back.
					TaskFilter filter = new TaskFilter();
					foreach (Task task in story.GetSecondaryWorkitems(filter))
					{
						workitems[story].Add(task);
					}
				}

				TaskFilter taskFilter = new TaskFilter();
				taskFilter.State.Add(State.Active);
				taskFilter.Owners.Add(currentUser);

				ICollection<Task> tasks = _v1.Get.Tasks(taskFilter);
				foreach (Task task in tasks)
				{
					Workitem story = task.Parent;
					if (ownedStories.Contains(story))
						continue;

					if (!workitems.ContainsKey(story))
						workitems.Add(story, new List<Task>());
					workitems[story].Add(task);
				}

				if (_workitemsReady != null)
					_workitemsReady(this, new WorkitemsReadyEventArgs(workitems));
			}
			catch (Exception ex)
			{
				if (_workitemsError != null)
					_workitemsError(this, new BackgroundErrorEventArgs(ex));
			}
		}

		private event EventHandler<WorkitemsReadyEventArgs> _workitemsReady;
		public event EventHandler<WorkitemsReadyEventArgs> WorkitemsReady
		{
			add { _workitemsReady += value; }
			remove { _workitemsReady -= value; }
		}
		
		private event EventHandler<BackgroundErrorEventArgs> _workitemsError;
		public event EventHandler<BackgroundErrorEventArgs> WorkitemsError
		{
			add { _workitemsError += value; }
			remove { _workitemsError -= value; }
		}
	}
}
