using System.Text.Json;
public class Game
{

/* requrements
      "gameId": 5679,
      "name": "The Elder Scrolls V: Skyrim",
      "added": 10616,
      "metacritic": 94,
      "rating": 4.42,
      "released": "2011-11-11",
      "updated": "2020-07-06T04:26:04"
*/
    public int Id { get; set; }
    public string Name { get; set; }
    public int Added { get; set; }
    public int? Metacritic { get; set; }
    public double? Rating { get; set; }
    public string Released { get; set; }
    public DateTime Updated { get; set; }

    private DateTime released;

    public override string ToString()
    {
        return JsonSerializer.Serialize(this).ToString();
    }
}