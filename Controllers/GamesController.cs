using Microsoft.AspNetCore.Mvc;
using CST_350_Milestone.Models;

namespace CST_350_Milestone.Controllers
{
    public class GamesController : Controller
    {
        // Static list shared across requests
        static List<CellModel> cells = new List<CellModel>();

        public GamesController()
        {
            // Initialize cells only once
            if (cells.Count == 0)
            {
                for (int i = 0; i < 64; i++)
                {
                    cells.Add(new CellModel(i, 0, "blue_button.png"));
                }
            }
        }
        [HttpGet]
        public IActionResult StartGame()
        {
            // Check authentication
            var userAuthenticated = HttpContext.Session.GetString("UserAuthenticated");
            if (string.IsNullOrEmpty(userAuthenticated) || userAuthenticated != "true")
                return RedirectToAction("Login", "Account");

            return View(new GameSettingsModel());
        }

        [HttpPost]
        public IActionResult StartGame(GameSettingsModel model)
        {
            // Check authentication
            var userAuthenticated = HttpContext.Session.GetString("UserAuthenticated");
            if (string.IsNullOrEmpty(userAuthenticated) || userAuthenticated != "true")
                return RedirectToAction("Login", "Account");

            // Validate form data
            if (!ModelState.IsValid)
                return View(model);

            // TODO MILESTONE 2: Create Board object and place mines
            // TODO MILESTONE 2: Store board in session

            return RedirectToAction("MineSweeperBoard", new { boardSize = model.BoardSize, difficulty = model.Difficulty });
        }

        public IActionResult MineSweeperBoard(string boardSize, string difficulty)
        {
            // Check authentication
            var userAuthenticated = HttpContext.Session.GetString("UserAuthenticated");
            if (string.IsNullOrEmpty(userAuthenticated) || userAuthenticated != "true")
                return RedirectToAction("Login", "Account");

            // Create cells for selected board size
            int boardDimension = int.Parse(boardSize);
            int totalCells = boardDimension * boardDimension;
            cells.Clear();
            
            for (int i = 0; i < totalCells; i++)
            {
                cells.Add(new CellModel(i, 0, "blue_button.png"));
            }

            // TODO MILESTONE 2: Create Board object and retrieve from session
            // TODO MILESTONE 2: Update cells with actual mine positions and adjacent counts

            ViewBag.BoardSize = boardDimension;
            ViewBag.Difficulty = difficulty;
            return View(cells);
        }

        public IActionResult Win()
        {
            // Check authentication
            var userAuthenticated = HttpContext.Session.GetString("UserAuthenticated");
            if (string.IsNullOrEmpty(userAuthenticated) || userAuthenticated != "true")
                return RedirectToAction("Login", "Account");

            // TODO MILESTONE 2: Retrieve game stats from session (time, cells revealed, score)
            // TODO MILESTONE 2: Pass stats to view via ViewBag

            return View();
        }

        public IActionResult Loss()
        {
            // Check authentication
            var userAuthenticated = HttpContext.Session.GetString("UserAuthenticated");
            if (string.IsNullOrEmpty(userAuthenticated) || userAuthenticated != "true")
                return RedirectToAction("Login", "Account");

            // TODO MILESTONE 2: Retrieve game stats from session (time, cells revealed)
            // TODO MILESTONE 2: Pass stats to view via ViewBag

            return View();
        }
    }
}
