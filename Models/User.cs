public class User
{
    public int Id { get; set; }
    public List<Game> Games { get; set; }

    public User(int id)
    {
        Id = id;
        Games = new List<Game>();
    }

    public override string ToString()
    {
        return "{ \"UserId\": " + Id + ", \"CountOfGames\": " + Games.Count() + " }"; 
    }
}