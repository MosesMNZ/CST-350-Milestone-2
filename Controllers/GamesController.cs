using Microsoft.AspNetCore.Mvc;
using CST_350_Milestone.Models;

namespace CST_350_Milestone.Controllers
{
    public class GamesController : Controller
    {
        [HttpGet]
        public IActionResult StartGame()
        {
            var user = HttpContext.Session.GetString("User");
            if (string.IsNullOrEmpty(user))
                return RedirectToAction("Login", "Account");

            return View(new GameSettingsModel());
        }

        [HttpPost]
        public IActionResult StartGame(GameSettingsModel model)
        {
            var user = HttpContext.Session.GetString("User");
            if (string.IsNullOrEmpty(user))
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(model);

            int boardSize = int.Parse(model.BoardSize);
            var board = new BoardModel(boardSize, model.Difficulty);
            HttpContext.Session.SetString("Board", board.ToJson());

            return RedirectToAction("MineSweeperBoard");
        }

        public IActionResult MineSweeperBoard()
        {
            var user = HttpContext.Session.GetString("User");
            if (string.IsNullOrEmpty(user))
                return RedirectToAction("Login", "Account");

            var boardJson = HttpContext.Session.GetString("Board");
            if (boardJson == null) return RedirectToAction("StartGame");

            var board = BoardModel.FromJson(boardJson)!;

            if (board.GameWon) return RedirectToAction("Win");
            if (board.GameOver) return RedirectToAction("Loss");

            ViewBag.BoardSize = board.Size;
            ViewBag.Difficulty = board.Difficulty;
            ViewBag.TotalMines = board.TotalMines;
            ViewBag.ElapsedSeconds = board.GetElapsedSeconds();
            return View(board.Cells);
        }

        [HttpPost]
        public IActionResult RevealCell(int id)
        {
            var user = HttpContext.Session.GetString("User");
            if (string.IsNullOrEmpty(user))
                return RedirectToAction("Login", "Account");

            var boardJson = HttpContext.Session.GetString("Board");
            if (boardJson == null) return RedirectToAction("StartGame");

            var board = BoardModel.FromJson(boardJson)!;
            board.RevealCell(id);
            HttpContext.Session.SetString("Board", board.ToJson());

            if (board.GameWon) return RedirectToAction("Win");
            if (board.GameOver) return RedirectToAction("Loss");

            return RedirectToAction("MineSweeperBoard");
        }

        public IActionResult RestartGame()
        {
            var user = HttpContext.Session.GetString("User");
            if (string.IsNullOrEmpty(user))
                return RedirectToAction("Login", "Account");

            var boardJson = HttpContext.Session.GetString("Board");
            if (boardJson == null) return RedirectToAction("StartGame");

            var old = BoardModel.FromJson(boardJson)!;
            var board = new BoardModel(old.Size, old.Difficulty);
            HttpContext.Session.SetString("Board", board.ToJson());

            return RedirectToAction("MineSweeperBoard");
        }

        public IActionResult Win()
        {
            var user = HttpContext.Session.GetString("User");
            if (string.IsNullOrEmpty(user))
                return RedirectToAction("Login", "Account");

            var boardJson = HttpContext.Session.GetString("Board");
            if (boardJson != null)
            {
                var board = BoardModel.FromJson(boardJson)!;
                ViewBag.ElapsedSeconds = board.FinalElapsedSeconds;
                ViewBag.Score = board.CalculateScore();
                ViewBag.BoardSize = board.Size;
                ViewBag.Difficulty = board.Difficulty;
            }

            return View();
        }

        public IActionResult Loss()
        {
            var user = HttpContext.Session.GetString("User");
            if (string.IsNullOrEmpty(user))
                return RedirectToAction("Login", "Account");

            var boardJson = HttpContext.Session.GetString("Board");
            if (boardJson != null)
            {
                var board = BoardModel.FromJson(boardJson)!;
                ViewBag.ElapsedSeconds = board.FinalElapsedSeconds;
                ViewBag.CellsRevealed = board.RevealedCellCount;
                ViewBag.BoardSize = board.Size;
                ViewBag.Difficulty = board.Difficulty;
            }

            return View();
        }
    }
}
