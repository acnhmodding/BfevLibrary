using BfevLibrary.Common;
using BfevLibrary.Parsers;

namespace BfevLibrary.Core;

public class RadixTree<T> : SortedList<string, T>, IBfevDataBlock
{
    internal string[]? _staticKeys;

    public RadixTree() : base(new KeyOrderComparer()) { }

    /// <summary>Creates a dictionary whose keys retain the supplied order.</summary>
    public RadixTree(IEnumerable<KeyValuePair<string, T>> values) : this()
    {
        var entries = values.ToArray();
        ((KeyOrderComparer)Comparer).SetKeys(entries.Select(entry => entry.Key));
        foreach (var entry in entries) {
            Add(entry.Key, entry.Value);
        }
    }

    public RadixTree(BfevReader reader, T[]? array = null) : this()
    {
        Read(reader);

        if (array != null) {
            LinkToArray(array);
        }
    }

    public void LinkToArray(T[] array)
    {
        if (array.Length != _staticKeys?.Length) {
            throw new Exception($"Could not link {typeof(T).Name}[{array.Length}] to RadixTree<{typeof(T).Name}> because the array lengths did not match.",
                new InvalidDataException($"Could not fit an array with length of '{array.Length}' into {_staticKeys?.Length}.")
            );
        }

        ((KeyOrderComparer)Comparer).SetKeys(_staticKeys!);
        try {
            for (int i = 0; i < array.Length; i++) {
                Add(_staticKeys[i], array[i]);
            }
        }
        catch { }
    }

    public IBfevDataBlock Read(BfevReader reader)
    {
        reader.CheckMagic(RadixTreeWriter.Magic);
        int count = reader.ReadInt32();
        reader.BaseStream.Position += 4 + 2 + 2 + 8; // Root entry

        _staticKeys = new string[count];
        for (int i = 0; i < count; i++) {
            reader.BaseStream.Position += 4 + 2 + 2;
            _staticKeys[i] = reader.ReadStringPtr();
        }

        return this;
    }

    public void Write(BfevWriter writer)
    {
        RadixTreeHelper.WriteRadixTree(writer, Keys.ToArray());
    }

    private sealed class KeyOrderComparer : IComparer<string>
    {
        private Dictionary<string, int>? _order;

        public void SetKeys(IEnumerable<string> keys) =>
            _order = keys.Select((key, index) => (key, index))
                .ToDictionary(item => item.key, item => item.index, StringComparer.Ordinal);

        public int Compare(string? x, string? y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (_order is not null) {
                int xIndex = 0, yIndex = 0;
                bool hasX = x is not null && _order.TryGetValue(x, out xIndex);
                bool hasY = y is not null && _order.TryGetValue(y, out yIndex);
                if (hasX && hasY) return xIndex.CompareTo(yIndex);
                if (hasX) return -1;
                if (hasY) return 1;
            }
            return Comparer<string>.Default.Compare(x, y);
        }
    }
}
