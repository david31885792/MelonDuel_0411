[System.Serializable]
public struct TileData
{
    public int number;
    public string name;

    public TileData(int number, string name)
    {
        this.number = number;
        this.name = name;
    }

    public int TileNumber => number;
}
