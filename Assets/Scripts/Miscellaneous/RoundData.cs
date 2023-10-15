[System.Serializable]
public class RoundData
{
    public int round;           // What number round it is
    public string tag;          // Tag name of the bonus
    public int time;            // Timer of the bonus


    public RoundData(int round, string tag, int time)
    {
        this.round = round;
        this.tag = tag;
        this.time = time;
    }
}
