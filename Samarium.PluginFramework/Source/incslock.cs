using System;
using System.Threading;

namespace System.Threading
{
	class Monitor
	{
		public static void Enter(object obj)
		{		
			slockimp.imp.DoLock(obj);			
		}	

		public static void Exit(object obj)
		{			
			slockimp.imp.DoUnlock(obj);
		}

		public static bool TryEnter(object obj)
		{
			return	slockimp.imp.DoTryEnter(obj);
		}
		
		public static bool TryEnter(object obj,	int millisecondsTimeout)
		{
			return  slockimp.imp.DoTryEnter(obj, millisecondsTimeout);
		}

		public static bool TryEnter(object obj,	TimeSpan timeout)
		{
			return slockimp.imp.DoTryEnter(obj, timeout);
		}
	}
}