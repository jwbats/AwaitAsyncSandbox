using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace AwaitAsyncSandbox
{
	class Program
	{

		private static void DoIndependentWork(string source)
		{
			Debug.WriteLine("Doing independent work from " + source);
		}




		private static async Task<PingReply[]> MultiPingAsync(List<string> hosts)
		{
			// kick off a task that gets ping replies
			Task<List<PingReply>> task = Task.Factory.StartNew(() => {
				System.Threading.Thread.Sleep(10000);

				return hosts
					.Select(host => new Ping().Send(host))
					.ToList();
			});

			// do some other work
			DoIndependentWork("method MultiPingAsync");

			// yield control to caller... get control back once task finished
			List<PingReply> _pingReplies = await task;

			// do some processing after receiving result
			PingReply[] __pingReplies = _pingReplies.ToArray();

			// return processed result
			return __pingReplies;
		}



	
		static void Main(string[] args)
		{
			// run an async multiping
			Task<PingReply[]> taskPingReplies = MultiPingAsync(new List<string>() { "google.com", "tweakers.net", "fok.nl" });

			// spend time doing something else
			DoIndependentWork("method Main");

			// wait for ping replies (this will take as much time as the task within MultiPingAsync still needs)
			PingReply[] pingReplies = taskPingReplies.Result;

			// output all ping replies
			foreach (PingReply pingReply in pingReplies)
			{
				Debug.WriteLine("{0} - {1}", pingReply.Address, pingReply.Status);
			}
			
			// end of program
			Console.ReadKey();
		}
	}
}
