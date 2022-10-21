public class User
{
    public int Id { get; set; }
    public IEnumerable<string> Games { get; set; }

    public User(int id)
    {
        Id = id;
        Games = new List<string>();
    }

    public override string ToString()
    {
        return "{ \"UserId\": " + Id + ", \"CountOfGames\": " + Games.Count() + " }"; 
    }
}