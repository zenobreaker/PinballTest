using System.Collections.Generic;


[System.Serializable]
public class StageInfo
{
    public int id;

    // 등장할 적 
    public List<int> groupIds = new List<int>();


    public int wave = 0;

    public StageInfo(StageInfo other)
    {
        id = other.id;
        groupIds = new List<int>(other.groupIds);
        wave = other.wave;
    }

    public StageInfo() { }

    public StageInfo Copy()
    {
        return new StageInfo()
        {
            id = this.id,
            groupIds = new List<int>(groupIds),
            wave = wave,
        };
    }
}

