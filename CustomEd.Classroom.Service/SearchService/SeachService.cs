using CustomEd.Shared.Data.Interfaces;

namespace CustomEd.Classroom.Service.Search.Service;
public class SearchService
{

    private readonly List<Model.Classroom> _classroomRepository;

    public SearchService(List<Model.Classroom> classroomRepository)
    {
        _classroomRepository = classroomRepository;
    }
    public List<Guid> FindClosestMatchs(string searchTerm)
    {
        var queue = new PriorityQueue<SearchItem, int>();
        var items = _classroomRepository.Select(x => new SearchItem{Id = x.Id, Name =  x.Name,Description =  x.Description}).ToList();
        foreach (SearchItem item in items)
        {
            int nameDistance = DamerauLevenshteinDistance(searchTerm, item.Name);
            int descriptionDistance = DamerauLevenshteinDistance(searchTerm, item.Description);
            int distance = Math.Min(nameDistance, descriptionDistance);

            queue.Enqueue(item, -distance);
            if (queue.Count > 10)
            {
                queue.Dequeue();
            }
    
        }

        var matches = new List<Guid>();
        while(queue.Count > 0)
        {
            matches.Add(queue.Dequeue().Id);
        }

        return matches;
    }

    private int DamerauLevenshteinDistance(string source, string target)
    {
        int m = source.Length;
        int n = target.Length;
        int[,] d = new int[m + 1, n + 1];

        for (int i = 0; i <= m; i++)
            d[i, 0] = i;

        for (int j = 0; j <= n; j++)
            d[0, j] = j;

        for (int i = 1; i <= m; i++)
        {
            for (int j = 1; j <= n; j++)
            {
                int cost = (target[j - 1] == source[i - 1]) ? 0 : 1;

                d[i, j] = Math.Min(
                    Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                    d[i - 1, j - 1] + cost);

                if (i > 1 && j > 1 && source[i - 1] == target[j - 2] && source[i - 2] == target[j - 1])
                    d[i, j] = Math.Min(d[i, j], d[i - 2, j - 2] + cost);
            }
        }

        return d[m, n];
    }
}