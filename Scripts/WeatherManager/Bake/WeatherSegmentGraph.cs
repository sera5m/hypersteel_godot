using System.Collections.Generic;

namespace Hypersteel.Weather;

public sealed class WeatherSegmentGraph
{
	public readonly Dictionary<int, List<int>> Edges = new();

	public void Connect(int a, int b)
	{
		if (!Edges.TryGetValue(a, out var la)) { la = new List<int>(); Edges[a] = la; }
		if (!Edges.TryGetValue(b, out var lb)) { lb = new List<int>(); Edges[b] = lb; }
		if (!la.Contains(b)) la.Add(b);
		if (!lb.Contains(a)) lb.Add(a);
	}

	public Dictionary<int, int> DistancesFrom(int start)
	{
		var dist = new Dictionary<int, int> { [start] = 0 };
		var q = new Queue<int>();
		q.Enqueue(start);
		while (q.Count > 0)
		{
			var n = q.Dequeue();
			if (!Edges.TryGetValue(n, out var next)) continue;
			foreach (var m in next)
			{
				if (dist.ContainsKey(m)) continue;
				dist[m] = dist[n] + 1;
				q.Enqueue(m);
			}
		}
		return dist;
	}
}
