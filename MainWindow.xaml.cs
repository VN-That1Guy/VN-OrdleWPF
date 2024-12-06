using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace VN_OrdleWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        int rows = 6;
        int cols = 5;
        TextBlock[,] TextBlocks;
        TextBox[] AnswerBoxes = new TextBox[5];
        int charindex = 0;
        //TextBlock[] AnswerBlocks = new TextBlock[5];
        //string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        bool isGameOver = false;
        public List<string> words = new List<string>();
        string solution = "";
        string previousInput = "";
        string defaultinstructions = "";
        int round = 0;

        public MainWindow()
        {
            InitializeComponent();
        }
        private void getRandomWord()
        {
            solution = words[new Random().Next(words.Count)].ToLower();
        }
        private void generateBoard()
        {
            words = File.ReadAllLines("../../../data/data.txt").ToList();
            TextBlocks = new TextBlock[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    TextBlock textBox = new TextBlock();
                    textBox.Name = $"Row{i}Column{j}";
                    textBox.Width = 55;
                    textBox.Height = 40;
                    textBox.Margin = new Thickness(2);
                    textBox.Padding = new Thickness(2);
                    textBox.FontFamily = new FontFamily("Verdana");
                    textBox.FontSize = 24;
                    textBox.Background = Brushes.White;
                    textBox.HorizontalAlignment = HorizontalAlignment.Center;
                    textBox.VerticalAlignment = VerticalAlignment.Center;
                    textBox.TextAlignment = TextAlignment.Center;
                    //textBox.Text = chars[random.Next(chars.Length)].ToString();
                    GameBoard.Children.Add(textBox);
                    TextBlocks[i, j] = textBox;
                }
            }
            GameBoard.UpdateLayout();
            getRandomWord();
        }

        private void GameBoard_Initialized(object sender, EventArgs e)
        {
            generateBoard();
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            previousInput = "";
            if (!isGameOver)
            {
                for (int i = 0; i < TextBlocks.GetLength(1); i++)
                {
                    TextBlocks[round, i].Text = AnswerBoxes[i].Text;
                    if (i == 0 )
                        TextBlocks[round, i].Text = TextBlocks[round, i].Text.ToUpper();
                    previousInput += AnswerBoxes[i].Text;
                }
                checkForValidWord(previousInput);
                round++;
                if (round >= rows)
                    endGame();
            }
            else
                Restart();

            foreach (TextBox textBox in AnswerBoxes)
            {
                textBox.Text = "";
            }
        }

        private void Restart()
        {
            Submit.Content = "Submit";
            round = 0;
            InstructionText.Text = defaultinstructions;
            InstructionText.Foreground = Brushes.White;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    TextBlocks[i, j].Text = string.Empty;
                    TextBlocks[i, j].Background = Brushes.White;
                }
            }
            getRandomWord();
            isGameOver = false;
        }
        private void OrdleWindow_Initialized(object sender, EventArgs e)
        {
            defaultinstructions = InstructionText.Text;
            for (int i = 0; i < AnswerBoxes.Count(); i++)
            {
                AnswerBoxes[i] = (TextBox)Answer_Grid.Children[i];
            }
        }
        private void checkForWin(string input)
        {
            //check if input matches solution - if so show win screen (# of rounds)
            if (input.ToLower().Contains(solution.ToLower()))
            {
                endGame();
                InstructionText.Text += " You win!";
                InstructionText.Foreground = Brushes.GreenYellow;
            }
            else
            { 
                InstructionText.Text = "Guess again";
                InstructionText.Foreground = Brushes.Red;
            }
        }

        private void checkForValidWord(string input)
        {
            previousInput = input.ToLower();
            //see if word / input is valid word
            if (isValidWord(input))
            {
                checkForWin(input);
                return;
            }
            else
                InstructionText.Text = "Word entered isn't in our dictionary";
        }
        private bool isValidWord(string word)
        {
            string Fixed = "";
            for (int c = 0; c < word.Length; c++)
            {
                if (c == 0)
                {
                    Fixed += word[c].ToString().ToUpper();
                }
                else
                    Fixed += word[c];
            }
            checkLetters(word.ToLower());
            if (words.Contains(Fixed)) { return true; }
            return false;
        }

        private void endGame()
        {
            InstructionText.Text = $"Game Over!";
            Submit.Content = "Retry?";
            isGameOver = true;
        }
        private void checkLetters(string word)

        {
            //if correct letter at correct spot change color to green
            for (int i = 0; i < word.Length; i++)
            {
                if (solution.Contains(word[i]))
                {
                    for (int i1 = 0; i1 < solution.Length; i1++)
                    {
                        //Console.BackgroundColor = ConsoleColor.DarkYellow;
                        TextBlocks[round, i].Background = Brushes.Yellow;
                        if (i1 == i && word[i].Equals(solution[i1]))
                        {
                            //Console.BackgroundColor = ConsoleColor.Green;
                            TextBlocks[round, i].Background = Brushes.Green;
                            break;
                        }
                    }
                }
                else
                    TextBlocks[round, i].Background = Brushes.White;
            }
            //if letter is in the target word but in the wrong spot make it yellow

            //if the letter is not in the word make it gray
        }

        private void Letter1_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (AnswerBoxes[charindex].Text != "" && charindex + 1 < AnswerBoxes.Length)
            {
                charindex++;
                AnswerBoxes[charindex].Focus();
            }
            else if (charindex > 0 && charindex != AnswerBoxes.Length)
            {
                charindex--;
                AnswerBoxes[charindex].Focus();
            }
        }

        private void Letter1_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            charindex = Array.FindIndex(AnswerBoxes, x => x.Equals(textBox));
        }

        private void Letter1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back)
                if (charindex > 0)
                {
                    charindex--;
                    AnswerBoxes[charindex].Focus();
                }
        }
    }
}