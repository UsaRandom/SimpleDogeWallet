
using System.Windows.Forms;

Application.EnableVisualStyles();
Application.SetCompatibleTextRenderingDefault(false);
using var game = new DogecoinTerminal.DogecoinTerminalGame();
game.Run();
