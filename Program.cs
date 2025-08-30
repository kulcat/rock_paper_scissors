using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

internal class Program
{
  static int PlayerWins = 0;
  static int OpponentWins = 0;
  static int MatchesPlayed = 0;
  static bool? isMultiplayer = null;

  static readonly Dictionary<string, string?> beats = new()  {
      { "rock", "scissors" },
      { "paper", "rock" },
      { "scissors", "paper" },
      { "dynamite", null },
  };

  static readonly Dictionary<string, int> PlayerChoiceCounts = new()  {
      { "rock", 0 },
      { "paper", 0 },
      { "scissors", 0 },
      { "dynamite", 0 },
  };

  static readonly List<string> hands = ["rock", "paper", "scissors", "dynamite"];

  private static void Main()
  {
    Console.Clear();

    if (isMultiplayer == null) isMultiplayer = MultiPlayer();

    if ((bool)isMultiplayer)
    {
      PlayerWins = 0;
      OpponentWins = 0;
      MatchesPlayed = 0;
    }
    else
    {
      LoadGame();
    }

    Console.WriteLine("Rock, Paper, Scissors");

    string playerHand = ChooseHand();
    string opponentHand = (bool)isMultiplayer ? ChooseHand() : GetComputerHand();

    bool? playerWon = null;

    Console.WriteLine($"You chose {playerHand}");
    Console.WriteLine($"Opponent chose {opponentHand}");

    if (playerHand == "dynamite")
    {
      playerWon = beats[opponentHand] != null;
      PlayerChoiceCounts["dynamite"]++;
    }
    else if (playerHand == "scissors")
    {
      playerWon = opponentHand == beats[playerHand];
      PlayerChoiceCounts["scissors"]++;
    }
    else if (playerHand == "paper")
    {
      playerWon = opponentHand == beats[playerHand];
      PlayerChoiceCounts["paper"]++;
    }
    else if (playerHand == "rock")
    {
      playerWon = opponentHand == beats[playerHand];
      PlayerChoiceCounts["rock"]++;
    }

    if (playerWon == null) Console.WriteLine("TIE");
    else if ((bool)playerWon)
    {
      Console.WriteLine("Player WIN");
      PlayerWins++;
    }
    else
    {
      Console.WriteLine("Opponent WIN");
      OpponentWins++;
    }

    MatchesPlayed++;

    if (!(bool)isMultiplayer)
    {
      SaveGame();
    }

    PlayAgain();
  }

  static bool MultiPlayer()
  {
    int selectedIndex = 0;
    bool output = false;

    ConsoleKey key;

    List<string> choices = [
      "1/c. Player vs Computer",
      "2/p. Player vs Player2 (note: stats are not saved)",
    ];

    do
    {
      Console.Clear();

      Console.WriteLine("Choose a Game Mode");
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

      if (key == ConsoleKey.Enter) output = selectedIndex == 1;
      else if (key == ConsoleKey.UpArrow && selectedIndex != 0) selectedIndex--;
      else if (key == ConsoleKey.DownArrow && selectedIndex != choices.Count - 1) selectedIndex++;
      else if (key is ConsoleKey.D1 or ConsoleKey.C) selectedIndex = 0;
      else if (key is ConsoleKey.D2 or ConsoleKey.P) selectedIndex = 1;
    }
    while (key != ConsoleKey.Enter);

    return output;
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
      else if (key is ConsoleKey.DownArrow && selectedIndex != choices.Count - 1) selectedIndex++;
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
    string? maxChoice = PlayerChoiceCounts.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;

    var matchingCounts = PlayerChoiceCounts.Where(kvp => kvp.Value == PlayerChoiceCounts[maxChoice]).Select(kvp => kvp.Key).ToList();

    Random rand = new();
    string randomHand = hands[rand.Next(0, hands.Count)];

    if (matchingCounts.Count == 2)
      return matchingCounts[rand.Next(0, 1)];
    else if (matchingCounts.Count == 3)
      return randomHand;

    double countAverage = PlayerChoiceCounts.Values.Average();
    double chance = PlayerChoiceCounts[maxChoice] > countAverage ? 1.00 : rand.NextDouble();
    string chosenHand = beats[maxChoice] ?? "dynamite";

    return chance < 0.7 ? chosenHand : randomHand;
  }

  static void PlayAgain()
  {
    string? userInput;

    do
    {
      Console.WriteLine($"\n{MatchesPlayed} matches played\n");
      Console.WriteLine($"Player has won {PlayerWins} matches. ({Math.Round((double)PlayerWins / MatchesPlayed * 100, 2)}%)\n");
      Console.WriteLine($"Opponent has won {OpponentWins} matches. ({Math.Round((double)OpponentWins / MatchesPlayed * 100, 2)}%)\n");

      Console.Write("Play again? (y/n) ");

      userInput = Console.ReadLine();

      if (userInput == "y")
      {
        Main();
        return;
      }
      else if (userInput == "n")
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
    SaveData save = new(PlayerWins, OpponentWins, MatchesPlayed);
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
      OpponentWins = data.OpponentWins;
      MatchesPlayed = data.MatchesPlayed;
    }
  }
}

internal class SaveData
{
  public int PlayerWins { get; set; }
  public int OpponentWins { get; set; }
  public int MatchesPlayed { get; set; }

  public SaveData(int playerWins, int opponentWins, int matchesPlayed)
  {
    PlayerWins = playerWins;
    OpponentWins = opponentWins;
    MatchesPlayed = matchesPlayed;
  }
}