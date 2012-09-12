using System;
using System.Collections.Generic;
using System.Text;
using VersionOne.SDK.ObjectModel;

namespace V1TortoiseSVN
{
	public class WorkitemsReadyEventArgs : EventArgs
	{
		public WorkitemsReadyEventArgs(Dictionary<Workitem, List<Task>> workitems)
		{
			Workitems = workitems;
		}
		public Dictionary<Workitem, List<Task>> Workitems;
	}

	public class BackgroundErrorEventArgs : EventArgs
	{
		public readonly Exception Exception;

		public BackgroundErrorEventArgs(Exception exception)
		{
			Exception = exception;
		}
	}

	public delegate void InvokeMethod();
}
