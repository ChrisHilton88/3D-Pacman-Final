[System.Serializable]
public class RoundData
{
    public int round;           // What number round it is
    public string bonus;        // Tag name of the bonus
    public int time;            // Timer of the bonus


    public RoundData(int round, string bonus, int time)
    {
        this.round = round;
        this.bonus = bonus;
        this.time = time;
    }
}
