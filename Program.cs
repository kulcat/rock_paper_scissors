using System.Collections;
using System.Text.Json;

internal class Program
{

  static int PlayerWins = 0;
  static int ComputerWins = 0;
  private static void Main()
  {
    Console.WriteLine("Rock, Paper, Scissors");

    string userHand = ChooseHand();
    string computerHand = GetComputerHand();

    Console.WriteLine($"You chose {userHand}");
    Console.WriteLine($"Computer chose {computerHand}");

    if (userHand == computerHand)
    {
      Console.WriteLine("TIE");
    }
    else if (userHand == "rock")
    {
      if (computerHand == "scissors")
      {
        Console.WriteLine("You WIN");
        PlayerWins++;

      }
      else
      {
        Console.WriteLine("Computer WIN");
        ComputerWins++;
      }
    }
    else if (userHand == "paper")
    {
      if (computerHand == "rock")
      {
        Console.WriteLine("You WIN");
        PlayerWins++;

      }
      else
      {
        Console.WriteLine("Computer WIN");
        ComputerWins++;

      }
    }
    else if (userHand == "scissors")
    {
      if (computerHand == "paper")
      {
        Console.WriteLine("You WIN");
        PlayerWins++;

      }
      else
      {
        Console.WriteLine("Computer WIN");
        ComputerWins++;

      }
    }

    playAgain();
  }

  static string ChooseHand()
  {
    string userInput;
    string output;

    do
    {
      Console.WriteLine("Choose a Hand");
      Console.WriteLine("1/r. Rock");
      Console.WriteLine("2/p. Paper");
      Console.WriteLine("3/s. Scissors");

      userInput = Console.ReadLine();

      switch (userInput)
      {
        case "1" or "r":
          output = "rock";
          break;
        case "2" or "p":
          output = "paper";
          break;
        case "3" or "s":
          output = "scissors";
          break;
        default:
          output = "";
          break;
      }
    }
    while (!(userInput is "1" or "2" or "3" or "r" or "p" or "s"));

    return output;
  }

  static string GetComputerHand()
  {
    string output;
    int randomNum = new Random().Next(1, 4);

    switch (randomNum)
    {
      case 1:
        output = "rock";
        break;
      case 2:
        output = "paper";
        break;
      case 3:
        output = "scissors";
        break;
      case 4:
        output = "well";
        break;
      case 5:
        output = "scissors";
        break;
      default:
        output = "";
        break;
    }

    return output;
  }

  static void playAgain()
  {
    string userInput;

    do
    {
      Console.WriteLine("Play again? (y/n)");

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
    SaveData save = new(PlayerWins, ComputerWins);
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
    }
  }

}
internal class SaveData
{
  public int PlayerWins { get; set; }
  public int ComputerWins { get; set; }

  public SaveData(int playerWins, int computerWins)
  {
    PlayerWins = playerWins;
    ComputerWins = computerWins;
  }
}