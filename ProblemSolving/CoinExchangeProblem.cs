using System.Diagnostics;

namespace ProblemSolving;
internal class CoinExchangeProblem(int Number, int MaxDelta = 6)
{
    public static void RunForAll(int maxNumber, int maxDelta)
    {
        var prevCoinsRequired = -1;
        Stopwatch sw = new();
        sw.Start();
        for (int k = 1; k <= maxNumber; k++)
        {
            CoinExchangeProblem c = new(k, maxDelta);
            sw.Restart();
            var result = c.SolveWithDiff(prevCoinsRequired);
            prevCoinsRequired = result.Count;

            var maxDiff = result
                .Select((value, index) => index == 0 ? value : value - result[index - 1])
                .Max();
            Console.WriteLine($"{k}. Sequence: {string.Join(',', result)}. Length: {result.Count}. MaxDiff: {maxDiff}. Time: {sw.ElapsedMilliseconds}ms");
        }
    }

    public List<int> SolveWithDiff(int prevRequiredCoins = -1)
    {
        if (Number < 3) return [1];
        List<int>[] results = [[], []];

        int minCoins = prevRequiredCoins == -1 ? Math.Max((int)Math.Log2(Number), 2) / 2 : Math.Max(1, prevRequiredCoins / 2);
        int maxCoins = prevRequiredCoins == -1 ? Math.Max(5, Number / 8) : (prevRequiredCoins / 2) + 1;

        Enumerable
            .Range(minCoins, maxCoins - minCoins + 1)
            .Any(i => GetPermutations(0, MaxDelta, i, [], 0).Any(perm => GetResults(results, MaxDelta, perm)));

        return results[1].Count < results[0].Count ? results[1] : results[0];
    }

    private bool GetResults(List<int>[] results, int maxDiff, IEnumerable<int> pos)
    {
        List<int> posList = new(pos);
        List<int> list = new((posList.Count * 2) + 1);
        GetEvenOrOddResult(results, posList, maxDiff, list, false);
        GetEvenOrOddResult(results, posList, maxDiff, list, true);

        return results[0].Count > 0 && results[1].Count > 0;
    }

    private void GetEvenOrOddResult(List<int>[] results, List<int> pos, int maxDiff, List<int> list, bool isOdd)
    {
        list.Clear();
        list.Add(pos[0]);
        list.AddRange(pos.Skip(1).Select((p, i) => list[i] + p));

        if (isOdd)
        {
            int c = list.Count;
            for (int i = 1; i <= maxDiff; i++)
            {
                list.Add(list[^1] + i);
                list.AddRange(pos.Select((p, i) => list[^1] + pos[pos.Count - 1 - i]));
                CheckAndStoreResult(results, list, 1);
                list.RemoveRange(c, list.Count - c);
            }

            return;
        }

        list.AddRange(pos.Select((p, i) => list[^1] + pos[pos.Count - 1 - i]));
        CheckAndStoreResult(results, list, 0);
    }

    private void CheckAndStoreResult(List<int>[] results, List<int> list, int index)
    {
        bool possible = Enumerable.Range(1, Number)
            .All(d => list.BinarySearch(d) >= 0 || list.Any(f => list.BinarySearch(d - f) >= 0));

        if (possible && (results[index].Count == 0 || list.Count < results[index].Count))
        {
            results[index] = [.. list];
        }
    }

    private IEnumerable<IEnumerable<int>> GetPermutations(int depth, int max, int length, List<int> current, int sum)
    {
        if ((sum * 4) - max > Number)
        {
            yield break;
        }

        if (depth == length - 1)
        {
            current.Add(1);
            if (Number - (sum * 4) <= max)
            {
                yield return current.Select((e, i) => current[current.Count - 1 - i]);
            }
            current.RemoveAt(current.Count - 1);

            yield break;
        }

        for (int j = max; j >= 1; j--)
        {
            current.Add(j);
            foreach (var permutation in GetPermutations(depth + 1, max, length, current, sum + j))
            {
                yield return permutation;
            }
            current.RemoveAt(current.Count - 1);
        }
    }
}