using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

internal class Program
{
  static int PlayerWins = 0;
  static int ComputerWins = 0;
  static int MatchesPlayed = 0;

  static readonly Dictionary<string, string?> beats = new()  {
      { "rock", "scissors" },
      { "paper", "rock" },
      { "scissors", "paper" },
      { "dynamite", null },
  };

  static Dictionary<string, int> PlayerChoiceCounts = new()  {
      { "rock", 0 },
      { "paper", 0 },
      { "scissors", 0 },
      { "dynamite", 0 },
  };

  static readonly List<string> hands = ["rock", "paper", "scissors", "dynamite"];

  private static void Main()
  {
    LoadGame();

    Console.WriteLine("Rock, Paper, Scissors");

    string userHand = ChooseHand();
    string computerHand = GetComputerHand();

    bool? playerWon = null;

    Console.WriteLine($"You chose {userHand}");
    Console.WriteLine($"Computer chose {computerHand}");

    if (beats[userHand] == null)
    {
      playerWon = true;
      PlayerChoiceCounts["dynamite"]++;
    }
    else if (beats[computerHand] == null) playerWon = false;
    else if (userHand == "scissors")
    {
      playerWon = computerHand == beats[userHand];
      PlayerChoiceCounts["scissors"]++;
    }
    else if (userHand == "paper")
    {
      playerWon = computerHand == beats[userHand];
      PlayerChoiceCounts["paper"]++;
    }
    else if (userHand == "rock")
    {
      playerWon = computerHand == beats[userHand];
      PlayerChoiceCounts["rock"]++;
    }

    if (playerWon == null)
    {
      Console.WriteLine("TIE");
    }
    else if ((bool)playerWon)
    {
      Console.WriteLine("You WIN");
      PlayerWins++;
    }
    else
    {
      Console.WriteLine("Computer WIN");
      ComputerWins++;
    }

    MatchesPlayed++;
    SaveGame();
    playAgain();
  }

  static string ChooseHand()
  {
    int selectedIndex = 0;
    string? output = hands[selectedIndex];
    ConsoleKey key;
    List<string> choices = [
      "1/r. Rock",
      "2/p. Paper",
      "3/s. Scissors",
      "4/d. Dynamite"
    ];

    do
    {
      Console.Clear();

      Console.WriteLine("Choose a Hand");
      Console.WriteLine("");

      for (int i = 0; i < choices.Count; i++)
      {
        if (i == selectedIndex)
        {
          Console.ForegroundColor = ConsoleColor.Green;
        }

        Console.WriteLine(choices[i]);
        Console.ResetColor();
      }

      Console.WriteLine("");
      Console.WriteLine("Type SYMBOL or use ARROW KEYS to select and ENTER: ");

      key = Console.ReadKey(true).Key;

      if (key is ConsoleKey.Enter) output = hands[selectedIndex];
      else if (key is ConsoleKey.UpArrow && selectedIndex != 0) selectedIndex--;
      else if (key is ConsoleKey.DownArrow && selectedIndex != hands.Count - 1) selectedIndex++;
      else if (key is ConsoleKey.D1 or ConsoleKey.R) selectedIndex = 0;
      else if (key is ConsoleKey.D2 or ConsoleKey.P) selectedIndex = 1;
      else if (key is ConsoleKey.D3 or ConsoleKey.S) selectedIndex = 2;
      else if (key is ConsoleKey.D4 or ConsoleKey.D) selectedIndex = 3;
    }
    while (key != ConsoleKey.Enter);

    return output;
  }

  static string GetComputerHand()
  {
    string maxChoice = PlayerChoiceCounts.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
    Console.WriteLine(PlayerChoiceCounts[maxChoice]);

    var matchingKeys = PlayerChoiceCounts.Where(kvp => kvp.Value == PlayerChoiceCounts[maxChoice])
      .Select(kvp => kvp.Key)
      .ToList();

    Random rand = new();
    string randomHand = hands[rand.Next(0, hands.Count)];

    if (matchingKeys.Count == 2)
    {
      return matchingKeys[rand.Next(0, 1)];
    }
    else if (matchingKeys.Count == 3) return randomHand;

    return rand.NextDouble() < 0.5 ? beats[maxChoice] : randomHand;
    // returning value of dynamite breaks it
  }

  static void playAgain()
  {
    string? userInput;

    do
    {
      Console.WriteLine($"\nYou have played {MatchesPlayed} matches\n");
      Console.WriteLine($"You have won {PlayerWins} matches. ({Math.Round((double)PlayerWins / MatchesPlayed * 100, 2)}%)\n");
      Console.WriteLine($"Computer has won {ComputerWins} matches. ({Math.Round((double)ComputerWins / MatchesPlayed * 100, 2)}%)\n");

      Console.Write("Play again? (y/n) ");

      userInput = Console.ReadLine();

      if (userInput == "y")
      {
        Main();
        return;
      }

      if (userInput == "n")
      {
        Console.WriteLine("Goodbye");
        Environment.Exit(0);
        return;
      }
    }
    while (!(userInput is "y" or "n"));
  }

  static void SaveGame()
  {
    SaveData save = new(PlayerWins, ComputerWins, MatchesPlayed);
    string saveData = JsonSerializer.Serialize(save);
    File.WriteAllText("saveGame.json", saveData);
  }

  static void LoadGame()
  {
    if (!File.Exists("saveGame.json")) return;
    string jsonString = File.ReadAllText("saveGame.json");
    SaveData? data = JsonSerializer.Deserialize<SaveData>(jsonString);

    if (data != null)
    {
      PlayerWins = data.PlayerWins;
      ComputerWins = data.ComputerWins;
      MatchesPlayed = data.MatchesPlayed;
    }
  }

}

internal class SaveData
{
  public int PlayerWins { get; set; }
  public int ComputerWins { get; set; }
  public int MatchesPlayed { get; set; }

  public SaveData(int playerWins, int computerWins, int matchesPlayed)
  {
    PlayerWins = playerWins;
    ComputerWins = computerWins;
    MatchesPlayed = matchesPlayed;
  }
}