using BlossomServer.Datas;
using System.Collections.Concurrent;

namespace BlossomServer.Database
{
	public class SharedDB
	{
		private readonly ConcurrentDictionary<string, UserConnection> _connection;
		public ConcurrentDictionary<string, UserConnection> connection => _connection;
	}
}
